/// QAS Pro Web > (c) Experian > www.edq.com
/// Intranet > Bulk Verification > BulkVerifySearch
/// Verify the addresses entered > Original address, verification level and verified address if found.
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Bulk Search on the Web" - Bulk Verification
    /// Verify the addresses in a batch
    /// display the verify result
    ///
    /// Main arrival routes:
    ///   - Initial Bulk Verification: arriving from input page
    ///
    /// This page is based on BulkBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class BulkVerifySearch : BulkBase
    {
        /* Members and constants */


        // Search result (contains either picklist or matched address or nothing)
        protected BulkSearchResult m_BulkSearchResult = null;
        // Field name - original input address lines - to ensure we don't re-verify the same address
        protected const string FIELD_ORIGINAL_INPUT = "OriginalLine";
        // Field name - does this picklist item require additional information (refinement)
        protected const string FIELD_MUST_REFINE = "MustRefine";


        /* Methods */


        /// <summary>
        /// Pick up values transfered from other pages
        /// </summary>
        private void Page_Load(object sender, System.EventArgs e)
        {
            // The following method will do the search and prepare the result for display.
            BulkVerifyAddress();
        }


        /// <summary>
        /// Perform a bulk verification search on the input lines
        /// </summary>
        /// <param name="tSearchResult">To populate with search results</param>
        /// <param name="bGoFinalPage">Set true to avoid user interaction & always move to final page </param>
        /// <returns>Move on to final address page?</returns>
        protected bool BulkVerifyAddress()
        {
            // Results
            CanSearch canSearch = null;
            try
            {
                // Retrieve settings from web.config
                string sLayout = GetLayout();

                // Verify the address
                AddressLookup.CurrentEngine.EngineType = Engine.EngineTypes.Verification;
                AddressLookup.CurrentEngine.Flatten = true;

                canSearch = AddressLookup.CanSearch(DataID, sLayout, null);
                if (!canSearch.IsOk)
                {
                    base.SetRoute(Constants.Routes.PreSearchFailed);
                    base.SetErrorInfo(canSearch.ErrorMessage);
                }
                else
                {
                    m_BulkSearchResult = AddressLookup.BulkSearch(DataID, GetBulkInputAddress, PromptSet.Types.Default, sLayout);
                    base.SetRoute (Constants.Routes.Okay);
                }
            }
            catch (Exception x)
            {
                base.SetRoute(Constants.Routes.Failed);
                base.SetErrorInfo(x.Message);
            }
            return true;
        }

        protected IList<string> GetBulkInputAddress
        {
            get
            {
                IList<string> searches;
                string[] asInputAddress;

                asInputAddress = Request.Form.GetValues(Constants.FIELD_INPUT_LINES);

                Regex match = new Regex ("[\\r\\n\\\\]+");
                searches = new List<string>(match.Split(asInputAddress[0].Trim()));
                
                base.SetRoute(Constants.Routes.Failed);
                base.SetErrorInfo(searches[0]);

                return searches;
            }
        }



        /// <summary>
        /// Provide member-wise string array comparison
        /// </summary>
        /// <param name="asLHS">First argument, left-hand side</param>
        /// <param name="asRHS">Second argument, right-hand side</param>
        /// <returns>Are the two string arrays equal, member-by-member</returns>
        public static bool Equals(string[] asLHS, string[] asRHS)
        {
            if (asLHS.Length != asRHS.Length)
            {
                return false;
            }
            // Same length. Now compare them, member-by-member
            for (int iIndex = 0; iIndex < asLHS.Length; ++iIndex)
            {
                if (asLHS[iIndex] != asRHS[iIndex])
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Must the picklist item be refined (text added) to form a final address?
        /// </summary>
        protected static bool MustRefine(PicklistItem item)
        {
            return (item.IsIncompleteAddress || item.IsUnresolvableRange || item.IsPhantomPrimaryPoint);
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
            this.Load += new System.EventHandler(this.Page_Load);

        }
        #endregion


        /** Page parameters **/


        // Country data identifier (i.e. AUS)
        protected string DataID
        {
            get
            {
                return Request[Constants.FIELD_DATA_ID];
            }
        }

        // Read-only properties
        public BulkSearchItem[] BulkSearchItems
        {
            get
            {
                return m_BulkSearchResult.BulkSearchItems.ToArray();
            }
        }

        public string BulkOriginalAddress(int iIndex)
        {
            if (iIndex >= m_BulkSearchResult.BulkSearchItems.Count) 
            {
                return "Index out of range for original address " + iIndex.ToString();
            }
            else
            {
                return m_BulkSearchResult.BulkSearchItems[iIndex].InputAddress;
            }
        }

        public VerificationLevel BulkVerifyLevel(int iIndex)
        {
            if (iIndex >= m_BulkSearchResult.BulkSearchItems.Count) 
            {
                return VerificationLevel.None;
            }
            else
            {
                return m_BulkSearchResult.BulkSearchItems[iIndex].VerifyLevel;
            }
        }

        public FormattedAddress BulkFormattedAddress(int iIndex)
        {
            if (iIndex >= m_BulkSearchResult.BulkSearchItems.Count) 
            {
                return null;
            }
            else
            {
                return m_BulkSearchResult.BulkSearchItems[iIndex].Address;
            }
        }

        public int Count
        {
            get
            {
                if (m_BulkSearchResult == null || m_BulkSearchResult.BulkSearchItems == null)
                    return 0;
                else
                    return m_BulkSearchResult.BulkSearchItems.Count;
            }
        }

        public int FormattedAddressLength(int iIndex)
        {
            if (iIndex >= m_BulkSearchResult.BulkSearchItems.Count) 
            {
                return 0;
            }
            else
            {
                if (m_BulkSearchResult.BulkSearchItems[iIndex].Address != null)
                {
                    return m_BulkSearchResult.BulkSearchItems[iIndex].Address.AddressLines.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public int ErrorCode()
        {
            return m_BulkSearchResult.ErrorCode;
        }

        public string ErrorMessage()
        {
            return m_BulkSearchResult.ErrorMessage;
        }
    }
}
