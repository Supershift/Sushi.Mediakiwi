    CREATE VIEW wim_ListView AS
	-- IGNORE UPDATE: 0 (1 = YES)
	select 
		ComponentList_Key
	,	Folder_Key
	,	ComponentList_Folder_Key
	,	ComponentList_IsInherited
	from 
		wim_ComponentLists 
		join wim_Folders on (Folder_Master_Key =ComponentList_Folder_Key or Folder_Key =ComponentList_Folder_Key)
	where 
		Folder_Type = 2 or Folder_Type = 3;

    CREATE VIEW wim_GalleryView AS
    -- IGNORE UPDATE: 0 (1 = YES)
    SELECT curr.Gallery_Key, curr.Gallery_GUID, curr.Gallery_Gallery_Key, curr.Gallery_Name, curr.Gallery_CompletePath, curr.Gallery_Type, 
                          curr.Gallery_Created, curr.Gallery_IsFixed, base.Gallery_Format, base.Gallery_FormatType, base.Gallery_BackgroundRgb, curr.Gallery_IsFolder, 
                          curr.Gallery_Base_Key
    FROM wim_Galleries AS curr LEFT OUTER JOIN wim_Galleries AS base ON base.Gallery_Key = curr.Gallery_Base_Key;

	CREATE VIEW wim_SearchView AS
	-- IGNORE UPDATE: 0 (1 = YES)
	select 
		'1_' + CAST(ComponentList_Key as varchar(10)) as SearchView_Key
	,	1 as SearchView_Type
	,	ComponentList_Key as SearchView_Item_Key
	,	ComponentList_Name as SearchView_Title
	,	ComponentList_Description as SearchView_Description
	,	ComponentList_Site_Key as SearchView_Site_Key
	,	ComponentList_Folder_Key as SearchView_Folder_Key
	,	ComponentList_SortOrder as SearchView_SortOrder
	from
		wim_ComponentLists
	where
		componentList_IsVisible = 1
	union
		select 
			'2_' + CAST(Folder_Key as varchar(10))
		,	2
		,	Folder_Key
		,	Folder_Name
		,	null as Folder_Description
		,	Folder_Site_Key
		,	Folder_Key
		,	null
		from
			wim_Folders
		where
			not Folder_Name ='/' and Folder_IsVisible = 1
	union
		select 
			'3_' + CAST(Page_Key as varchar(10))	
		,	3
		,	Page_Key
		,	Page_Title
		,	Page_Description
		,	Folder_Site_Key
		,	Folder_Key
		,	Page_SortOrder
		from
			dbo.wim_Pages join wim_Folders on Page_Folder_Key =Folder_Key
		where
			Folder_IsVisible = 1
	union
		select 
			'5_' + CAST(Gallery_Key as varchar(10))
		,	5
		,	Gallery_Key
		,	Gallery_Name
		,	null 
		,	null
		,	null
		,	null
		from
			wim_Galleries
		where
			not Gallery_Name ='/'
			and Gallery_IsActive = 1
			and Gallery_IsHidden = 0
	union
		select 
			'6_' + CAST(Site_Key as varchar(10))	
		,	6
		,	Site_Key
		,	Site_Displayname
		,	null
		,	null
		,	null
		,	null
		from
			wim_Sites
		where
			Site_IsActive = 1
			and Site_HasPages = 1
			and Site_Type is null
	union
		select '7_1' , 7 , 1 , 'Website' , null , null , null , null
	union
		select '7_2' , 7 , 2 , 'Lists' , null , null , null , null
	union
		select '7_3' , 7 , 3 , 'Galleries' , null , null , null , null
	union
		select '7_4' , 7 , 4 , 'Admin' , null , null , null , null			
	union
		select 
			'8_' + CAST(Folder_Key as varchar(10))
		,	8
		,	Folder_Key
		,	Folder_Name
		,	null as Folder_Description
		,	Folder_Site_Key
		,	Folder_Key
		,	null
		from
			wim_Folders
		where
			not Folder_Name ='/'			
			and Folder_IsVisible = 1;

	CREATE VIEW wim_SharedFieldView AS
	-- IGNORE UPDATE: 0 (1 = YES)
	select 
		SharedFieldTranslation_Key
	,	SharedFieldTranslation_Field_Key
	,	SharedFieldTranslation_Site_Key
	,	SharedFieldTranslation_EditValue
	,	SharedFieldTranslation_Value
	,	SharedField_FieldName
	,	SharedField_ContentTypeID
	from 
		wim_SharedFieldTranslations
		left join wim_SharedFields as fields on SharedFieldTranslation_Field_Key = fields.SharedField_Key;