/// QAS Pro Web integration code
/// (c) Experian, www.edq.com
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Web;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Key Search" - flattened picklists
    /// This is the base class for the pages of the scenario
    /// It provides common functionality and facilitates inter-page value passing through hidden fields.
    /// </summary>
    public class KeyBasePage : System.Web.UI.Page
    {
        // Page filenames
        protected const string PAGE_BEGIN = "KeyCountry.aspx";
        protected const string PAGE_INPUT = "KeyPrompt.aspx";
        protected const string PAGE_SEARCH = "KeySearch.aspx";
        protected const string PAGE_FORMAT = "KeyAddress.aspx";

        // Field names specific to the Keyfinder scenario
        // Which prompt set is selected - set on PAGE_INPUT, also used by PAGE_SEARCH
        protected const string FIELD_PROMPTSET = "PromptSet";

        // Used to recreate the picklist - set and used by PAGE_SEARCH
        protected const string FIELD_PICKLIST_MONIKER = "PicklistMoniker";

        // The picklist item requiring refinement - set on PAGE_SEARCH, used by PAGE_REFINE
        protected const string FIELD_REFINE_MONIKER = "RefineMoniker";

        /// <summary>
        /// Initialises a new instance of the class using default values for members (No construction 
        /// necessary, provides shared functionality.)
        /// </summary>
        public KeyBasePage()
        {
        }

        /// <summary>
        /// Transfer to the initial page, to select the country.
        /// </summary>
        protected void GoFirstPage()
        {
            Server.Transfer(PAGE_BEGIN);
        }

        /// <summary>
        /// Transfer to the input page, which prompts for address terms.
        /// </summary>
        protected void GoInputPage()
        {
            Server.Transfer(PAGE_INPUT);
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
        protected void GoFormatPage(string sMoniker)
        {
            if (sMoniker != null)
            {
                SetMoniker(sMoniker);
            }

            SetRoute(Constants.Routes.Okay);
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry, after capture failed.
        /// </summary>
        /// <param name="route">Constants.Routes</param>
        protected void GoErrorPage(Constants.Routes route)
        {
            SetRoute(route);
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry - include reason for failure.
        /// </summary>
        /// <param name="route">Constants.Routes</param>
        /// <param name="sMessage">string</param>
        protected void GoErrorPage(Constants.Routes route, string sMessage)
        {
            SetRoute(route);
            SetErrorInfo(sMessage);
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer to the address confirmation page for manual address entry, after exception thrown.
        /// </summary>
        /// <param name="x">Exception</param>
        protected void GoErrorPage(Exception x)
        {
            SetRoute(Constants.Routes.Failed);
            SetErrorInfo(x.Message);
            Server.Transfer(PAGE_FORMAT);
        }

        /// <summary>
        /// Transfer out of the scenario to the final (summary) page.
        /// </summary>
        protected void GoFinalPage()
        {
            Server.Transfer(Constants.PAGE_FINAL_ADDRESS);
        }

        /* Common field rendering routines */

        /// <summary>
        /// Propagate a value through, from the Request into a hidden field on our page.
        /// </summary>
        /// <param name="sKey">string</param>
        protected void RenderRequestString(string sKey)
        {
            string sValue = Request[sKey];
            RenderHiddenField(sKey, sValue);
        }

        /// <summary>
        /// Propagate values through, from the Request to hidden fields on our page.
        /// </summary>
        /// <param name="sKey">string</param>
        protected void RenderRequestArray(string sKey)
        {
            string[] asValues = Request.Params.GetValues(sKey);
            if (asValues != null)
            {
                foreach (string sValue in asValues)
                {
                    RenderHiddenField(sKey, sValue);
                }

                // Add dummy entry to 1-sized arrays to allow array subscripting in JavaScript
                if (asValues.Length == 1)
                {
                    RenderHiddenField(sKey, null);
                }
            }
        }

        /// <summary>
        /// Render a hidden field directly into the page.
        /// </summary>
        /// <param name="sKey">string</param>
        /// <param name="sValue">string</param>
        protected void RenderHiddenField(string sKey, string sValue)
        {
            Response.Write("<input type=\"hidden\" name=\"");
            Response.Write(sKey);
            if (sValue != null)
            {
                Response.Write("\" value=\"");
                Response.Write(HttpUtility.HtmlEncode(sValue));
            }

            Response.Write("\" />\n");
        }

        /// <summary>
        /// Render a boolean hidden field directly into the page.
        /// </summary>
        /// <param name="sKey">string</param>
        /// <param name="bValue">bool</param>
        protected void RenderHiddenField(string sKey, bool bValue)
        {
            Response.Write("<input type=\"hidden\" name=\"");
            Response.Write(sKey);
            if (bValue)
            {
                Response.Write("\" value=\"");
                Response.Write(true.ToString());
            }

            Response.Write("\" />\n");
        }

        // Stored parameters **/

        /// <summary>
        /// Country data identifier (i.e. AUS).
        /// </summary>
        ///<returns>The Data ID / country currently in use.</returns>
        protected string GetDataID()
        {
            return Request[Constants.FIELD_DATA_ID];
        }

        /// <summary>
        /// Gets the display name of the Country being used (i.e. Australia)
        /// </summary>
        /// <returns>string</returns>
        protected string GetCountryName()
        {
            return Request[Constants.FIELD_COUNTRY_NAME];
        }

        /// <summary>
        /// Gets layout based on the datamap selected.
        /// </summary>
        /// <returns>string</returns>
        protected string GetLayout()
        {
            string sLayout;
            string sDataID = GetDataID();

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
        /// Gets the selected promtset.
        /// </summary>
        /// <returns>PromptSet.Types</returns>
        protected PromptSet.Types GetPromptSet()
        {
            string sValue = Request[FIELD_PROMPTSET];
            return (sValue != null)
                ? (PromptSet.Types)Enum.Parse(typeof(PromptSet.Types), sValue)
                : PromptSet.Types.Optimal;
        }

        /// <summary>
        /// Sets the promtset to be used by the search.
        /// </summary>
        /// <param name="ePromptSet">PromptSet.Types</param>
        protected void SetPromptSet(PromptSet.Types ePromptSet)
        {
            Request.Cookies.Set(new HttpCookie(FIELD_PROMPTSET, ePromptSet.ToString()));
        }

        /// <summary>
        /// Formats the initial user search (i.e. "14 main street", "boston")
        /// </summary>
        /// <returns>string[]</returns>
        protected string GetInputLines()
        {
            string[] asValues = Request.Params.GetValues(Constants.FIELD_INPUT_LINES);
            return (asValues != null)
                ? asValues[0]
                : string.Empty;
        }

        /// <summary>
        /// Current search state, how we arrived on the address format page (i.e. too many matches)
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
        /// Sets the route to follow.
        /// </summary>
        /// <param name="eRoute">Constants.Routes</param>
        private void SetRoute(Constants.Routes eRoute)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ROUTE, eRoute.ToString()));
        }

        /// <summary>
        /// Gets error information if there are errrors.
        /// </summary>
        /// <returns>string</returns>
        protected string GetErrorInfo()
        {
            return Request[Constants.FIELD_ERROR_INFO];
        }

        /// <summary>
        /// Sets error information, if errors are thrown.
        /// </summary>
        /// <param name="sErrorInfo">string</param>
        protected void SetErrorInfo(string sErrorInfo)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_ERROR_INFO, sErrorInfo));
        }

        /// <summary>
        /// Gets the moniker of the final address.
        /// </summary>
        /// <returns>string</returns>
        protected string GetMoniker()
        {
            return Request[Constants.FIELD_MONIKER];
        }

        /// <summary>
        /// Sets the moniker to be used in the next step.
        /// </summary>
        /// <param name="sMoniker">string</param>
        private void SetMoniker(string sMoniker)
        {
            Request.Cookies.Set(new HttpCookie(Constants.FIELD_MONIKER, sMoniker));
        }

        /// <summary>
        /// Gets moniker of the initial flattened picklist
        /// </summary>
        /// <returns>string</returns>
        protected string GetPicklistMoniker()
        {
            return Request[FIELD_PICKLIST_MONIKER];
        }

        /// <summary>
        /// Sets the moniker of the initial flattened picklist
        /// </summary>
        /// <param name="sMoniker">string</param>
        protected void SetPicklistMoniker(string sMoniker)
        {
            Request.Cookies.Set(new HttpCookie(FIELD_PICKLIST_MONIKER, sMoniker));
        }
    }
}
