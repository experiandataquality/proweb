namespace Experian.Qas.Proweb
{
    /// <summary>
    /// Enumeration of verification levels.
    /// </summary>
    public enum VerificationLevel
    {
        /// <summary> 
        /// This indicates that there was no verification upon the search.
        /// </summary>
        None = 0,

        /// <summary>
        /// This indicates that the address that was searched upon has been matched to a
        /// single deliverable address.
        /// </summary>
        Verified,

        /// <summary>
        /// This indicates that the address that was searched upon has been matched to a
        /// single deliverable address, although user interaction is recommended to confirm
        /// that it is correct.
        /// </summary>
        InteractionRequired,

        /// <summary>
        /// This indicates that the address that was searched upon could not be matched to a
        /// complete deliverable result, and instead has been matched to a partiallycomplete
        /// address at premises level. This implies that there is also a picklist
        /// associated with the partial address.
        /// </summary>
        PremisesPartial,

        /// <summary>
        /// The address that was searched upon could not be matched to a complete
        /// deliverable result, and instead has been matched to a partially-complete address
        /// at street level. This implies that there is also a picklist associated with the partial
        /// address.
        /// </summary>
        StreetPartial,

        /// <summary>
        /// This indicates that the address searched could not be matched to a single
        /// deliverable result, and instead has matched equally to more than one result.
        /// </summary>
        Multiple,

        /// <summary> 
        /// Address was verified to multiple addresses (picklist returned).
        /// </summary>
        VerifiedPlace,

        /// <summary>
        /// Address was verified to multiple addresses (picklist returned).
        /// </summary>
        VerifiedStreet,
    }
}