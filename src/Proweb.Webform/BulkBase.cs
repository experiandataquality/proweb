/// QAS Pro Web > (c) Experian > www.edq.com
/// Web > Bulk Verification > BulkBase
/// Provide common functionality and value transfer
namespace Experian.Qas.Prowebintegration 
{
    using System;
    using System.Web;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Web > Bulk Verification > BulkBase
    /// This is the base class for the pages of the scenario
    /// It provides common functionality and simple inter-page value passing.
    /// </summary>
    public class BulkBase : System.Web.UI.Page 
    {       
        private IAddressLookup addressLookup = null;
                
        /// <summary>
        /// Gets a new instance of the QAS service, connected to the configured server
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
        /// Get the layout from the config.
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
        /// Error information returned through the exception.
        /// </summary>
        /// <returns>string</returns>
        protected string GetErrorInfo() 
        {
            return Request[Constants.FIELD_ERROR_INFO];
        }

        /// <summary>
        /// Sets error information, when an error is thrown.
        /// </summary>
        /// <param name="sErrorInfo">string</param>
        protected void SetErrorInfo(string sErrorInfo) 
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ERROR_INFO, sErrorInfo));
        }
        
        /// <summary>
        /// Returns current search state, how we arrived on the address format page (i.e. too many matches)
        /// </summary>
        /// <returns>Constants.Routes</returns>
        protected Constants.Routes GetRoute() 
        {
            string sValue = Request[Constants.FIELD_ROUTE];
            return (sValue != null)
            ? (Constants.Routes)Enum.Parse(typeof(Constants.Routes), sValue)
            : Constants.Routes.Undefined;
        }

        /// <summary>
        /// Set the route to follow.
        /// </summary>
        /// <param name="eRoute">Constants.Routes</param>
        protected void SetRoute(Constants.Routes eRoute) 
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ROUTE, eRoute.ToString()));
        }
    }
}