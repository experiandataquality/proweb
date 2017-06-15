/// QAS Pro Web integration code
/// (c) Experian, www.edq.com
namespace Experian.Qas.Prowebintegration
{
    using System;
    using System.Collections.Generic;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using Experian.Qas.Proweb;

    /// <summary>
    /// Scenario "Address Capture on the Intranet" - hierarchical picklists
    /// Retrieve the final formatted address, display to user for confirmation
    /// This page is based on HierBasePage, which provides functionality common to the scenario
    /// </summary>
    public partial class HierAddress : HierBasePage
    {
        // page controls


        protected void Page_Load(object sender, System.EventArgs e)
        {
            Page_BaseLoad(sender, e);

            if (!IsPostBack)
            {
                // Store values in transit from other pages
                StoredDataID = StoredPage.StoredDataID;
                StoredCountryName = StoredPage.StoredCountryName;
                StoredUserInput = StoredPage.StoredUserInput;
                SetStoredHistory(StoredPage.GetStoredHistory());

                // Pick up values we need
                StoredRoute = StoredPage.StoredRoute;
                StoredMoniker = StoredPage.StoredMoniker;
                StoredWarning = StoredPage.StoredWarning;
                StoredErrorInfo = StoredPage.StoredErrorInfo;

                // Address result
                string[] asLabels = null;
                string[] asLines = null;

                // Retrieve address
                FormatAddress(ref asLabels, ref asLines);

                // Set welcome message
                SetWelcomeMessage(StoredRoute);
                // Set stepin warning message
                SetWarningMessage(StoredWarning);
            // Display address lines
                DisplayAddress(asLabels, asLines);
                // Set integrator information: route and error
                SetErrorMessage();
            }
            // Else leave it to the event handlers (New, Back, Accept)
        }



        /** Search operations **/


        /// <summary>
        /// Retrieve the formatted address based on the StoredMoniker, or create a set of blank lines
        /// </summary>
        /// <param name="asLabels">Array of labels for each line (address type descriptions)</param>
        /// <param name="asLines">Array of address lines</param>
        protected void FormatAddress(ref string[] asLabels, ref string[] asLines)
        {
            // Retrieve formatted address
            if (StoredRoute.Equals(Constants.Routes.Okay))
            {
                try
                {
                    IAddressLookup addressLookup = QAS.GetSearchService(); 

                    // Perform address formatting
                    FormattedAddress objAddress = addressLookup.GetFormattedAddress(StoredMoniker, GetLayout());
                    List<AddressLine> lines = objAddress.AddressLines;

                    // Build display address arrays
                    int iSize = lines.Count;
                    asLabels = new string[iSize];
                    asLines = new String[iSize];
                    for (int i = 0; i < iSize; i++)
                    {
                        asLabels[i] = lines[i].Label;
                        asLines[i] = lines[i].Line;
                    }
                }
                catch (Exception x)
                {
                    StoredRoute = Constants.Routes.Failed;
                    StoredErrorInfo = x.Message;
                }
            }

            // Provide default (empty) address for manual entry
            if (!StoredRoute.Equals(Constants.Routes.Okay))
            {
                asLabels = new string[]
                {
                    "Address Line 1", "Address Line 2", "Address Line 3",
                    "City", "State or Province", "ZIP or Postal Code"
                };
                asLines = new string[]
                {
                    "", "", "", "", "", ""
                };
            }
        }



        /** Page updating **/


        /// <summary>
        /// Dynamically populate the TableAddress table control from the arguments
        /// </summary>
        protected void DisplayAddress(string[] asLabels, string[] asLines)
        {
            for (int iIndex = 0; iIndex < asLines.Length; ++iIndex)
            {
                AddAddressLine(asLabels[iIndex], asLines[iIndex]);
            }

            // Add country row
            HtmlInputText InputCountry = AddAddressLine("Datamap or Country", StoredCountryName);
            // Modify country field ID and look
            InputCountry.ID = Constants.FIELD_COUNTRY_NAME;
            InputCountry.Attributes["readonly"] = "readonly";
            InputCountry.Attributes["class"] = "readonly";
        }

        /// <summary>
        /// Add a table row, with cells for the label, a gap, and a text input control
        /// </summary>
        /// <returns>The text input control</returns>
        protected HtmlInputText AddAddressLine(string sLabel, string sLine)
        {
            TableRow row = new TableRow();

            TableCell cellLabel = new TableCell();
            LiteralControl label = new LiteralControl(sLabel);
            cellLabel.Controls.Add(label);
            row.Cells.Add(cellLabel);

            TableCell cellGap = new TableCell();
            cellGap.Width = new Unit(1, UnitType.Em);
            row.Cells.Add(cellGap);

            TableCell cellAddress = new TableCell();
            HtmlInputText addressLine = new HtmlInputText();
            addressLine.Value = sLine;
            addressLine.ID = Constants.FIELD_ADDRESS_LINES;
            addressLine.Size = 50;
            cellAddress.Controls.Add(addressLine);
            row.Cells.Add(cellAddress);

            TableAddress.Rows.Add(row);

            return addressLine;
        }



        /** Page event handlers **/


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
        /// 'New' button clicked: go to the first page
        /// </summary>
        protected void ButtonNew_ServerClick(object sender, System.EventArgs e)
        {
            // Clear the search terms (but retain the country selection)
            StoredUserInput = "";
            GoFirstPage();
        }

        /// <summary>
        /// 'Back' button clicked: recreate last picklist or go to the first page
        /// </summary>
        protected void ButtonBack_ServerClick(object sender, System.EventArgs e)
        {
            if (StoredRoute.Equals(Constants.Routes.Okay))
            {
                GoSearchPage();
            }
            else
            {
                GoFirstPage();
            }
        }

        /// <summary>
        /// 'Accept' button clicked: move out of this scenario
        /// </summary>
        protected void ButtonAccept_Click(object sender, System.EventArgs e)
        {
            GoFinalPage();
        }



        /** Page controls **/


        private void SetWelcomeMessage(Constants.Routes eRoute)
        {
         // Set the welcome message depending on how we got here (the route)
            switch (eRoute)
            {
                case Constants.Routes.Okay:
                    LiteralMessage.Text = "Please confirm your address below.";
                    break;
                case Constants.Routes.NoMatches:
                case Constants.Routes.Timeout:
                case Constants.Routes.TooManyMatches:
                    LiteralMessage.Text = "Automatic address capture did not succeed.<br /><br />Please search again or enter your address below.";
                    break;
                default:
                    LiteralMessage.Text = "Automatic address capture is not available.<br /><br />Please enter your address below.";
                    break;
            }
        }
        private void SetWarningMessage(StepinWarnings eWarn)
        {
            // Make the panel visible as appropriate
            PlaceHolderWarning.Visible = (eWarn != StepinWarnings.None);

         // Set the step-in message depending on the warning
            switch (eWarn)
            {
                case StepinWarnings.CloseMatches:
                    LiteralWarning.Text = "There are also close matches available &#8211; click <a href=\"javascript:goBack();\">back</a> to see them";
                    break;
                case StepinWarnings.CrossBorder:
                    LiteralWarning.Text = "Address selected is outside of the entered locality";
                    break;
                case StepinWarnings.PostcodeRecode:
                    LiteralWarning.Text = "Postal code has been updated by the Postal Authority";
                    break;
                default:
                    LiteralWarning.Text = "";
                    break;
            }
        }
        private void SetErrorMessage()
        {
            // Make the panel visible as appropriate
            PlaceholderInfo.Visible = !StoredRoute.Equals(Constants.Routes.Okay);
            // Update the content
            LiteralRoute.Text = StoredRoute.ToString();
            if (StoredErrorInfo != null)
            {
                LiteralError.Text = "<br />" + StoredErrorInfo;
            }
        }
    }
}
