/// QAS Pro Web > (c) Experian > www.edq.com
/// Intranet > Rapid Addressing > Standard > RapidAddress
/// Format the final address
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Intranet > Rapid Addressing > Standard > Full QAS searching with hierarchical picklists
    /// Retrieve and format the final address, or provide manual entry on failure; pass address back to calling window
    ///
    /// This page is based on RapidBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class RapidAddress : RapidBasePage
    {
        /** Page members **/

        private string m_sDataID = "";
        protected List<MultiDataplusDisplayGroup> m_MultiDataplusDisplayGroups;
        protected MultiDataplusControl[] m_MultiDataplusControls;

        /** Methods **/


        /// <summary>
        /// Pick up values transfered from other pages
        /// </summary>
        override protected void Page_Load(object sender, System.EventArgs e)
        {
            base.Page_Load(sender, e);

            DataID = Request.Form[Constants.FIELD_DATA_ID];

            if (DataID == "")
            {
                DataID = StoredDataID;
            }
            else
            {
                StoredDataID = DataID;
            }

            if ( country.Items.Count > 0 )
            {
                country.SelectedValue = m_sDataID;
            }

            // Load datasets from server
            if (StoredDataMapList == null)
            {
                try
                {
                    m_atDatasets = AddressLookup.GetAllDatasets();
                    StoredDataMapList = m_atDatasets;
                }
                catch( Exception)
                {
                    m_atDatasets = null;
                }
            }
            else
            {
                m_atDatasets = StoredDataMapList;
            }

            if (!IsPostBack)
            {
                // Display values in transit from Searching page
                SearchEngine = StoredSearchEngine;

                // Address result
                FormattedAddress addr = null;

                string[] asLines = null;
                string[] asLabels = null;

                // Retrieve address
                FormatAddress(ref addr, ref asLabels, ref asLines);

                if ( addr != null )
                {
                    DisplayAddress(addr);
                }
                else
                {
                    DisplayAddress( asLabels, asLines );
                }

                RenderMultiDataplusControls();

                // Set display messages
                SetWelcomeMessage(StoredRoute);
                SetWarningMessage(StoredWarning);
                SetErrorMessage();

            }

            PopulateDatasets();  

            // Else leave it to the event handlers (New, Back, Accept)
        }


        protected void PopulateDatasets()
        {
            country.Items.Clear();
            
            // Populate drop down list of countries
            country.Attributes.CssStyle["width"] = SELECT_WIDTH;

            ListItem itemheader1 = new ListItem("-- Datamaps Available --", "");
            itemheader1.Attributes["class"] = "heading";
            country.Items.Add(itemheader1);

            string sDatamapName;

            if (m_atDatasets != null)
            {
                foreach (Dataset dset in m_atDatasets)
                {
                    sDatamapName = dset.Name;

                    if (sDatamapName.Length > MAX_DATAMAP_NAME_LENGTH)
                    {
                        sDatamapName = sDatamapName.Substring(0, MAX_DATAMAP_NAME_LENGTH - 3) + "...";
                    }

                    ListItem litem = new ListItem(sDatamapName, dset.ID);
                    country.Items.Add(litem);
                }
            }

            ListItem itemheader2 = new ListItem("-- Other --", "");
            itemheader2.Attributes["class"] = "heading";
            country.Items.Add(itemheader2);

            foreach (Dataset dset in Constants.CoreCountries)
            {
                bool bDuplicate = false;

                if ( m_atDatasets != null )
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
                    sDatamapName = dset.Name;

                    if (sDatamapName.Length > MAX_DATAMAP_NAME_LENGTH)
                    {
                        sDatamapName = sDatamapName.Substring(0, MAX_DATAMAP_NAME_LENGTH - 3) + "...";
                    }

                    ListItem litem = new ListItem(sDatamapName, dset.ID);
                    country.Items.Add(litem);
                }
            }

            if ( m_sDataID.Length > 0 )
            {
                country.SelectedValue = m_sDataID;
            }
            else
            {
                country.SelectedIndex = 1;
            }
        }

        /// <summary>
        /// Start a new search - initialise/blank values
        /// </summary>
        protected void NewSearch()
        {
            // Empty history - to forget initial search string
            m_aHistory.Clear();

            ResetWarningMessage();

            GoSearchPage(DataID, SearchEngine);
        }

        /// <summary>
        /// Start a new search, but retrieve the search string from the history top
        /// </summary>
        protected void FreshSearch()
        {
            // Cut down history - to keep just initial search string for new search
            m_aHistory.Truncate(1);

            ResetWarningMessage();

            GoSearchPage(DataID, SearchEngine);
        }

        /// <summary>
        /// Retrieve the formatted address based on the StoredMoniker, or create a set of blank lines
        /// </summary>
        /// <param name="asLabels">Array of labels for each line (address type descriptions)</param>
        /// <param name="asLines">Array of address lines</param>
        protected void FormatAddress(ref FormattedAddress addr, ref string[] asLabels, ref string[] asLines )
        {
            if (StoredRoute.Equals(Constants.Routes.Okay))
            {
                try
                {
                    // Format final address using Moniker and Layout
                    addr = AddressLookup.GetFormattedAddress(StoredMoniker, GetLayout());
                    List<AddressLine> lines = addr.AddressLines;

                    // Address layout issues override other warnings
                    if (addr.IsOverflow)
                    {
                        StoredWarning = StepinWarnings.Overflow;
                    }
                    else if (addr.IsTruncated)
                    {
                        StoredWarning = StepinWarnings.Truncate;
                    }
                }
                catch (Exception x)
                {
                    addr = null;
                    StoredRoute = Constants.Routes.Failed;
                    StoredErrorInfo = x.Message;
                }
            }

            // Provide default (empty) address for manual entry
            if (!StoredRoute.Equals(Constants.Routes.Okay))
            {
                asLabels = new string[]
                {
                    "Address Line 1", "Address Line 2", "Address Line 3",
                    "City", "State or Province", "ZIP or Postal Code"
                };
                asLines = new string[]
                {
                    "", "", "", "", "", ""
                };
            }
        }


        /** Page updating **/


        protected void DisplayAddress( FormattedAddress addr )
        {
            for( int iIndex = 0; iIndex < addr.AddressLines.Count; ++iIndex )
            {
                if (addr.AddressLines[iIndex].LineType == LineType.DataPlus)
                {
                    if ( addr.AddressLines[iIndex].DataplusGroups != null )
                    {
                        AddMultiDataplusLine( addr.AddressLines[iIndex].Label,
                                              addr.AddressLines[iIndex].DataplusGroups,
                                              iIndex );
                    }
                    else
                    {
                        AddDataplusLine(addr.AddressLines[iIndex].Label, addr.AddressLines[iIndex].Line, iIndex);
                    }
                }
                else
                {
                    AddAddressLine(addr.AddressLines[iIndex].Label, addr.AddressLines[iIndex].Line, iIndex);
                }
            }

        }

        /// <summary>
        /// Dynamically populate the TableAddress table control from the arguments
        /// </summary>
        protected void DisplayAddress(string[] asLabels, string[] asLines)
        {
            for (int iIndex = 0; iIndex < asLines.Length; ++iIndex)
            {
                AddAddressLine(asLabels[iIndex], asLines[iIndex], iIndex);
            }
        }

        /// <summary>
        /// Add a table row, with cells for the label, a gap, and a text input control
        /// </summary>
        /// <returns>The text input control</returns>
        protected HtmlInputText AddAddressLine(string sLabel, string sLine, int iLineNum)
        {
            TableRow row = new TableRow();

            TableCell cellLabel = new TableCell();
            cellLabel.CssClass = "label";
            LiteralControl label = new LiteralControl(sLabel);
            cellLabel.Controls.Add(label);
            row.Cells.Add(cellLabel);

            TableCell cellAddress = new TableCell();
            cellAddress.CssClass = "line";
            HtmlInputText addressLine = new HtmlInputText();
            addressLine.Value = sLine;
            addressLine.ID = Constants.FIELD_ADDRESS_LINES + iLineNum.ToString();
            cellAddress.Controls.Add(addressLine);
            row.Cells.Add(cellAddress);

            TableAddress.Rows.Add(row);

            return addressLine;
        }

        protected void AddDataplusLine( string sLabel, string sLine, int iLineNum )
        {
            TableRow row = new TableRow();

            TableCell cellLabel = new TableCell();
            cellLabel.CssClass = "label";
            LiteralControl label = new LiteralControl(sLabel);
            cellLabel.Controls.Add(label);
            row.Cells.Add(cellLabel);

            TableCell cellAddress = new TableCell();
            cellAddress.CssClass = "line";
            LiteralControl addressLine = new LiteralControl( sLine );
            cellAddress.ID = Constants.FIELD_ADDRESS_LINES + iLineNum.ToString();
            cellAddress.Controls.Add(addressLine);
            row.Cells.Add(cellAddress);

            TableAddress.Rows.Add(row);
        }

        protected void AddMultiDataplusLine( string sLabel, List<DataplusGroup> aGroups, int iLineNum )
        {
            if ( m_MultiDataplusDisplayGroups == null )
            {
                m_MultiDataplusDisplayGroups = new List<MultiDataplusDisplayGroup>();
            }
            
            TableRow row = new TableRow();

            TableCell cellLabel = new TableCell();
            cellLabel.CssClass = "label";
            LiteralControl label = new LiteralControl(sLabel);
            cellLabel.Controls.Add(label);
            row.Cells.Add(cellLabel);

            TableCell cellAddress = new TableCell();
            cellAddress.CssClass = "line multidp";
            cellAddress.ID = Constants.FIELD_ADDRESS_LINES + iLineNum.ToString();

            string sElemID = "";

            for( int i = 0; i < aGroups.Count; ++i )
            {
                if (i != 0)
                {
                    Label comma = new Label();
                    comma.Text = ",&nbsp;";
                    cellAddress.Controls.Add(comma);
                } 
                
                DataplusGroup grp = aGroups[i];
                
                sElemID = grp.Name + m_MultiDataplusDisplayGroups.Count.ToString();
                
                MultiDataplusDisplayGroup dispGrp = new MultiDataplusDisplayGroup();

                dispGrp.sGroup = grp.Name;
                dispGrp.iLineNum = iLineNum;
                dispGrp.sElemID = sElemID;
                dispGrp.asItems = grp.Items;
                
                Label addressLine = new Label();
                addressLine.Text = "";

                addressLine.ID = sElemID;
                cellAddress.Controls.Add(addressLine);

                m_MultiDataplusDisplayGroups.Add(dispGrp);
            }

            row.Cells.Add(cellAddress);
            TableAddress.Rows.Add(row);

        }

        // Take the list of multi dataplus display groups & work out the controls we need to draw
        protected MultiDataplusControl[] GetMultiDPControls()
        {
            // If we have any multi dataplus groups, and we haven't done this already...
            if ( m_MultiDataplusControls == null && m_MultiDataplusDisplayGroups != null )
            {
                List<MultiDataplusControl> lControls = new List<MultiDataplusControl>();
                List<string> lGroupsUsed = new List<string>();

                foreach( MultiDataplusDisplayGroup grp in m_MultiDataplusDisplayGroups )
                {
                    // Check if we've already created a control for this group..
                    if ( !lGroupsUsed.Contains(grp.sGroup) && grp.sGroup != "" )
                    {
                        MultiDataplusControl ctrl = new MultiDataplusControl();

                        ctrl.sGroup = grp.sGroup;

                        ctrl.sFwdID = "fwd" + lControls.Count.ToString();
                        ctrl.sBackID = "bck" + lControls.Count.ToString();
                        ctrl.sReturnID = "rtn" + lControls.Count.ToString();
                        ctrl.sIndexID = "idx" + lControls.Count.ToString();

                        // Don't draw controls if there's only one item in the group
                        ctrl.bRender = (grp.asItems.Length > 1) ? true : false;

                        lGroupsUsed.Add(grp.sGroup);
                        lControls.Add(ctrl);
                    }
                }

                m_MultiDataplusControls = lControls.ToArray();
            }

            return m_MultiDataplusControls;
        }

        // Add any multi dataplus controls to the page
        private void RenderMultiDataplusControls()
        {
            MultiDataplusControl[] aCtrls = GetMultiDPControls();

            if (aCtrls != null)
            {
                foreach (MultiDataplusControl ctrl in aCtrls)
                {
                    if (ctrl.bRender == true)
                    {
                        TableRow row = new TableRow();

                        TableCell cellName = new TableCell();
                        cellName.Text = "&nbsp;" + ctrl.sGroup + "&nbsp;";
                        row.Cells.Add(cellName);

                        TableCell cellBack = new TableCell();
                        HtmlInputButton btnBack = new HtmlInputButton();
                        btnBack.Value = "<";
                        btnBack.ID = ctrl.sBackID;
                        cellBack.Controls.Add(btnBack);
                        row.Cells.Add(cellBack);

                        TableCell cellIndex = new TableCell();
                        cellIndex.ID = ctrl.sIndexID;
                        row.Cells.Add(cellIndex);

                        TableCell cellFwd = new TableCell();
                        HtmlInputButton btnFwd = new HtmlInputButton();
                        btnFwd.Value = ">";
                        btnFwd.ID = ctrl.sFwdID;
                        cellFwd.Controls.Add(btnFwd);
                        row.Cells.Add(cellFwd);

                        TableCell cellRtn = new TableCell();
                        HtmlInputCheckBox chkRtn = new HtmlInputCheckBox();
                        chkRtn.ID = ctrl.sReturnID;
                        cellRtn.Controls.Add(chkRtn);
                        row.Cells.Add(cellRtn);

                        TableCell cellRtnLabel = new TableCell();
                        cellRtnLabel.Text = "&nbsp;Return this";
                        row.Cells.Add(cellRtnLabel);

                        TableMultiDPCtrl.Rows.Add(row);
                    }
                }
            }
        }

        /// <summary>
        /// Update the welcome message displayed below the toolbar, depending on searching success
        /// </summary>
        /// <param name="eRoute"></param>
        private void SetWelcomeMessage(Constants.Routes eRoute)
        {
            // Set the welcome message depending on how we got here (the route)
            switch (eRoute)
            {
                case Constants.Routes.Okay:
                    LabelPrompt.Text = "Please confirm the address";
                    break;
                default:
                    LabelPrompt.Text = "Address capture is not available &#8211; please enter the address below";
                    break;
            }
        }

        /// <summary>
        /// Update the warning message displayed in the status bar, depending on preceding step-in
        /// </summary>
        /// <param name="eWarn"></param>
        private void SetWarningMessage(StepinWarnings eWarn)
        {
            // Set the step-in message depending on the warning
            switch (eWarn)
            {
                case StepinWarnings.CloseMatches:
                    infoStatus.InnerHtml = "There are also close matches available &#8211; click <a href=\"javascript:stepBack();\">back</a> to see them";
                    statusData.Attributes["class"] += " message";
                    break;
                case StepinWarnings.CrossBorder:
                    infoStatus.InnerHtml = "Address selected is outside of the entered locality";
                    statusData.Attributes["class"] += " warning";
                    break;
                case StepinWarnings.ForceAccept:
                    infoStatus.InnerHtml = "Address not verified";
                    statusData.Attributes["class"] += " alert";
                    break;
                case StepinWarnings.Overflow:
                    infoStatus.InnerHtml = "Address has overflowed the layout &#8211; elements lost";
                    statusData.Attributes["class"] += " alert";
                    break;
                case StepinWarnings.PostcodeRecode:
                    infoStatus.InnerHtml = "Postal code has been updated by the Postal Authority";
                    statusData.Attributes["class"] += " warning";
                    break;
                case StepinWarnings.Truncate:
                    infoStatus.InnerHtml = "Address elements have been truncated";
                    statusData.Attributes["class"] += " alert";
                    break;
                default:
                    break;
            }
        }

        private void ResetWarningMessage()
        {
            // Clear any warning info
            infoStatus.InnerHtml = "&nbsp;";
            statusData.Attributes["Class"] = "status";
            StoredWarning = StepinWarnings.None;
        }

        /// <summary>
        /// Show the panel and populate if appropriate
        /// </summary>
        private void SetErrorMessage()
        {
            // Make the panel visible as appropriate
            PlaceholderInfo.Visible = !StoredRoute.Equals(Constants.Routes.Okay);
            // Update the content
            LiteralRoute.Text = HttpUtility.HtmlEncode(StoredRoute.ToString());
            if (StoredErrorInfo != null)
            {
                LiteralError.Text = HttpUtility.HtmlEncode("<br />" + StoredErrorInfo);
            }
        }


        /** Page events **/


        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        /// <summary>
        /// 'New' button clicked: transfer to the searching page, new (blank) search
        /// </summary>
        protected void ButtonNew_ServerClick(object sender, System.EventArgs e)
        {
            NewSearch();
        }

        /// <summary>
        /// 'Back' button clicked: transfer to the searching page, display last picklist shown
        /// </summary>
        protected void ButtonBack_ServerClick(object sender, System.EventArgs e)
        {
            GoSearchPage(DataID, SearchEngine);
        }

        /// <summary>
        /// Search engine changed: transfer to the searching page, fresh search (retain initial search string)
        /// </summary>
        protected void RadioEngine_Changed(object sender, System.EventArgs e)
        {
            FreshSearch();
        }

        /// <summary>
        /// Country database changed: transfer to the searching page, fresh search (retain initial search string)
        /// </summary>
        protected void Country_Changed(object sender, System.EventArgs e)
        {
            FreshSearch();
        }


        /** Page controls **/


        /// Name of Javascript function to call on completion
        protected string CallBackFunction
        {
            get
            {
                return StoredCallback;
            }
        }

        /// Country data identifier (i.e. AUS)
        protected string DataID
        {
            
            get
            {
                if ( m_sDataID == null )
                {
                    return "";
                }
                
                return m_sDataID;
            }
            set
            {
                if ( value == null )
                {
                    m_sDataID = "";
                }
                else
                {
                    m_sDataID = value;
                }
            }
        }

        /// Search engine selected
        private Engine.EngineTypes SearchEngine
        {
            get
            {
                if (RadioSingleline.Checked) { return Engine.EngineTypes.Singleline; }
                if (RadioTypedown.Checked) { return Engine.EngineTypes.Typedown; }
                if (RadioKeyfinder.Checked) { return Engine.EngineTypes.Keyfinder; }

                return Engine.EngineTypes.Singleline;

            }
            set
            {
                RadioSingleline.Checked = (value == Engine.EngineTypes.Singleline);
                RadioTypedown.Checked = (value == Engine.EngineTypes.Typedown);
                RadioKeyfinder.Checked = (value == Engine.EngineTypes.Keyfinder);
            }
        }
    }

    // Helper class - Describes a group of multi dataplus items
    public class MultiDataplusDisplayGroup
    {
        public string sGroup;       // Name of the group these items belong to e.g. 'GBRELC'
        public int iLineNum;        // Line of the address these items appear on
        public string sElemID;      // Id of the element which displays the current item
        public string[] asItems;    // The items.
    }

    // Helper class - Describes a multi dataplus control to draw on the page
    public class MultiDataplusControl
    {
        public string sGroup;       // Name of the group this controls e.g. 'GBRGAS'
        public string sFwdID;       // Id of the element used as the 'increment' button
        public string sBackID;      // Id of the element used as the 'decrement' button
        public string sReturnID;    // Id of the checkbox used to determine whether to 'return this'
        public string sIndexID;     // Id of the element displaying the current position
        public bool bRender;        // Whether to draw this control
    }
}
