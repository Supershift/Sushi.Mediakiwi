using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sushi.Mediakiwi.Framework;
using System.Reflection;
using System.Web;

namespace Sushi.Mediakiwi.Data.Parsers
{
    /// <summary>
    /// The environmentVersion entity parser. The following methods are connected to the database and should be overridden when apply a new database Access Layer:
    /// </summary>
    public class EnvironmentVersionParser : IEnvironmentVersionParser
    {
        static ISqlEntityParser _DataParser;
        static ISqlEntityParser DataParser
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
            }
        }

        #region Methods
        public IEnvironmentVersion Select()
        {
            // CB; Cache parameter veranderd van true naar false. Deze moet altijd vers uit de db
            return DataParser.SelectOne<IEnvironmentVersion>(false);
        }
        /// <summary>
        /// Flush all the cached content and it's servernodes
        /// </summary>
        /// <param name="setChacheVersion">if set to <c>true</c> [set chache version].</param>
        /// <param name="context">The context.</param>
        public void Flush(bool setChacheVersion = true, HttpContext context = null)
        {
            if (context == null)
                context = HttpContext.Current;

            Wim.Utilities.CacheItemManager.FlushAll(false);

            IEnvironmentVersion entity = Select();
            if (setChacheVersion)
            {
                entity.Updated = DateTime.UtcNow;
                DataParser.Save<IEnvironmentVersion>(entity);
            }
            entity.ServerEnvironmentVersion = entity.Updated.GetValueOrDefault();
        }
        public bool Save(IEnvironmentVersion entity)
        {
            DataParser.Save<IEnvironmentVersion>(entity);
            return true;
        }
        #endregion Methods
    }
}
