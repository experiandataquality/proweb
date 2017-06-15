/// QAS Pro Web integration code
/// (c) Experian, www.edq.com
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Web;
    using System.Web.SessionState;
    using System.Web.UI;
    using System.Web.UI.WebControls;
    using System.Web.UI.HtmlControls;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Key Search on the Web" - flattened picklists
    /// Retrieve the final formatted address, display to user for confirmation
    /// This page is based on FlatBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class KeyAddress : KeyBasePage
    {
        // Result arrays
        protected string[] m_asAddressLines = null;
        protected string[] m_asAddressLabels = null;
        // Route (how we got here, search status); keep a local copy, for efficiency and so we can change it
        protected Constants.Routes m_eRoute = Constants.Routes.Undefined;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            SetRoute(base.GetRoute());
            if (!IsPostBack)
            {
                FormatAddress(ref m_asAddressLabels, ref m_asAddressLines);
                SetWelcomeMessage(GetRoute());
            }
            // Else leave it to the event handlers (Accept, Back)
        }



        /** Operations **/


        /// <summary>
        /// Retrieve the formatted address from the Moniker, or create a set of blank lines
        /// </summary>
        protected void FormatAddress(ref string[] asLabels, ref string[] asLines)
        {
            if (!(GetRoute().Equals(Constants.Routes.PreSearchFailed) || GetRoute().Equals(Constants.Routes.Failed)))
            {
                try
                {
                    IAddressLookup addressLookup = QAS.GetSearchService();

                    List<AddressLine> lines;
                    if (GetRoute().Equals(Constants.Routes.Okay))
                    {
                        // Perform address formatting
                        lines = addressLookup.GetFormattedAddress(GetMoniker(), GetLayout()).AddressLines;
                    }
                    else
                    {
                        // Use first example address to get line labels
                        lines = addressLookup.GetExampleAddresses(GetDataID(), GetLayout())[0].AddressLines;
                    }

                    // Build display address arrays
                    int iSize = lines.Count;
                    asLabels = new string[iSize];
                    asLines = new String[iSize];
                    for (int i = 0; i < iSize; i++)
                    {
                        asLabels[i] = lines[i].Label;
                        asLines[i] = lines[i].Line;
                    }
                }
                catch (Exception x)
                {
                    SetRoute(Constants.Routes.Failed);
                    SetErrorInfo(x.Message);
                }
            }

            if (asLabels == null || asLines == null)
            {
                // Provide default (empty) address for manual entry
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
        /// 'Back' button clicked
        /// </summary>
        protected void ButtonBack_ServerClick(object sender, System.EventArgs e)
        {
            switch (GetRoute())
            {
                case Constants.Routes.NoMatches:
                case Constants.Routes.Timeout:
                case Constants.Routes.TooManyMatches:
                    GoInputPage();
                    break;
                case Constants.Routes.Okay:
                    GoSearchPage();
                    break;
                default:
                    GoFirstPage();
                    break;
            }
        }

        /// <summary>
        /// 'Accept' button clicked: move out of this scenario
        /// </summary>
        protected void ButtonAccept_ServerClick(object sender, System.EventArgs e)
        {
            GoFinalPage();
        }



        /** Page controls **/


        /// <summary>
        /// Update the page welcome depending on the route we took to get here
        /// </summary>
        private void SetWelcomeMessage(Constants.Routes eRoute)
        {
            switch (eRoute)
            {
                case Constants.Routes.Okay:
                    LiteralMessage.Text = "Please confirm your address below.";
                    break;
                case Constants.Routes.NoMatches:
                case Constants.Routes.Timeout:
                case Constants.Routes.TooManyMatches:
                    LiteralMessage.Text = "Automatic key search did not succeed.<br /><br />Please search again or enter your address below.";
                    break;
                default:
                    LiteralMessage.Text = "Automatic key search is not available.<br /><br />Please enter your address below.";
                    break;
            }
        }

        /// Current search state, how we arrived on the address format page (i.e. too many matches)
        protected new Constants.Routes GetRoute()
        {
            return m_eRoute;
        }
        protected void SetRoute(Constants.Routes eRoute)
        {
            m_eRoute = eRoute;
        }
    }
}
