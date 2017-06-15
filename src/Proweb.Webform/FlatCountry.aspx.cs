/// QAS Pro Web integration code
/// (c) Experian, www.edq.com
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections.Generic;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Address Capture on the Web" - flattened picklists
    /// Ask the user for the country, then take them to the address input page
    /// This page is based on FlatBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class FlatCountry : FlatBasePage
    {
        // Array of datasets available on server
        protected IList<Dataset> m_atDatasets;
        protected string m_asError;

        // Maximum size of a datamap name to display in drop down list
        protected const int MAX_DATAMAP_NAME_SIZE = 32;
        
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Load a list of all datamaps available on the server
            // Silently fail if no connection can be made - will fall back to manual address
            // input.
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

        protected void ButtonNext_ServerClick(object sender, System.EventArgs e)
        {
            GoInputPage();
        }
    }
}
