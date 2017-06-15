/// QAS Pro Web integration code
/// (c) Experian, www.edq.com
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections.Generic;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Address Capture on the Web" - flattened picklists
    /// Prompt the user for specific address terms, then take them to the picklist results page
    /// This page is based on FlatBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class FlatPrompt : FlatBasePage
    {
        protected List<PromptLine> m_atPromptLines;


        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack)
            {
                // Populate hidden field
                PagePromptSet = GetPromptSet();
                // Get the search prompts for the page
                RetrieveSearchPrompts();
            }
            // Else leave it to the event handlers (Next, Back, Alternate Prompts)
        }


        /** Operations **/


        /// <summary>
        /// Check if searching is available for this country/layout then retrieve the appropriate prompts
        /// </summary>
        void RetrieveSearchPrompts()
        {
            bool bPreSearchFailed = false;
            CanSearch canSearch = null;

            try
            {
                IAddressLookup addressLookup = QAS.GetSearchService(); 
                addressLookup.CurrentEngine.EngineType = Engine.EngineTypes.Singleline;
                addressLookup.CurrentEngine.Flatten = true;

                canSearch = addressLookup.CanSearch(GetDataID(), GetLayout(), PagePromptSet);

                // Is automatic address capture available for this country & layout
                if (IsAlternate() || canSearch.IsOk)
                {
                    m_atPromptLines = addressLookup.GetPromptSet(GetDataID(), PagePromptSet).Lines;
                }
                else
                {
                    // QAS is not available for this country
                    bPreSearchFailed = true;
                }
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            if (bPreSearchFailed)
            {
                GoErrorPage(Constants.Routes.PreSearchFailed, canSearch.ErrorMessage);
            }
            else
            {
                HyperlinkAlternate.Visible = !IsAlternate();
                // And let page render prompts
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
        /// Hyperlink for alternate prompt set clicked: update and redisplay
        /// </summary>
        protected void HyperlinkAlternate_Click(object sender, System.EventArgs e)
        {
            PagePromptSet = PromptSet.Types.Alternate;
            RetrieveSearchPrompts();
        }

        /// <summary>
        /// 'Back' button clicked: either redisplay with standard prompt set, or go back to the first page
        /// </summary>
        protected void ButtonBack_ServerClick(object sender, System.EventArgs e)
        {
            if (IsAlternate())
            {
                PagePromptSet = PromptSet.Types.Optimal;
                RetrieveSearchPrompts();
            }
            else
            {
                GoFirstPage();
            }
        }

        /// <summary>
        /// 'Next' button clicked: move on to the search page
        /// </summary>
        protected void ButtonNext_ServerClick(object sender, System.EventArgs e)
        {
            SetPromptSet(PagePromptSet);
            GoSearchPage();
        }



        /** Page properties **/


        /// Get/set the value of the Prompt Set field on the form
        private PromptSet.Types PagePromptSet
        {
            get
            {
                string sValue = HiddenPromptSet.Value;
                return (sValue != "")
                    ? (PromptSet.Types) Enum.Parse(typeof(PromptSet.Types), sValue)
                    : PromptSet.Types.Optimal;
            }
            set
            {
                HiddenPromptSet.Value = value.ToString();
            }
        }

        /// Are we currently using the standard or alternate prompt set?
        private bool IsAlternate()
        {
            return !(PagePromptSet.Equals(PromptSet.Types.Optimal));
        }
    }
}
