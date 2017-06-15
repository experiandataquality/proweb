/// QAS Pro Web integration code
/// (c) Experian, www.edq.com
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Web;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Address Capture on the Intranet" - hierarchical picklists
    /// Perform the search, display the list of results, respond to actions, go to the format address page
    /// Main arrival routes:
    ///   - Initial search: arriving from previous page
    ///   - Recreate search: arriving from next page
    ///   - Refinement: text has been entered on this page and 'Search' clicked
    ///   - Step-in: picklist item (containing a sub-picklist) on this page has been clicked
    ///   - Format: picklist item (containing a final address) on this page has been clicked
    /// This page is based on HierBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class HierSearch : HierBasePage
    {
        // enumerate operations that can be performed on a picklist item
        protected enum Commands
        {
            StepIn,									// Step in to another picklist
            Format,									// Format into final address
            HaltRange,								// User must enter a value within the range shown
            HaltIncomplete,						// User must enter premise details
            None										// Self-explanatory informational - no hyperlink
        };

        // field names
        protected const string FIELD_MONIKER	= "PrivateMoniker";
        protected const string FIELD_PICKTEXT	= "PrivateText";
        protected const string FIELD_POSTCODE	= "PrivatePostcode";
        protected const string FIELD_SCORE		= "PrivateScore";
        protected const string FIELD_WARNING	= "PrivateWarning";

        // data members
        protected HistoryStack m_aHistory = null;
        protected Picklist m_Picklist = null;

        // page controls



        /// <summary>
        /// Initialise history and perform search
        /// </summary>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Page_BaseLoad(sender, e);
            m_aHistory = StoredPage.GetStoredHistory();		// Pick up history

            // Pre-set event to call when <Enter> is hit (otherwise no event will be raised)
            ClientScript.RegisterHiddenField("__EVENTTARGET", ButtonRefine.ClientID);

            if (!IsPostBack)
            {
                // Store values in transit from other pages
                StoredDataID = StoredPage.StoredDataID;
                StoredCountryName = StoredPage.StoredCountryName;
                StoredUserInput = StoredPage.StoredUserInput;

                // If there is any history, recreate the last picklist
                if (!RecreateSearch())
                {
                    // Otherwise perform a new search
                    InitialSearch();
                }
            }
            // Else leave it to the event handlers (New, Back, Refine, Hyperlink commands)
        }

        /// <summary>
        /// Store the history back to the view state prior to rendering
        /// </summary>
        /// <returns></returns>
        protected override object SaveViewState()
        {
            SetStoredHistory(m_aHistory);
            return base.SaveViewState();
        }



        /** Search operations **/


        /// <summary>
        /// Perform initial search using StoredDataID and StoredUserInput
        /// </summary>
        protected void InitialSearch()
        {
            bool bPreSearchFailed = false;	// country is not available for searching
            bool bFinalAddress = false;		// jump straight to the final address page?
            // Remember warnings caused by auto-stepping through picklist items:
            bool bStepPastClose = false;		// have we auto-stepped past close matches?
            bool bCrossBorder = false;			// have we auto-stepped through a cross-border warning?
            bool bPostcodeRecode = false; 	// have we auto-stepped through a postcode recode?

            CanSearch canSearch = null;

            try
            {
                IAddressLookup addressLookup = QAS.GetSearchService();
                addressLookup.CurrentEngine.EngineType = Engine.EngineTypes.Singleline;
                addressLookup.CurrentEngine.Flatten = false;

                canSearch = addressLookup.CanSearch(StoredDataID, GetLayout(), PromptSet.Types.OneLine);

                // Perform a pre-search check, to ensure user can proceed all the way to getting a formatted address
                if (!canSearch.IsOk)
                {
                    bPreSearchFailed = true;
                }
                else
                {
                    // Perform initial search on UserInput in selected country
                    m_Picklist = addressLookup.Search(StoredDataID, StoredUserInput, PromptSet.Types.OneLine, GetLayout()).Picklist;  // throws Exception

                    Moniker = m_Picklist.Moniker;
                    string sDisplay = "Searching on... '" + StoredUserInput + "'";

                    // Initialise the history
                    m_aHistory.Clear();
                    m_aHistory.Push(Moniker, sDisplay, "", "");

                    // Auto-step-in logic
                    while (m_Picklist.IsAutoStepinSafe || m_Picklist.IsAutoStepinPastClose)
                    {
                        Moniker = m_Picklist.PicklistItems[0].Moniker;
                        bStepPastClose  = bStepPastClose  || m_Picklist.IsAutoStepinPastClose;
                        bCrossBorder    = bCrossBorder	 || m_Picklist.PicklistItems[0].IsCrossBorderMatch;
                        bPostcodeRecode = bPostcodeRecode || m_Picklist.PicklistItems[0].IsPostcodeRecoded;
                        // Add this step-in to history
                        m_aHistory.Push(m_Picklist.PicklistItems[0]);
                        // Auto-step into first item
                        m_Picklist = addressLookup.StepIn(Moniker, GetLayout());  // throws Exception
                    }

                    // Auto-formatting logic
                    if (m_Picklist.IsAutoFormatSafe || m_Picklist.IsAutoFormatPastClose)
                    {
                        Moniker = m_Picklist.PicklistItems[0].Moniker;
                        bStepPastClose  = bStepPastClose  || m_Picklist.IsAutoFormatPastClose;
                        bCrossBorder = bCrossBorder || m_Picklist.PicklistItems[0].IsCrossBorderMatch;
                        bPostcodeRecode = bPostcodeRecode || m_Picklist.PicklistItems[0].IsPostcodeRecoded;
                        // Go straight to formatted address page
                        bFinalAddress = true;
                    }
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

            // Convert flags into a single step-in warning enumeration
            StepinWarnings eWarn = AsWarning(bStepPastClose, bCrossBorder, bPostcodeRecode);

            if (bFinalAddress)
            {
                // Store the history back to the view state
                StoredPage.SetStoredHistory(m_aHistory);
                // Display final formatted address
                GoFormatPage(Moniker, eWarn);
            }
            else
            {
                // Display results picklist (rendered by page)
                Prompt = m_Picklist.Prompt;
                SetWarningMessage(eWarn);
            }
        }

        /// <summary>
        /// Perform refinement search on Moniker and RefineText
        /// </summary>
        protected void RefinementSearch()
        {
            bool bFinalAddress = false;		// jump straight to the final address page?
            // Remember warnings caused by auto-stepping through picklist items
            bool bCrossBorder = false;			// have we auto-stepped through a cross-border warning?
            bool bPostcodeRecode = false; 	// have we auto-stepped through a postcode recode?

            try
            {
                IAddressLookup addressLookup = QAS.GetSearchService();

                // Perform refinement search using Moniker and RefineText
                // Add space to ensure we only get perfect matches
                m_Picklist = addressLookup.Refine(Moniker, RefineText + " ", GetLayout());	// throws Exception

                // Auto-step-in logic
                if (m_Picklist.IsAutoStepinSafe)
                {
                    Moniker = m_Picklist.PicklistItems[0].Moniker;
                    bCrossBorder = bCrossBorder || m_Picklist.PicklistItems[0].IsCrossBorderMatch;
                    bPostcodeRecode = bPostcodeRecode || m_Picklist.PicklistItems[0].IsPostcodeRecoded;
                    RefineText = "";  // no longer refining
                    // Add this step-in to history
                    m_aHistory.Push(m_Picklist.PicklistItems[0]);
                    // Auto-step into first item
                    m_Picklist = addressLookup.StepIn(Moniker, GetLayout());	// throws Exception
                }
                // Auto-format logic - if a solo address item
                else if (m_Picklist.IsAutoFormatSafe)
                {
                    Moniker = m_Picklist.PicklistItems[0].Moniker;
                    bCrossBorder = bCrossBorder || m_Picklist.PicklistItems[0].IsCrossBorderMatch;
                    bPostcodeRecode = bPostcodeRecode || m_Picklist.PicklistItems[0].IsPostcodeRecoded;
                    bFinalAddress = true;
                }
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            // Convert flags into a single step-in warning enumeration
            StepinWarnings eWarn = AsWarning(false, bCrossBorder, bPostcodeRecode);

            if (bFinalAddress)
            {
                // Display final formatted address
                GoFormatPage(Moniker, eWarn);
            }
            else
            {
                // Display results picklist (rendered by page)
                Prompt = m_Picklist.Prompt;
                SetWarningMessage(eWarn);
            }
        }

        /// <summary>
        /// Perform stepin search / recreate picklist
        /// </summary>
        protected void StepinSearch()
        {
            try
            {
                IAddressLookup addressLookup = QAS.GetSearchService();

                // Perform search: step-in to Moniker
                m_Picklist = addressLookup.StepIn(Moniker, GetLayout());  // throws Exception

                // Display results picklist (rendered by page)
                Prompt = m_Picklist.Prompt;
                // Clear refinement text
                RefineText = "";
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }
        }

        /// <summary>
        /// Perform a picklist recreation from the top of the history; or return false if empty
        /// </summary>
        /// <returns>Whether a recreate is possible for the current history state</returns>
        protected bool RecreateSearch()
        {
            if (m_aHistory.Count == 0)
            {
                return false;
            }
            Moniker = m_aHistory.Peek().Moniker;
            StepinSearch();
            return true;
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
        /// 'New' button clicked: go to the first page
        /// </summary>
        protected void ButtonNew_ServerClick(object sender, System.EventArgs e)
        {
            // Clear the search terms (but retain the country selection)
            StoredUserInput = "";
            GoFirstPage();
        }

        /// <summary>
        /// 'Back' button clicked: trim the history, recreate last picklist or go to the first page
        /// </summary>
        protected void ButtonBack_ServerClick(object sender, System.EventArgs e)
        {
            m_aHistory.Pop();
            // If there is any history, recreate the last picklist
            if (!RecreateSearch())
            {
                // Otherwise go back to the first page
                GoFirstPage();
            }
        }

        /// <summary>
        /// 'Search' button clicked: generate a picklist using the refinement text
        /// </summary>
        protected void ButtonRefine_Click(object sender, System.EventArgs e)
        {
            RefinementSearch();
        }

        /// <summary>
        /// Pick up fields and format the selected picklist item
        /// </summary>
        protected void ButtonFormat_Click(object sender, System.EventArgs e)
        {
            // Transfer to the formatting page
            GoFormatPage(Moniker, StepinWarning);
        }

        /// <summary>
        /// Pick up fields and step in to the selected picklist item
        /// </summary>
        protected void ButtonStepIn_Click(object sender, System.EventArgs e)
        {
            // Add this step-in to history
            m_aHistory.Push(Moniker, PickText, PickPostcode, PickScore);

            // Display warnings from item selected
            SetWarningMessage(StepinWarning);

            // Perform step-in
            StepinSearch();
        }



        /** Page updating **/


        /// <summary>
        /// Write the picklist history as table rows, indenting as you go
        /// </summary>
        protected void RenderHistory()
        {
            for (int i=0; i < m_aHistory.Count; ++i)
            {
                HistoryItem item = m_aHistory[i];
                RenderRow(i, HttpUtility.HtmlEncode(item.Text), item.Postcode, item.Score);
            }
        }

        /// <summary>
        /// Write the picklist items as table rows, with hyperlink action and icon as appropriate
        /// </summary>
        protected void RenderPicklist()
        {
            int iIndent = m_aHistory.Count - 1;
            int index = 0;
            foreach (PicklistItem item in m_Picklist.PicklistItems)
            {
                // Build hyperlink action string
                string sHyperlinkStart = "", sHyperlinkEnd = "";
                string sFunction = PicklistCommand(item);
                string sClass = item.IsSubsidiaryData ? "sub" : "pre";

                if (sFunction != null)
                {
                    sHyperlinkStart = String.Format("<a href=\"javascript:{0}('{1}');\" class=\"{2}\">", sFunction, index, sClass);
                    sHyperlinkEnd = "</a>";
                }

                // Build main picklist string: icon, hyperlink, picklist text
                string sPicktext = String.Format("{0}<img class=\"icon\" src=\"{2}\" alt=\"{3}\">{1}{0}{4}{1}",
                    sHyperlinkStart,
                    sHyperlinkEnd,
                    PicklistIcon(item),
                    HttpUtility.HtmlEncode(item.PartialAddress),
                    HttpUtility.HtmlEncode(item.Text));

                RenderRow(iIndent, sPicktext, item.Postcode, item.ScoreAsString);

                index++;
            }
        }

        /// <summary>
        /// Write a table row, with indent, main text element, postcode and score columns
        /// </summary>
        /// <param name="iIndent">Indent level, converted into non-breaking spaces</param>
        /// <param name="sBody">Main body element</param>
        /// <param name="sPostcode">Postcode column contents</param>
        /// <param name="sScore">Score column contents</param>
        protected void RenderRow(int iIndent, string sBody, string sPostcode, string sScore)
        {
         Response.Write("<tr>\n");
         Response.Write("<td class=\"pickle\">");
            while (iIndent-- > 0)
            {
                Response.Write("&nbsp;&nbsp;&nbsp;");
            }
            Response.Write(sBody);
         Response.Write("</td>\n");
         Response.Write("<td width=\"20\"></td>\n");
         Response.Write("<td nowrap>");
         Response.Write(HttpUtility.HtmlEncode(sPostcode));
         Response.Write("</td>");
         Response.Write("<td width=\"15\"></td>");
         Response.Write("<td>");
         Response.Write(sScore);
         Response.Write("</td>\n");
         Response.Write("</tr>\n");
        }

        /// <summary>
        /// Write additional picklist information as a series of hidden fields
        /// When a picklist item is selected, client-side JavaScript will pick up the appropriate values
        /// and send them back to the server in hidden fields in the main form
        /// </summary>
        protected void RenderPrivateData()
        {
            foreach (PicklistItem item in m_Picklist.PicklistItems)
            {
                RenderHiddenField(FIELD_MONIKER, item.Moniker);
                RenderHiddenField(FIELD_PICKTEXT, item.Text);
                RenderHiddenField(FIELD_POSTCODE, item.Postcode);
                RenderHiddenField(FIELD_SCORE, item.ScoreAsString);
                RenderHiddenField(FIELD_WARNING, AsWarning(false, item.IsCrossBorderMatch, item.IsPostcodeRecoded).ToString());
            }
        }

        /// <summary>
        /// Write a single hidden field to the page
        /// </summary>
        protected void RenderHiddenField(string sKey, string sValue)
        {
            Response.Write("<input type=\"hidden\" name=\"");
            Response.Write(sKey);
            if (sValue != null && sValue != "")
            {
                Response.Write("\" value=\"");
                Response.Write(HttpUtility.HtmlEncode(sValue));
            }
            Response.Write("\" />\n");
        }

        /// <summary>
        /// Return the command appropriate to this picklist item
        /// </summary>
        protected static string PicklistCommand(PicklistItem item)
        {
            if (item.CanStep)
            {
                return Commands.StepIn.ToString();
            }
            else if (item.IsFullAddress)
            {
                return Commands.Format.ToString();
            }
            else if (item.IsIncompleteAddress)
            {
                return Commands.HaltIncomplete.ToString();
            }
            else if (item.IsUnresolvableRange)
            {
                return Commands.HaltRange.ToString();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Return the icon filename appropriate for this picklist item
        /// </summary>
        protected static string PicklistIcon(PicklistItem item)
        {
            if (item.CanStep)
            {
                if (item.IsInformation)
                {
                    return "img/iInfoStep.gif";
                }
                else if (item.IsAliasMatch)
                {
                    return "img/iAliasStep.gif";
                }
                else
                {
                    return "img/iPicklist.gif";
                }
            }
            else if (item.IsInformation)
            {
                return "img/iInfo.gif";
            }
            else if (item.IsFullAddress)
            {
                if (item.IsAliasMatch)
                {
                    return "img/iAlias.gif";
                }
                else
                {
                    return "img/iFinal.gif";
                }
            }
            else
            {
                return "img/iNogo.gif";
            }
        }

        /// <summary>
        /// Return the warning enumeration based on properties of the picklist item stepped through
        /// </summary>
        /// <param name="bStepPastClose">Have we stepped past close matches</param>
        /// <param name="bCrossBorder">Is this item a match in a different locality to the entered search</param>
        /// <param name="bPostcodeRecode">Is this item's postcode a recode of the entered search</param>
        protected static StepinWarnings AsWarning(bool bStepPastClose, bool bCrossBorder, bool bPostcodeRecode)
        {
            if (bStepPastClose)
            {
                return StepinWarnings.CloseMatches;
            }
            else if (bCrossBorder)
            {
                return StepinWarnings.CrossBorder;
            }
            else if (bPostcodeRecode)
            {
                return StepinWarnings.PostcodeRecode;
            }
            else
            {
                return StepinWarnings.None;
            }
        }



        /** Page controls **/


        private string Moniker
        {
            get
            {
                return HiddenMoniker.Value;
            }
            set
            {
                HiddenMoniker.Value = value;
            }
        }
        private string PickPostcode
        {
            get
            {
                return HiddenPostcode.Value;
            }
        }
        private string PickScore
        {
            get
            {
                return HiddenScore.Value;
            }
        }
        private string PickText
        {
            get
            {
                return HiddenText.Value;
            }
        }
        private string Prompt
        {
            set
            {
                LabelPrompt.InnerHtml = value;
            }
        }
        private string RefineText
        {
            get
            {
                return TextBoxRefine.Text;
            }
            set
            {
                TextBoxRefine.Text = value;
            }
        }
        private StepinWarnings StepinWarning
        {
            get
            {
                string sValue = HiddenWarning.Value;
                return (sValue != null)
                    ? (StepinWarnings) Enum.Parse(typeof(StepinWarnings), sValue)
                    : StepinWarnings.None;
            }
        }
        private void SetWarningMessage(StepinWarnings eWarn)
        {
            // Make the panel visible as appropriate
            PlaceHolderWarning.Visible = (eWarn != StepinWarnings.None);

            // Set the step-in message depending on the warning
            switch (eWarn)
            {
                case StepinWarnings.CloseMatches:
                    LiteralWarning.Text = "There are also close matches available &#8211; click <a href=\"javascript:goBack();\">back</a> to see them";
                    break;
                case StepinWarnings.CrossBorder:
                    LiteralWarning.Text = "Address selected is outside of the entered locality";
                    break;
                case StepinWarnings.PostcodeRecode:
                    LiteralWarning.Text = "Postal code has been updated by the Postal Authority";
                    break;
                default:
                    break;
            }
        }
    }
}
