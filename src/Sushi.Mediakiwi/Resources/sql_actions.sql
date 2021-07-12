    -- Cleanup wim_Registry (obsolete);
    IF>: select COUNT(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'wim_Registry';
    THEN: drop table wim_Registry; 	

    -- Cleanup wim_VisitorLogs (obsolete);
    IF>: select COUNT(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'wim_VisitorLogs';
    THEN: drop table wim_VisitorLogs; 	

    -- Cleanup wim_VisitorClicks (obsolete);
    IF>: select COUNT(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'wim_VisitorClicks';
    THEN: drop table wim_VisitorClicks; 

    -- Cleanup wim_VisitorDownloads (obsolete);
    IF>: select COUNT(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'wim_VisitorDownloads';
    THEN: drop table wim_VisitorDownloads; 

    -- Cleanup wim_VisitorUrls (obsolete);
    IF>: select COUNT(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'wim_VisitorUrls';
    THEN: drop table wim_VisitorUrls; 

    -- Cleanup wim_VisitorPages (obsolete);
    IF>: select COUNT(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'wim_VisitorPages';
    THEN: drop table wim_VisitorPages; 

    -- Cleanup wim_Subscriptions (obsolete);
    IF>: select COUNT(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'wim_Subscriptions';
    THEN: drop table wim_Subscriptions; 

    -- Cleanup wim_Catalogs (obsolete);
    IF>: select COUNT(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'wim_Catalogs';
    THEN: drop table wim_Catalogs; 

    -- Cleanup wim_Dashboards (obsolete);
    IF>: select COUNT(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'wim_Dashboards';
    THEN: drop table wim_Dashboards; 

    -- Cleanup wim_DashboardLists (obsolete);
    IF>: select COUNT(*) from INFORMATION_SCHEMA.TABLES where TABLE_NAME = 'wim_DashboardLists';
    THEN: drop table wim_DashboardLists; 

    -- Dashboard;
    IF>: select count(*) from wim_ComponentLists where ComponentList_GUID = 'DD2F174A-4828-4838-A4D0-3138F844574F';
    THEN: delete from wim_ComponentLists where ComponentList_GUID = 'DD2F174A-4828-4838-A4D0-3138F844574F';
    
    -- ComponentListSense;
    IF>: select count(*) from wim_ComponentLists where ComponentList_GUID = '937A7D82-EABD-4747-A74A-7B9106E697B1';
    THEN: delete from wim_ComponentLists where ComponentList_GUID = '937A7D82-EABD-4747-A74A-7B9106E697B1';
    
    -- Subscriptions;
    IF>: select count(*) from wim_ComponentLists where ComponentList_GUID = '28C4171D-C219-45EA-A7A8-13D1675F3B25';
    THEN: delete from wim_ComponentLists where ComponentList_GUID = '28C4171D-C219-45EA-A7A8-13D1675F3B25';
    
    -- Visitor tracking export;
    IF>: select count(*) from wim_ComponentLists where ComponentList_GUID = 'DE20BC46-4486-45EB-A0B2-5FBDF4B1E485';
    THEN: delete from wim_ComponentLists where ComponentList_GUID = 'DE20BC46-4486-45EB-A0B2-5FBDF4B1E485';

    -- Registry;
    IF>: select count(*) from wim_ComponentLists where ComponentList_GUID = 'D4334EAF-DE8F-4540-9988-2787D09DD50C';
    THEN: delete from wim_ComponentLists where ComponentList_GUID = 'D4334EAF-DE8F-4540-9988-2787D09DD50C';

    --Profile folder;
    IF>: select COUNT(*) from wim_ComponentLists left join wim_Folders on ComponentList_Folder_Key = Folder_Key where ComponentList_GUID = '46B59076-FEF3-4BAA-9BE0-2C9D71DF3379' and Folder_GUID = '0CBA2CA4-25FD-48B4-A3BB-B760C345CF8B';
    THEN: update wim_ComponentLists set ComponentList_Folder_Key = (select Folder_Key FROM wim_Folders where Folder_GUID = '0CBA2CA4-25FD-48B4-A3BB-B760C345CF8B') where ComponentList_GUID in ('46B59076-FEF3-4BAA-9BE0-2C9D71DF3379', 'd6170f90-01ba-4700-b9c8-bdfdf12b5438');
    
    -- Role access;
    IF>: select count(*) from wim_Roles where Role_All_Folders is null;
    THEN: update wim_Roles set Role_All_Folders=1, Role_All_Galleries=1, Role_List_IsAccessList=1, Role_List_IsAccessSite=1, Role_List_IsAccessFolder=1, Role_List_IsAccessGallery=1 where Role_All_Folders is null;
    
    -- Gallery count;
    IF>: select count(*) from wim_Galleries where Gallery_Count is null;
    THEN: update wim_Galleries set Gallery_Count = (select count(*) from wim_Assets where Asset_Gallery_Key = Gallery_Key);
    
    -- Component template;
    IF>: select count(*) from wim_ComponentTemplates where ComponentTemplate_AjaxType is null;
    THEN: update wim_ComponentTemplates set ComponentTemplate_AjaxType = 0 where ComponentTemplate_AjaxType is null;
    
    -- Notification selection;
    IF>: select count(*) from wim_Notifications where Notification_Selection is null;
    THEN: update wim_Notifications set Notification_Selection = 1 where Notification_Selection is null;
    
    -- Gallery activity;
    IF>: select count(*) from wim_Galleries where Gallery_IsActive is null;
    THEN: update wim_Galleries set Gallery_IsActive = 1 where Gallery_IsActive is null;    
