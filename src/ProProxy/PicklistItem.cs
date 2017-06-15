/// QAS Pro Web > (c) Experian > www.edq.com
/// Common Classes > PicklistItem.cs
/// This is a wrapper class that encapsulates the data associated with one item in a
/// picklist, which may be related to an address item or to an informational item.
namespace Experian.Qas.Proweb
{
    /// <summary>
    /// Simple class to encapsulate the data associated with one line of a picklist.
    /// </summary>
    public class PicklistItem
    {
        /// <summary>
        /// Gets or sets the moniker representing this item .
        /// </summary>
        public string Moniker
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the string which is displayed to the user for the picklist text.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the postcode for display, may be empty.
        /// </summary>
        public string Postcode
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the percentage score of this item; 0 if not applicable.
        /// </summary>
        public int Score
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the score of this item for display, as "100%", or "" if score not applicable.
        /// </summary>
        public string ScoreAsString
        {
            get
            {
                if (Score > 0)
                {
                    return Score.ToString() + "%";
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets or sets the full address details captured thus far.
        /// </summary>
        public string PartialAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item represents a full deliverable address, so can be formatted.
        /// </summary>
        public bool IsFullAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this item represents multiple addresses (for display purposes).
        /// </summary>
        public bool IsMultipleAddresses
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the item can be stepped into.
        /// </summary>
        public bool CanStep
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is an alias match, which you may wish to highlight to the user.
        /// </summary>
        public bool IsAliasMatch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry has a recoded postcode, which you may wish to highlight to the user.
        /// </summary>
        public bool IsPostcodeRecoded
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is a dummy (for DataSets without premise information)
        /// It can neither be stepped into nor formatted, but must be refined against with premise details.
        /// </summary>
        public bool IsIncompleteAddress
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is a range dummy (for DataSets with only ranges of premise information)
        /// It can neither be stepped into nor formatted, but must be refined against with premise details.
        /// </summary>
        public bool IsUnresolvableRange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is a phantom primary point.
        /// </summary>
        public bool IsPhantomPrimaryPoint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is subsidiary data.
        /// </summary>
        public bool IsSubsidiaryData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry represents a nearby area, outside the strict initial
        /// boundaries of the search, which you may wish to highlight to the user.
        /// </summary>
        public bool IsCrossBorderMatch
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is a dummy PO Box (which you may wish to display differently).
        /// </summary>
        public bool IsDummyPOBox
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is a Names item (which you may wish to display differently).
        /// </summary>
        public bool IsName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is an informational prompt, rather than an address.
        /// </summary>
        public bool IsInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this entry is a warning prompt, indicating that it is not possible to
        /// proceed any further (due to no matches, too many matches, e.t.c.).
        /// </summary>
        public bool IsWarnInformation
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is extended data.
        /// </summary>
        public bool IsExtendedData
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this is enhanced data. 
        /// </summary>
        public bool IsEnhancedData
        {
            get;
            set;
        }
    }
}
