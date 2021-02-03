using System;
namespace Wim.Templates
{
    public interface IGeneric
    {
        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        DateTime Created { get; set; }
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        Wim.Data.CustomData Data { get; set; }
        /// <summary>
        /// Gets or sets the GUID.
        /// </summary>
        /// <value>The GUID.</value>
        Guid GUID { get; set; }
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        int ID { get; set; }
        /// <summary>
        /// Gets or sets the list ID.
        /// </summary>
        /// <value>The list ID.</value>
        int ListID { get; set; }
        /// <summary>
        /// Saves this instance.
        /// </summary>
        /// <returns></returns>
        bool Save();
        /// <summary>
        /// Deletes this instance.
        /// </summary>
        /// <returns></returns>
        bool Delete();
        /// <summary>
        /// Gets or sets the site ID.
        /// </summary>
        /// <value>The site ID.</value>
        int SiteID { get; set; }
    }
}
