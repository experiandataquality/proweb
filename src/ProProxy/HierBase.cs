/// QAS Pro Web integration code
/// (c) Experian, www.edq.com
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Address Capture on the Intranet" - hierarchical picklists
    /// This is the base class for the pages of the scenario
    /// It provides common functionality and facilitates inter-page value passing through the ViewState.
    /// </summary>
    public class HierBasePage : System.Web.UI.Page
    {
        // Retrieve the state of the previous page
        protected HierBasePage StoredPage = null;

        // Page filenames
        private const string PAGE_BEGIN = "HierInput.aspx";
        private const string PAGE_SEARCH = "HierSearch.aspx";
        private const string PAGE_FORMAT = "HierAddress.aspx";

        /// <summary>
        /// Picklist step-in warnings, to be displayed on next page
        /// </summary>
        public enum StepinWarnings
        {
            None,
            CloseMatches,
            CrossBorder,
            PostcodeRecode
        }

        /// <summary>
        /// Gets or sets the Country data identifier (i.e. AUS).
        /// </summary>
        public string StoredDataID
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
        /// Gets or sets the  Country display name (i.e. Australia).
        /// </summary>
        public string StoredCountryName
        {
            get
            {
                return (string)ViewState[Constants.FIELD_COUNTRY_NAME];
            }

            set
            {
                ViewState[Constants.FIELD_COUNTRY_NAME] = value;
            }
        }

        /// <summary>
        /// Gets or sets the  Initial user search (i.e. 14 main street, boston).
        /// </summary>
        public string StoredUserInput
        {
            get
            {
                return (string)ViewState[Constants.FIELD_INPUT_LINES];
            }

            set
            {
                ViewState[Constants.FIELD_INPUT_LINES] = value;
            }
        }

        /// <summary>
        /// Gets or sets the how we arrived on the formatting page (i.e. country not available).
        /// </summary>
        public Constants.Routes StoredRoute
        {
            get
            {
                object objValue = ViewState[Constants.FIELD_ROUTE];
                return (objValue != null) ? (Constants.Routes)objValue : Constants.Routes.Okay;
            }

            set
            {
                ViewState[Constants.FIELD_ROUTE] = value;
            }
        }

        // For transfering values between Search -> Address pages

        /// <summary>
        /// Gets or sets the Moniker of the address.
        /// </summary>
        public string StoredMoniker
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
        /// Gets or sets the Step-in warning (i.e. Postcode has been recoded).
        /// </summary>
        public StepinWarnings StoredWarning
        {
            get
            {
                object objValue = ViewState["Warning"];
                return (objValue != null) ? (StepinWarnings)objValue : StepinWarnings.None;
            }

            set
            {
                ViewState["Warning"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the Additional address/error information.
        /// </summary>
        public string StoredErrorInfo
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
        /// Pick up the preceding page, so we can access it's ViewState (see Stored properties section).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        virtual protected void Page_BaseLoad(object sender, System.EventArgs e)
        {
            if (!IsPostBack && Context.Handler is HierBasePage)
            {
                // Retrieve the state of the previous page, so it is available to us
                StoredPage = Context.Handler as HierBasePage;
            }
            else
            {
                // Point stored page to us, as we are the previous page
                StoredPage = this;
            }
        }

        /// <summary>
        /// Fetch the layout from the config.
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
        /// Transfer to the initial page, to select the country and enter search terms.
        /// </summary>
        protected void GoFirstPage()
        {
            Server.Transfer(PAGE_BEGIN);
        }

        /// <summary>
        /// Transfer to the address searching and picklist display page.
        /// </summary>
        protected void GoSearchPage()
        {
            Server.Transfer(PAGE_SEARCH);
        }

        /// <summary>
        /// Transfer to the address confirmation page to retrieve the found address.
        /// </summary>
        /// <param name="sMoniker">string</param>
        /// <param name="eWarn">StepinWarnings</param>
        protected void GoFormatPage(string sMoniker, StepinWarnings eWarn)
        {
            StoredPage.StoredRoute = Constants.Routes.Okay;
            StoredPage.StoredMoniker = sMoniker;
            StoredPage.StoredWarning = eWarn;
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry, after capture failure.
        /// </summary>
        /// <param name="route"></param>
        protected void GoErrorPage(Constants.Routes route)
        {
            StoredPage.StoredRoute = route;
            Server.Transfer(PAGE_FORMAT);
        }

        protected void GoErrorPage(Constants.Routes route, string sMessage)
        {
            StoredPage.StoredRoute = route;
            StoredPage.StoredErrorInfo = sMessage;
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry, after exception thrown.
        /// </summary>
        /// <param name="x">Exception</param>
        protected void GoErrorPage(Exception x)
        {
            StoredPage.StoredRoute = Constants.Routes.Failed;
            StoredPage.StoredErrorInfo = x.Message;
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer out of the scenario to the final (summary) page
        /// </summary>
        protected void GoFinalPage()
        {
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /// <summary>
        /// Picklist history.
        /// </summary>
        /// <returns>HistoryStack</returns>
        public HistoryStack GetStoredHistory()
        {
            object objValue = ViewState["History"];
            if (objValue is ArrayList)
            {
                HistoryStack stack = new HistoryStack((ArrayList)objValue);
                return stack;
            }

            return new HistoryStack();
        }


        /// <summary>
        /// Store history.
        /// </summary>
        /// <param name="vValue">HistoryStack</param>
        public void SetStoredHistory(HistoryStack vValue)
        {
            ViewState["History"] = vValue;
        }

        /** Helper classes **/

        /// <summary>
        /// Helper class: stack of all the search picklists we've stepped through
        /// Implemented using an ArrayList so we can enumerate forwards through them for display,
        /// the 'bottom' of the stack is element 0, the 'top' is element Count - 1, where items are pushed and popped.
        /// </summary>
        [Serializable]
        public class HistoryStack : ArrayList
        {
            public HistoryStack()
            {
            }

            public HistoryStack(ArrayList vValue)
            {
                foreach (object obj in vValue)
                {
                    Add((HistoryItem)obj);
                }
            }

            /// <summary>
            /// Gets or sets the element at the specified index.
            /// </summary>
            /// <param name="iIndex"></param>
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
            /// Returns the object at the top of the stack without removing it.
            /// </summary>
            /// <returns>HistoryItem</returns>
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
            /// Inserts an object at the top of the stack: prevents duplicates
            /// </summary>
            /// <param name="item"></param>
            public void Add(HistoryItem item)
            {
                if (Count == 0 || !Peek().Moniker.Equals(item.Moniker))
                {
                    base.Add(item);
                }
            }

            /// <summary>
            /// Inserts an object at the top of the stack
            /// </summary>
            /// <param name="sMoniker">string</param>
            /// <param name="sText">string</param>
            /// <param name="sPostcode">string</param>
            /// <param name="sScore">string</param>
            public void Push(string sMoniker, string sText, string sPostcode, string sScore)
            {
                HistoryItem item = new HistoryItem(sMoniker, sText, sPostcode, sScore);
                Add(item);
            }


            /// <summary>
            /// Inserts an object at the top of the stack.
            /// </summary>
            /// <param name="item"></param>
            public void Push(PicklistItem item)
            {
                Push(item.Moniker, item.Text, item.Postcode, item.ScoreAsString);
            }
        }

        /// <summary>
        /// Helper class: store details of a search picklist we've seen.
        /// </summary>
        [Serializable]
        public class HistoryItem
        {
            public string Moniker = string.Empty;
            public string Text = string.Empty;
            public string Postcode = string.Empty;
            public string Score = string.Empty;

            public HistoryItem(string sMonikerIn, string sTextIn, string sPostcodeIn, string sScoreIn)
            {
                Moniker = sMonikerIn;
                Text = sTextIn;
                Postcode = sPostcodeIn;
                Score = sScoreIn;
            }
        }
    }
}
