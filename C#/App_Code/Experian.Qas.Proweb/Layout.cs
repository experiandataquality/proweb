/// QAS Pro Web > (c) Experian > www.edq.com
/// Common Classes > Layout.cs
/// A wrapper class that encapsulates data that identifies a layout to be used in
/// formatting a final address. The layouts that are relevant for a particular data
/// mapping are defined in the server configuration file.
namespace Experian.Qas.Proweb
{
    /// <summary>
    /// Simple class to encapsulate layout data.
    /// </summary>
    public class Layout
    {
        /// <summary>
        /// Gets or sets the name of a layout. This is the layout name that should be passed to
        /// the getFormattedAddress method to format a picklist item.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a description of the layout.
        /// </summary>
        public string Comment
        {
            get;
            set;
        }

        /// <summary>
        /// Enables the name of a layout to be searched for. This then returns the Layout
        /// object for that layout.
        /// </summary>
        /// <param name="layouts">Array of layouts to search.</param>
        /// <param name="layoutName">Layout name to search for.</param>
        /// <returns>The layout if found, else null.</returns>
        public static Layout FindByName(Layout[] layouts, string layoutName)
        {
            for (int i = 0; i < layouts.GetLength(0); i++)
            {
                if (layouts[i].Name.Equals(layoutName))
                {
                    return layouts[i];
                }
            }

            return null;
        }
    }
}
