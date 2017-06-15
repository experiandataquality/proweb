/// QAS Pro Web integration code
/// (c) Experian, www.edq.com
namespace Experian.Qas.Proweb
{
    using System;

    /// <summary>
    /// This class contains common constants used throughout the Integration Pages.
    /// </summary>
    public class Constants
    {
        // Common configuration keys; values are read in from config.web
        public const string KEY_SERVER_URL = "com.qas.proweb.serverURL";
        public const string KEY_USERNAME = "com.qas.proweb.username";
        public const string KEY_PASSWORD = "com.qas.proweb.password";
        public const string KEY_LAYOUT = "com.qas.proweb.layout";
        public const string KEY_PROXY_ADDRESS = "com.qas.proweb.proxy.address";
        public const string KEY_PROXY_USERNAME = "com.qas.proweb.proxy.username";
        public const string KEY_PROXY_PASSWORD = "com.qas.proweb.proxy.password";

        // Filename of common final, address confirmation, page
        public const string PAGE_FINAL_ADDRESS = "Address.aspx";

        // Filename of shared page for Verification scenario
        public const string PAGE_VERIFY_SEARCH = "VerifySearch.aspx";

        // Filename of verification input screen
        public const string PAGE_VERIFY_INPUT = "VerifyInput.aspx";

        // Page for bulk verification search
        public const string PAGE_BULK_VERIFY_SEARCH = "BulkVerifySearch.aspx";

        // Common field names
        public const string FIELD_DATA_ID = "DataID";
        public const string FIELD_COUNTRY_NAME = "CountryName";
        public const string FIELD_INPUT_LINES = "InputLine";
        public const string FIELD_ADDRESS_LINES = "AddressLine";
        public const string FIELD_ERROR_INFO = "ErrorInfo";
        public const string FIELD_ADDRESS_INFO = "AddressInfo";
        public const string FIELD_VERIFY_INFO = "VerifyInfo";
        public const string FIELD_MONIKER = "Moniker";
        public const string FIELD_ROUTE = "Route";
        public const string FIELD_DPVSTATUS = "DPVStatus";
        public const string FIELD_REQUEST_TAG = "RequestTag";

        // FIELD_ROUTE values: current search state; how we arrived at the final page
        public enum Routes
        {
            /// <summary>
            /// State not defined.
            /// </summary>
            Undefined,

            /// <summary>
            /// An address was successfully returned
            /// </summary>
            Okay,

            /// <summary>
            /// An exception was thrown during the capture process
            /// </summary>
            Failed,

            /// <summary>
            /// CanSearch() returned false - searching not available
            /// </summary>
            PreSearchFailed,

            /// <summary>
            /// Country is not in page's list of installed countries
            /// </summary>
            UnsupportedCountry,

            /// <summary>
            /// Picklist returned with TooManyMatches flag
            /// </summary>
            TooManyMatches,

            /// <summary>
            /// Picklist returned empty
            /// </summary>
            NoMatches,

            /// <summary>
            /// Picklist returned with Timeout flag
            /// </summary>
            Timeout
        }

        public enum DPVStatus
        {
            DPVNotConfigured,
            DPVConfigured,
            DPVConfirmed,
            DPVConfirmedMissingSec,
            DPVNotConfirmed,
            DPVLocked,
            DPVSeedHit
        }

        #region All Countries

        /// <summary>
        /// Array representation of all countries and their associated dataset code.
        /// </summary>
        public static Dataset[] AllCountries =
        {
            new Dataset("AFG", "Afghanistan"),
            new Dataset("ALA", "Aland Islands"),
            new Dataset("ALB", "Albania"),
            new Dataset("DZA", "Algeria"),
            new Dataset("ASM", "American Samoa"),
            new Dataset("AND", "Andorra"),
            new Dataset("AGO", "Angola"),
            new Dataset("AIA", "Anguilla"),
            new Dataset("ATA", "Antarctica"),
            new Dataset("ATG", "Antigua And Barbuda"),
            new Dataset("ARG", "Argentina"),
            new Dataset("ARM", "Armenia"),
            new Dataset("ABW", "Aruba"),
            new Dataset("AUS", "Australia"),
            new Dataset("AUT", "Austria"),
            new Dataset("AZE", "Azerbaijan"),
            new Dataset("BHS", "Bahamas"),
            new Dataset("BHR", "Bahrain"),
            new Dataset("BGD", "Bangladesh"),
            new Dataset("BRB", "Barbados"),
            new Dataset("BLR", "Belarus"),
            new Dataset("BEL", "Belgium"),
            new Dataset("BLZ", "Belize"),
            new Dataset("BEN", "Benin"),
            new Dataset("BMU", "Bermuda"),
            new Dataset("BTN", "Bhutan"),
            new Dataset("BOL", "Bolivia"),
            new Dataset("BIH", "Bosnia And Herzegowina"),
            new Dataset("BWA", "Botswana"),
            new Dataset("BVT", "Bouvet Island"),
            new Dataset("BRA", "Brazil"),
            new Dataset("IOT", "British Indian Ocean Territory"),
            new Dataset("BRN", "Brunei Darussalam"),
            new Dataset("BGR", "Bulgaria"),
            new Dataset("BFA", "Burkina Faso"),
            new Dataset("BDI", "Burundi"),
            new Dataset("KHM", "Cambodia"),
            new Dataset("CMR", "Cameroon"),
            new Dataset("CAN", "Canada"),
            new Dataset("CPV", "Cape Verde"),
            new Dataset("CYM", "Cayman Islands"),
            new Dataset("CAF", "Central African Republic"),
            new Dataset("TCD", "Chad"),
            new Dataset("CHL", "Chile"),
            new Dataset("CHN", "China"),
            new Dataset("CXR", "Christmas Island"),
            new Dataset("CCK", "Cocos (Keeling) Islands"),
            new Dataset("COL", "Colombia"),
            new Dataset("COM", "Comoros"),
            new Dataset("COG", "Congo"),
            new Dataset("COD", "Congo, The Democratic Republic Of The"),
            new Dataset("COK", "Cook Islands"),
            new Dataset("CRI", "Costa Rica"),
            new Dataset("CIV", "Cote D'Ivoire"),
            new Dataset("HRV", "Croatia (Local Name: Hrvatska)"),
            new Dataset("CUB", "Cuba"),
            new Dataset("CYP", "Cyprus"),
            new Dataset("CZE", "Czech Republic"),
            new Dataset("DNK", "Denmark"),
            new Dataset("DJI", "Djibouti"),
            new Dataset("DMA", "Dominica"),
            new Dataset("DOM", "Dominican Republic"),
            new Dataset("TMP", "East Timor"),
            new Dataset("ECU", "Ecuador"),
            new Dataset("EGY", "Egypt"),
            new Dataset("SLV", "El Salvador"),
            new Dataset("GNQ", "Equatorial Guinea"),
            new Dataset("ERI", "Eritrea"),
            new Dataset("EST", "Estonia"),
            new Dataset("ETH", "Ethiopia"),
            new Dataset("FLK", "Falkland Islands (Malvinas)"),
            new Dataset("FRO", "Faroe Islands"),
            new Dataset("FJI", "Fiji"),
            new Dataset("FIN", "Finland"),
            new Dataset("FRP", "France"),
            new Dataset("FXX", "France, Metropolitan"),
            new Dataset("GUF", "French Guiana"),
            new Dataset("PYF", "French Polynesia"),
            new Dataset("ATF", "French Southern Territories"),
            new Dataset("GAB", "Gabon"),
            new Dataset("GMB", "Gambia"),
            new Dataset("GEO", "Georgia"),
            new Dataset("DEU", "Germany"),
            new Dataset("GHA", "Ghana"),
            new Dataset("GIB", "Gibraltar"),
            new Dataset("GRC", "Greece"),
            new Dataset("GRL", "Greenland"),
            new Dataset("GRD", "Grenada"),
            new Dataset("GLP", "Guadeloupe"),
            new Dataset("GUM", "Guam"),
            new Dataset("GTM", "Guatemala"),
            new Dataset("GIN", "Guinea"),
            new Dataset("GNB", "Guinea-Bissau"),
            new Dataset("GUY", "Guyana"),
            new Dataset("HTI", "Haiti"),
            new Dataset("HMD", "Heard And McDonald Islands"),
            new Dataset("VAT", "Holy See (Vatican City State)"),
            new Dataset("HND", "Honduras"),
            new Dataset("HKG", "Hong Kong"),
            new Dataset("HUN", "Hungary"),
            new Dataset("ISL", "Iceland"),
            new Dataset("IND", "India"),
            new Dataset("IDN", "Indonesia"),
            new Dataset("IRN", "Iran (Islamic Republic Of)"),
            new Dataset("IRQ", "Iraq"),
            new Dataset("IRL", "Ireland"),
            new Dataset("ISR", "Israel"),
            new Dataset("ITA", "Italy"),
            new Dataset("JAM", "Jamaica"),
            new Dataset("JPN", "Japan"),
            new Dataset("JOR", "Jordan"),
            new Dataset("KAZ", "Kazakhstan"),
            new Dataset("KEN", "Kenya"),
            new Dataset("KIR", "Kiribati"),
            new Dataset("PRK", "Korea, Democratic People's Republic Of"),
            new Dataset("KOR", "Korea, Republic Of"),
            new Dataset("KWT", "Kuwait"),
            new Dataset("KGZ", "Kyrgyzstan"),
            new Dataset("LAO", "Lao People's Democratic Republic"),
            new Dataset("LVA", "Latvia"),
            new Dataset("LBN", "Lebanon"),
            new Dataset("LSO", "Lesotho"),
            new Dataset("LBR", "Liberia"),
            new Dataset("LBY", "Libyan Arab Jamahiriya"),
            new Dataset("LIE", "Liechtenstein"),
            new Dataset("LTU", "Lithuania"),
            new Dataset("LUX", "Luxembourg"),
            new Dataset("MAC", "Macau"),
            new Dataset("MKD", "Macedonia, The Former Yugoslav Republic Of"),
            new Dataset("MDG", "Madagascar"),
            new Dataset("MWI", "Malawi"),
            new Dataset("MYS", "Malaysia"),
            new Dataset("MDV", "Maldives"),
            new Dataset("MLI", "Mali"),
            new Dataset("MLT", "Malta"),
            new Dataset("MHL", "Marshall Islands"),
            new Dataset("MTQ", "Martinique"),
            new Dataset("MRT", "Mauritania"),
            new Dataset("MUS", "Mauritius"),
            new Dataset("MYT", "Mayotte"),
            new Dataset("MEX", "Mexico"),
            new Dataset("FSM", "Micronesia, Federated States Of"),
            new Dataset("MDA", "Moldova, Republic Of"),
            new Dataset("MCO", "Monaco"),
            new Dataset("MNE", "Montenegro"),
            new Dataset("MNG", "Mongolia"),
            new Dataset("MSR", "Montserrat"),
            new Dataset("MAR", "Morocco"),
            new Dataset("MOZ", "Mozambique"),
            new Dataset("MMR", "Myanmar"),
            new Dataset("NAM", "Namibia"),
            new Dataset("NRU", "Nauru"),
            new Dataset("NPL", "Nepal"),
            new Dataset("NLD", "Netherlands"),
            new Dataset("ANT", "Netherlands Antilles"),
            new Dataset("NCL", "New Caledonia"),
            new Dataset("NZL", "New Zealand"),
            new Dataset("NIC", "Nicaragua"),
            new Dataset("NER", "Niger"),
            new Dataset("NGA", "Nigeria"),
            new Dataset("NIU", "Niue"),
            new Dataset("NFK", "Norfolk Island"),
            new Dataset("MNP", "Northern Mariana Islands"),
            new Dataset("NOR", "Norway"),
            new Dataset("OMN", "Oman"),
            new Dataset("PAK", "Pakistan"),
            new Dataset("PLW", "Palau"),
            new Dataset("P>P", "Palestinian Territory, Occupied"),
            new Dataset("PAN", "Panama"),
            new Dataset("PNG", "Papua New Guinea"),
            new Dataset("PRY", "Paraguay"),
            new Dataset("PER", "Peru"),
            new Dataset("PHL", "Philippines"),
            new Dataset("PCN", "Pitcairn"),
            new Dataset("POL", "Poland"),
            new Dataset("PRT", "Portugal"),
            new Dataset("PRI", "Puerto Rico"),
            new Dataset("QAT", "Qatar"),
            new Dataset("REU", "Reunion"),
            new Dataset("ROM", "Romania"),
            new Dataset("RUS", "Russian Federation"),
            new Dataset("RWA", "Rwanda"),
            new Dataset("KNA", "Saint Kitts And Nevis"),
            new Dataset("LCA", "Saint Lucia"),
            new Dataset("VCT", "Saint Vincent And The Grenadines"),
            new Dataset("WSM", "Samoa"),
            new Dataset("SMR", "San Marino"),
            new Dataset("STP", "Sao Tome And Principe"),
            new Dataset("SAU", "Saudi Arabia"),
            new Dataset("SRB", "Serbia"),
            new Dataset("SEN", "Senegal"),
            new Dataset("SYC", "Seychelles"),
            new Dataset("SLE", "Sierra Leone"),
            new Dataset("SGF", "Singapore"),
            new Dataset("SVK", "Slovakia (Slovak Republic)"),
            new Dataset("SVN", "Slovenia"),
            new Dataset("SLB", "Solomon Islands"),
            new Dataset("SOM", "Somalia"),
            new Dataset("ZAF", "South Africa"),
            new Dataset("SGS", "South Georgia And The South Sandwich Islands"),
            new Dataset("ESP", "Spain"),
            new Dataset("LKA", "Sri Lanka"),
            new Dataset("SHN", "St. Helena"),
            new Dataset("SPM", "St. Pierre And Miquelon"),
            new Dataset("SDN", "Sudan"),
            new Dataset("SUR", "Suriname"),
            new Dataset("SJM", "Svalbard And Jan Mayen Islands"),
            new Dataset("SWZ", "Swaziland"),
            new Dataset("SWE", "Sweden"),
            new Dataset("CHE", "Switzerland"),
            new Dataset("SYR", "Syrian Arab Republic"),
            new Dataset("TWN", "Taiwan, Province Of China"),
            new Dataset("TJK", "Tajikistan"),
            new Dataset("TZA", "Tanzania, United Republic Of"),
            new Dataset("THA", "Thailand"),
            new Dataset("TGO", "Togo"),
            new Dataset("TKL", "Tokelau"),
            new Dataset("TLS", "Timor-Leste"), 
            new Dataset("TON", "Tonga"),
            new Dataset("TTO", "Trinidad And Tobago"),
            new Dataset("TUN", "Tunisia"),
            new Dataset("TUR", "Turkey"),
            new Dataset("TKM", "Turkmenistan"),
            new Dataset("TCA", "Turks And Caicos Islands"),
            new Dataset("TUV", "Tuvalu"),
            new Dataset("UGA", "Uganda"),
            new Dataset("UKR", "Ukraine"),
            new Dataset("ARE", "United Arab Emirates"),
            new Dataset("GBR", "United Kingdom"),
            new Dataset("USA", "United States"),
            new Dataset("UMI", "United States Minor Outlying Islands"),
            new Dataset("URY", "Uruguay"),
            new Dataset("UZB", "Uzbekistan"),
            new Dataset("VUT", "Vanuatu"),
            new Dataset("VEN", "Venezuela"),
            new Dataset("VNM", "Vietnam"),
            new Dataset("VGB", "Virgin Islands (British)"),
            new Dataset("VIR", "Virgin Islands (U.S.)"),
            new Dataset("WLF", "Wallis And Futuna Islands"),
            new Dataset("ESH", "Western Sahara"),
            new Dataset("YEM", "Yemen"),
            new Dataset("YUG", "Yugoslavia"),
            new Dataset("ZMB", "Zambia"),
            new Dataset("ZWE", "Zimbabwe")
        };
        
        public static Dataset[] gatEuCountries =
        {
            new Dataset(""   ,"Albania"),
            new Dataset(""   ,"Andorra"),
            new Dataset(""   ,"Austria"),
            new Dataset(""   ,"Belarus"),
            new Dataset("BEL","Belgium"),
            new Dataset(""   ,"Bosnia-Herzegovina"),
            new Dataset(""   ,"Bulgaria"),
            new Dataset(""   ,"Croatia"),
            new Dataset(""   ,"Cyprus"),
            new Dataset(""   ,"Czech Republic"),
            new Dataset("DNK","Denmark"),
            new Dataset(""   ,"Estonia"),
            new Dataset("FIN","Finland"),
            new Dataset("FRP","France"),
            new Dataset("DEU","Germany"),
            new Dataset(""   ,"Greece"),
            new Dataset(""   ,"Hungary"),
            new Dataset(""   ,"Iceland"),
            new Dataset("IRL","Ireland"),
            new Dataset(""   ,"Italy"),
            new Dataset(""   ,"Latvia"),
            new Dataset(""   ,"Liechtenstein"),
            new Dataset(""   ,"Lithuania"),
            new Dataset("LUX","Luxembourg"),
            new Dataset(""   ,"Malta"),
            new Dataset(""   ,"Macedonia"),
            new Dataset(""   ,"Moldova"),
            new Dataset(""   ,"Monaco"),
            new Dataset("NLD","Netherlands"),
            new Dataset("NOR","Norway"),
            new Dataset(""   ,"Poland"),
            new Dataset(""   ,"Portugal"),
            new Dataset(""   ,"Romania"),
            new Dataset(""   ,"Russia"),
            new Dataset(""   ,"San Marino"),
            new Dataset(""   ,"Serbia and Montenegro"),
            new Dataset(""   ,"Slovakia"),
            new Dataset(""   ,"Slovenia"),
            new Dataset("ESP","Spain"),
            new Dataset("SWE","Sweden"),
            new Dataset(""   ,"Switzerland"),
            new Dataset(""   ,"Turkey"),
            new Dataset("GBR","United Kingdom"),
            new Dataset(""   ,"Ukraine"),
            new Dataset(""   ,"Vatican City")
        };
        
        public static Dataset[] CoreCountries =
        {
            new Dataset("AUS", "Australia"),
            new Dataset("BEL", "Belgium"),
            new Dataset("CAN", "Canada"),
            new Dataset("DNK", "Denmark"),
            new Dataset("FIN", "Finland"),
            new Dataset("FRP", "France"),
            new Dataset("DEU", "Germany"),
            new Dataset("IRL", "Ireland"),
            new Dataset("LUX", "Luxembourg"),
            new Dataset("NLD", "Netherlands"),
            new Dataset("NZL", "New Zealand"),
            new Dataset("NOR", "Norway"),
            new Dataset("SGF", "Singapore"),
            new Dataset("ESP", "Spain"),
            new Dataset("SWE", "Sweden"),
            new Dataset("GBR", "United Kingdom"),
            new Dataset("GBB", "UK Businesses"),
            new Dataset("GBN", "UK Names"),
            new Dataset("USA", "United States"),
            new Dataset("USN", "US Names")
        };

        #endregion
    }
}
