namespace Experian.Qas.Proweb
{
    /// <summary>
    /// What information is held on this address line.
    /// </summary>
    public enum LineType
    {
        /// <summary>
        /// Default - this address line is not used. 
        /// </summary>
        None,

        /// <summary>
        /// This address line contains standard address information such as house numbers or postcodes.
        /// </summary>
        Address,

        /// <summary>
        /// This address line contains peoples names.
        /// </summary>
        Name,

        /// <summary>
        /// This contains ancillary information such as TOIDS (UK) or DPIDs (AUS).
        /// </summary>
        Ancillary,

        /// <summary>
        /// This address line contains dataplus information such as lat / long information.
        /// </summary>
        DataPlus,
    }
}