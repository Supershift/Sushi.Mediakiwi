
delete from [wim_ComponentLists] where [ComponentList_ClassName] = 'Sushi.Mediakiwi.AppCentre.Data.Implementation.TaskOutbox'
delete from [wim_ComponentLists] where [ComponentList_ClassName] = 'Sushi.Mediakiwi.AppCentre.Data.Implementation.TaskInbox'
delete from [wim_ComponentLists] where [ComponentList_ClassName] = 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Task'
drop table [dbo].[wim_Tasks]
drop table [dbo].[wim_TaskNotes]

alter table wim_Environments drop column Environment_Path;
alter table wim_Environments drop column Environment_Url;
alter table wim_Environments drop column Environment_LogoL;


alter table [dbo].[wim_PageTemplates] add PageTemplate_Source nvarchar(max)
alter table [dbo].[wim_PageTemplates] add PageTemplate_IsSourceBased bit
alter table [dbo].[wim_AvailableTemplates ] add AvailableTemplates_Slot int

alter table [dbo].[wim_ComponentTemplates] add ComponentTemplate_NestType int
alter table [dbo].[wim_ComponentTemplates] alter column ComponentTemplate_NestType int

alter table [dbo].[wim_Properties] add Property_Template_Key int
alter table [dbo].[wim_Properties] add Property_Help nvarchar(max)
alter table [dbo].[wim_Properties] add Property_IsRequired bit
alter table [dbo].[wim_Properties] add Property_MaxInput int
alter table [dbo].[wim_Properties] add Property_Default nvarchar(max)
