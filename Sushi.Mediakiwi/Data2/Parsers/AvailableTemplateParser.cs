using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public class AvailableTemplateParser : IAvailableTemplateParser
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

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public virtual void Clear()
        {
            if (!Wim.CommonConfiguration.IS_LOAD_BALANCED) return;
            DataParser.Execute("truncate table wim_AvailableTemplates");
        }

        ///TODO ConnectionString2?
        /// Geen mogelijkheid voor enkel cacheResult true 
        /// <summary>
        /// Select all available sites
        /// </summary>
        /// <returns>Array of site objects</returns>
        public virtual IAvailableTemplate[] SelectAll()
        {
            return DataParser.SelectAll<IAvailableTemplate>().ToArray();
        }

        /// <summary>
        /// Deletes the specified page template ID.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="isSecundary">if set to <c>true</c> [is secundary].</param>
        /// <param name="target">The target.</param>
        public virtual void Delete(int pageTemplateID, bool isSecundary, string target)
        {
            DataParser.Execute(string.Format("delete from wim_AvailableTemplates where AvailableTemplates_PageTemplate_Key = {0} and AvailableTemplates_IsSecundary = {1}{2}", pageTemplateID, isSecundary ? "1" : "0"
                , string.IsNullOrEmpty(target) ? " and AvailableTemplates_Target is null " : string.Format(" and AvailableTemplates_Target = '{0}'", target)));
        }

        ///Todo separate connectionstring? en connectionType?
        /// <summary>
        /// Deletes the specified page template ID.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <param name="portal">The portal.</param>
        public virtual void Delete(int pageTemplateID, string portal)
        {
            //if (!string.IsNullOrEmpty(portal))
            //{
            //    AvailableTemplate.SqlConnectionString = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Connection;
            //    AvailableTemplate.ConnectionType = Sushi.Mediakiwi.Data.Common.GetPortal(portal).Type;
            //}
            DataParser.Execute(string.Format("delete from wim_AvailableTemplates where AvailableTemplates_PageTemplate_Key = {0}", pageTemplateID));
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="pageTemplateID">The page template ID.</param>
        /// <returns></returns>
        public virtual IAvailableTemplate[] SelectAll(int pageTemplateID, string target = null)
        {
            //if (!string.IsNullOrEmpty(SqlConnectionString2)) IAvailableTemplate.SqlConnectionString = SqlConnectionString2;

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("AvailableTemplates_PageTemplate_Key", SqlDbType.Int, pageTemplateID));

            if (!string.IsNullOrEmpty(target))
                where.Add(new DatabaseDataValueColumn("AvailableTemplates_Target", SqlDbType.VarChar, target));

            string cacheReference = $"AvailableTemplate.{pageTemplateID}${target??"None"}";
            return DataParser.SelectAll<IAvailableTemplate>(where, cacheReference).ToArray();
        }

        public virtual IAvailableTemplate[] SelectAllBySlot(int slotID)
        {
            //if (!string.IsNullOrEmpty(SqlConnectionString2)) IAvailableTemplate.SqlConnectionString = SqlConnectionString2;

            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("AvailableTemplates_Slot", SqlDbType.Int, slotID));

            string cacheReference = $"AvailableTemplate.{slotID}";
            return DataParser.SelectAll<IAvailableTemplate>(where, cacheReference).ToArray();
        }

        public virtual IAvailableTemplate[] SelectAllByComponentTemplate(int templateID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("AvailableTemplates_ComponentTemplate_Key", SqlDbType.Int, templateID));

            string cacheReference = $"AvailableTemplate.{templateID}";
            return DataParser.SelectAll<IAvailableTemplate>(where, cacheReference).ToArray();
        }

        /// <summary>
        /// Selects the all template that are currently not on the page.
        /// </summary>
        /// <param name="pageTemplateID"></param>
        /// <param name="pageID"></param>
        /// <param name="onlyReturnFixedInCode"></param>
        /// <returns></returns>
        public virtual IAvailableTemplate[] SelectAll(int pageTemplateID, int pageID, bool onlyReturnFixedInCode = false)
        {

            string sql = $"select [*] from wim_AvailableTemplates join wim_ComponentTemplates on ComponentTemplate_Key = AvailableTemplates_ComponentTemplate_Key left join wim_ComponentVersions on ComponentVersion_AvailableTemplate_Key = AvailableTemplates_Key and ComponentVersion_Page_Key = {pageID} order by AvailableTemplates_SortOrder ASC";

            DataRequest request = new DataRequest();
            request.AddWhere(nameof(IAvailableTemplate.PageTemplateID), pageTemplateID);

            if (onlyReturnFixedInCode)
            {
                request.AddWhere("not AvailableTemplates_Fixed_Id is null");
            }
            request.AddWhere("ComponentVersion_key is null");
            request.AddWhere("AvailableTemplates_PageTemplate_Key", SqlDbType.Int, pageTemplateID);

            return DataParser.ExecuteList<IAvailableTemplate>(sql, request).ToArray();
            
            //List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            //where.Add(new DatabaseDataValueColumn("AvailableTemplates_PageTemplate_Key", SqlDbType.Int, pageTemplateID));
            //where.Add(new DatabaseDataValueColumn("ComponentVersion_Key", SqlDbType.Int, null));
            //if (onlyReturnFixedInCode)
            //    where.Add(new DatabaseDataValueColumn("not AvailableTemplates_Fixed_Id is null"));

            //return DataParser.SelectAll<IAvailableTemplate>(where).ToArray();
        }


        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="availableTemplateID">The available template identifier.</param>
        /// <returns></returns>
        public virtual IAvailableTemplate SelectOne(int availableTemplateID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("AvailableTemplates_Key", SqlDbType.Int, availableTemplateID));
            return DataParser.SelectOne<IAvailableTemplate>(where, "ID", availableTemplateID.ToString());
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="pageTemplateID">The page template identifier.</param>
        /// <param name="fixedTag">The fixed tag.</param>
        /// <returns></returns>
        public virtual IAvailableTemplate SelectOne(int pageTemplateID, string fixedTag)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("AvailableTemplates_PageTemplate_Key", SqlDbType.Int, pageTemplateID));
            where.Add(new DatabaseDataValueColumn("AvailableTemplates_Fixed_Id", SqlDbType.VarChar, fixedTag));

            return DataParser.SelectOne<IAvailableTemplate>(where);
        }

        public virtual bool Delete(IAvailableTemplate entity)
        {
            return DataParser.Delete(entity);
        }
        public virtual void Save(IAvailableTemplate entity)
        {
            DataParser.Save(entity);
        }
    }
}