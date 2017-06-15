/// QAS Pro Web > (c) Experian > www.edq.com
/// Common Classes > PromptLine.cs
/// A wrapper class that encapsulates information about a prompt set line. An
/// ordered list (array) of prompt lines make up a prompt set.
namespace Experian.Qas.Proweb
{
    /// <summary>
    /// This class encapsulates one line of a search prompt set.
    /// </summary>
    public class PromptLine
    {
        /// <summary>
        /// Gets or sets a hint as to what should be entered into the input field (e.g. Town, Street).
        /// </summary>
        public string Prompt
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets an example of which address element can be entered (e.g. London, Oxford St)
        /// into the prompt line.
        /// </summary>
        public string Example
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the length in characters that is suggested for an input field for this line.
        /// </summary>
        public int SuggestedInputLength
        {
            get;
            set;
        }
    }
}
