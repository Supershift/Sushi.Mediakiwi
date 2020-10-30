
delete from [wim_ComponentLists] where [ComponentList_ClassName] = 'Sushi.Mediakiwi.AppCentre.Data.Implementation.TaskOutbox'
delete from [wim_ComponentLists] where [ComponentList_ClassName] = 'Sushi.Mediakiwi.AppCentre.Data.Implementation.TaskInbox'
delete from [wim_ComponentLists] where [ComponentList_ClassName] = 'Sushi.Mediakiwi.AppCentre.Data.Implementation.Task'
drop table [dbo].[wim_Tasks]
drop table [dbo].[wim_TaskNotes]
