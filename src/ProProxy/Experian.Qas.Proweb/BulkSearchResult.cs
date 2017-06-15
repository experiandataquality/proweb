namespace Experian.Qas.Proweb
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple class to encapsulate the array of results returned by a bulk verification.
    /// </summary>
    public class BulkSearchResult
    {
        private List<BulkSearchItem> bulkSearchItems = new List<BulkSearchItem>();
        private string errorMessage;
        private int errorCode = 0;

        /// <summary>
        /// Gets the bulk search results.
        /// </summary>
        public List<BulkSearchItem> BulkSearchItems
        {
            get
            {
                return bulkSearchItems;
            }
        }

        /// <summary>
        /// Gets or sets the Error Message for this bulk search.
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }

            set
            {
                errorMessage = value;
            }
        }

        /// <summary>
        /// Gets or sets the error code for this Bulk Search.        
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return errorCode;
            }

            set
            {
                errorCode = value;
            }
        }

        public void AddBulkSearchItem(BulkSearchItem item)
        {
            bulkSearchItems.Add(item);
        }
    }
}
