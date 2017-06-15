namespace Experian.Qas.Proweb
{
    /// <summary>
    /// Simple class to encapsulate the search items returned by a bulk verification.
    /// </summary>
    public class BulkSearchItem
    {
        /// <summary>
        /// Gets or sets the address (may be null).
        /// </summary>
        public FormattedAddress Address
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the original search address.
        /// </summary>
        /// <returns></returns>
        public string InputAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the verification level of the result (only relevant when using the verification engine).
        /// </summary>
        public VerificationLevel VerifyLevel
        {
            get;
            set;
        }
    }
}
