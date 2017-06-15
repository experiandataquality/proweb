/// QAS Pro Web > (c) Experian > www.edq.com
/// Common Classes > PromptSet.cs
/// A wrapper class that encapsulates data representing a prompt set. A prompt set
/// consists of an array of prompt set lines. See page 130 for more information about
/// prompt sets.
namespace Experian.Qas.Proweb
{
    using System.Collections.Generic;

    /// <summary>
    /// Simple class to encapsulate data representing a search prompt set.
    /// </summary>
    public class PromptSet
    {
        private List<PromptLine> lines = new List<PromptLine>();

        /// <summary>
        /// Enumeration of available search prompt sets.
        /// </summary>
        public enum Types
        {
            /// <summary>
            /// If just entering one line of address, use this prompt.
            /// </summary>
            OneLine,

            /// <summary>
            /// The default prompt set, tailored to the country.
            /// </summary>
            Default,

            /// <summary>
            /// Generic, country independent.
            /// </summary>
            Generic,

            /// <summary>
            /// The optimal prompt.
            /// </summary>
            Optimal,

            /// <summary>
            /// Alternate 1.
            /// </summary>
            Alternate,

            /// <summary>
            /// Alternate 2.
            /// </summary>
            Alternate2,

            /// <summary>
            /// Alternate 3.
            /// </summary>
            Alternate3
        }

        /// <summary>
        /// Gets or sets a value indicating whether dynamic searching should be used (submitting the search as they type).
        /// </summary>
        public bool IsDynamic
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the array of search prompt lines that make up this search prompt set.
        /// </summary>
        public List<PromptLine> Lines
        {
            get
            {
                return lines;
            }
        }

        public void AddPromptLine(PromptLine promptLine)
        {
            lines.Add(promptLine);
        }

        /// <summary>
        /// Returns a <code>string[]</code> of prompts (from the search prompt line array).
        /// </summary>
        /// <returns>Array of prompt lines.</returns>
        public string[] GetLinePrompts()
        {
            int length = Lines.Count;
            string[] asResults = new string[length];
            for (int i = 0; i < length; i++)
            {
                asResults[i] = Lines[i].Prompt;
            }

            return asResults;
        }
    }
}
