/// QAS Pro Web integration code
/// (c) Experian, www.edq.com
namespace Experian.Qas.Prowebintegration
{
    using System;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Key Search on the Web" - flattened picklists
    /// Search on the input lines and display a picklist of results, then take them to the final address page,
    /// or refinement page if required. Will skip past this page if there's only one match.
    /// This page is based on FlatBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class KeySearch : KeyBasePage
    {
        // Name of field in private data form
        protected const string FIELD_MUST_REFINE = "MustRefine";
        // Member
        protected Picklist m_Picklist = null;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                DoSearch();
            }
            // Else leave it to the event handlers (Next, Back)
        }



        /** Search operations **/


        /// <summary>
        /// Perform search using DataID and InputLines
        /// </summary>
        private void DoSearch()
        {
            Constants.Routes eRoute = Constants.Routes.Undefined;

            // If the picklist moniker is not present then we haven't been here before, and can auto-skip forwards
            bool bAutoSkip = (GetPicklistMoniker() == null);

            try
            {
                IAddressLookup addressLookup = QAS.GetSearchService();
                addressLookup.CurrentEngine.EngineType = Engine.EngineTypes.Keyfinder;
                addressLookup.CurrentEngine.Flatten = true;

                if (bAutoSkip)
                {
                    m_Picklist = addressLookup.Search(GetDataID(), GetInputLines(), GetPromptSet(), string.Empty).Picklist;
                    SetPicklistMoniker(m_Picklist.Moniker);
                }
                else
                {
                    // Recreate picklist from moniker
                    m_Picklist = addressLookup.StepIn(GetPicklistMoniker(), GetLayout());
                }

                // Handle 'failure' cases
                if (m_Picklist.IsTimeout)
                {
                    eRoute = Constants.Routes.Timeout;
                }
                else if (m_Picklist.IsMaxMatches)
                {
                    eRoute = Constants.Routes.TooManyMatches;
                }
                else if (m_Picklist.PicklistItems == null || m_Picklist.Total == 0)
                {
                    eRoute = Constants.Routes.NoMatches;
                }
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            if (!eRoute.Equals(Constants.Routes.Undefined))
            {
                GoErrorPage(eRoute);
            }
            else if (bAutoSkip)
            {
                if (m_Picklist.IsAutoFormatSafe || m_Picklist.IsAutoFormatPastClose)
                {
                    // Auto-step past picklist to format page
                    GoFormatPage(m_Picklist.PicklistItems[0].Moniker);
                }
            }
            // Else let page render the picklist itself
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
        /// 'Back' button clicked: return to the search terms Input page
        /// </summary>
        protected void ButtonBack_ServerClick(object sender, System.EventArgs e)
        {
            GoInputPage();
        }

        /// <summary>
        /// 'Next' button clicked: go forward to the refinement page if required, or format address page
        /// </summary>
        protected void ButtonNext_ServerClick(object sender, System.EventArgs e)
        {
            GoFormatPage(null);					// Moniker already set on page
        }
    }
}

