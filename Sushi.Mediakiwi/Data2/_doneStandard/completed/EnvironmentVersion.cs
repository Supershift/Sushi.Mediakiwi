using System;
using System.Linq;
using System.Data;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Web;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("wim_Environments")]
    public class EnvironmentVersion : IEnvironmentVersion
    {
        static IEnvironmentVersionParser _Parser;
        static IEnvironmentVersionParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IEnvironmentVersionParser>();
                return _Parser;
            }
        }

        #region MOVED to EXTENSION / LOGIC

        ///// <summary>
        ///// Flush all the cached content and it's servernodes
        ///// </summary>
        ///// <param name="setChacheVersion">if set to <c>true</c> [set chache version].</param>
        ///// <param name="context">The context.</param>
        //public static void Flush(bool setChacheVersion = true, HttpContext context = null)
        //{
        //    Parser.Flush(setChacheVersion, context);
        //}

        #endregion MOVED to EXTENSION / LOGIC

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        internal static DateTime? m_serverEnvironmentVersion;

        #region Properties
        public virtual DateTime ServerEnvironmentVersion
        {
            get
            {
                return m_serverEnvironmentVersion.GetValueOrDefault();
            }
            set
            {
                m_serverEnvironmentVersion = value;
            }
        }
        /// <summary>
        /// The primary key
        /// </summary>
        [DatabaseColumn("Environment_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        [DatabaseColumn("Environment_Update", SqlDbType.DateTime, IsNullable = true)]
        public virtual DateTime? Updated { get; set; }

        [DatabaseColumn("Environment_Version", SqlDbType.Decimal)]
        public virtual decimal Version { get; set; }
        #endregion Properties

        #region Methods
        public static IEnvironmentVersion Select()
        {
            return Parser.Select();
        }
      
        public bool Save()
        {
            return Parser.Save(this);
        }
        #endregion Methods

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}