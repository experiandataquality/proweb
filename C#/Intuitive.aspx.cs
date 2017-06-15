namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web.UI.WebControls;
    using Experian.Qas.Proweb;

    public partial class Intuitive : System.Web.UI.Page
    {
        // Error string
        protected string m_asError;

        // Dataset array for dropdown box
        protected IList<Dataset> m_atDatasets;

        // Search result (contains either picklist or matched address or nothing)
        protected SearchResult m_SearchResult = null;

        protected String m_sPicklistPrompt;

        // Picklist refinement text box prompt
        protected string PicklistPrompt
        {
            get
            {
                return m_sPicklistPrompt;
            }
        }
        
        /// <summary>
        /// Page load sets up Javascript event handlers for input elements and deals with postback events
        /// from these functions.
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {
            InputBox.Attributes.Add("onkeyup", "javascript: InputAddressKeyPressed(event)");

            AddressList.Attributes.Add("onkeypress", "javascript: AddressKeypress(this, event)");
            AddressList.Attributes.Add("onclick",    "javascript: AddressClick(this)");
            AddressList.Attributes.Add("ondblclick", "javascript: AddressDoubleClick(this)");

            if (Request.Params["__EVENTTARGET"] == InputBox.UniqueID && Request.Params["__EVENTARGUMENT"] == "onkeyup")
            {
                InputBox_TextChanged();
            }
            else if (Request.Params["__EVENTTARGET"] == InputBox.UniqueID && Request.Params["__EVENTARGUMENT"] == "onenter")
            {
                SelectButton_Click(null, null);
            }
            else if (Request.Params["__EVENTTARGET"] == InputBox.UniqueID && Request.Params["__EVENTARGUMENT"] == "ondown")
            {
                if (AddressList.Visible)
                {
                    AddressList.Focus();
                    AddressList.SelectedIndex = 0;
                }
            }
            else if (Request.Params["__EVENTTARGET"] == AddressList.UniqueID && Request.Params["__EVENTARGUMENT"] == "onclick")
            {
                InputBox.Focus();
            }
            else
            {
                InputBox.Focus();
            }
        }

        /// <summary>
        /// Looks up a datamapping relating to the selected country
        /// </summary>
        protected string GetLayout()
        {
           string sLayout = "";
           string sDataID = CountryList.SelectedValue;

           if (sDataID != null && sDataID != "")
           {
               // Look for a layout specific to this datamap 
               sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT + "." + sDataID];

               if (sLayout == null || sLayout == "")
               {
                   // No layout found specific to this datamap - try the default
                   sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT];
               }
           } 
           
           return sLayout;
        }

        /// <summary>
        /// The main search routine which is called when input box text changes. Tests the connection with a
        /// CanSearch operation then if server is available performs Intuitive Search and populates and 
        /// address list with the resulting picklist elements. The moniker is stored upon a successful search
        /// to be used by formatting to replay the search operation.
        /// </summary>
        protected void InputBox_TextChanged()
        {
            Session["useTypedAddress"] = true;
            Session.Contents.Remove("inputMoniker");

            // Using com.qas.proweb
            CanSearch canSearch = null;
            
            ResultBox.Text = "";

            try
            {
                // Create new search connection
                IAddressLookup addressLookup = QAS.GetSearchService();
                addressLookup.CurrentEngine.EngineType = Engine.EngineTypes.Intuitive;
                addressLookup.CurrentEngine.Flatten = false;

                // Get the layout
                string sLayout = GetLayout();

                // Check that searching with this engine and layout is available
                canSearch = addressLookup.CanSearch(CountryList.SelectedValue, sLayout, null);

                if (canSearch.IsOk) 
                {
                    Prompt.Enabled = true;
                    
                    // Search on the address
                    m_SearchResult = addressLookup.Search(CountryList.SelectedValue, 
                                                          InputBox.Text, 
                                                          PromptSet.Types.Default, 
                                                          sLayout);

                    // Clear address list
                    AddressList.Items.Clear();

                    if ( m_SearchResult.Picklist != null )
                    {
                        // Store picklist moniker for replaying the search
                        Session["inputMoniker"] = m_SearchResult.Picklist.Moniker;

                        Prompt.Text = m_SearchResult.Picklist.Prompt;

                        // Too many matches so return prompt to address list
                        if (m_SearchResult.Picklist.IsMaxMatches)
                        {
                            AddressList.Enabled = false;
                            PicklistItem entry = m_SearchResult.Picklist.PicklistItems[0];
                            AddressList.Items.Add(new ListItem("Continue typing (too many matches) ", entry.Moniker));
                        }
                        else
                        {
                            if (m_SearchResult.Picklist.PicklistItems != null)
                            {
                                if (m_SearchResult.Picklist.PicklistItems.Count == 1 &&
                                    m_SearchResult.Picklist.PicklistItems[0].IsInformation)
                                {
                                    Prompt.Text = m_SearchResult.Picklist.PicklistItems[0].Text;
                                }
                                else
                                {
                                    // Get pick list entries
                                    AddressList.Enabled = true;

                                    // Write the results to the address list. Also store the SPMs for use in formatting
                                    foreach (PicklistItem entry in m_SearchResult.Picklist.PicklistItems)
                                    {
                                        if (entry.PartialAddress != "")
                                        {
                                            AddressList.Items.Add(new ListItem(entry.Text, entry.Moniker));
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Prompt.Text = "Prompt: ";
                    }

                    // Address list can grow from 3 to 10 items to aid display of larger picklists
                    if (AddressList.Items.Count >= 1)
                    {
                        int RowsMin = 3;
                        int RowsMax = 10;
                        
                        AddressList.Visible = true;
                        
                        if (AddressList.Items.Count > RowsMin)
                        {
                            if (AddressList.Items.Count < RowsMax)
                            {
                                AddressList.Rows = AddressList.Items.Count;
                            } 
                            else 
                            {
                                AddressList.Rows = RowsMax;
                            }
                        } 
                        else 
                        {
                            AddressList.Rows = RowsMin;
                        }
                    }
                    // Address list should disappear if empty
                    else
                    {
                        AddressList.Visible = false;
                        ResultBox.Text = "";
                    }
                }
                else if (canSearch != null)
                {
                    m_asError = canSearch.ErrorMessage;
                }
            }
            catch (System.Exception e)
            {
               m_asError = e.Message;
            }

            if (m_asError != null)
            {
                UpdatePanelLogging.Update();
            }
        }

        /// <summary>
        /// The main formatting routine which is called when an address is selected and the formatting button 
        /// pressed.
        /// 
        /// When an Address is selected from address list the stored moniker of the address element is used to format the 
        /// classified address.
        /// </summary>
        protected void SelectButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure that we don't search on blank strings
                Regex rNonWhitespace = new Regex(@"\S");
                if (!rNonWhitespace.IsMatch(InputBox.Text))
                {
                    return;
                }

                // If an address has been manually typed into the input box, standardise and format it
                // using the Intuitive Search engine
                if (AddressList.SelectedValue == "")
                {
                    IAddressLookup addressLookup = QAS.GetSearchService();                    
                    
                    string sLayout = GetLayout();

                    if ( Session.Contents["inputMoniker"] == null ||
                         Session["inputMoniker"].ToString().Length == 0 )
                    {
                        // Need to do a search first to obtain the moniker
                        addressLookup.CurrentEngine.EngineType = Engine.EngineTypes.Intuitive;
                        addressLookup.CurrentEngine.Flatten = true;

                        // Using com.qas.proweb
                        CanSearch canSearch = addressLookup.CanSearch(CountryList.SelectedValue, sLayout, null);

                        if (canSearch.IsOk)
                        {
                            m_SearchResult = addressLookup.Search(CountryList.SelectedValue,
                                                          InputBox.Text,
                                                          PromptSet.Types.Default,
                                                          sLayout);
                            
                            if (m_SearchResult.Picklist != null)
                            {
                                Session["inputMoniker"] = m_SearchResult.Picklist.Moniker;
                            }
                        }
                    }

                    if (Session["inputMoniker"].ToString().Length != 0)
                    {
                        // Use SPM to format address
                        FormattedAddress canFormat = addressLookup.GetFormattedAddress(Session["inputMoniker"].ToString(), sLayout);

                        StringBuilder theText = new StringBuilder();

                        if (canFormat.AddressLines != null)
                        {
                            foreach (AddressLine addrline in canFormat.AddressLines)
                            {
                                theText.Append(addrline.Line);
                                theText.Append("\n");
                            
                            }
                        }

                        ResultBox.Text = theText.ToString();
                    } 
                    else 
                    {
                        m_asError = "No input to format - please enter address details";
                    }
                }
                else
                {
                    // If an address has been selected from the picklist, and has not been subsequently edited,
                    // format that instead
                    FormatAddress(AddressList.SelectedValue);
                }
            }
            catch (System.Exception ex)
            {
                m_asError = ex.Message;
            }

            if (m_asError != null)
            {
                UpdatePanelLogging.Update();
            }
        }

        /// <summary>
        /// The onclick event for the Clear button. This is used to clear the search box and any result
        /// fields and reset the input moniker for formatting.
        /// </summary>
        protected void ClearButton_Click(object sender, EventArgs e)
        {
            InputBox.Text = "";
            InputBox.Focus();
            AddressList.Items.Clear();
            AddressList.Visible = false;
            ResultBox.Text = "";

            Session["useTypedAddress"] = false;
            Session.Contents.Remove("inputMoniker");
        }

        /// <summary>
        /// Formats the selected address from the address list, by passing the SPM to the Pro Web server
        /// </summary>
        protected void FormatAddress(String sMoniker)
        {
            try
            {
                // Format the address
                IAddressLookup addressLookup = QAS.GetSearchService();
                FormattedAddress tAddressResult = addressLookup.GetFormattedAddress(sMoniker, GetLayout());

                StringBuilder theText = new StringBuilder();

                if (tAddressResult.AddressLines != null)
                {
                    foreach (AddressLine addrline in tAddressResult.AddressLines)
                    {
                        theText.Append(addrline.Line);
                        theText.Append("\n");
                    }
                }

                ResultBox.Text = theText.ToString();
                Session["useTypedAddress"] = false;
                
            }
            catch (Exception x)
            {
                m_asError = x.Message;
            }
        }


        /// <summary>
        /// Set up country list by querying Pro Web server for list of available countries
        /// </summary>
        protected void PopulateDatamaps(object sender, EventArgs e)
        {
            if (m_atDatasets == null)
            {
                // Retrieve a list of all datasets ( maps ) available on the server
                try
                {
                    IAddressLookup addressLookup = QAS.GetSearchService();
                    m_atDatasets = addressLookup.GetAllDatasets();
                }
                catch (Exception x)
                {
                    m_asError = x.Message;
                }
            }

            // Populate drop down list of countries
            if (m_atDatasets != null)
            {
                ListItem itemheader1 = new ListItem("-- Datamaps available --", "");
                itemheader1.Attributes["class"] = "heading";
                CountryList.Items.Add(itemheader1);

                foreach (Dataset dset in m_atDatasets)
                {
                    ListItem litem = new ListItem(dset.Name, dset.ID);
                    CountryList.Items.Add(litem);
                }

                ListItem itemheader2 = new ListItem("-- Other --", "");
                itemheader2.Attributes["class"] = "heading";
                CountryList.Items.Add(itemheader2);
            }

            foreach (Dataset dset in Constants.AllCountries)
            {
                bool bDuplicate = false;

                if (m_atDatasets != null)
                {
                    foreach (Dataset serverDset in m_atDatasets)
                    {
                        if (serverDset.Name == dset.Name || serverDset.ID == dset.ID)
                        {
                            bDuplicate = true;
                            break;
                        }
                    }
                }

                if (!bDuplicate)
                {
                    ListItem litem = new ListItem(dset.Name, dset.ID);
                    CountryList.Items.Add(litem);
                }
            }
        }
    }
}

