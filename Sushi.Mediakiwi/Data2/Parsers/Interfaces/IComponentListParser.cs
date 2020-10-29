using System;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IComponentListParser
    {
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
        IComponentList Add(Guid guid, Type type, string name, string singleItemName, int? folder, int? site, bool isVisible = true, string description = null, ComponentListTarget target = ComponentListTarget.List);
        /// <summary>
        /// Select all componentList entities
        /// </summary>
        /// <returns></returns>
        IComponentList[] SelectAll();
        /// <summary>
        /// Updates the sort order.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="sortOrder">The sort order.</param>
        /// <returns></returns>
        bool UpdateSortOrder(int listID, int sortOrder);
        /// <summary>
        /// Selects all componentList entities from a specific portal
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        IComponentList[] SelectAll(Sushi.Mediakiwi.Framework.WimServerPortal portal);
        /// <summary>
        /// Select all componentList entities by site(channel).
        /// </summary>
        /// <param name="siteID"></param>
        /// <returns></returns>
        IComponentList[] SelectAllBySite(int siteID);
        /// <summary>
        /// Select all componentList entities by searching by text and optionaly by siteID.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        IComponentList[] SelectAll(string text, int? siteID);
        /// <summary>
        /// Select all componentList entities based on folder and visibility.
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="includeHidden">if set to <c>true</c> [include hidden].</param>
        /// <returns></returns>
        IComponentList[] SelectAll(int folderID, bool includeHidden = false);
        /// <summary>
        /// Select all componentList entities based on folder, the right of the applicationUser and visibility
        /// </summary>
        /// <param name="folderID">The folder ID.</param>
        /// <param name="user">The current application User</param>
        /// <returns></returns>
        IComponentList[] SelectAll(int folderID, IApplicationUser user, bool includeHidden = false);
        /// <summary>
        /// Select all componentList entities based on the right of the user and the specific rightType 
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IComponentList[] SelectAllAccessibleLists(IApplicationUser user, RoleRightType type);
        /// <summary>
        /// Extracts the componentLists that are allowed for the specifc user.
        /// </summary>
        /// <param name="lists">The lists.</param>
        /// <param name="user">The user.</param>
        /// <param name="siteID">The site ID.</param>
        /// <returns></returns>
        Sushi.Mediakiwi.Data.IComponentList[] ValidateAccessRight(Sushi.Mediakiwi.Data.IComponentList[] lists, Sushi.Mediakiwi.Data.IApplicationUser user, int siteID);
        /// <summary>
        /// Select all componentList entities that are assigned to a dashboard column
        /// </summary>
        /// <param name="dashboardID">The dashboard ID.</param>
        /// <param name="column">The column.</param>
        /// <returns></returns>
        IComponentList[] SelectAllDashboardLists(int dashboardID, int column);
        /// <summary>
        /// Select all scheduled componentLists
        /// </summary>
        /// <returns></returns>
        IComponentList[] SelectAllScheduled();
        /// <summary>
        /// Select a componentList based on it's primary key
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        IComponentList SelectOne(int ID);
        /// <summary>
        /// Select a componentList based on it's primary key and the corresponding portal
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        IComponentList SelectOne(int ID, Sushi.Mediakiwi.Framework.WimServerPortal portal);
        /// <summary>
        /// Select a componentList based on it's predefined type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IComponentList SelectOne(ComponentListType type);
        /// <summary>
        /// Select a componentList based on it's class
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <returns></returns>
        IComponentList SelectOne(string className);
        /// <summary>
        /// Select a componentList based on it's class beloging to a specific portal
        /// </summary>
        /// <param name="className">Name of the class.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        IComponentList SelectOne(string className, Sushi.Mediakiwi.Framework.WimServerPortal portal);
        /// <summary>
        /// Select a componentList based on it's type
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        IComponentList SelectOne(System.Type type);
        /// <summary>
        /// Select a componentList based on it's type belonging to a specific portal
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="portal">The portal.</param>
        /// <returns></returns>
        IComponentList SelectOne(System.Type type, string portal);
        /// <summary>
        /// Select a componentList based on it's reference
        /// </summary>
        /// <param name="referenceID">The reference ID.</param>
        /// <returns></returns>
        IComponentList SelectOneByReference(int referenceID);
        /// <summary>
        /// Select a componentList based on it's GUID
        /// </summary>
        /// <param name="componentListGUID">The component list GUID.</param>
        /// <returns></returns>
        IComponentList SelectOne(Guid componentListGUID);
        /// <summary>
        /// Select a componentList based on it's GUID and assign a handler that creates a new list if it does not exists
        /// </summary>
        /// <param name="componentListGUID"></param>
        /// <param name="ifNotPresentCreate">The creation handler</param>
        /// <returns></returns>
        IComponentList SelectOne(Guid componentListGUID, Func<Guid, IComponentList> ifNotPresentCreate);
        /// <summary>
        /// Select a componentList based on it's GUID belonging to a specific portal
        /// </summary>
        /// <param name="componentListGUID"></param>
        /// <param name="portal"></param>
        /// <param name="defaultIsEmpty"></param>
        /// <returns></returns>
        IComponentList SelectOne(Guid componentListGUID, Sushi.Mediakiwi.Framework.WimServerPortal portal, bool defaultIsEmpty = true);

        /// <summary>
        /// Determine if the user is allowed to view this componentList
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns>
        /// 	<c>true</c> if [has role access] [the specified role id]; otherwise, <c>false</c>.
        /// </returns>
        bool HasRoleAccess(int componentListID, Sushi.Mediakiwi.Data.IApplicationUser user);
        /// <summary>
        /// Determine if the user is allowed to view this componentList
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="portal">The portal.</param>
        /// <returns>
        ///   <c>true</c> if [has role access2] [the specified user]; otherwise, <c>false</c>.
        /// </returns>
        bool HasRoleAccess(int componentListID, Sushi.Mediakiwi.Data.IApplicationUser user, string portal);
        /// <summary>
        /// Delete an implementation record.
        /// </summary>
        /// <returns></returns>
        bool Delete(IComponentList entity);
        /// <summary>
        /// Saves the entity. This method will indentify the need for an INSERT or an UPDATE.
        /// </summary>
        /// <returns></returns>
        int Save(IComponentList entity);
    }
}
