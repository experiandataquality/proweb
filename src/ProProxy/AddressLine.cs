/// A wrapper class that encapsulates data associated with an address line, namely
/// the formatted text itself plus various flags.
/// Experian Data Quality > www.edq.com
/// Common Classes > AddressLine.cs
namespace Experian.Qas.Proweb
{
    using System.Collections.Generic;

    /// <summary>
    /// AddressLine encapsulates data associated with an address line of a formatted address.
    /// </summary>
    public class AddressLine
    {
        private List<DataplusGroup> dataplusGroups = new List<DataplusGroup>();

        #region Public Properties

        /// <summary>
        /// Gets or sets a text label that describes the contents of a line.
        /// </summary>
        public string Label
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the final formatted address line.
        /// </summary>
        public string Line
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of Data plus groups.
        /// </summary>
        public List<DataplusGroup> DataplusGroups
        {
            get
            {
                return dataplusGroups;
            }
        }

        /// <summary>
        /// Gets or sets the line type (for example, ADDRESS).
        /// </summary>
        public LineType LineType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the address line was too short to fit all of the
        /// formatted address, so the address was cut short.
        /// </summary>
        public bool IsTruncated
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether not all the address line could not fit on this line,
        /// and that the text had to overflow onto other lines.
        /// </summary>
        public bool IsOverflow
        {
            get;
            set;
        }

        #endregion

        /// <summary>
        /// Add a Dataplus group to our line. Dataplus group contain data plus information!
        /// This may include electricity meter numbers in the UK, Tiger co-cordinates for the US.
        /// </summary>
        /// <param name="group">The dataplus group to associate with this address line.</param>
        public void AddDataPlusGroup(DataplusGroup group)
        {
            dataplusGroups.Add(group);
        }
    }
}
