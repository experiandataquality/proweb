/// QAS Pro Web integration code
/// (c) Experian, www.edq.com
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI.WebControls;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Address Capture on the Intranet" - hierarchical picklists
    /// Ask the user for the country and search terms, then take them to the search page
    /// This page is based on HierBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class HierInput : HierBasePage
    {

        protected IList<Dataset> m_atDatasets;
        protected string m_asError;

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Page_BaseLoad(sender, e);

            // Pre-set event to call when <Enter> is hit (otherwise no event will be raised)
            ClientScript.RegisterHiddenField("__EVENTTARGET", ButtonSearch.ClientID);

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
            
            PopulateDatamaps();

            DataIDOnPage = Request.Form["country"];
                      
            if (!IsPostBack)
            {
                // Populate page with values in transit from other pages
                DataIDOnPage = StoredPage.StoredDataID;
                UserInputOnPage = StoredPage.StoredUserInput;
            }
            // Else leave it to the event handler (Search)
        }

        protected void PopulateDatamaps()
        {
            // Populate drop down list of countries
            if (m_atDatasets != null)
            {
                ListItem itemheader1 = new ListItem("-- Datamaps available --", "");
                itemheader1.Attributes["class"] = "heading";
                country.Items.Add(itemheader1);
                
                foreach (Dataset dset in m_atDatasets)
                {
                    ListItem litem = new ListItem(dset.Name, dset.ID);
                    country.Items.Add(litem);
                }

                ListItem itemheader2 = new ListItem("-- Other --", "");
                itemheader2.Attributes["class"] = "heading";
                country.Items.Add(itemheader2);
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
                
                if ( !bDuplicate )
                {
                    ListItem litem = new ListItem( dset.Name, dset.ID );
                    country.Items.Add( litem );
                }
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
        /// 'Search' button clicked: go to the Search page if country 'available', otherwise go manual
        /// </summary>
        protected void ButtonSearch_Click(object sender, System.EventArgs e)
        {
            // Store page values for transit to next page
            StoredDataID = DataIDOnPage;
            StoredCountryName = CountryNameOnPage;
            StoredUserInput = UserInputOnPage;

            // Transfer to next page depending on country selection
            if (!StoredDataID.Equals(""))
            {
                GoSearchPage();
            }
            else
            {
                GoErrorPage(Constants.Routes.UnsupportedCountry);
            }
        }


        /** Page controls **/


        private string DataIDOnPage
        {
            get
            {
                return country.SelectedItem.Value;
            }
            set
            {
                ListItem item = country.Items.FindByValue(value);
                if (item != null)
                {
                    country.SelectedIndex = country.Items.IndexOf(item);
                }
            }
        }

        private string CountryNameOnPage
        {
            get
            {
                return country.SelectedItem.Text;
            }
        }

        private string UserInputOnPage
        {
            get
            {
                return TextBoxSearch.Text;
            }
            set
            {
                TextBoxSearch.Text = value;
            }
        }
    }
}
