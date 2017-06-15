/// Experian Data Quality > www.edq.com
/// Common Classes > CanSearch.cs
/// Details about searching availability
namespace Experian.Qas.Proweb
{
    /// <summary>
    /// Simple class to encapsulate the result of a CanSearch operation:
    /// searching availability, and the reasons when unavailable.
    /// </summary>
    public class CanSearch
    {
        /// <summary>
        /// Gets or sets a value indicating whether searching is possible for the requested data-engine-layout combination.
        /// </summary>
        public bool IsOk
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets error information relating why it is not possible to search the requested data-engine-layout.
        /// </summary>
        public string ErrorMessage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the error code for this Bulk Search.
        /// </summary>
        public int ErrorCode
        {
            get;
            set;
        }
    }
}
