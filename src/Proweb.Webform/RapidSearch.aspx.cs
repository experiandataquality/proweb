/// QAS Pro Web > (c) Experian > www.edq.com
/// Intranet > Rapid Addressing > Standard > RapidSearch
/// Main searching dialog, perform searching commands
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Web;
    using System.Web.UI.WebControls;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Intranet > Rapid Addressing > Standard > Full QAS searching with hierarchical picklists
    /// Perform searching, display the list of results, respond to actions, go to the format address page
    ///
    /// Main actions:
    ///   - Initialise: arriving from calling window
    ///   - Recreate: arriving from next (formatted address) page
    ///   - New search, fresh search, step back: caused by click/change to page controls
    ///   - Main search: text has been submitted for a non-dynamic search
    ///   - Step in: picklist item containing a sub-picklist has been clicked
    ///   - Format: picklist item containing a final address has been clicked
    ///
    /// This page is based on RapidBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class RapidSearch : RapidBasePage
    {
        /** Page members **/


        // Current picklist, to display
        protected Picklist m_Picklist = null;
        // Are we at the initial search stage?
        private bool m_bInitial;
        // Is searching dynamic (responds to individual key presses)
        private bool m_bDynamic = true;
        // Current Data ID
        private string m_sDataID = "";
        // Page controls


        /** Methods **/


        /// <summary>
        /// Pick up values transfered from other pages
        /// </summary>
        override protected void Page_Load(object sender, System.EventArgs e)
        {
            base.Page_Load(sender, e);

            DataID = Request.Form[Constants.FIELD_DATA_ID];

            if ( DataID == "" )
            {
                DataID = StoredDataID;
            }
            else
            {
                StoredDataID = DataID;
            }

            // Load datasets from server
            if ( StoredDataMapList == null )
            {
                try
                {
                    m_atDatasets = AddressLookup.GetAllDatasets();
                    StoredDataMapList = m_atDatasets;
                }
                catch( Exception x )
                {
                    GoErrorPage(x);
                }
            }
            else
            {
                m_atDatasets = StoredDataMapList;
            }
            
            PopulateDatasets();

            if ( country.Items.Count > 0 && m_sDataID.Length > 0 )
            {
                country.SelectedValue = m_sDataID;
            }

            m_bInitial = (m_aHistory.Count == 0);

            if (!IsPostBack)
            {
                // Pick up values from outer/other pages
                StoredCallback = Request[FIELD_CALLBACK];
                SearchEngine = StoredSearchEngine;

                StepBackRecreate();
            }

            

            // Else leave it to the event handlers (New, Back, Change engine/country, Hyperlink commands)
        }

        protected void PopulateDatasets()
        {
            // Populate drop down list of countries
            country.Items.Clear();

            country.Attributes.CssStyle["width"] = SELECT_WIDTH;

            ListItem itemheader1 = new ListItem("-- Datamaps Available --", "");
            itemheader1.Attributes["class"] = "heading";
            country.Items.Add(itemheader1);

            string sDatamapName;

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

            ListItem itemheader2 = new ListItem("-- Other --", "");
            itemheader2.Attributes["class"] = "heading";
            country.Items.Add(itemheader2);

            foreach (Dataset dset in Constants.CoreCountries)
            {
                bool bDuplicate = false;

                foreach (Dataset serverDset in m_atDatasets)
                {
                    if (serverDset.Name == dset.Name || serverDset.ID == dset.ID)
                    {
                        bDuplicate = true;
                        break;
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

            if (DataID.Length > 0)
            {
                country.SelectedValue = m_sDataID;
            }
            else
            {
                country.SelectedIndex = 1;
                DataID = country.SelectedValue;
            }
        }

        /// <summary>
        /// Update page controls prior to drawing
        /// </summary>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            ButtonBack.Disabled = (IsInitialSearch);

            // Submit button is titled 'Select' except for non-dynamic page ('Search')
            SearchButton = (IsSearchDynamic) ? "Select" : "Search";

            /*
            // Prevent clicking on current engine from firing an event
            if ( RadioTypedown.Checked )
            {
                RadioTypedown.Attributes["onclick"] = "return false;";
            }
            if ( RadioSingleline.Checked )
            {
                RadioSingleline.Attributes["onclick"] = "return false;";
            }
            if ( RadioKeyfinder.Checked )
            {
                RadioKeyfinder.Attributes["onclick"] = "return false;";
            }
            */

            if (m_Picklist != null)
            {
                TotalMatches = m_Picklist.Total;
                Moniker = m_Picklist.Moniker;
                Prompt = m_Picklist.Prompt;
            }

            
        }

        /// <summary>
        /// Start a new search - initialise/blank values
        /// </summary>
        protected void NewSearch()
        {
            SearchString = "";

            ResetWarningMessage();

            m_aHistory.Clear();
            // Start new search indirectly, through StepBackRecreate
            StepBackRecreate();
        }

        /// <summary>
        /// Start a new search, but retrieve the search string from the history top
        /// </summary>
        protected void FreshSearch()
        {
            // Cut down history - to keep just initial search string for new search
            m_aHistory.Truncate(1);

            ResetWarningMessage();

            StepBackRecreate();
        }

        /// <summary>
        /// Grab details from the history and recreate the previous picklist, or start a new one
        /// </summary>
        protected void StepBackRecreate()
        {
            ResetWarningMessage();
            
            if (m_aHistory.Count > 0)
            {
                // Grab search text from the last history entry
                SearchString = m_aHistory.Peek().Refine;
                m_aHistory.Pop();
            }

            if (m_aHistory.Count > 0)
            {
                // Recreate last picklist shown
                Moniker = m_aHistory.Peek().Moniker;
                StepinSearch();
            }
            else
            {
                // Otherwise start a new searc
                InitialSearch();
            }
        }

        /// <summary>
        /// Perform search initialisation - check availability and perform initial dynamic search
        /// </summary>
        protected void InitialSearch()
        {
            CanSearch result = null;

            try
            {
                // Perform a pre-search check, to ensure user can proceed all the way to getting a formatted address
                AddressLookup.CurrentEngine.EngineType = SearchEngine;
                StoredSearchEngine = SearchEngine;
                AddressLookup.CurrentEngine.Flatten = false;

                result = AddressLookup.CanSearch(DataID, GetLayout(), null);

                if (result.IsOk)
                {
                    // Get initial prompt
                    PromptSet prompts = AddressLookup.GetPromptSet(DataID, PromptSet.Types.Default);
                    IsSearchDynamic = prompts.IsDynamic;
                    m_aHistory.Clear();

                    if (IsSearchDynamic)
                    {
                        m_Picklist = AddressLookup.Search(DataID, SearchString, PromptSet.Types.Default, GetLayout()).Picklist;
                    }
                    else
                    {
                        TotalMatches = 0;
                        Moniker = "";
                        string thePrompt = "";
                        foreach (PromptLine line in prompts.Lines)
                        {
                            thePrompt = thePrompt + line.Prompt + " ";
                        }
                        Prompt = thePrompt;
                    }
                }
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            if (!result.IsOk)
            {
                GoErrorPage(Constants.Routes.PreSearchFailed, result.ErrorMessage);
            }
            // else: Display results picklist: done by page
        }

        /// <summary>
        /// Perform a submitted non-dynamic search, handling auto-step in and format
        /// </summary>
        protected void SubmitSearch()
        {
            // Step in all the way to the final address page?
            bool bFinalAddress = false;
            StepinWarnings eWarn = StepinWarnings.None;

            try
            {
                // Perform initial static (Singleline) search
                AddressLookup.CurrentEngine.EngineType = SearchEngine;
                m_Picklist = AddressLookup.Search(DataID, SearchString, PromptSet.Types.Default, GetLayout()).Picklist;

                Moniker = m_Picklist.Moniker;
                // Initialise the history
                m_aHistory.Clear();
                m_aHistory.Push(Moniker, "Searching on... '" + SearchString + "'", "", "", SearchString);
                SearchString = "";

                // Auto-step-in logic
                AutoStep(ref bFinalAddress, ref eWarn);
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            if (bFinalAddress)
            {
                // Display final formatted address
                GoFormatPage(DataID, SearchEngine, Moniker, eWarn);
            }
            // else: Display results picklist: done by page
        }

        /// <summary>
        /// Step in to a picklist item / Recreate picklist
        /// </summary>
        protected void StepinSearch()
        {
            // Step in all the way to the final address page?
            bool bFinalAddress = false;
            StepinWarnings eWarn = StepinWarnings.None;

            try
            {
                // Get picklist: step-in/recreate from Moniker
                m_Picklist = AddressLookup.Refine(Moniker, SearchString, GetLayout());

                // Special case: initial (dynamic searching) step-in can require auto-step-in
                if (m_bInitial)
                {
                    AutoStep(ref bFinalAddress, ref eWarn);
                }
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            if (bFinalAddress)
            {
                // Display final formatted address
                GoFormatPage(DataID, SearchEngine, Moniker, eWarn);
            }
            // else: Display results picklist: done by page
        }


        /** Helper methods **/


        /// <summary>
        /// Automatically step through picklists which do not need to be displayed
        /// </summary>
        /// <param name="bFinalAddress">Step in all the way to the final address page?</param>
        /// <param name="eWarn">Any warnings associated with picklists we've stepped through</param>
        protected void AutoStep(ref bool bFinalAddress, ref StepinWarnings eWarn)
        {
            // Remember warnings caused by auto-stepping through picklist items:
            bool bStepPastClose = false;		// have we auto-stepped past close matches?
            bool bCrossBorder = false;			// have we auto-stepped through a cross-border warning?
            bool bPostcodeRecode = false; 	// have we auto-stepped through a postcode recode?

            try
            {
                // Auto-step-in logic
                while (m_Picklist.IsAutoStepinSafe || m_Picklist.IsAutoStepinPastClose)
                {
                    Moniker = m_Picklist.PicklistItems[0].Moniker;
                    bStepPastClose  = bStepPastClose  || m_Picklist.IsAutoStepinPastClose;
                    bCrossBorder = bCrossBorder || m_Picklist.PicklistItems[0].IsCrossBorderMatch;
                    bPostcodeRecode = bPostcodeRecode || m_Picklist.PicklistItems[0].IsPostcodeRecoded;
                    // Add this step-in to history
                    m_aHistory.Push(m_Picklist.PicklistItems[0]);
                    // Auto-step into first item
                    m_Picklist = AddressLookup.StepIn(Moniker, GetLayout());
                }

                // Auto-formatting logic
                if (m_Picklist.IsAutoFormatSafe || m_Picklist.IsAutoFormatPastClose)
                {
                    Moniker = m_Picklist.PicklistItems[0].Moniker;
                    bStepPastClose  = bStepPastClose  || m_Picklist.IsAutoFormatPastClose;
                    bCrossBorder = bCrossBorder || m_Picklist.PicklistItems[0].IsCrossBorderMatch;
                    bPostcodeRecode = bPostcodeRecode || m_Picklist.PicklistItems[0].IsPostcodeRecoded;
                    // Add this step-in to history
                    m_aHistory.Push(m_Picklist.PicklistItems[0]);
                    // Go straight to formatted address page
                    bFinalAddress = true;
                }
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            // Convert flags into a single step-in warning enumeration
            eWarn = AsWarning(bStepPastClose, bCrossBorder, bPostcodeRecode);
            SetWarningMessage(eWarn);
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

        /// <summary>
        /// Update the warning message displayed in the status bar, depending on preceding step-in
        /// </summary>
        /// <param name="eWarn">Warning enumeration</param>
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
                    statusData.Attributes["class"] += " message";
                    break;
                case StepinWarnings.PostcodeRecode:
                    infoStatus.InnerHtml = "Postal code has been updated by the Postal Authority";
                    statusData.Attributes["class"] += " message";
                    break;
                default:
                    ResetWarningMessage();
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
        /// Write the picklist history as table rows, indenting as you go
        /// </summary>
        protected void RenderHistory()
        {
            if (m_aHistory.Count > 0)
            {
                Response.Write("<table class=\"history picklist\">\n");

                for (int i = 0; i < m_aHistory.Count; i++)
                {
                    string sIndent = "indent" + i.ToString();
                    string sText = Server.HtmlEncode(m_aHistory[i].Text);

                    Response.Write("<tr class=\"picklist\">");
                    Response.Write("<td class=\"pickitem opened " + sIndent + "\"><a href=\"javascript:stepBack();\" tabindex=\"-1\"><div>" + sText + "</div></a></td>");
                    Response.Write("<td class=\"postcode\">" + m_aHistory[i].Postcode + "</td>");
                    Response.Write("<td class=\"score\">" + m_aHistory[i].Score + "</td>");
                    Response.Write("</tr>\n");
                }

                Response.Write("</table>");
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
        /// 'New' button clicked: start a new blank search
        /// </summary>
        protected void ButtonNew_ServerClick(object sender, System.EventArgs e)
        {
            NewSearch();
        }

        /// <summary>
        /// 'Back' button clicked: display last picklist shown
        /// </summary>
        protected void ButtonBack_ServerClick(object sender, System.EventArgs e)
        {
            StepBackRecreate();
        }

        /// <summary>
        /// Search engine changed: start a fresh search (retain initial search string)
        /// </summary>
        protected void RadioEngine_Changed(object sender, System.EventArgs e)
        {            
            FreshSearch();
        }

        /// <summary>
        /// Country database changed: start a fresh search (retain initial search string)
        /// </summary>
        protected void country_SelectedIndexChanged(object sender, System.EventArgs e)
        {   
            FreshSearch();
        }

        /// <summary>
        /// 'Search' button clicked: perform a non-dynamic search
        /// </summary>
        protected void ButtonSearch_ServerClick(object sender, System.EventArgs e)
        {
            SubmitSearch();
        }

        /// <summary>
        /// Final address picklistitem hyperlink hit: format the selected picklist item
        /// </summary>
        protected void ActionFormat_Click(object sender, System.EventArgs e)
        {
            // Add this step-in to history
            m_aHistory.Push(Moniker, PickText, PickPostcode, PickScore, SearchString);

            // Transfer to the formatting page
            GoFormatPage(DataID, SearchEngine, Moniker, StepinWarning);
        }

        /// <summary>
        /// Multiple address picklistitem hyperlink hit: step in to the selected picklist item
        /// </summary>
        protected void ActionStepIn_Click(object sender, System.EventArgs e)
        {
            // Add this step-in to history
            m_aHistory.Push(Moniker, PickText, PickPostcode, PickScore, SearchString);

            // Stepping in, so clear refine text UNLESS it was an informational (i.e. 'Click to Show All')
            if (!StepinWarning.Equals(StepinWarnings.Info))
            {
                SearchString = "";
            }

            // Display warnings from item selected
            SetWarningMessage(StepinWarning);

            // Perform step-in
            StepinSearch();
        }


        /** Page controls **/


        /// Country data identifier (i.e. AUS)
        protected string DataID
        {
            get
            {
                if (m_sDataID == null)
                {
                    return "";
                }

                return m_sDataID;
            }
            set
            {
                if (value == null)
                {
                    m_sDataID = "";
                }
                else
                {
                    m_sDataID = value;
                }
            }
        }

        /// Whether auto-complete should be enabled in the browser
        protected string IsAutoComplete
        {
            get
            {
                return m_bDynamic ? "off" : "on";
            }
        }

        /// Whether this is an initial search (no history yet): affects selection of search terms text box
        protected bool IsInitialSearch
        {
            get
            {
                return (m_aHistory.Count == 0);
            }
        }

        /// Whether we are searching dynamically (updating picklist as you type)
        protected bool IsSearchDynamic
        {
            get
            {
                return m_bDynamic;
            }
            set
            {
                m_bDynamic = value;
            }
        }

        /// Page control: update match count shown in div
        private int TotalMatches
        {
            set
            {
                matchCount.InnerText = (value >= 9999) ? "Too many" : "Matches: " + value.ToString();
            }
        }

        /// Moniker of the picklist item selected (set by browser Javascript)
        protected string Moniker
        {
            get
            {
                return HttpUtility.HtmlDecode(HiddenMoniker.Value);
            }
            set
            {
                HiddenMoniker.Value = value;
            }
        }

        /// Postcode of the picklist item selected (set by browser Javascript)
        private string PickPostcode
        {
            get
            {
                return HttpUtility.HtmlDecode(HiddenPostcode.Value);
            }
        }

        /// Score of the picklist item selected (set by browser Javascript)
        private string PickScore
        {
            get
            {
                return HttpUtility.HtmlDecode(HiddenScore.Value);
            }
        }

        /// Display text of the picklist item selected (set by browser Javascript)
        private string PickText
        {
            get
            {
                return HttpUtility.HtmlDecode(HiddenPickText.Value);
            }
        }

        /// Page control: update the searching prompt
        private string Prompt
        {
            set
            {
                LabelPrompt.Text = value;
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
                StoredSearchEngine = value;
            }
        }

        /// Title of the Search button
        private string SearchButton
        {
            set
            {
                ButtonSearch.Value = value;
            }
        }

        /// Search/refinement terms
        protected string SearchString
        {
            get
            {
                return HttpUtility.HtmlDecode(searchText.Text);
            }
            set
            {
                searchText.Text = value;
            }
        }

        /// Warning relating to the picklist item selected (set by browser Javascript)
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
    }
}
