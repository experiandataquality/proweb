/// QAS Pro Web > (c) Experian > www.edq.com
/// Common Classes > DataSet.cs
/// A wrapper class that encapsulates the name and ID of a data mapping that may
/// be searched upon by QAS Pro Web.
namespace Experian.Qas.Proweb
{
    using System;

    /// <summary>
    /// Simple class to encapsulate a data set - a searchable 'country'.
    /// </summary>
    [Serializable]
    public class Dataset : IComparable
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Dataset" /> class - default constructor.
        /// </summary>
        public Dataset()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Dataset" /> class using the supplied arguments.
        /// </summary>
        /// <param name="id">The ID to use.</param>
        /// <param name="name">The name to use.</param>
        public Dataset(string id, string name)
        {
            ID = id;
            Name = name;
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the data mapping, as text, for display to the
        /// user.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the ID of the data set (DataId).
        /// </summary>
        public string ID
        {
            get;
            private set;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the Dataset which matches the data ID, otherwise null.
        /// </summary>
        /// <param name="datasets">Dataset array to search.</param>
        /// <param name="dataID">Data identifier to search for.</param>
        /// <returns>The found dataset or null.</returns>
        public static Dataset FindByID(Dataset[] datasets, string dataID)
        {
            for (int i = 0; i < datasets.GetLength(0); i++)
            {
                if (datasets[i].ID.Equals(dataID))
                {
                    return datasets[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Implement IComparable interface.
        /// </summary>
        /// <param name="obj">The Dataset to compare to this.</param>
        /// <returns>The result of the comparison.</returns>
        public int CompareTo(object obj)
        {
            if (obj is Dataset)
            {
                Dataset dset = (Dataset)obj;

                return Name.CompareTo(dset.Name);
            }
            else
            {
                throw new ArgumentException("Object is not a Dataset");
            }
        }

        #endregion
    }
}
