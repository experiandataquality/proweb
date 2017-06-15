/// QAS Pro Web > (c) Experian > www.edq.com
/// Common Classes > SearchResult.cs
/// A wrapper class that encapsulates data that may be returned by an initial search:
/// a Picklist and/or FormattedAddress and VerifyLevel (for verification searches).
namespace Experian.Qas.Proweb
{
    using System;

    /// <summary>
    /// Class to encapsulate data returned by a search.
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// Gets or sets the address (may be null).
        /// </summary>
        public FormattedAddress Address
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the picklist (may be null).
        /// </summary>
        /// <returns></returns>
        public Picklist Picklist
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the verification level of the result (only relevant when using the verification engine).
        /// </summary>
        public VerificationLevel VerifyLevel
        {
            get;
            set;
        }
    }
}