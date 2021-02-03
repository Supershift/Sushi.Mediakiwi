using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_PageVersions")]
    public partial class PageVersion : IPageVersion
    {
        static IPageVersionParser _Parser;
        static IPageVersionParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IPageVersionParser>();
                return _Parser;
            }
        }

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        public PageVersion()
        {
            this.Created = DateTime.UtcNow;// Sushi.Mediakiwi.Data.Common.DatabaseDateTime;
        }

        public static IPageVersion SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        public virtual bool Save()
        {
            return Parser.Save(this);
        }

        public virtual bool IsNewInstance
        {
            get { return this.ID == 0; }
        }

        internal static List<IPageVersion> SelectAllOfPage(int pageID)
        {
            return Parser.SelectAllOfPage(pageID);
        }

        #region Properties
        [DatabaseColumn("PageVersion_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        [DatabaseColumn("PageVersion_Page_Key", SqlDbType.Int)]
        public virtual int PageID { get; set; }

        [DatabaseColumn("PageVersion_PageTemplate_Key", SqlDbType.Int)]
        public virtual int TemplateID { get; set; }

        [DatabaseColumn("PageVersion_User_Key", SqlDbType.Int)]
        public virtual int UserID { get; set; }

        [DatabaseColumn("PageVersion_Created", SqlDbType.DateTime)]
        public virtual DateTime Created { get; set; }

        [DatabaseColumn("PageVersion_Content", SqlDbType.Xml)]
        public virtual string ContentXML { get; set; }

        [DatabaseColumn("PageVersion_MetaData", SqlDbType.Xml)]
        public virtual string MetaDataXML { get; set; }

        [DatabaseColumn("PageVersion_IsArchived", SqlDbType.Bit)]
        public virtual bool IsArchived { get; set; }

        [DatabaseColumn("PageVersion_Name", SqlDbType.NVarChar)]
        public virtual string Name { get; set; }

        [DatabaseColumn("PageVersion_CompletePath", SqlDbType.VarChar)]
        public virtual string CompletePath { get; set; }

        [DatabaseColumn("PageVersion_Hash", SqlDbType.VarChar)]
        public virtual string Hash { get; set; }

        public virtual string RollBackTo { get; set; }

        #endregion properties

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}