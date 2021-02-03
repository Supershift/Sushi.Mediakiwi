using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_ComponentVersions")]
    public partial class ComponentTargetPage : IComponentTargetPage
    {
        static IComponentTargetPageParser _Parser;
        static IComponentTargetPageParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IComponentTargetPageParser>();
                return _Parser;
            }
        }
        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public static IComponentTargetPage[] SelectAll(int templateID, int siteID)
        {
            return Parser.SelectAll(templateID, siteID);
        }

        #region Properties

        [DatabaseColumn("ComponentVersion_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        [DatabaseColumn("Page_Key", SqlDbType.Int)]
        public virtual int PageID { get; set; }

        [DatabaseColumn("Page_CompletePath", SqlDbType.NVarChar)]
        public virtual string Path { get; set; }

        [DatabaseColumn("Page_IsPublished", SqlDbType.Bit)]
        public virtual bool IsActivePage { get; set; }

        [DatabaseColumn("ComponentVersion_IsActive", SqlDbType.Bit)]
        public virtual bool IsActive { get; set; }

        public virtual int Position { get; set; }

        [DatabaseColumn("ComponentVersion_GUID", SqlDbType.UniqueIdentifier, IsNullable = true)]
        public virtual Guid Version_GUID { get; set; }

        [DatabaseColumn("ComponentTarget_Component_Source", SqlDbType.UniqueIdentifier, IsNullable = true)]
        public virtual Guid Component_Source { get; set; }

        [DatabaseColumn("Name", SqlDbType.NVarChar, IsOnlyRead = true)]
        public virtual string AssignedComponent { get; set; }

        [DatabaseColumn("PublishedCount", SqlDbType.Int)]
        public virtual int PublishedCount { get; set; }

        public virtual bool IsPublished
        {
            get { return PublishedCount > 0; }
        }

        public virtual bool IsNewInstance
        {
            get { return ID == 0; }
        }
        #endregion Properties

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}