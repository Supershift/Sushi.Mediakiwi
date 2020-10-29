using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Sushi.Mediakiwi.Framework;
using System.Reflection;

namespace Sushi.Mediakiwi.Data.Parsers
{
    /// <summary>
    /// The componentList entity parser. The following methods are connected to the database and should be overridden when apply a new database Access Layer:
    /// </summary>
    public class ComponentListParser : IComponentListParser
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
        /// Adds a thread safe component list entity
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <param name="singleItemName"></param>
        /// <param name="folder"></param>
        /// <param name="site"></param>
        /// <param name="isVisible"></param>
        /// <param name="description"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public virtual IComponentList Add(Guid guid, Type type, string name, string singleItemName, int? folder, int? site, bool isVisible = true, string description = null, ComponentListTarget target = ComponentListTarget.List)
        {
            if (folder.HasValue && !site.HasValue)
                site = Folder.SelectOne(folder.Value).SiteID;


            var assembly = Assembly.GetAssembly(type);
            var instance = Sushi.Mediakiwi.Data.Environment.GetInstance<IComponentList>();
            instance.GUID = guid;
            instance.ClassName = type.ToString();
            instance.AssemblyName = assembly.ManifestModule.Name;
            instance.Name = name;
            instance.SingleItemName = singleItemName;
            instance.IsVisible = isVisible;
            instance.Description = description;
            instance.Target = target;
            instance.FolderID = folder;
            instance.SiteID = site;
            instance.Save();
            return instance;
        }
        /// <summary>
        /// Select all componentList entities
        /// </summary>
        /// <returns></returns>
        public virtual IComponentList[] SelectAll()
        {
            var list = DataParser.SelectAll<IComponentList>(null, "all");
            return list.ToArray();
        }
        /// <summary>
        /// Updates the sort order.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        public virtual bool UpdateSortOrder(int listID, int sortOrder)
        {
            ComponentList tmp = new ComponentList();
            DataParser.Execute(string.Format("update wim_ComponentLists set ComponentList_SortOrder = {0} where ComponentList_Key = {1}", sortOrder, listID));
            return true;
        }
        /// <summary>
        /// Selects all componentList entities from a specific portal
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public virtual IComponentList[] SelectAll(Sushi.Mediakiwi.Framework.WimServerPortal portal)
        {
            string connection = null;
            if (portal != null) connection = portal.Connection;

            var list = DataParser.SelectAll<IComponentList>(null, "all", null, connection);
            return list.ToArray();
        }
        /// <summary>
        /// Select all componentList entities by site(channel).
        /// </summary>
        /// <param name="siteID"></param>
        /// <returns></returns>
        public virtual IComponentList[] SelectAllBySite(int siteID)
        {
            var candidate = (from item in SelectAll() where item.SiteID.HasValue && item.SiteID.Value == siteID orderby item.Name select item);
            return candidate.ToArray();
        }
        /// <summary>
        /// Select all componentList entities by searching by text and optionaly by siteID.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public virtual IComponentList[] SelectAll(string text, int? siteID)
        {
            if (siteID.HasValue && siteID.Value > 0)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var candidate = (from item in SelectAll()
                                     where item.SiteID.HasValue && item.SiteID.Value == siteID.Value
                                        && (item.Target == ComponentListTarget.List || item.Target == ComponentListTarget.Administration)
                                        && item.Name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) > -1
                                     orderby item.Name
                                     select item);
                    return candidate.ToArray();
                }
                else
                {
                    var candidate = (from item in SelectAll()
                                     where item.SiteID.HasValue && item.SiteID.Value == siteID.Value
                                        && (item.Target == ComponentListTarget.List || item.Target == ComponentListTarget.Administration)
                                     orderby item.Name
                                     select item);
                    return candidate.ToArray();
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var candidate = (from item in SelectAll()
                                     where (item.Target == ComponentListTarget.List || item.Target == ComponentListTarget.Administration)
                                        && item.Name.IndexOf(text, StringComparison.CurrentCultureIgnoreCase) > -1
                                     orderby item.Name
                                     select item);
                    return candidate.ToArray();
                }
                else
                {
                    var candidate = (from item in SelectAll()
                                     where (item.Target == ComponentListTarget.List || item.Target == ComponentListTarget.Administration)
                                     orderby item.Name
                                     select item);
                    return candidate.ToArray();
                }
            }
        }
        /// <summary>
        /// Select all componentList entities based on folder and visibility.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        public virtual IComponentList[] SelectAll(int folderID, bool includeHidden = false)
        {
            Sushi.Mediakiwi.Data.Folder folder = Sushi.Mediakiwi.Data.Folder.SelectOne(folderID);

            var all = Sushi.Mediakiwi.Data.ComponentList.SelectAll();

            List<Sushi.Mediakiwi.Data.IComponentList> local;
            if (includeHidden)
                local = (from item in all where item.FolderID.HasValue && item.FolderID.Value == folderID select item).ToList();
            else
                local = (from item in all where item.FolderID.HasValue && item.FolderID.Value == folderID && item.IsVisible select item).ToList();

            //  Get all inherited lists
            while (folder.MasterID.HasValue)
            {
                if (!folder.MasterID.HasValue) break;

                List<Sushi.Mediakiwi.Data.IComponentList> parent;
                if (includeHidden)
                    parent = (from item in all where item.FolderID.HasValue && item.FolderID.Value == folder.MasterID.Value && item.IsInherited select item).ToList();
                else
                    parent = (from item in all where item.FolderID.HasValue && item.FolderID.Value == folder.MasterID.Value && item.IsInherited && item.IsVisible select item).ToList();

                //foreach(var listitem in parent)
                //{
                //    listitem.FolderID = folderID;
                //}

                if (parent.Count > 0)
                    local.AddRange(parent);

                folder = Sushi.Mediakiwi.Data.Folder.SelectOne(folder.MasterID.Value);
            }
            return local.ToArray();
        }
        /// <summary>
        /// Select all componentList entities based on folder, the right of the applicationUser and visibility
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="user">The current application User</param>
        /// <returns></returns>
        public virtual IComponentList[] SelectAll(int folderID, IApplicationUser user, bool includeHidden = false)//, int roleID)
        {
            IComponentList[] lists = null;
            if (!user.Role().All_Lists)
            {
                if (user.Role().IsAccessList)
                {
                    lists = (
                        from item in SelectAll(folderID, includeHidden)
                        join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.Role().ID, RoleRightType.List) on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in SelectAll(folderID, includeHidden)
                        join relation in Sushi.Mediakiwi.Data.RoleRight.SelectAll(user.Role().ID, RoleRightType.List) on item.ID equals relation.ItemID
                        into combination
                        from relation in combination.DefaultIfEmpty()
                        select new { ID = item.ID, HasAccess = relation == null });

                    lists = (
                        from item in acl
                        join relation in SelectAll(folderID, includeHidden) on item.ID equals relation.ID
                        where item.HasAccess
                        select relation).ToArray();
                }
            }
            else
                lists = SelectAll(folderID, includeHidden);
            return lists;
        }
        /// <summary>
        /// Select all componentList entities based on the right of the user and the specific rightType 
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public virtual IComponentList[] SelectAllAccessibleLists(IApplicationUser user, RoleRightType type)
        {
            IComponentList[] lists = null;
            if (!user.Role().All_Lists)
            {
                if (user.Role().IsAccessList)
                {
                    lists = (
                        from item in SelectAll()
                        join relation in RoleRight.SelectAll(user.Role().ID, type) on item.ID equals relation.ItemID
                        select item).ToArray();
                }
                else
                {
                    var acl = (
                        from item in SelectAll()
                        join relation in RoleRight.SelectAll(user.Role().ID, type) on item.ID equals relation.ItemID
                        into combination
                        from relation in combination.DefaultIfEmpty()
                        select new { ID = item.ID, HasAccess = relation == null });

                    lists = (
                        from item in acl
                        join relation in SelectAll() on item.ID equals relation.ID
                        where item.HasAccess
                        select relation).ToArray();
                }
            }
            else
                lists = SelectAll();
            return lists;
        }
        /// <summary>
        /// Extracts the componentLists that are allowed for the specifc user.
        /// </summary>
        /// <param name="lists">The lists.</param>
        /// <param name="user">The user.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        public virtual Sushi.Mediakiwi.Data.IComponentList[] ValidateAccessRight(Sushi.Mediakiwi.Data.IComponentList[] lists, Sushi.Mediakiwi.Data.IApplicationUser user, int siteID)
        {
            return (from item in lists join relation in SelectAllAccessibleLists(user, RoleRightType.List) on item.ID equals relation.ID select item).ToArray();
        }
        /// <summary>
        /// Select all componentList entities that are assigned to a dashboard column
        /// </summary>
        /// <param name="dashboardID">The dashboard ID.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        public virtual IComponentList[] SelectAllDashboardLists(int dashboardID, int column)
        {
            var candidate =
                from item in SelectAll()
                join relation in Dashboard.ListItem.SelectAll(dashboardID) on item.ID equals relation.ListID
                where relation.ColumnID == column
                orderby relation.SortOrder
                select item;
            return candidate.ToArray();
        }
        /// <summary>
        /// Select all scheduled componentLists
        /// </summary>
        /// <returns></returns>
        public virtual IComponentList[] SelectAllScheduled()
        {
            var candidate = (from item in SelectAll() where item.SenseScheduled.HasValue select item);
            return candidate.ToArray();
        }
        /// <summary>
        /// Select a componentList based on it's primary key
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public virtual IComponentList SelectOne(int ID)
        {
            var candidate = (from item in SelectAll() where item.ID == ID select item);
            return candidate.Count() == 0 ? new ComponentList() : candidate.ToArray()[0];
        }
        /// <summary>
        /// Select a componentList based on it's primary key and the corresponding portal
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public virtual IComponentList SelectOne(int ID, Sushi.Mediakiwi.Framework.WimServerPortal portal)
        {
            var candidate = (from item in SelectAll(portal) where item.ID == ID select item);
            return candidate.Count() == 0 ? new ComponentList() : candidate.ToArray()[0];
        }
        /// <summary>
        /// Select a componentList based on it's predefined type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public virtual IComponentList SelectOne(ComponentListType type)
        {
            var candidate = (from item in SelectAll() where item.Type == type select item);
            return candidate.Count() == 0 ? new ComponentList() : candidate.ToArray()[0];
        }
        /// <summary>
        /// Select a componentList based on it's class
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        public virtual IComponentList SelectOne(string className)
        {
            var candidate = (from item in SelectAll() where item.ClassName == className select item);
            return candidate.Count() == 0 ? new ComponentList() : candidate.ToArray()[0];
        }
        /// <summary>
        /// Select a componentList based on it's class beloging to a specific portal
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public virtual IComponentList SelectOne(string className, Sushi.Mediakiwi.Framework.WimServerPortal portal)
        {
            var candidate = (from item in SelectAll(portal) where item.ClassName == className select item);
            return candidate.Count() == 0 ? new ComponentList() : candidate.ToArray()[0];
        }
        /// <summary>
        /// Select a componentList based on it's type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public virtual IComponentList SelectOne(System.Type type)
        {
            return SelectOne(type.ToString());
        }
        /// <summary>
        /// Select a componentList based on it's type belonging to a specific portal
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        public virtual IComponentList SelectOne(System.Type type, string portal)
        {
            if (string.IsNullOrEmpty(portal))
                return SelectOne(type);

            return SelectOne(type.ToString(), Sushi.Mediakiwi.Data.Common.GetPortal(portal));
        }
        /// <summary>
        /// Select a componentList based on it's reference
        /// </summary>
        /// <param name="referenceID">The reference ID.</param>
        /// <returns></returns>
        public virtual IComponentList SelectOneByReference(int referenceID)
        {
            var candidate = (from item in SelectAll() where item.ReferenceID == referenceID select item);
            return candidate.Count() == 0 ? new ComponentList() : candidate.ToArray()[0];
        }
        /// <summary>
        /// Select a componentList based on it's GUID
        /// </summary>
        /// <param name="componentListGUID">The component list GUID.</param>
        /// <returns></returns>
        public virtual  IComponentList SelectOne(Guid componentListGUID)
        {
            var candidate = (from item in SelectAll() where item.GUID == componentListGUID select item);
            IComponentList tmp = (candidate.Count() == 0 ? new ComponentList() : candidate.ToArray()[0]);

            return tmp;
        }
        /// <summary>
        /// Select a componentList based on it's GUID and assign a handler that creates a new list if it does not exists
        /// </summary>
        /// <param name="componentListGUID"></param>
        /// <param name="ifNotPresentCreate">The creation handler</param>
        /// <returns></returns>
        public virtual IComponentList SelectOne(Guid componentListGUID, Func<Guid, IComponentList> ifNotPresentCreate)
        {
            var candidate = (from item in SelectAll() where item.GUID == componentListGUID select item);
            IComponentList tmp;
            if (candidate.Count() == 0)
            {
                if (ifNotPresentCreate == null)
                    tmp = new ComponentList();
                else
                    tmp = ifNotPresentCreate(componentListGUID);
            }
            else
                tmp = candidate.ToArray()[0];

            return tmp;
        }
        /// <summary>
        /// Select a componentList based on it's GUID belonging to a specific portal
        /// </summary>
        /// <param name="componentListGUID"></param>
        /// <param name="portal"></param>
        /// <param name="defaultIsEmpty"></param>
        /// <returns></returns>
        public virtual IComponentList SelectOne(Guid componentListGUID, Sushi.Mediakiwi.Framework.WimServerPortal portal, bool defaultIsEmpty = true)
        {
            var candidate = (from item in SelectAll(portal) where item.GUID == componentListGUID select item);
            return candidate.Count() == 0 ? new ComponentList() : candidate.ToArray()[0];
        }
        /// <summary>
        /// Determine if the user is allowed to view this componentList
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        /// 	<c>true</c> if [has role access] [the specified role id]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasRoleAccess(int componentListID, Sushi.Mediakiwi.Data.IApplicationUser user)
        {
            if (componentListID == 0 || user.Role().All_Lists) return true;
            var selection = from item in SelectAllAccessibleLists(user, RoleRightType.List) where item.ID == componentListID select item;
            bool xs = selection.Count() == 1;
            return xs;
        }
        /// <summary>
        /// Determine if the user is allowed to view this componentList
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="portal">The portal.</param>
        /// <returns>
        ///   <c>true</c> if [has role access2] [the specified user]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool HasRoleAccess(int componentListID, Sushi.Mediakiwi.Data.IApplicationUser user, string portal)
        {
            if (componentListID == 0 || user.Role().All_Lists) return true;
            var selection = RoleRightAccessItem.Select(user.RoleID, (int)RoleRightType.List, (int)RoleRightType.List, portal);
            var candidate = from item in selection where item.ID == componentListID select item;
            bool xs = selection.Count() == 1;
            return xs;
        }
        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        public virtual bool Delete(IComponentList entity)
        {
            DataParser.Execute<object>(string.Concat("delete from wim_RoleRights where RoleRight_Child_Type = 1 and RoleRight_Child_Key = ", entity.ID));
            DataParser.Execute<object>(string.Concat("delete from wim_RoleRights where RoleRight_Child_Type = 1 and RoleRight_Child_Key in (select ComponentList_Key from wim_ComponentLists where ComponentList_Template_Key = ", entity.ID, ")"));
            DataParser.Execute<object>(string.Concat("delete from wim_ComponentLists where ComponentList_Template_Key = ", entity.ID));
            return DataParser.Delete<IComponentList>(entity);
        }
        /// <summary>
        /// Saves the entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        public virtual int Save(IComponentList entity)
        {
            return DataParser.Save<IComponentList>(entity);
        }
    }
}
