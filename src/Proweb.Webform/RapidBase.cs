/// QAS Pro Web > (c) Experian > www.edq.com
/// Intranet > Rapid Addressing > Standard > RapidBase
/// Provide common functionality and value transfer
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Intranet > Rapid Addressing > Standard > RapidBase
    /// This is the base class for the pages of the scenario
    /// It provides common functionality and facilitates inter-page value passing through the ViewState.
    /// </summary>
    public class RapidBasePage : System.Web.UI.Page
    {
        /** Attributes & Constants **/
        
        // Picklist history, stored in ViewState
        protected HistoryStack m_aHistory = null;

        // List of datamaps available on server
        protected IList<Dataset> m_atDatasets = null;

        private static IAddressLookup addressLookup = null;

        // Store the state of the previous page
        private RapidBasePage StoredPage = null;

        // Filenames
        private const string PAGE_SEARCH = "RapidSearch.aspx";
        private const string PAGE_FORMAT = "RapidAddress.aspx";

        // Viewstate names
        protected const string FIELD_CALLBACK = "Callback";
        private const string FIELD_ENGINE = "Engine";
        private const string FIELD_HISTORY = "History";
        private const string FIELD_WARNING = "Warning";
        private const string FIELD_DATALIST = "Datalist";

        // Select Box width constants 
        protected const int MAX_DATAMAP_NAME_LENGTH = 26;
        protected const string SELECT_WIDTH = "16em";

        // Enumerate operations that can be performed on a picklist item
        protected enum Commands
        {
            /// <summary>
            /// No hyperlink action - self-explanatory informational.
            /// </summary>
            None,

            /// <summary>
            /// Step in to sub-picklist.
            /// </summary>
            StepIn,

            /// <summary>
            /// Force-accept an unrecognized address.
            /// </summary>
            ForceFormat,

            /// <summary>
            /// Format into final address.
            /// </summary>
            Format,

            /// <summary>
            /// User must enter a value within the range shown.
            /// </summary>
            HaltRange,

            /// <summary>
            /// User must enter premise details.
            /// </summary>
            HaltIncomplete
        }

        // Enumerate the picklist item types (affects icon displayed)
        protected enum Types
        {
            /// <summary>
            /// Picklist item is an alias (synonym).
            /// </summary>
            Alias,

            /// <summary>
            /// Picklist item is an informational.
            /// </summary>
            Info,

            /// <summary>
            /// Picklist item is a warning informational.
            /// </summary>
            InfoWarn,

            /// <summary>
            /// Picklist item is a name/person.
            /// </summary>
            Name,

            /// <summary>
            /// Picklist item is a name alias (i.e. forename synonym).
            /// </summary>
            NameAlias,

            /// <summary>
            /// Picklist item is a PO Box grouping.
            /// </summary>
            POBox,

            /// <summary>
            /// Picklist item is standard.
            /// </summary>
            Standard
        }

        // Picklist step-in warnings (displayed on next page)
        protected enum StepinWarnings
        {
            /// <summary>
            ///  No warning.
            /// </summary>
            None,

            /// <summary>
            /// Auto-stepped past close matches.
            /// </summary>
            CloseMatches,

            /// <summary>
            /// Stepped into cross-border match.
            /// </summary>
            CrossBorder,

            /// <summary>
            /// Force-format step-in performed.
            /// </summary>
            ForceAccept,

            /// <summary>
            /// Stepped into informational item (i.e. 'Click to Show All').
            /// </summary>
            Info,

            /// <summary>
            /// Address elements have overflowed the layout.
            /// </summary>
            Overflow,

            /// <summary>
            /// Stepped into postcode recode.
            /// </summary>
            PostcodeRecode,

            /// <summary>
            /// Address elements have been truncated by the layout.
            /// </summary>
            Truncate
        }

        #region properties

        /// <summary>
        /// Gets the Javascript function to call on completion
        /// </summary>
        protected string StoredCallback
        {
            get
            {
                return (string)ViewState[FIELD_CALLBACK];
            }

            set
            {
                ViewState[FIELD_CALLBACK] = value;
            }
        }

        /// <summary>
        /// Gets the country data identifier (i.e. AUS)
        /// </summary>
        protected string StoredDataID
        {
            get
            {
                return (string)ViewState[Constants.FIELD_DATA_ID];
            }

            set
            {
                ViewState[Constants.FIELD_DATA_ID] = value;
            }
        }

        /// <summary>
        /// Gets or sets the stored List of datamaps available on server.
        /// </summary>
        protected IList<Dataset> StoredDataMapList
        {
            get
            {
                return (IList<Dataset>)ViewState[FIELD_DATALIST];
            }

            set
            {
                ViewState[FIELD_DATALIST] = value;
            }
        }

        /// <summary>
        /// Gets or sets the additional address/error information.
        /// </summary>
        protected string StoredErrorInfo
        {
            get
            {
                return (string)ViewState[Constants.FIELD_ERROR_INFO];
            }

            set
            {
                ViewState[Constants.FIELD_ERROR_INFO] = value;
            }
        }

        /// <summary>
        /// Gets or sets the moniker of the address.
        /// </summary>
        protected string StoredMoniker
        {
            get
            {
                return (string)ViewState[Constants.FIELD_MONIKER];
            }

            set
            {
                ViewState[Constants.FIELD_MONIKER] = value;
            }
        }

        /// <summary>
        /// Gets or sets the how we arrived on the formatting page (i.e. pre-search check failed).
        /// </summary>
        protected Constants.Routes StoredRoute
        {
            get
            {
                object objValue = ViewState[Constants.FIELD_ROUTE];
                return (objValue != null) ? (Constants.Routes)objValue : Constants.Routes.Undefined;
            }

            set
            {
                ViewState[Constants.FIELD_ROUTE] = value;
            }
        }

        /// <summary>
        /// Gets or sets the search engine selected.
        /// </summary>
        protected Engine.EngineTypes StoredSearchEngine
        {
            get
            {
                object objValue = ViewState[FIELD_ENGINE];
                return (objValue != null) ? (Engine.EngineTypes)objValue : Engine.EngineTypes.Typedown;
            }

            set
            {
                ViewState[FIELD_ENGINE] = value;
            }
        }

        /// <summary>
        /// Gets or sets the step-in warning (i.e. Postcode has been recoded).
        /// </summary>
        protected StepinWarnings StoredWarning
        {
            get
            {
                object objValue = ViewState[FIELD_WARNING];
                return (objValue != null) ? (StepinWarnings)objValue : StepinWarnings.None;
            }

            set
            {
                ViewState[FIELD_WARNING] = value;
            }
        }

        #endregion

        /** Base methods **/

        /// <summary>
        /// Pick up the preceding page, so we can access it's ViewState (see Stored properties section).
        /// </summary>
        virtual protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!IsPostBack && (Context.Handler is RapidBasePage))
            {
                // Retrieve the state of the previous page, so it is available to us
                StoredPage = Context.Handler as RapidBasePage;

                StoredCallback = StoredPage.StoredCallback;
                if (StoredCallback == null)
                {
                    StoredCallback = Request[FIELD_CALLBACK];
                }

                StoredDataID = StoredPage.StoredDataID;
                StoredSearchEngine = StoredPage.StoredSearchEngine;
                StoredErrorInfo = StoredPage.StoredErrorInfo;
                StoredMoniker = StoredPage.StoredMoniker;
                StoredRoute = StoredPage.StoredRoute;
                StoredWarning = StoredPage.StoredWarning;
                                StoredDataMapList = StoredPage.StoredDataMapList;
            }
            else
            {
                // Point stored page to us, as we are the previous page
                StoredPage = this;
            }
            
            // Pick up history, passed around in viewstate
            m_aHistory = StoredPage.GetStoredHistory();
        }

        /// <summary>
        /// Store the history back to the view state prior to rendering.
        /// </summary>
        /// <returns>object</returns>
        protected override object SaveViewState()
        {
            SetStoredHistory(m_aHistory);
            return base.SaveViewState();
        }

        /// <summary>
        /// Gets Picklist history.
        /// </summary>
        /// <returns>HistoryStack</returns>
        protected HistoryStack GetStoredHistory()
        {
            object objValue = ViewState[FIELD_HISTORY];
            if (objValue is ArrayList)
            {
                HistoryStack stack = new HistoryStack((ArrayList)objValue);
                return stack;
            }

            return new HistoryStack();
        }

        /** Common methods **/

        /// <summary>
        /// Gets the QAS service, connected to the configured server
        /// Singleton pattern: maintain a single instance, created only on demand.
        /// </summary>
        protected IAddressLookup AddressLookup
        {
            get
            {
                if (addressLookup == null)
                {
                    // Create QAS search object
                    addressLookup = QAS.GetSearchService();
                }

                return addressLookup;
            }
        }

        /// <summary>
        /// Get the layout from the config file.
        /// </summary>
        /// <returns>string</returns>
        protected string GetLayout()
        {
            string sLayout;
            string sDataID = StoredDataID;

            // Look for a layout specific to this datamap 
            sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT + "." + sDataID];

            if (sLayout == null || sLayout == string.Empty)
            {
                // No layout found specific to this datamap - try the default
                sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT];
            }

            return sLayout;
        }

        /// <summary>
        /// Transfer to the address searching and picklist display page.
        /// </summary>
        /// <param name="sDataID">string</param>
        /// <param name="eEngine">QAS.EngineTypes</param>
        protected void GoSearchPage(string sDataID, Engine.EngineTypes eEngine)
        {
            // Store values back to the view state
            StoredPage.StoredDataID = sDataID;
            StoredPage.SetStoredHistory(m_aHistory);
            StoredPage.StoredSearchEngine = eEngine;

            Server.Transfer(PAGE_SEARCH);
        }

        /// <summary>
        /// Transfer to the address confirmation page to retrieve the found address.
        /// </summary>
        /// <param name="sDataID">string</param>
        /// <param name="eEngine">EngineTypes</param>
        /// <param name="sMoniker">string</param>
        /// <param name="eWarn">StepinWarnings</param>
        protected void GoFormatPage(string sDataID, Engine.EngineTypes eEngine, string sMoniker, StepinWarnings eWarn)
        {
            // Store values back to the view state
            StoredPage.StoredDataID = sDataID;
            StoredPage.StoredMoniker = sMoniker;
            StoredPage.SetStoredHistory(m_aHistory);
            StoredPage.StoredRoute = Constants.Routes.Okay;
            StoredPage.StoredSearchEngine = eEngine;
            StoredPage.StoredWarning = eWarn;

            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry, after capture failure.
        /// </summary>
        /// <param name="route">Constants.Routes</param>
        /// <param name="sReason">string</param>
        protected void GoErrorPage(Constants.Routes route, string sReason)
        {
            StoredPage.StoredErrorInfo = sReason;
            StoredPage.StoredRoute = route;
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry, after exception thrown.
        /// </summary>
        /// <param name="x">Exception</param>
        protected void GoErrorPage(Exception x)
        {
            StoredPage.StoredErrorInfo = x.Message;
            StoredPage.StoredRoute = Constants.Routes.Failed;
            Server.Transfer(PAGE_FORMAT);
        }

        /** Page display **/

        /// <summary>
        /// Write out picklist HTML and associated action array to Javascript variables
        /// This is included in two distinct places:
        ///   - main searching page, for whole page updating
        ///   - picklist results frame, for dynamic picklist updating
        /// The picklist picks up and uses the values from the appropriate place.
        /// </summary>
        /// <param name="picklist">Picklist</param>
        /// <param name="sDepth">string</param>
        protected void RenderPicklistData(Picklist picklist, string sDepth)
        {
            if (picklist == null)
            {
                // No picklist in this context: write out empty values
                Response.Write("var sPicklistHTML = '';\n");
                Response.Write("var asActions = new Array();");
            }
            else
            {
                // Build sActions into a string, while writing picklistHTML out to Response
                StringBuilder sActions = new StringBuilder();

                // Build picklist HTML into JavaScript string
                //   - icon, operation, display text, hover text, postcode, score

                Response.Write("var sPicklistHTML = \"<table class='picklist indent" + sDepth + "'>\\\n");
               
                int index = 0;
                foreach (PicklistItem item in picklist.PicklistItems)
                {
                    // Step-in warning
                    StepinWarnings eWarn = StepinWarnings.None;
                    if (item.IsCrossBorderMatch)
                    {
                        eWarn = StepinWarnings.CrossBorder;
                    }
                    else if (item.IsPostcodeRecoded)
                    {
                        eWarn = StepinWarnings.PostcodeRecode;
                    }

                    // Commands: what to do if they click on the item
                    Commands eCmd = Commands.None;
                    if (item.CanStep)
                    {
                        eCmd = Commands.StepIn;
                    }
                    else if (item.IsFullAddress)
                    {
                        eCmd = item.IsInformation ? Commands.ForceFormat : Commands.Format;
                    }
                    else if (item.IsUnresolvableRange)
                    {
                        eCmd = Commands.HaltRange;
                    }
                    else if (item.IsIncompleteAddress)
                    {
                        eCmd = Commands.HaltIncomplete;
                    }

                    // Type: indicates the type of icon to display (used in combination with the operation)
                    Types eType = Types.Standard;
                    if (item.IsInformation)
                    {
                        eType = item.IsWarnInformation ? Types.InfoWarn : Types.Info;
                        eWarn = StepinWarnings.Info;
                    }
                    else if (item.IsDummyPOBox)
                    {
                        eType = Types.POBox;
                    }
                    else if (item.IsName)
                    {
                        eType = item.IsAliasMatch ? Types.NameAlias : Types.Name;
                    }
                    else if (item.IsAliasMatch || item.IsCrossBorderMatch || item.IsPostcodeRecoded)
                    {
                        eType = Types.Alias;
                    }

                    // Start building HTML

                    // Set the class depending on the function & type -> displayed icon
                    string sClass = "stop";
                    if (eCmd == Commands.StepIn)
                    {
                        if (eType == Types.Alias)
                        {
                            sClass = "aliasStep";
                        }
                        else if (eType == Types.Info)
                        {
                            sClass = "infoStep";
                        }
                        else if (eType == Types.POBox)
                        {
                            sClass = "pobox";
                        }
                        else
                        {
                            sClass = "stepIn";
                        }
                    }
                    else if (eCmd == Commands.Format)
                    {
                        if (eType == Types.Alias)
                        {
                            sClass = "alias";
                        }
                        else if (eType == Types.Name)
                        {
                            sClass = "name";
                        }
                        else if (eType == Types.NameAlias)
                        {
                            sClass = "nameAlias";
                        }
                        else
                        {
                            sClass = "format";
                        }
                    }
                    else if ((eCmd == Commands.HaltIncomplete) || (eCmd == Commands.HaltRange))
                    {
                        sClass = "halt";
                    }
                    else if (eType == Types.Info)
                    {
                        sClass = "info";
                    }

                    if (index == 0)
                    {
                        sClass += " first";                        
                    }

                    if (item.IsSubsidiaryData)
                    {
                        sClass += " sub";
                    }

                    // Hyperlink
                    string sAnchorStart = string.Empty, sAnchorEnd = string.Empty;

                    if (item.Text != null && eCmd != Commands.None)
                    {
                        sAnchorStart = "<a href='javascript:action(" + index.ToString() + ");' "
                            + "tabindex='" + (index + 1) + "' "
                            + "title=\\\"" + JavascriptEncode(item.Text) + "\\\">";
                        sAnchorEnd = "</a>";
                    }

                    string sScore = (item.Score > 0) ? item.Score + "%" : string.Empty;

                    // Write out HTML

                    Response.Write("<tr>");
                    Response.Write("<td class='pickitem " + sClass + "'>" + sAnchorStart + "<div>");
                    Response.Write(JavascriptEncode(Server.HtmlEncode(item.Text)) + "</div>" + sAnchorEnd + "</td>");
                    if (item.Postcode == null)
                    {
                        Response.Write("<td class='postcode'></td>");
                    }
                    else
                    {
                        Response.Write("<td class='postcode'>" + JavascriptEncode(Server.HtmlEncode(item.Postcode)) + "</td>");
                    }

                    Response.Write("<td class='score'>" + sScore + "</td>");
                    Response.Write("</tr>\\\n");

                    // Picklist actions - javascript array variable

                    sActions.Append("'" + (eCmd != Commands.None ? eCmd.ToString() : string.Empty));
                    switch (eCmd)
                    {
                        case Commands.StepIn:
                            sActions.Append("(\"" + item.Moniker + "\",\"" + JavascriptEncode(Server.HtmlEncode(item.Text)) + "\",");
                            sActions.Append("\"" + item.Postcode + "\",\"" + item.ScoreAsString + "\",\"" + eWarn.ToString() + "\")");
                            break;
                        case Commands.Format:
                            sActions.Append("(\"" + item.Moniker + "\",\"" + eWarn.ToString() + "\")");
                            break;
                        case Commands.ForceFormat:
                            sActions.Append("(\"" + item.Moniker + "\")");
                            break;
                        case Commands.HaltIncomplete:
                        case Commands.HaltRange:
                            sActions.Append("()");
                            break;
                    }

                    sActions.Append("',");

                    index++;
                }

                // Close off picklist HTML
                Response.Write("</table>\";\n");

                // Write out Actions

                Response.Write("var asActions = new Array(");
                Response.Write(sActions.ToString());
                Response.Write("'');\n");
            }
        }

        /// <summary>
        /// Method override: Use picklist history in order to work out indent depth.
        /// </summary>
        /// <param name="picklist">Picklist</param>
        protected void RenderPicklistData(Picklist picklist)
        {
            RenderPicklistData(picklist, m_aHistory.Count.ToString());
        }

        /// <summary>
        /// Encode the string so it's value is correct when used as a Javascript string
        /// i.e. Jack's "friendly" dog  ->  Jack\'s \"friendly\" dog.
        /// </summary>
        /// <param name="str">Plain text string to encode.</param>
        /// <returns>String with special characters escaped.</returns>
        protected string JavascriptEncode(string str)
        {
            return str.Replace("\\", "\\\\").Replace("'", "\\'").Replace("\"", "\\\"");
        }

        /// <summary>
        /// Picklist history, set.
        /// </summary>
        /// <param name="value">HistoryStack</param>
        protected void SetStoredHistory(HistoryStack value)
        {
            ViewState[FIELD_HISTORY] = value;
        }

        /** Support classes: Picklist history **/

        /// <summary>
        /// Helper class: stack of all the search picklists we've stepped through
        /// Implemented using an ArrayList so we can enumerate forwards through them for display,
        /// the 'bottom' of the stack is element 0, the 'top' is element Count - 1, where items are pushed and popped.
        /// </summary>
        [Serializable]
        protected class HistoryStack : ArrayList
        {
            /// <summary>
            /// Initialises a new instance of the class using default values for members (Default constructor)
            /// </summary>
            public HistoryStack()
            {
            }

            /// <summary>
            /// Initialises a new instance of HistryItem(s) (Construct from an ArrayList)
            /// </summary>
            /// <param name="vValue">ArrayList</param>
            public HistoryStack(ArrayList vValue)
            {
                foreach (object obj in vValue)
                {
                    base.Add((HistoryItem)obj);
                }
            }

            /// <summary>
            /// Gets or sets the element at the specified index
            /// </summary>
            /// <param name="iIndex">int</param>
            /// <returns>HistoryItem</returns>
            public new HistoryItem this[int iIndex]
            {
                get
                {
                    return (HistoryItem)base[iIndex];
                }

                set
                {
                    base[iIndex] = value;
                }
            }

            /// <summary>
            /// Inserts an object at the top of the stack: prevents duplicates
            /// </summary>
            /// <param name="item">HistoryItem</param>
            public void Add(HistoryItem item)
            {
                if (Count == 0 || !Peek().Moniker.Equals(item.Moniker))
                {
                    base.Add(item);
                }
            }

            /// <summary>
            /// Returns the object at the top of the stack without removing it
            /// </summary>
            /// <returns></returns>
            public HistoryItem Peek()
            {
                return (HistoryItem)this[Count - 1];
            }

            /// <summary>
            /// Removes and returns the object at the top of the stack
            /// </summary>
            /// <returns>HistoryItem</returns>
            public HistoryItem Pop()
            {
                HistoryItem tail = Peek();
                RemoveAt(Count - 1);
                return tail;
            }

            /// <summary>
            /// Inserts an object at the top of the stack
            /// </summary>
            /// <param name="sMoniker">string</param>
            /// <param name="sText">string</param>
            /// <param name="sPostcode">string</param>
            /// <param name="sScore">string</param>
            /// <param name="sRefine">string</param>
            public void Push(string sMoniker, string sText, string sPostcode, string sScore, string sRefine)
            {
                HistoryItem item = new HistoryItem(sMoniker, sText, sPostcode, sScore, sRefine);
                Add(item);
            }

            /// <summary>
            /// Inserts an object at the top of the stack
            /// </summary>
            /// <param name="item">PicklistItem</param>
            public void Push(PicklistItem item)
            {
                Push(item.Moniker, item.Text, item.Postcode, item.ScoreAsString, string.Empty);
            }

            /// <summary>
            /// Truncate the stack down to a certain size
            /// </summary>
            /// <param name="iCount">int</param>
            public void Truncate(int iCount)
            {
                if (Count > iCount)
                {
                    RemoveRange(iCount, Count - iCount);
                }
            }
        }

        /// <summary>
        /// Helper class: stores details of search picklists visited.
        /// </summary>
        [Serializable]
        protected class HistoryItem
        {
            public string Moniker = string.Empty;
            public string Text = string.Empty;
            public string Postcode = string.Empty;
            public string Score = string.Empty;
            public string Refine = string.Empty;

            public HistoryItem(string sMonikerIn, string sTextIn, string sPostcodeIn, string sScoreIn, string sRefineIn)
            {
                Moniker = sMonikerIn;
                Text = sTextIn;
                Postcode = sPostcodeIn;
                Score = sScoreIn;
                Refine = sRefineIn;
            }
        }
    }
}
