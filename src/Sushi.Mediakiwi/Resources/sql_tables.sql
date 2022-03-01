	CREATE TABLE wim_Installers(
	    Installer_Key int IDENTITY(1,1) NOT NULL,
	    Installer_GUID uniqueidentifier NOT NULL,
	    Installer_Folder_Key int NULL,
	    Installer_Name nvarchar(50) NOT NULL,
	    Installer_Assembly varchar(150) NULL,
	    Installer_ClassName varchar(250) NULL,
	    Installer_Description nvarchar(1024) NULL,
	    Installer_Settings xml NULL,
	    Installer_Version int NOT NULL,
	    Installer_Installed datetime NULL);
	
	--PK_Installer_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Installer_Key';    
	THEN:ALTER TABLE wim_Installers WITH NOCHECK ADD CONSTRAINT PK_Installer_Key PRIMARY KEY CLUSTERED (Installer_Key);

    CREATE TABLE wim_Galleries(
	    Gallery_Key int IDENTITY(1,1) NOT NULL,
	    Gallery_GUID uniqueidentifier NOT NULL,
	    Gallery_Gallery_Key int NULL,
	    Gallery_Name nvarchar(50) NOT NULL,
	    Gallery_CompletePath nvarchar(1000) NOT NULL,
	    Gallery_Type smallint NOT NULL,
	    Gallery_Created datetime NOT NULL,
	    Gallery_IsFixed bit NOT NULL,
	    Gallery_Format varchar(50) NULL,
	    Gallery_FormatType int NULL,
	    Gallery_BackgroundRgb varchar(14) NULL,
	    Gallery_IsFolder bit NOT NULL,
	    Gallery_Base_Key int NULL,
	    Gallery_IsActive bit NULL,
		Gallery_IsHidden bit NULL,
	    Gallery_Count int NULL);
	
	--PK_Gallery_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Gallery_Key';    
	THEN:ALTER TABLE wim_Galleries WITH NOCHECK ADD CONSTRAINT PK_Gallery_Key PRIMARY KEY CLUSTERED (Gallery_Key);
	
	CREATE TABLE wim_PageVersions(
		PageVersion_Key int IDENTITY(1,1) NOT NULL,
		PageVersion_User_Key int NOT NULL,
		PageVersion_Page_Key int NOT NULL,
		PageVersion_PageTemplate_Key int NOT NULL,
		PageVersion_Content xml NOT NULL,
		PageVersion_MetaData xml NOT NULL,
		PageVersion_Created datetime NOT NULL,
		PageVersion_IsArchived bit NOT NULL,
		PageVersion_Name nvarchar(150) NOT NULL,
		PageVersion_CompletePath varchar(500) NOT NULL,
		PageVersion_Hash varchar(40) NOT NULL);
		
	--PK_Gallery_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_PageVersion_Key';    
	THEN:ALTER TABLE wim_PageVersions WITH NOCHECK ADD CONSTRAINT PK_PageVersion_Key PRIMARY KEY CLUSTERED (PageVersion_Key);
	
	CREATE TABLE wim_CacheItems(
		CacheItem_Key int IDENTITY(1,1) NOT NULL,
		CacheItem_Name varchar(512) NOT NULL,
		CacheItem_IsIndex bit NULL,
		CacheItem_Created datetime NOT NULL);
		
	--PK_Gallery_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_CacheItem_Key';    
	THEN:ALTER TABLE wim_CacheItems WITH NOCHECK ADD CONSTRAINT PK_CacheItem_Key PRIMARY KEY CLUSTERED (CacheItem_Key);

	CREATE TABLE wim_AssetTypes(
	AssetType_Key int IDENTITY(1,1) NOT NULL,
	AssetType_Guid uniqueidentifier NOT NULL,
	AssetType_Name nvarchar(50) NOT NULL,
	AssetType_Tag varchar(20) NOT NULL,
	AssetType_Created datetime NOT NULL,
	AssetType_IsVariant bit NULL);

	--PK_AssetType_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_AssetType_Key';  
	ALTER TABLE wim_AssetTypes WITH NOCHECK ADD CONSTRAINT PK_AssetType_Key PRIMARY KEY CLUSTERED (AssetType_Key);

	CREATE TABLE wim_NewsItems(
		NewsItem_Key int IDENTITY(1,1) NOT NULL,
		NewsItem_List_Key int NOT NULL,
		NewsItem_CreatedBy int NOT NULL,
		NewsItem_Guid uniqueidentifier NOT NULL,
		NewsItem_Title nvarchar(255) NULL,
		NewsItem_Description nvarchar(2048) NULL,
		NewsItem_Data xml NULL,
		NewsItem_Created datetime NOT NULL,
		NewsItem_Updated datetime NULL);

	--PK_NewsItem_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_NewsItem_Key';    
	THEN:ALTER TABLE wim_NewsItems WITH NOCHECK ADD CONSTRAINT PK_NewsItem_Key PRIMARY KEY CLUSTERED (NewsItem_Key);	

	CREATE TABLE wim_AuditTrails(
		AuditTrail_Key int IDENTITY(1,1) NOT NULL,
		AuditTrail_Type int NULL,
		AuditTrail_Action int NULL,
		AuditTrail_Entity_Key int NULL,
		AuditTrail_Listitem_Key int NULL,
		AuditTrail_Versioning_Key int NULL,
		AuditTrail_Message nvarchar(512) NULL,
		AuditTrail_Created datetime NULL);    	

	--PK_AuditTrail_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_NewsItem_Key';    
	THEN:ALTER TABLE wim_AuditTrails WITH NOCHECK ADD CONSTRAINT PK_AuditTrail_Key PRIMARY KEY CLUSTERED (AuditTrail_Key);

    CREATE TABLE wim_Links(
	    Link_Key int IDENTITY(1,1) NOT NULL,
	    Link_GUID uniqueidentifier NOT NULL,
	    Link_Text nvarchar(500) NOT NULL,
	    Link_Target int NOT NULL,
	    Link_IsInternal bit NOT NULL,
	    Link_Page_Key int NULL,
		link_asset_key int NULL,
	    Link_ExternalUrl varchar(500) NULL,
	    Link_Description nvarchar(500) NULL,
	    Link_Created datetime NULL);
	    
    --PK_Link_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Link_Key';    
	THEN:ALTER TABLE wim_Links WITH NOCHECK ADD CONSTRAINT PK_Link_Key PRIMARY KEY CLUSTERED (Link_Key);

    CREATE TABLE wim_Assets(
	    Asset_Key int IDENTITY(1,1) NOT NULL,
	    Asset_Asset_Key int NULL,
		Asset_GUID uniqueidentifier NOT NULL,
	    Asset_AssetType_Key int NULL,
		Asset_Migration_Key int NULL,
	    Asset_Gallery_Key int NOT NULL,
	    Asset_Filename varchar(255) NULL,
	    Asset_Title varchar(255) NULL,
	    Asset_Description ntext NULL,
	    Asset_Type varchar(150) NOT NULL,
	    Asset_Extention varchar(5) NULL,
	    Asset_Size int NOT NULL,
	    Asset_Created datetime NOT NULL,
		Asset_Updated datetime NULL,
	    Asset_Width int NULL,
	    Asset_Height int NULL,
	    Asset_IsActive bit NOT NULL,
	    Asset_OldStyle bit NOT NULL,
	    Asset_NewStyle bit NOT NULL,
	    Asset_IsImage bit NOT NULL,
	    Asset_Data xml NULL,
	    Asset_RemoteDownload bit null,
	    Asset_RemoteLocation varchar(512) NULL, 
	    Asset_IsArchived bit NULL DEFAULT(0),
		Asset_RemoteLocation_Thumb varchar(512) NULL,
		Asset_SortOrder int NULL);
	    
    --PK_Asset_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Asset_Key';    
	THEN:ALTER TABLE wim_Assets WITH NOCHECK ADD CONSTRAINT PK_Asset_Key PRIMARY KEY CLUSTERED (Asset_Key);

	--Asset Sortorder update;
	IF>:select COUNT(*) from wim_Assets where Asset_SortOrder is null;    
	THEN:update wim_Assets set Asset_SortOrder = Asset_Key where Asset_SortOrder is null;

    CREATE TABLE wim_Environments(
	    Environment_Key int IDENTITY(1,1) NOT NULL,
	    Environment_Name nvarchar(50) NOT NULL,
	    Environment_Timezone varchar(30) NULL,
	    Environment_Created datetime NOT NULL,
	    Environment_Repository varchar(255) NULL,
	    Environment_RepositoryFolder varchar(255) NOT NULL,
	    Environment_Smtp varchar(250) NULL,
	    Environment_DefaultMail varchar(255) NULL,
	    Environment_ErrorMail varchar(255) NULL,
	    Environment_Version decimal(18,2) NOT NULL,
	    Environment_Default_Site_Key int NULL,
	    Environment_Update datetime NULL,
	    Environment_Title nvarchar(50) NULL,
	    Environment_Password varchar(50) NULL,
	    Environment_SmtpUser varchar(250) NULL,
	    Environment_SmtpPass varchar(250) NULL,
		Environment_SmtpEnableSSL bit NULL,
		Environment_ApiKey varchar(250) NULL,
		);
	    
    --PK_Environment_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Environment_Key';    
	THEN:ALTER TABLE wim_Environments WITH NOCHECK ADD CONSTRAINT PK_Environment_Key PRIMARY KEY CLUSTERED (Environment_Key);

    CREATE TABLE wim_PageTemplates(
	    PageTemplate_Key int IDENTITY(1,1) NOT NULL,
	    PageTemplate_GUID uniqueidentifier NOT NULL,
	    PageTemplate_Name nvarchar(50) NOT NULL,
		PageTemplate_Data xml NULL,
	    PageTemplate_Description nvarchar(500) NULL,
	    PageTemplate_Location varchar(250) NOT NULL,
	    PageTemplate_HasSecundaryContainer bit NOT NULL,
	    PageTemplate_AddToOutputCache bit NOT NULL,
		PageTemplate_OutputCache int NULL,
	    PageTemplate_Site_Key int NULL,
	    PageTemplate_OnlyOneInstance bit NOT NULL,
	    PageTemplate_ReferenceId int NULL,
	    PageTemplate_HasCustomDate bit NULL,
	    PageTemplate_LastWriteTimeUtc datetime NULL,
		PageTemplate_OverwriteSite_Key int null,
		PageTemplate_OverwritePageTemplate_Key int null,
		PageTemplate_IsSourceBased bit null,
		PageTemplate_Source nvarchar(max) null
		);
	    
    --PK_PageTemplate_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_PageTemplate_Key';    
	THEN:ALTER TABLE wim_PageTemplates WITH NOCHECK ADD CONSTRAINT PK_PageTemplate_Key PRIMARY KEY CLUSTERED (PageTemplate_Key);

    CREATE TABLE wim_Countries(
	    Country_Key int IDENTITY(1,1) NOT NULL,
	    Country_Guid uniqueidentifier NOT NULL,
	    Country_Name_EN nvarchar(100) NOT NULL,
	    Country_Name_NL nvarchar(100) NOT NULL,
	    Country_ISO2 varchar(2) NULL,
	    Country_ISO3 varchar(3) NULL,
	    Country_IsActive bit NOT NULL);
	    
    --PK_Country_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Country_Key';    
	THEN:ALTER TABLE wim_Countries WITH NOCHECK ADD CONSTRAINT PK_Country_Key PRIMARY KEY CLUSTERED (Country_Key);

    --IX_wim_Countries;
	IF:select COUNT(*) from sys.indexes where name = 'IX_wim_Countries';    
	THEN:CREATE UNIQUE NONCLUSTERED INDEX IX_wim_Countries ON wim_Countries(Country_Guid); 

    CREATE TABLE wim_ComponentTemplates(
	    ComponentTemplate_Key int IDENTITY(1,1) NOT NULL,
	    ComponentTemplate_GUID uniqueidentifier NOT NULL,
	    ComponentTemplate_Name nvarchar(50) NOT NULL,
	    ComponentTemplate_Description nvarchar(500) NULL,
	    ComponentTemplate_Location varchar(250) NOT NULL,
	    ComponentTemplate_Type nvarchar(250) NULL,
	    ComponentTemplate_IsListTemplate bit NOT NULL,
	    ComponentTemplate_IsFixed bit NOT NULL,
	    ComponentTemplate_IsSecundaryContainerItem bit NOT NULL,
	    ComponentTemplate_Site_Key int NULL,
	    ComponentTemplate_IsSearchable bit NOT NULL,
	    ComponentTemplate_MetaData xml NULL,
	    ComponentTemplate_ReferenceId int NULL,
	    ComponentTemplate_CacheParams varchar(50) NULL,
	    ComponentTemplate_LastWriteTimeUtc datetime NULL,
	    ComponentTemplate_CanReplicate bit NOT NULL,
	    ComponentTemplate_CanDeactivate bit NOT NULL,
	    ComponentTemplate_CanMove bit NOT NULL,
	    ComponentTemplate_IsHeader bit NOT NULL,
	    ComponentTemplate_IsFooter bit NOT NULL,
	    ComponentTemplate_CacheLevel int NULL,
	    ComponentTemplate_AjaxType int NULL,
	    ComponentTemplate_IsShared bit NULL,
		ComponentTemplate_Source ntext NULL,
	    ComponentTemplate_Tag varchar(25) NULL,
		ComponentTemplate_NestType int NULL);
	    
    --PK_ComponentTemplate_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_ComponentTemplate_Key';    
	THEN:ALTER TABLE wim_ComponentTemplates WITH NOCHECK ADD CONSTRAINT PK_ComponentTemplate_Key PRIMARY KEY CLUSTERED (ComponentTemplate_Key);

    CREATE TABLE wim_Portals(
	    Portal_Key int IDENTITY(1,1) NOT NULL,
	    Portal_GUID uniqueidentifier NOT NULL,
	    Portal_User_Key int NULL,
	    Portal_Name nvarchar(50) NOT NULL,
	    Portal_Domain nvarchar(50) NOT NULL,
	    Portal_Created datetime NOT NULL,
	    Portal_Authenticode varchar(150) NULL,
	    Portal_Authentication varchar(150) NULL,
	    Portal_IsActive bit NOT NULL,
	    Portal_Data xml NULL);
	    
    --PK_Portal_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Portal_Key';    
	THEN:ALTER TABLE wim_Portals WITH NOCHECK ADD CONSTRAINT PK_Portal_Key PRIMARY KEY CLUSTERED (Portal_Key);

    CREATE TABLE wim_Sites(
	    Site_Key int IDENTITY(1,1) NOT NULL,
	    Site_Master_Key int NULL,
	    Site_Type int NULL,
	    Site_Country int NULL,
	    Site_HomePage_Key int NULL,
	    Site_PageNotFoundPage_Key int NULL,
	    Site_ErrorPage_Key int NULL,
	    Site_Displayname nvarchar(250) NOT NULL,
	    Site_ImageFolder varchar(500) NULL,
	    Site_ImageThumbnailFolder varchar(500) NULL,
	    Site_DocumentFolder varchar(500) NULL,
	    Site_DefaultFolder nvarchar(500) NULL,
	    Site_DefaultTitle nvarchar(150) NULL,
	    Site_Staging varchar(100) NULL,
	    Site_Production varchar(100) NULL,
	    Site_Created datetime NOT NULL,
	    Site_Culture varchar(15) NULL,
	    Site_Language varchar(8) NULL,
	    Site_IsActive bit NOT NULL,
	    Site_HasPages bit NOT NULL,
	    Site_HasLists bit NOT NULL,
	    Site_InheritStructure bit NOT NULL DEFAULT(0),
	    Site_GUID uniqueidentifier NOT NULL,
	    Site_TimeZone nvarchar(50) NULL,
	    Site_IsRemote bit NOT NULL DEFAULT(0),
	    Site_AutoPublishInherited bit NOT NULL DEFAULT(0),
	    Site_Domain varchar(255) NULL);
	    
    --PK_Site_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Site_Key';    
	THEN:ALTER TABLE wim_Sites WITH NOCHECK ADD CONSTRAINT PK_Site_Key PRIMARY KEY CLUSTERED (Site_Key);

    CREATE TABLE wim_PropertySearchColumns(
	    PropertySearchColumn_Key int IDENTITY(1,1) NOT NULL,
	    PropertySearchColumn_Property_Key int NOT NULL,
	    PropertySearchColumn_List_Key int NOT NULL,
	    PropertySearchColumn_Title nvarchar(50) NULL,
	    PropertySearchColumn_TotalType int NOT NULL,
	    PropertySearchColumn_Width int NULL,
	    PropertySearchColumn_IsHighlight bit NOT NULL,
	    PropertySearchColumn_IsExport bit NOT NULL,
	    PropertySearchColumn_SortOrder int NULL);
	    
    --PK_PropertySearchColumn_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_PropertySearchColumn_Key';    
	THEN:ALTER TABLE wim_PropertySearchColumns WITH NOCHECK ADD CONSTRAINT PK_PropertySearchColumn_Key PRIMARY KEY CLUSTERED (PropertySearchColumn_Key);

    CREATE TABLE wim_ComponentSearch(
	    ComponentSearch_Key int IDENTITY(1,1) NOT NULL,
	    ComponentSearch_Type smallint NOT NULL,
	    ComponentSearch_Ref_Key int NOT NULL,
	    ComponentSearch_Text nvarchar(4000) NULL,
	    ComponentSearch_Site_Key int NULL);
	    
    --PK_ComponentSearch_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_ComponentSearch_Key';    
	THEN:ALTER TABLE wim_ComponentSearch WITH NOCHECK ADD CONSTRAINT PK_ComponentSearch_Key PRIMARY KEY CLUSTERED (ComponentSearch_Key);

    CREATE TABLE wim_Properties(
	    Property_Key int IDENTITY(1,1) NOT NULL,
	    Property_Property_Key int NULL,
	    Property_GUID uniqueidentifier NOT NULL,
	    Property_List_Key int NOT NULL,
	    Property_List_Type_Key int NULL,
	    Property_FieldName varchar(35) NOT NULL,
	    Property_Title nvarchar(255) NULL,
	    Property_Type int NOT NULL,
	    Property_Data xml NULL,
	    Property_IsFixed bit NOT NULL,
	    Property_IsShort bit NOT NULL,
	    Property_ListBase_Key int NULL,
	    Property_ListCollection varchar(50) NULL,
	    Property_IsFilter bit NULL,
	    Property_SortOrder int NULL,
	    Property_Column varchar(50) NULL,
	    Property_ColumnType varchar(15) NULL,
	    Property_CodeType int NULL,
	    Property_IsPresent bit NULL,
	    Property_OptionList_Key int NULL,
	    Property_CanFilter bit NULL,
	    Property_IsHidden bit NULL,
	    Property_OnlyInput bit NULL,
		Property_Template_Key int NULL,
		Property_Help nvarchar(max) NULL,
		Property_IsRequired bit NULL,
		Property_MaxInput INT NULL,
		Property_Default nvarchar(max) NULL,
		Property_IsShared BIT NULL DEFAULT(0));
	    
    --PK_Property_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Property_Key';    
	THEN:ALTER TABLE wim_Properties WITH NOCHECK ADD CONSTRAINT PK_Property_Key PRIMARY KEY CLUSTERED (Property_Key);

	CREATE TABLE wim_ComponentTargets(
		ComponentTarget_Key int IDENTITY(1,1) NOT NULL,
		ComponentTarget_Page_Key int NOT NULL,
		ComponentTarget_Component_Source uniqueidentifier NOT NULL,
		ComponentTarget_Component_Target uniqueidentifier NULL);

	--PK_ComponentTarget_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_ComponentTarget_Key';    
	THEN:ALTER TABLE wim_ComponentTargets WITH NOCHECK ADD CONSTRAINT PK_ComponentTarget_Key PRIMARY KEY CLUSTERED (ComponentTarget_Key);

    CREATE TABLE wim_Notifications(
	    Notification_Key int IDENTITY(1,1) NOT NULL,
	    Notification_Page_Key int NULL,
	    Notification_Visitor_Key int NULL,
	    Notification_Type nvarchar(50) NOT NULL,
	    Notification_Text text NULL,
	    Notification_XML xml NULL,
	    Notification_User int NULL,
	    Notification_Created datetime NULL,
	    Notification_Selection int NULL);
	    
    --PK_Notification_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Notification_Key';    
	THEN:ALTER TABLE wim_Notifications WITH NOCHECK ADD CONSTRAINT PK_Notification_Key PRIMARY KEY CLUSTERED (Notification_Key);

    CREATE TABLE wim_Visitors(
	    Visitor_Key int IDENTITY(1,1) NOT NULL,
	    Visitor_GUID uniqueidentifier NOT NULL,
	    Visitor_Profile_Key int NULL,
	    Visitor_Created datetime NOT NULL,
		Visitor_Updated dateTime NULL,
		Visitor_IsLoggedIn bit NULL,
	    Visitor_RememberMe bit NOT NULL,
	    Visitor_Data xml NULL);
	    
    --PK_Visitor_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Visitor_Key';    
	THEN:ALTER TABLE wim_Visitors WITH NOCHECK ADD CONSTRAINT PK_Visitor_Key PRIMARY KEY CLUSTERED (Visitor_Key);

	--IX_wim_Visitors_GUID;
	IF:select COUNT(*) from sys.indexes where name = 'IX_wim_Visitors_GUID';   
	THEN:CREATE NONCLUSTERED INDEX IX_wim_Visitors_GUID ON dbo.wim_Visitors(Visitor_GUID);


    CREATE TABLE wim_Profiles(
	    Profile_Key int IDENTITY(1,1) NOT NULL,
	    Profile_Guid uniqueidentifier NOT NULL,
	    Profile_Created datetime NOT NULL,
	    Profile_RememberMe bit NULL,
	    Profile_Email nvarchar(255) NULL,
	    Profile_Password nvarchar(15) NULL,
	    Profile_Data xml NULL);
	    
    --PK_Profile_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Profile_Key';    
	THEN:ALTER TABLE wim_Profiles WITH NOCHECK ADD CONSTRAINT PK_Profile_Key PRIMARY KEY CLUSTERED (Profile_Key);

    CREATE TABLE wim_Roles(
	    Role_Key int IDENTITY(1,1) NOT NULL,
	    Role_Name nvarchar(50) NOT NULL,
	    Role_Description nvarchar(255) NULL,
	    Role_CanSeePage bit NOT NULL,
	    Role_CanSeeList bit NOT NULL,
	    Role_CanSeeAdmin bit NOT NULL,
	    Role_CanSeeGallery bit NOT NULL,
		Role_CanSeeFolder bit NOT NULL,
	    Role_Page_CanChange bit NOT NULL,
	    Role_Page_CanCreate bit NOT NULL,
	    Role_Page_CanPublish bit NOT NULL,
	    Role_Page_CanDelete bit NOT NULL,
	    Role_All_Sites bit NOT NULL,
	    Role_All_Lists bit NOT NULL,
	    Role_List_CanCreate bit NULL,
	    Role_List_CanChange bit NULL,
	    Role_Dashboard int NULL,
	    Role_GUID uniqueidentifier NULL,
	    Role_List_IsAccessList bit NULL,
	    Role_List_IsAccessSite bit NULL,
	    Role_List_IsAccessFolder bit NULL,
	    Role_List_IsAccessGallery bit NULL,
	    Role_All_Folders bit NULL,
		Role_Gallery_Key int NULL,
	    Role_All_Galleries bit NULL);
	    
    --PK_Role_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Role_Key';    
	THEN:ALTER TABLE wim_Roles WITH NOCHECK ADD CONSTRAINT PK_Role_Key PRIMARY KEY CLUSTERED (Role_Key);

    CREATE TABLE wim_Users(
	    User_Key int IDENTITY(1,1) NOT NULL,
	    User_Role_Key int NULL,
	    User_Name nvarchar(50) NOT NULL,
	    User_Email nvarchar(255) NOT NULL,
	    User_Password nvarchar(50) NULL,
		User_Reset uniqueidentifier NULL,
	    User_IsActive bit NOT NULL,
	    User_Created datetime NULL,
	    User_LastLoggedVisit datetime NULL,
	    User_Language int NOT NULL,
	    User_RememberMe bit NULL,
	    User_Guid uniqueidentifier NOT NULL,
	    User_Displayname nvarchar(50) NOT NULL,
	    User_Properties xml NULL,
	    User_Skin nvarchar(25) NULL,
	    User_DetailView bit NOT NULL,
	    User_Translationview bit NULL,
	    User_Type int NULL,
	    User_Network varchar(50) NULL,
	    User_Data xml NULL);
	    
    --PK_User_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_User_Key';    
	THEN:ALTER TABLE wim_Users WITH NOCHECK ADD CONSTRAINT PK_User_Key PRIMARY KEY CLUSTERED (User_Key);

	CREATE TABLE wim_Menus(
	    Menu_Key int IDENTITY(1,1) NOT NULL,
		Menu_Name nvarchar(50) NOT NULL,
		Menu_Role_Key int NULL,
		Menu_Site_key int NULL,
		Menu_IsActive bit NOT NULL,
		Menu_Group_Key int NULL);
	    
    --PK_Menu_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Menu_Key';    
	THEN:ALTER TABLE wim_Menus WITH NOCHECK ADD CONSTRAINT PK_Menu_Key PRIMARY KEY CLUSTERED (Menu_Key);

	
	CREATE TABLE wim_MenuItems(
	    MenuItem_Key int IDENTITY(1,1) NOT NULL,
		MenuItem_Menu_Key int NULL,
		MenuItem_Dashboard_Key int NULL,
		MenuItem_Type_Key int NOT NULL,
		MenuItem_Item_Key int NOT NULL,
		MenuItem_Position int NOT NULL,
		MenuItem_Order int NOT NULL,
		MenuItem_ShowChildren bit NULL,
		MenuItem_Section int);
	    
    --PK_Menu_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_MenuItem_Key';    
	THEN:ALTER TABLE wim_MenuItems WITH NOCHECK ADD CONSTRAINT PK_MenuItem_Key PRIMARY KEY CLUSTERED (MenuItem_Key);


    CREATE TABLE wim_Folders(
	    Folder_Key int IDENTITY(1,1) NOT NULL,
	    Folder_GUID uniqueidentifier NOT NULL,
	    Folder_Type int NOT NULL,
	    Folder_Site_Key int NOT NULL,
	    Folder_Folder_Key int NULL,
	    Folder_Master_Key int NULL,
	    Folder_Name nvarchar(50) NOT NULL,
	    Folder_CompletePath nvarchar(1000) NULL,
		Folder_Description nvarchar(1024) NULL,
	    Folder_Created datetime NOT NULL,
	    Folder_IsVisible bit NULL,
	    Folder_SortOrderMethod int NULL);
	    
    --PK_Folder_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Folder_Key';    
	THEN:ALTER TABLE wim_Folders WITH NOCHECK ADD CONSTRAINT PK_Folder_Key PRIMARY KEY CLUSTERED (Folder_Key);

    CREATE TABLE wim_Pages(
	    Page_Key int IDENTITY(1,1) NOT NULL,
	    Page_GUID uniqueidentifier NOT NULL,
	    Page_Master_Key int NULL,
	    Page_Folder_Key int NOT NULL,
	    Page_Template_Key int NOT NULL,
	    Page_Created_User_Key int NULL,
	    Page_Published_User_Key int NULL,
	    Page_Changed_User_Key int NULL,
	    Page_Name nvarchar(150) NOT NULL,
	    Page_Title nvarchar(250) NULL,
	    Page_LinkText nvarchar(150) NULL,
	    Page_Description nvarchar(500) NULL,
	    Page_Created datetime NOT NULL,
	    Page_Published datetime NULL,
	    Page_Updated datetime NOT NULL,
	    Page_Publish datetime NULL,
	    Page_Expire datetime NULL,
	    Page_IsPublished bit NOT NULL,
	    Page_IsFixed bit NOT NULL,
	    Page_IsDefault bit NOT NULL,
	    Page_InheritContent bit NOT NULL,
	    Page_InheritContentEdited bit NOT NULL,
	    Page_IsSearchable bit NOT NULL,
	    Page_CustomDate datetime NULL,
	    Page_KeyWords nvarchar(500) NULL,
	    Page_SortOrder int NULL,
	    Page_SubFolder_Key int NULL,
	    Page_CompletePath varchar(500) NULL,
	    Page_IsSecure bit NULL
		);
	    
    --PK_Page_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Page_Key';    
	THEN:ALTER TABLE wim_Pages WITH NOCHECK ADD CONSTRAINT PK_Page_Key PRIMARY KEY CLUSTERED (Page_Key);
	  
    --IX_wim_Pages_Page_Template_Key;
	IF:select COUNT(*) from sys.indexes where name = 'IX_wim_Pages_Page_Template_Key';   
	THEN:CREATE NONCLUSTERED INDEX IX_wim_Pages_Page_Template_Key ON dbo.wim_Pages(Page_Template_Key);

    CREATE TABLE wim_ComponentVersions(
	    ComponentVersion_Key int IDENTITY(1,1) NOT NULL,
	    ComponentVersion_Master_Key int NULL,
	    ComponentVersion_GUID uniqueidentifier NOT NULL,
	    ComponentVersion_User_Key int NULL,
	    ComponentVersion_Page_Key int NULL,
	    ComponentVersion_ComponentTemplate_Key int NULL,
	    ComponentVersion_AvailableTemplate_Key int NULL,
		ComponentVersion_Target varchar(25) NULL,
	    ComponentVersion_SortOrder int NULL,
	    ComponentVersion_Name nvarchar(50) NULL,
	    ComponentVersion_Fixed_Id nvarchar(50) NULL,
	    ComponentVersion_Created datetime NOT NULL,
	    ComponentVersion_Updated datetime NULL,
	    ComponentVersion_IsFixedOnTemplate bit NOT NULL,
	    ComponentVersion_IsAlive bit NOT NULL,
	    ComponentVersion_IsSecundary bit NULL,
	    ComponentVersion_XML xml NULL,
	    ComponentVersion_SortDate datetime NULL,
	    ComponentVersion_IsActive bit NOT NULL,
		ComponentVersion_Site_Key int NULL);
	    
    --PK_ComponentVersion_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_ComponentVersion_Key';    
	THEN:ALTER TABLE wim_ComponentVersions WITH NOCHECK ADD CONSTRAINT PK_ComponentVersion_Key PRIMARY KEY CLUSTERED (ComponentVersion_Key);

    CREATE TABLE wim_Components(
	    Component_Key int IDENTITY(1,1) NOT NULL,
	    Component_GUID uniqueidentifier NOT NULL,
	    Component_Page_Key int NULL,
	    Component_ComponentTemplate_Key int NULL,
		Component_Target varchar(25) NULL,
	    Component_Name nvarchar(50) NULL,
	    Component_Fixed_Id nvarchar(50) NULL,
	    Component_Created datetime NOT NULL,
	    Component_Updated datetime NULL,
	    Component_IsFixedOnTemplate bit NULL,
	    Component_IsAlive bit NULL,
	    Component_IsSecundary bit NULL,
	    Component_XML xml NULL,
	    Component_SortDate datetime NULL,
	    Component_SortOrder int NULL);
	    
    --PK_Component_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_Component_Key';    
	THEN:ALTER TABLE wim_Components WITH NOCHECK ADD CONSTRAINT PK_Component_Key PRIMARY KEY CLUSTERED (Component_Key);

    --IX_wim_Components_PageKey;
	IF:select COUNT(*) from sys.indexes where name = 'IX_wim_Components_PageKey';   
	THEN:CREATE NONCLUSTERED INDEX IX_wim_Components_PageKey ON dbo.wim_Components (Component_Page_Key);

    CREATE TABLE wim_AvailableTemplates(
	    AvailableTemplates_Key int IDENTITY(1,1) NOT NULL,
	    AvailableTemplates_Guid uniqueidentifier NOT NULL,
	    AvailableTemplates_PageTemplate_Key int NOT NULL,
	    AvailableTemplates_ComponentTemplate_Key int NOT NULL,
		AvailableTemplates_Target varchar(25) NULL,
	    AvailableTemplates_IsPossible bit NOT NULL,
	    AvailableTemplates_IsSecundary bit NOT NULL,
	    AvailableTemplates_Timestamp timestamp NULL,
	    AvailableTemplates_IsPresent bit NOT NULL,
	    AvailableTemplates_SortOrder int NULL,
	    AvailableTemplates_Fixed_Id nvarchar(50) NULL,
		AvailableTemplates_Slot int NULL
		);
	    
    --PK_AvailableTemplates_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_AvailableTemplates_Key';    
	THEN:ALTER TABLE wim_AvailableTemplates WITH NOCHECK ADD CONSTRAINT PK_AvailableTemplates_Key PRIMARY KEY CLUSTERED (AvailableTemplates_Key);

    CREATE TABLE wim_ComponentLists(
	    ComponentList_Key int IDENTITY(1,1) NOT NULL,
	    ComponentList_GUID uniqueidentifier NOT NULL,
	    ComponentList_Site_Key int NULL,
	    ComponentList_ComponentTemplate_Key int NULL,
	    ComponentList_TargetType int NOT NULL,
	    ComponentList_Name nvarchar(50) NOT NULL,
	    ComponentList_Assembly varchar(150) NULL,
	    ComponentList_ClassName varchar(250) NULL,
	    ComponentList_Description nvarchar(250) NULL,
	    ComponentList_Type int NULL,
	    ComponentList_ContainsOneChild bit NOT NULL,
	    ComponentList_IsVisible bit NOT NULL,
	    ComponentList_ScheduleInterval int NULL,
	    ComponentList_Scheduled datetime NULL,
	    ComponentList_ReferenceId int NULL,
	    ComponentList_Folder_Key int NULL,
	    ComponentList_SortOrder int NULL,
	    ComponentList_Template_Key int NULL,
	    ComponentList_IsTemplate bit NULL,
	    ComponentList_IsInherited bit NOT NULL,
	    ComponentList_SingleItemName nvarchar(30) NULL,
	    ComponentList_Catalog_Key int NULL,
	    ComponentList_IsSingle bit NULL,
	    ComponentList_CanSort bit NULL,
	    ComponentList_Data xml NULL,
		ComponentList_Settings xml NULL,
	    ComponentList_Group varchar(50) NULL,
	    ComponentList_Class varchar(50) NULL,
	    ComponentList_Icon varchar(50) NULL);
	    
    --PK_ComponentList_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_ComponentList_Key';    
	THEN:ALTER TABLE wim_ComponentLists WITH NOCHECK ADD CONSTRAINT PK_ComponentList_Key PRIMARY KEY CLUSTERED (ComponentList_Key);

    --IX_wim_ComponentLists;
	IF:select COUNT(*) from sys.indexes where name = 'IX_wim_ComponentLists';    
	THEN:CREATE UNIQUE NONCLUSTERED INDEX IX_wim_ComponentLists ON wim_ComponentLists(ComponentList_GUID);

    CREATE TABLE wim_PropertyOptions(
	    PropertyOption_Key int IDENTITY(1,1) NOT NULL,
	    PropertyOption_Property_Key int NOT NULL,
	    PropertyOption_Text nvarchar(250) NOT NULL,
	    PropertyOption_Value nvarchar(250) NULL,
	    PropertyOption_SortOrder int NULL);
	    
    --PK_PropertyOption_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_PropertyOption_Key';    
	THEN:ALTER TABLE wim_PropertyOptions WITH NOCHECK ADD CONSTRAINT PK_PropertyOption_Key PRIMARY KEY CLUSTERED (PropertyOption_Key);

	CREATE TABLE wim_PageMappings(
		PageMap_Key int IDENTITY(1,1) NOT NULL,
		PageMap_Page_Key int NOT NULL,
		PageMap_Type int NULL,
		PageMap_Path nvarchar(150) NOT NULL,
		PageMap_Expression nvarchar(200) NULL,
		PageMap_Title nvarchar(150) NULL,
		PageMap_Query nvarchar(50) NULL,
		PageMap_Created datetime NOT NULL,
		PageMap_IsActive bit NULL,
		PageMap_TargetType int NULL,
		PageMap_Asset_Key int NULL);

    --PK_PageMap_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_PageMap_Key';    
	THEN:ALTER TABLE wim_PageMappings WITH NOCHECK ADD CONSTRAINT PK_PageMap_Key PRIMARY KEY CLUSTERED (PageMap_Key);

    --IX_wim_PageMappings_PageMapPath;
	IF:select COUNT(*) from sys.indexes where name = 'IX_wim_PageMappings_PageMapPath';   
	THEN:CREATE NONCLUSTERED INDEX IX_wim_PageMappings_PageMapPath ON dbo.wim_PageMappings(PageMap_Path);

    CREATE TABLE wim_ComponentListVersions(
	    ComponentListVersion_Key int IDENTITY(1,1) NOT NULL,
	    ComponentListVersion_Site_Key int NULL,
	    ComponentListVersion_ComponentList_Key int NOT NULL,
	    ComponentListVersion_User_Key int NULL,
	    ComponentListVersion_Listitem_Key int NOT NULL,
	    ComponentListVersion_Created datetime NOT NULL,
	    ComponentListVersion_XML xml NULL,
	    ComponentListVersion_IsActive bit NOT NULL,
	    ComponentListVersion_DescriptionTag nvarchar(100) NULL,
	    ComponentListVersion_Version int NULL,
	    ComponentListVersion_Type int NULL);
	    
    --PK_ComponentListVersion_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_ComponentListVersion_Key';    
	THEN:ALTER TABLE wim_ComponentListVersions WITH NOCHECK ADD CONSTRAINT PK_ComponentListVersion_Key PRIMARY KEY CLUSTERED (ComponentListVersion_Key);

    CREATE TABLE wim_RoleRights(
	    RoleRight_Key int IDENTITY(1,1) NOT NULL,
	    RoleRight_Role_Key int NOT NULL,
	    RoleRight_Child_Key int NOT NULL,
	    RoleRight_Child_Type int NOT NULL,
	    RoleRight_Update bit NOT NULL,
		RoleRight_Type int NOT NULL,
	    RoleRight_SortOrder int NOT NULL);
	    
    --PK_RoleRight_Key;
	IF:select COUNT(*) from sys.indexes where name = 'PK_RoleRight_Key';    
	THEN:ALTER TABLE wim_RoleRights WITH NOCHECK ADD CONSTRAINT PK_RoleRight_Key PRIMARY KEY CLUSTERED (RoleRight_Key);

	CREATE TABLE wim_WikiArticles(
		Wiki_Key [int] IDENTITY(1,1) NOT NULL,
		Wiki_ComponentList_Key [int] NOT NULL,
		Wiki_Author_Key int,
		Wiki_Section_Key [int] NULL,
		Wiki_Title nvarchar(100) NOT NULL,
		Wiki_Summary nvarchar(max) NULL,
		Wiki_Data [xml] NULL,
		Wiki_Created [datetime] NULL,
		Wiki_Updated [datetime] NULL,
		Wiki_IsActive [bit] NOT NULL,
		Wiki_BelongsToList_Key int null,
		Wiki_BelongsToPage_Key int null);

	--PK_Wiki_Key;
	ALTER TABLE wim_WikiArticles WITH NOCHECK ADD CONSTRAINT PK_Wiki_Key PRIMARY KEY CLUSTERED (Wiki_Key);	

	--Wim_PortalRights
	CREATE TABLE wim_PortalRights(
		PortalRight_Key int IDENTITY(1,1) NOT NULL,
		PortalRight_Role_Key int NOT NULL,
		PortalRight_Portal_Key int NOT NULL);
	
	--PK_Wim_PortalRights
	ALTER TABLE wim_PortalRights WITH NOCHECK ADD CONSTRAINT PK_PortalRight_Key PRIMARY KEY CLUSTERED (PortalRight_Key);	  

	-- Wim_SharedFields
	CREATE TABLE wim_SharedFields(
		SharedField_Key int IDENTITY(1,1) NOT NULL,
		SharedField_ContentTypeID int NOT NULL,
		SharedField_FieldName nvarchar (200) NOT NULL,
		SharedField_IsHiddenOnPage bit NOT NULL);

	--PK_Wim_PortalRights
	ALTER TABLE wim_SharedFields WITH NOCHECK ADD CONSTRAINT PK_Wim_SharedFields PRIMARY KEY CLUSTERED (SharedField_Key);	  

	-- Add Wim_SharedFieldTranslations
	CREATE TABLE wim_SharedFieldTranslations(
		SharedFieldTranslation_Key int IDENTITY(1,1) NOT NULL,
		SharedFieldTranslation_Field_Key int NOT NULL,
		SharedFieldTranslation_Site_Key int NOT NULL,
		SharedFieldTranslation_EditValue nvarchar(max) NOT NULL,
		SharedFieldTranslation_Value nvarchar(max) NULL);
		
	--PK_Wim_PortalRights
	ALTER TABLE wim_SharedFieldTranslations WITH NOCHECK ADD CONSTRAINT PK_Wim_SharedFieldTranslations PRIMARY KEY CLUSTERED (SharedFieldTranslation_Key);	  

	 
	-- Add wim_MenuGroups
	CREATE TABLE wim_MenuGroups(
		MenuGroup_Key int IDENTITY(1,1) NOT NULL,
		MenuGroup_Title nvarchar(50) NOT NULL,
		MenuGroup_Description nvarchar(500) NOT NULL,
		MenuGroup_Tag nvarchar(20) NOT NULL);
		
	--PK_wim_MenuGroups
	ALTER TABLE wim_MenuGroups WITH NOCHECK ADD CONSTRAINT PK_wim_MenuGroups PRIMARY KEY CLUSTERED (MenuGroup_Key);	  	

	--FK_wim_Menus_wim_MenuGroups
	ALTER TABLE wim_Menus ADD CONSTRAINT FK_wim_Menus_wim_MenuGroups FOREIGN KEY (Menu_Group_Key) REFERENCES wim_MenuGroups (MenuGroup_Key);