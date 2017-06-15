/// QAS Pro Web > (c) Experian > www.edq.com
/// Common Classes > LicensedSet.cs
/// A wrapper class that provides detailed status information about installed data
/// resources, such as days until data and licence expiry, and data version. This
/// method returns information about all datasets, DataPlus files and other third-party
/// datasets. This class is designed for the integrator to obtain diagnostic information regarding
/// the data resources and is not designed for display to web users.
namespace Experian.Qas.Proweb
{
    /// <summary>
    /// Simple class to encapsulates data for a single licensed set.
    /// </summary>
    public class LicensedSet
    {
        /// <summary>
        /// Enumeration of warning levels that can be returned.
        /// </summary>
        public enum WarningLevels
        {
            /// <summary>
            /// This indicates that there are no warnings relating to the data.
            /// </summary>
            None = 0,

            /// <summary>
            /// This indicates that the data is close to its expiry date. The number of days before
            /// this warning is returned can be controlled using the NotifyDataWarning
            /// configuration setting.
            /// </summary>
            DataExpiring,

            /// <summary>
            /// This indicates that the licence that controls the usage of the data is close to its
            /// expiry date. The number of days before this warning is returned can be controlled
            /// using the NotifyLicenceWarning configuration setting.
            /// </summary>
            LicenceExpiring,

            /// <summary>
            /// This indicates that the number of available clicks for the data have run out and
            /// that overdraft clicks are being used.
            /// </summary>
            ClicksLow,

            /// <summary>
            /// This indicates that an evaluation licence is being used for the data.
            /// </summary>
            Evaluation,

            /// <summary>
            /// This indicates that there are no more clicks remaining which can be used to
            /// search against the data, so the data cannot be used.
            /// </summary>
            NoClicks,

            /// <summary>
            /// This indicates that the data has passed its expiry date and so cannot be used.
            /// </summary>
            DataExpired,

            /// <summary>
            /// This indicates that the evaluation licence which controls the use of this data has
            /// expired.
            /// </summary>
            EvalLicenceExpired,

            /// <summary>
            /// This indicates that the full (non-evaluation) licence which controls the use of this
            /// data has expired.
            /// </summary>
            FullLicenceExpired,

            /// <summary>
            /// This indicates that the product is unable to locate a licence for this data, and so it
            /// cannot be used.
            /// </summary>
            LicenceNotFound,

            /// <summary>
            /// This indicates that this data cannot be opened or read, and so is unusable.
            /// </summary>
            DataUnreadable,
        }

        /// <summary>
        /// Gets or sets a short identifier for the data resource.
        /// </summary>
        public string ID
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a text description of the data resource.
        /// </summary>
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a text description of the copyright message for the data.
        /// </summary>
        public string Copyright
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a text description of the data version.
        /// </summary>
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a text description of the base country.
        /// </summary>
        public string BaseCountry
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a text description of the data status.
        /// </summary>
        public string Status
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the server where the data is situated.
        /// </summary>
        public string Server
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the warning level for the data resource.
        /// </summary>
        public WarningLevels WarningLevel
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets or sets the number of days remaining until the data becomes unusable.
        /// </summary>
        public int DaysLeft
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of days before the data resource expires.
        /// </summary>
        public int DataDaysLeft
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the number of days remaining before the licence that
        /// controls the data resource expires.
        /// </summary>
        public int LicenceDaysLeft
        {
            get;
            set;
        }
    }
}
