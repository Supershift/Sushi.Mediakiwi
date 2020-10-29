using System;
namespace Wim.Templates
{
    public interface ISimpleGenerics
    {
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
        /// Gets or sets the data.
        /// </summary>
        /// <value>The data.</value>
        Wim.Data.CustomData Data { get; set; }
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
    }
}
