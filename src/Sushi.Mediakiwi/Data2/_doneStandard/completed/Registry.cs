using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_Registry", Order = "Registry_Name")]
    public class Registry : IRegistry
    {
        static IRegistryParser _Parser;
        static IRegistryParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IRegistryParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public Registry()
        {
            this.GUID = Guid.NewGuid();
        }

        #region Properties
        [DatabaseColumn("Registry_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        /// <value>The GUID.</value>
        [DatabaseColumn("Registry_GUID", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
        public virtual Guid GUID { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Name", 50, true)]
        [DatabaseColumn("Registry_Name", SqlDbType.VarChar, Length = 50)]
        public virtual string Name { get; set; }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        [DatabaseColumn("Registry_Type", SqlDbType.Int)]
        public virtual int Type { get; set; }
        /// <summary>
        /// Gets the name description.
        /// </summary>
        /// <value>The name description.</value>
        public virtual string NameDescription
        {
            get
            {
                return string.Format("<b>{0}</b><br/>{1}", this.Name, this.Description);
            }
        }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Value", 512, false)]
        [DatabaseColumn("Registry_Value", SqlDbType.NVarChar, Length = 512, IsNullable = true)]
        public virtual string Value { get; set; }
        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("Description", 512, false)]
        [DatabaseColumn("Registry_Description", SqlDbType.NVarChar, Length = 512, IsNullable = true)]
        public virtual string Description { get; set; }
        #endregion Properties

        public virtual void Save()
        {
            Parser.Save(this);
        }

        public virtual void Delete()
        {
            Parser.Delete(this);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public static IRegistry[] SelectAll()
        {
            return Parser.SelectAll();
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The identifier.</param>
        /// <returns></returns>
        public static IRegistry SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        /// <summary>
        /// Selects the name of the one by.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public static IRegistry SelectOneByName(string name)
        {
            return Parser.SelectOneByName(name);
        }

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}
