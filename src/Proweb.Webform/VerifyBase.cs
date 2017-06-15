/// QAS Pro Web > (c) Experian > www.edq.com
/// Web > Verification > VerifyBase
/// Provide common functionality and value transfer
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Text;
    using System.Web;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Web > Verification > VerifyBase
    /// This is the base class for the pages of the scenario
    /// It provides common functionality and simple inter-page value passing.
    /// </summary>
    public class VerifyBase : System.Web.UI.Page
    {
        private IAddressLookup addressLookup = null;
        private string[] m_asInputAddress;

        // Page filenames
        private const string PAGE_REFINE = "VerifyRefine.aspx";

        /** Methods **/
        
        /// <summary>
        /// Gets a new QAS service, connected to the configured server 
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
        /// Get the layout from the config
        /// </summary>
        /// <returns>string</returns>
        protected string GetLayout()
        {
            string sLayout = string.Empty;
            string sDataID = Request[Constants.FIELD_DATA_ID];

            if (sDataID != null && sDataID != string.Empty)
            {
                // Look for a layout specific to this datamap 
                sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT + "." + sDataID];

                if (sLayout == null || sLayout == string.Empty)
                {
                    // No layout found specific to this datamap - try the default
                    sLayout = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_LAYOUT];
                }
            }

            return sLayout;
        }

        /// <summary>
        /// Transfer out of the scenario to display the formatted address
        /// </summary>
        /// <param name="sMoniker">string</param>
        protected void GoFinalPage(string sMoniker)
        {
            FormatAddress(sMoniker);
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /// <summary>
        /// Transfer out of the scenario to the original input screen
        /// </summary>
        protected void GoInputPage()
        {
            Server.Transfer(Constants.PAGE_VERIFY_INPUT);
        }
            
        /// <summary>
        /// Transfer out of the scenario to display the found address
        /// </summary>
        protected void GoFinalPage()
        {
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /// <summary>
        /// Transfer out of the scenario to display the input address, after exception thrown
        /// </summary>
        /// <param name="x">Exception</param>
        protected void GoErrorPage(Exception x)
        {
            // Copy input lines through to output
            SetAddressResult(GetInputAddress);
            SetAddressInfo("address verification " + Constants.Routes.Failed + ", so the entered address has been used");
            SetErrorInfo(x.Message);
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /// <summary>
        /// Transfer out of the scenario to display the input address, after verification failed
        /// </summary>
        /// <param name="route">Constants.Routes</param>
        /// <param name="sReason">string</param>
        protected void GoErrorPage(Constants.Routes route, string sReason)
        {
            // Copy input lines through to output
            SetAddressResult(GetInputAddress);
            SetAddressInfo("address verification " + route + ", so the entered address has been used");
            SetErrorInfo(sReason);
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /// <summary>
        /// Retrieve a final formatted address from the moniker, which came from the picklist.
        /// </summary>
        /// <param name="sMoniker">Search Point Moniker of address to retrieve.</param>
        protected void FormatAddress(string sMoniker)
        {
            try
            {
                if (!string.IsNullOrEmpty(sMoniker))
                {
                    // Format the address
                    FormattedAddress tAddressResult = AddressLookup.GetFormattedAddress(sMoniker, GetLayout());
                    SetAddressResult(tAddressResult);
                }
                else
                {
                    SetAddressResult(GetInputAddress);
                    SetAddressInfo("Address verification is not available, so the entered address has been used");
                }
            }
            catch (Exception x)
            {
                SetAddressResult(GetInputAddress);
                SetAddressInfo("Address verification is not available, so the entered address has been used");
                SetErrorInfo(x.Message);
            }
        }

        // Stored properties **/

        /// <summary>
        /// Write out the address result - into the Request as cookies (server side only)
        /// </summary>
        /// <param name="tAddressResult">FormattedAddress</param>
        protected void SetAddressResult(FormattedAddress tAddressResult)
        {
            Request.Cookies.Remove(Constants.FIELD_ADDRESS_LINES);
            foreach (AddressLine tLine in tAddressResult.AddressLines)
            {
                Request.Cookies.Add(new HttpCookie(Constants.FIELD_ADDRESS_LINES, tLine.Line));
            }

            AddAddressWarnings(tAddressResult);
        }

        /// <summary>
        /// Write out the address result - into the Request as cookies (server side only)
        /// </summary>
        /// <param name="asAddress">string[]</param>
        protected void SetAddressResult(string[] asAddress)
        {
            Request.Cookies.Remove(Constants.FIELD_ADDRESS_LINES);
            foreach (string sLine in asAddress)
            {
                Request.Cookies.Add(new HttpCookie(Constants.FIELD_ADDRESS_LINES, sLine));
            }
        }

        /// <summary>
        /// Gets the display name of the Country in use (i.e. Australia)
        /// </summary>
        /// <returns>string</returns>
        protected string GetCountry()
        {
            return Request[Constants.FIELD_COUNTRY_NAME];
        }

        /// <summary>
        /// Country display name (i.e. Australia)
        /// </summary>
        /// <param name="sCountry">string</param>
        protected void SetCountry(string sCountry)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_COUNTRY_NAME, sCountry));
        }

        /// <summary>
        /// Error information returned through the exception
        /// </summary>
        /// <param name="sErrorInfo">string</param>
        protected void SetErrorInfo(string sErrorInfo)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ERROR_INFO, sErrorInfo));
        }

        /// <summary>
        /// Gets entered address to check
        /// </summary>
        protected string GetSearch
        {
            get
            {
                string[] values = Request.Form.GetValues(Constants.FIELD_INPUT_LINES);
                if (values != null)
                {
                    StringBuilder search = new StringBuilder();
                    foreach (string line in values)
                    {
                        search.Append(line);
                        search.Append(',');
                    }

                    return search.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        protected string[] GetInputAddress
        {
            get
            {
                if (m_asInputAddress == null)
                {
                    m_asInputAddress = Request.Form.GetValues(Constants.FIELD_INPUT_LINES);
                }

                return m_asInputAddress;
            }
        }

        /// <summary>
        /// Gets moniker of the final address
        /// </summary>
        /// <returns>string</returns>
        protected string GetMoniker()
        {
            return Request[Constants.FIELD_MONIKER];
        }

        /// <summary>
        /// Get the state of the verification searching
        /// </summary>
        /// <returns>string</returns>
        protected string GetAddressInfo()
        {
            return Request[Constants.FIELD_ADDRESS_INFO];
        }

        /// <summary>
        /// Get the address information, HTML transformed
        /// </summary>
        /// <returns>string</returns>
        protected string GetAddressInfoHTML()
        {
            return GetAddressInfo().Replace("\n", "<br />");
        }

        /// <summary>
        /// Set the state of the verification searching
        /// </summary>
        /// <param name="sAddressInfo">string</param>
        protected void SetAddressInfo(string sAddressInfo)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ADDRESS_INFO, sAddressInfo));
        }

        /// <summary>
        /// Add formatted address warnings to the address info
        /// </summary>
        /// <param name="tAddressResult">FormattedAddress</param>
        protected void AddAddressWarnings(FormattedAddress tAddressResult)
        {
            if (tAddressResult.IsOverflow)
            {
                SetAddressInfo(GetAddressInfo() + "\nWarning: Address has overflowed the layout &#8211; elements lost");
            }

            if (tAddressResult.IsTruncated)
            {
                SetAddressInfo(GetAddressInfo() + "\nWarning: Address elements have been truncated");
            }
        }
    }
}
