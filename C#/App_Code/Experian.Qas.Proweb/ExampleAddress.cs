/// QAS Pro Web > (c) Experian > www.edq.com
/// Common Classes > ExampleAddress.cs
/// A wrapper class that encapsulates example address data, consisting of a sample
/// FormattedAddress and a comment. This is commonly used to see how a given
/// layout works with different styles of address.
namespace Experian.Qas.Proweb
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple class to encapsulate example address data.
    /// </summary>
    public class ExampleAddress
    {
        #region Public Properties

        public FormattedAddress ExAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a comment describing the example address.
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the formatted example address.
        /// </summary>
        public List<AddressLine> AddressLines
        {
            get
            {
                return ExAddress.AddressLines;
            }
        }
        #endregion
    }
}
