/// QAS Pro Web > (c) Experian > www.edq.com
/// Common Classes > Picklist.cs
/// This is a wrapper class that encapsulates picklist data. This class defines a set of
/// result items and properties that may be returned at certain stages in the search
/// process. The picklist will be sorted in the order in which it is strongly advised to
/// display the entries.
namespace Experian.Qas.Proweb
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple class to encapsulate Picklist data.
    /// </summary>
    public class Picklist
    {
        private List<PicklistItem> picklistItems = new List<PicklistItem>();

        /// <summary>
        /// Gets or sets the full picklist moniker; that is, the moniker that describes this entire picklist.
        /// </summary>
        public string Moniker
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the collection of PicklistItems.
        /// </summary>
        public List<PicklistItem> PicklistItems
        {
            get
            {
                return picklistItems;
            }
        }

        /// <summary>
        /// Gets or sets the prompt indicating what should be entered next by the user.
        /// </summary>
        public string Prompt
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total number of addresses (excluding informationals) within this address location (approx).
        /// </summary>
        public int Total
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the total number of available results.
        /// </summary>
        public int PotentialMatches
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is safe to automatically step-in to the first (and only) picklist item.
        /// </summary>
        public bool IsAutoStepinSafe
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether you may wish to automatically step-in to the first item, as 
        /// there was only one exact match, and other close matches.
        /// </summary>
        public bool IsAutoStepinPastClose
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether it is safe to automatically format the first (and only) picklist item.
        /// </summary>
        public bool IsAutoFormatSafe
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether you wish to automatically format the first item, as
        /// there was only one exact match, and other close matches.
        /// </summary>
        public bool IsAutoFormatPastClose
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the picklist potentially contains too many items to display.
        /// </summary>
        public bool IsLargePotential
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the number of matches exceeded the maximum allowed.
        /// </summary>
        public bool IsMaxMatches
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether there are more matches.
        /// </summary>
        public bool IsMoreOtherMatches
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether there are additional matches that can be displayed
        /// Only exact matches to the refinement text have been shown, as including all matches would be over threshold
        /// They can be shown by stepping into the informational at the bottom of the picklist.
        /// </summary>
        public bool AreMoreMatches
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the number of matches exceeded the threshold.
        /// </summary>
        public bool IsOverThreshold
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the search timed out.
        /// </summary>
        public bool IsTimeout
        {
            get;
            set;
        }

        /// <summary>
        /// Add a PicklistItem to this Picklist. The picklist items are usually suggestions for full
        /// addresses or address elements (such as towns) to step into further.
        /// </summary>
        /// <param name="item">The populate PicklistItem to add.</param>
        public void AddPicklistItem(PicklistItem item)
        {
            picklistItems.Add(item);
        }
    }
}