namespace Experian.Qas.Proweb
{
    /// <summary>
    /// The Engine describes the type of search we want to perform. The correct engine to use depends
    /// upon the use case, the country that you are capturing an address for and the information you are 
    /// gathering. Consult the descriptions below and your manual for help to make this decision.
    /// </summary>
    public class Engine
    {
        public enum PromptSets
        {
            /// <summary>
            /// When prompting for just one line of input (e.g. in the UK house number and postcode).
            /// </summary>
            OneLine,

            /// <summary>
            /// The best prompt set for the country. For example in the UK this will be two prompts - house
            /// number and postcode. For the US, this will be 5 lines - Address line 1, address line 2, city
            /// state and zip.
            /// </summary>
            Default,

            /// <summary>
            /// Standard input across all countries.
            /// </summary>
            Generic,

            /// <summary>
            /// Minimum input for a country.
            /// </summary>
            Optimal,

            /// <summary>
            /// Country specific alternative. Not available for all countries. 
            /// </summary>
            Alternate,

            /// <summary>
            /// Country specific alternative. Not available for all countries. 
            /// </summary>
            Alternate2,

            /// <summary>
            /// Country specific alternative. Not available for all countries. 
            /// </summary>
            Alternate3,
        }

        /// <summary>
        /// Enumeration of engine types.
        /// </summary>
        public enum EngineTypes
        {
            /// <summary>
            /// The single line engine. 
            /// This works best for rapid address capture. Give it a few address (e.g. house number and postcode in GBR)
            /// components and it will complete the address for you. 
            /// </summary>
            Singleline,

            /// <summary>
            /// The Typedown engine.
            /// This allows you to drill down into an address. Start with a town for example, and then search within
            /// that town.
            /// </summary>
            Typedown,

            /// <summary>
            /// The Verification engine.
            /// Validates an address. Give it most of an address and the verification engine will correct and enhance
            /// the address with missing or incorrect elements.
            /// </summary>
            Verification,

            /// <summary>
            /// The Keyfinder engine.
            /// Used with certain data sets that we have keys for. These keys vary but include Utility meter numbers,
            /// phone numbers, TOIDs.
            /// </summary>
            Keyfinder,

            /// <summary>
            /// The Intuitive engine.
            /// Enter an address exactly as you would say it and the intuitive engine will complete it after just a few keys.
            /// </summary>
            Intuitive,
        }

        /// <summary>
        /// Enumeration of engine searching intensity levels.
        /// </summary>
        public enum EngineIntensity
        {
            /// <summary>
            /// Exact matching. Quickest search mode. Only use with high quality input. 
            /// Does not allow for user spelling mistakes.
            /// </summary>
            Exact,

            /// <summary>
            /// Default setting. Allows some fuzzy matching, the best trade off between speed and time spent
            /// searching for additional matches.
            /// </summary>
            Close,

            /// <summary>
            /// When using dirty input data and performance is not important. This will allow for more mistakes in
            /// your input data. 
            /// </summary>
            Extensive,
        }

        /// <summary>
        /// Gets or sets a value indicating whether search results will be 'flattened' to a single 
        /// picklist of deliverable results, or returned as hierarchical picklists of results that 
        /// can be stepped into. 
        /// Flattened picklists are simpler for untrained users to navigate (i.e. website users),
        /// although can potentially lead to larger number of results. Hierarchical picklists are 
        /// more suited to users who are trained in using the product (i.e. intranet users), and 
        /// produce smaller picklists with results that can be stepped into. 
        /// If this property is not set before search, the default value of False is used,
        /// resulting in hierarchical picklists.
        /// </summary>
        public bool Flatten
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets engine intensity; that is, how hard the search engine will work to
        /// obtain a match. Higher intensity values may yield more results than lower
        /// intensity values, but will also result in longer search times. If this is not set
        /// before search, the server will use the value from the configuration file.
        /// </summary>
        public EngineIntensity Intensity
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the PromptSet to use. Whilst for Experian Core countries, the prompt set supplied
        /// is not important - our partner sources data sets require that you supply this information.
        /// </summary>
        public PromptSets Prompt
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the threshold that is used to control the number of items 
        /// displayed in a picklist, following a refinement or a step-in. Where the number of 
        /// matches exceeds the threshold, an informational picklist result will be returned, 
        /// advising the user to refine the picklist further.
        /// </summary>
        public string Threshold
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the search timeout period; that is, the time threshold in 
        /// milliseconds that will cause a search to abort if exceeded. If this property is not 
        /// set before search, stepIn or refine, the server will use the value from the 
        /// configuration file.
        /// </summary>
        public string Timeout
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets any search constraints. This is only used by some country / engine combinations
        /// such as USA intuitive where it allows you to restrict your searches and responses to a particular
        /// state.
        /// </summary>
        public string Constraint
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the current engine; if left unset, the search will use the default, SingleLine.
        /// </summary>
        public EngineTypes EngineType
        {
            get;
            set;
        }
    }
}
