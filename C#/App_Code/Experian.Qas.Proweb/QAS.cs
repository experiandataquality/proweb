namespace Experian.Qas.Proweb
{
    using System.Net;

    /// <summary>
    /// This class provides a factory for generating the classes to communicate with your
    /// AddressLookup service. 
    /// </summary>
    public static class QAS
    {
        /// <summary>
        /// There are two possible ways that you can get Address Capture from Experian Data Quality. Either
        /// our on premise or hosted.  Depending upon what you have configured in your web.config, we 
        /// generate the correct service here.
        /// If using Pro Web, we only expect the endpoint URL to be populated else we create an OnDemand endpoint.
        /// </summary>
        /// <returns>Either an OnDemand or ProWeb implementation of IAddressLookup.</returns>
        public static IAddressLookup GetSearchService()
        {
            // Retrieve server URL from web.config
            string endpointURL = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_SERVER_URL];

            // Retrieve Username from web.config
            string username = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_USERNAME];

            // Retrieve Password from web.config
            string password = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_PASSWORD];

            // Retrieve proxy address Value from web.config
            string proxyAddress = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_PROXY_ADDRESS];

            // Retrieve proxy username Value from web.config
            string proxyUsername = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_PROXY_USERNAME];

            // Retrieve proxy password Value from web.config
            string proxyPassword = System.Configuration.ConfigurationManager.AppSettings[Constants.KEY_PROXY_PASSWORD];

            // We are now either going to generate a Pro Web proxy - for local on premise solutions or 
            // an OnDemand proxy - for Experian's SaaS Service.
            if (string.IsNullOrEmpty(username))
            {
                // We are creating a Pro Web proxy - does not need a user name
                return new ProWeb(endpointURL);
            }
            else
            {
                IWebProxy proxy = null;
                if (!string.IsNullOrEmpty(proxyAddress))
                {
                    proxy = new WebProxy(proxyAddress, true);

                    NetworkCredential credentials = new NetworkCredential(proxyUsername, proxyPassword);
                    proxy.Credentials = credentials;
                }

                // We need to create an OnDemand:
                return new OnDemand(endpointURL, username, password, proxy);
            }
        }
    }
}
