namespace Experian.Qas.Proweb
{
    /// <summary>
    /// DataplusGroup is a named collection of items attached to an address line.
    /// </summary>
    public class DataplusGroup
    {
        /// <summary>
        /// Gets or sets the name of this data plus group ( may be an empty string ).
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the data plus items in this group.
        /// </summary>
        public string[] Items
        {
            get;
            set;
        }
    }
}