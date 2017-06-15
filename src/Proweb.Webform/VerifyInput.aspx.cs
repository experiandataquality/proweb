/// QAS Pro Web > (c) Experian > www.edq.com
/// Web > Verification > VerifyInput
/// Verify the address entered > Initial page - select country, enter address
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
    public partial class VerifyInput : VerifyBase
    {
        /* Members and constants */

        protected IList<Dataset> m_atDatasets;
        protected string m_asError;

        /* Methods */


        /// <summary>
        /// Pick up values transfered from other pages
        /// </summary>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                m_atDatasets = AddressLookup.GetAllDatasets();
            }
            catch ( Exception x )
            {
                m_asError = x.Message;
            }
        }
    }
}
