/// QAS Pro Web > (c) Experian > www.edq.com
/// Web > Verification > VerifySearch
/// Verify the address entered > Perfect match, best match, picklist of matches
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections.Generic;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Address Verification on the Web" - verification with interaction
    /// Verify the address, go straight to final address page for verified matches, otherwise
    /// re-display entered address and best match or list of matches (rendered directly in page)
    ///
    /// Main arrival routes:
    ///   - Initial verification: arriving from input page
    ///   - Re-verification: manual address is re-submitted: re-verify then go to final page
    ///   - Picklist selection: retrieve address from moniker, then go to final page
    ///
    /// This page is based on VerifyBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class VerifySearch : VerifyBase
    {
        /* Members and constants */


        // Search result (contains either picklist or matched address or nothing)
        protected SearchResult m_SearchResult = null;
        protected Picklist m_Picklist = null;
        // Picklist support JavaScript strings: moniker values; refinement values
        protected System.Text.StringBuilder m_sMonikers = new System.Text.StringBuilder();
        protected System.Text.StringBuilder m_sExtras = new System.Text.StringBuilder();
        // Picklist display: explanation message - refinement prompt
        protected String m_sPicklistMessage;
        protected String m_sPicklistPrompt;
        // Picklist display: offer refinement?
        protected bool m_bPicklistRefine;
        protected bool m_bIsIncompleteAddress;
        protected bool m_bCanStep;
        protected bool m_bUnresolvableRange;

        // Field name - original input address lines - to ensure we don't re-verify the same address
        protected const string FIELD_ORIGINAL_INPUT = "OriginalLine";
        // Field name - does this picklist item require additional information (refinement)
        protected const string FIELD_IS_REFINE = "IsRefine";
        // Field name - does this picklist item require additional information (refinement)
        protected const string FIELD_REFINEMENT = "RefineText";


        /* Methods */


        /// <summary>
        /// Pick up values transfered from other pages
        /// </summary>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            // Has an address been entered? 
            if (null == GetInputAddress)
            {
                GoInputPage();
            }

            if (IsInitialSearch)
            {
                // Should we jump past the interaction page, even when the match isn't perfect?
                bool bAvoidInteraction = false;
                // Copy input lines through to output by default
                SetAddressResult(GetInputAddress);

                // Check to see if they have interacted already
                if (OriginalInputAddress != null)
                {
                    // If they have re-accepted their entered address (input lines unchanged), don't re-verify it
                    if (Equals(OriginalInputAddress, GetInputAddress))
                    {
                        SetAddressInfo("address accepted unchanged, so the entered address has been used");
                        GoFinalPage();
                    }

                    // Whatever happens, go to final address page after verifying the (updated) entered address
                    bAvoidInteraction = true;
                }

                bool bGoToFinalPage = VerifyAddress(bAvoidInteraction);

                if (bGoToFinalPage)
                {
                    GoFinalPage();
                }
                else if (m_SearchResult.Picklist != null && m_SearchResult.Picklist.Total > 0 )
                {
                    // Pre-process picklist
                    m_Picklist = m_SearchResult.Picklist;
                    PreparePicklist(true);
                    // Display picklist of matches for user-interaction: done by page
                }
                else if (m_SearchResult.Address != null)
                {
                    // Display address match for user-interaction: done by page
                }
            }
            else if (IsRefining)
            {
                RefinePicklist();
            }
            else
            {
                // Format final address
                GoFinalPage(GetMoniker());
            }
        }


        /// <summary>
        /// Perform a verification search on the input lines
        /// </summary>
        /// <param name="tSearchResult">To populate with search results</param>
        /// <param name="bGoFinalPage">Set true to avoid user interaction & always move to final page </param>
        /// <returns>Move on to final address page?</returns>
        protected bool VerifyAddress(bool bGoFinalPage)
        {
            // Results
            CanSearch canSearch = null;
            try
            {
                // Verify the address
                AddressLookup.CurrentEngine.EngineType = Engine.EngineTypes.Verification;
                AddressLookup.CurrentEngine.Flatten = true;

                // Get the layout
                string sLayout = GetLayout();

                canSearch = AddressLookup.CanSearch(DataID, sLayout, null);
                if (canSearch.IsOk)
                {
                    m_SearchResult = AddressLookup.Search(DataID, GetSearch, PromptSet.Types.Default, sLayout);
                    SetAddressInfo("address verification level was " + m_SearchResult.VerifyLevel.ToString());

                    if ((m_SearchResult.VerifyLevel == VerificationLevel.Verified) || (m_SearchResult.VerifyLevel == VerificationLevel.VerifiedPlace) || (m_SearchResult.VerifyLevel == VerificationLevel.VerifiedStreet))
                    {
                        // Copy found address through to output
                        SetAddressResult(m_SearchResult.Address);
                        bGoFinalPage = true;
                    }
                    else if (bGoFinalPage)
                    {
                        // Second time round - use input address and explain why
                        SetAddressInfo(GetAddressInfo() + ", so the entered address has been used");
                    }
                    else if (m_SearchResult.Address != null)
                    {
                        // We're going to offer the found address, so display warnings
                        AddAddressWarnings(m_SearchResult.Address);
                    }
                }
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            if (!canSearch.IsOk)
            {
                GoErrorPage(Constants.Routes.PreSearchFailed, canSearch.ErrorMessage);
            }

            return bGoFinalPage;
        }


        /// <summary>
        /// Refine (filter) the picklist
        /// - if it produces a single good match, format it
        /// - otherwise display picklist with appropriate warning messages
        /// </summary>
        protected void RefinePicklist()
        {
            PicklistItem item = null;
            bool bGoFinalPage = false;		// go to the final address page?

            try
            {
                // Perform the refinement
                AddressLookup.CurrentEngine.EngineType = Engine.EngineTypes.Verification;
                AddressLookup.CurrentEngine.Flatten = true;

                m_Picklist = AddressLookup.Refine(GetMoniker(), RefinementText + " ", GetLayout());

                // If the refined search produces no results, recreate the picklist and update the message
                if (m_Picklist.Total == 0)
                {
                    // No acceptable address match - recreate without using refinement text
                    m_Picklist = AddressLookup.Refine(GetMoniker(), "", GetLayout());
                    m_sPicklistMessage = "You entered '" + RefinementText + "', but this value is outside of the range. Please try again.";
                }

                // Update page content
                item = m_Picklist.PicklistItems[0];

                if (RefinementText == "")
                {
                    // First time through - simply display appropriate message
                    if (item.IsPhantomPrimaryPoint)
                    {
                        m_sPicklistMessage = "Your selection requires secondary information. Please enter your exact details.";
                    }
                }
                else if (item.IsUnresolvableRange)
                {
                    // Redisplay with explanation
                    m_sPicklistMessage = "You entered '" + RefinementText + "', but this address is outside of the range. Please try again.";
                }
                else if (m_Picklist.Total == 1)
                {
                    // Accept (or force accept in the Phantom case)
                    bGoFinalPage = true;
                }
            }
            catch (Exception x)
            {
                GoErrorPage(x);
            }

            if (bGoFinalPage)
            {
                GoFinalPage(item.Moniker);
            }

            PreparePicklist(false);
        }


        /// <summary>
        /// Prepare picklist control and display properties:
        /// - JavaScript string array of monikers, and 'must-refine' booleans
        /// - whether to offer refinement, or initially collapse picklist
        /// </summary>
        protected void PreparePicklist(bool bInitial)
        {
            m_sMonikers.Length = 0;
            m_sMonikers.Append("'");
            m_sExtras.Length = 0;

            // Always refine after the initial search, or if street partial
            m_bPicklistRefine = !bInitial || m_SearchResult.VerifyLevel.Equals(VerificationLevel.StreetPartial);
            // Or, if any of the picklist items require resolution

            for (int i = 0; i < m_Picklist.PicklistItems.Count; ++i)
            {
                // Build JavaScript string arrays
                m_sMonikers.Append(m_Picklist.PicklistItems[i].Moniker);
                m_sMonikers.Append("','");
                bool bRefine = MustRefine(m_Picklist.PicklistItems[i]);
                m_sExtras.Append(bRefine ? "true," : "false,");
                m_bPicklistRefine |= bRefine;
                m_bIsIncompleteAddress = m_Picklist.PicklistItems[i].IsIncompleteAddress;
                m_bCanStep = m_Picklist.PicklistItems[i].CanStep;
                m_bUnresolvableRange = m_Picklist.PicklistItems[i].IsUnresolvableRange;
            }
            
            // Remove trailing characters
            m_sMonikers.Length -= 2;
            m_sExtras.Length--;
        }


        /** Helpers **/


        /// <summary>
        /// Provide member-wise string array comparison
        /// </summary>
        /// <param name="asLHS">First argument, left-hand side</param>
        /// <param name="asRHS">Second argument, right-hand side</param>
        /// <returns>Are the two string arrays equal, member-by-member</returns>
        protected static bool Equals(string[] asLHS, string[] asRHS)
        {
            if (asLHS.Length != asRHS.Length)
            {
                return false;
            }
            // Same length, so compare them member-by-member
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
        }
        #endregion


        /** Page parameters **/


        // Input //


        // Country data identifier (i.e. AUS)
        protected string DataID
        {
            get
            {
                return Request[Constants.FIELD_DATA_ID];
            }
        }

        // Is this the result of the initial search, or a subsequent picklist selection/refinement
        protected bool IsInitialSearch
        {
            get
            {
                return (GetMoniker() == null);
            }
        }

        // Should we now refine the picklist item selected by the user
        private bool IsRefining
        {
            get
            {
                return bool.Parse(Request[FIELD_IS_REFINE]);
            }
        }

        // Originally submitted address
        private string[] OriginalInputAddress
        {
            get
            {
                return Request.Form.GetValues(FIELD_ORIGINAL_INPUT);
            }
        }

        // Refinement string entered
        protected string RefinementText
        {
            get
            {
                return Request[FIELD_REFINEMENT] != null ? Request[FIELD_REFINEMENT] : "";
            }
        }


        // Output //


        // Address found
        protected List<AddressLine> AddressLines
        {
            get
            {
                return (m_SearchResult != null && m_SearchResult.Address != null)
                    ? m_SearchResult.Address.AddressLines
                    : null;
            }
        }

        // Array of picklist items
        protected List<PicklistItem> PicklistItems
        {
            get
            {
                return (m_Picklist != null) ? m_Picklist.PicklistItems : null;
            }
        }

        // Display picklist initially collapsed, with a link to expand?
        protected bool IsPicklistCollapse
        {
            get
            {
                return (IsInitialSearch && m_bPicklistRefine && m_Picklist.PicklistItems.Count > 8);
            }
        }

        // Offer picklist refinement?
        protected bool IsPicklistRefine
        {
            get
            {
                return m_bPicklistRefine;
            }
        }

        // Initially collapse (hide) the picklist, HTML style
        protected string PicklistCollapseStyle
        {
            get
            {
                return (IsPicklistCollapse ? "display:none" : "");
            }
        }

        // Picklist refinement overview/description text
        protected string PicklistMessage
        {
            get
            {
                if (m_sPicklistMessage != null)
                {
                    return m_sPicklistMessage;
                }
                else if (m_SearchResult != null)
                {
                    switch (m_SearchResult.VerifyLevel)
                    {
                        case VerificationLevel.PremisesPartial:
                            return "Your address appears to be missing secondary information. " +
                                (IsPicklistRefine ? "Please enter the required details below." : "Please select from the list below.");
                        case VerificationLevel.StreetPartial:
                            return "Your house information appears to be missing or not recognised. " +
                                (IsPicklistRefine ? "Please enter the required details below." : "Please select from the list below.");
                        case VerificationLevel.Multiple:
                            return "Your details have matched a number of addresses. " +
                                (IsPicklistRefine ? "Please enter more information below." : "Please select from the list below.");
                    }
                }

                return "Your selection covers a range of addresses. Please enter your exact details.";
            }
        }

        // Picklist refinement text box prompt
        protected string PicklistPrompt
        {
            get
            {
                if (m_sPicklistPrompt != null)
                {
                    return m_sPicklistPrompt;
                }
                else if (m_SearchResult != null)
                {
                    switch (m_SearchResult.VerifyLevel)
                    {
                        case VerificationLevel.PremisesPartial:
                            return "Enter apartment, flat or unit number";
                        case VerificationLevel.StreetPartial:
                            return "Enter house number or organisation";
                    }
                }

                return m_Picklist.Prompt;
            }
        }

        // Moniker for current picklist
        protected string PicklistMoniker
        {
            get
            {
                return m_Picklist.Moniker;
            }
        }

        // JavaScript boolean array of 'must refine?', one for each picklist item
        protected string PicklistExtrasStringArray
        {
            get
            {
                return m_sExtras.ToString();
            }
        }

        // JavaScript string array of monikers, one for each picklist item
        protected string PicklistMonikersStringArray
        {
            get
            {
                return m_sMonikers.ToString();
            }
        }

        // Display warning that there are more possible mathces?
        protected bool IsMoreOtherMatches
        {
            get
            {
                return m_SearchResult.Picklist.AreMoreMatches;
            }

        }

        protected bool IsIncompleteAddress
        {
            get
            {

                return m_bIsIncompleteAddress;
            }

        }

        protected bool CanStep
        {
            get
            {
                return m_bCanStep;
            }

        }

        protected bool IsUnresolvableRange
        {
            get
            {
                return m_bUnresolvableRange;
            }

        }
       

    }
}
