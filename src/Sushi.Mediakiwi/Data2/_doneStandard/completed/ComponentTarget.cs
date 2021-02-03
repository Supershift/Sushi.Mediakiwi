using System;
using System.Data;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace  Sushi.Mediakiwi.Data
{
    [DatabaseTable("wim_ComponentTargets")]
    public partial class ComponentTarget : IComponentTarget
    {
        static IComponentTargetParser _Parser;
        static IComponentTargetParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<IComponentTargetParser>();
                return _Parser;
            }
        }
        
        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

        #region Context
        /// <summary>
        /// Deletes this instance.
        /// </summary>
        public void Delete()
        {
            Parser.Delete(this);
        }

        public static IComponentTarget SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        public static IComponentTarget[] SelectAll(int pageID)
        {
            return Parser.SelectAll(pageID);
        }


        public virtual void DeleteCompetion()
        {
            Parser.DeleteCompetion(this);
        }

        public static IComponentTarget[] SelectAll(Guid componentGuid)
        {
            return Parser.SelectAll(componentGuid);
        }

        public virtual bool Save()
        {
            return Parser.Save(this);
        }

        public bool IsNewInstance
        {
            get { return this.ID == 0; }
        }

        #endregion Context

        #region Properties

        private int m_ID;
        [DatabaseColumn("ComponentTarget_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID { get; set; }

        [DatabaseColumn("ComponentTarget_Page_Key", SqlDbType.Int)]
        public int PageID { get; set; }

        [DatabaseColumn("ComponentTarget_Component_Source", SqlDbType.UniqueIdentifier)]
        public Guid Source { get; set; }

        [DatabaseColumn("ComponentTarget_Component_Target", SqlDbType.UniqueIdentifier, IsNullable = true)]
        public Guid Target { get; set; }

        #endregion Properties

        #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    }
}