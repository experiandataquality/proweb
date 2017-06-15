/// QAS Pro Web > (c) Experian > www.edq.com
/// Common Classes > FormattedAddress.cs
/// A wrapper class that encapsulates data associated with a final formatted address,
/// namely an array of AddressLines plus various flags.
namespace Experian.Qas.Proweb
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple class to encapsulate data associated with a formatted address.
    /// </summary>
    public class FormattedAddress
    {
        private List<AddressLine> addressLines = new List<AddressLine>();

        /// <summary>
        /// Gets the array of address line objects.
        /// </summary>
        public List<AddressLine> AddressLines
        {
            get
            {
                return this.addressLines;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there were not enough address lines configured to contain the address.
        /// </summary>
        public bool IsOverflow
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether one or more address lines were truncated.
        /// </summary>
        public bool IsTruncated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the DPV State.
        /// </summary>
        public Constants.DPVStatus DPVStatus
        {
            get;
            set;
        }

        /// <summary>
        /// Add an address line to this address. 
        /// </summary>
        /// <param name="line">The line to associate with this address.</param>
        public void AddAddressLine(AddressLine line)
        {
            addressLines.Add(line);
        }
    }
}
