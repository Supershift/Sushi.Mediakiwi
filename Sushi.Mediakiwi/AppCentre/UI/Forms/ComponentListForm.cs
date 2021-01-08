using Sushi.Mediakiwi.Framework;
using Sushi.Mediakiwi.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Sushi.Mediakiwi.AppCentre.Data.Implementation.Forms
{
    public class ComponentListForm : FormMap<Mediakiwi.Data.ComponentList>
    {
        public ComponentListForm(Mediakiwi.Data.IComponentList implement)
        {
            Load(implement as Mediakiwi.Data.ComponentList);
            Map(x => x.Name).TextField("Name", 50, true).Expression(OutputExpression.Alternating);
            Map(x => x.SingleItemName).TextField("Form name", 30, true).Expression(OutputExpression.Alternating);
            Map(x => x.Description).TextArea("Description", 250);

            Map<ComponentListForm>(x => x.Section0, this).Section("Settings");

            Map(x => x.SiteID).Dropdown("Channel", "Sites", true, true).Expression(OutputExpression.Alternating);

            Map(x => x.AssemblyName).Dropdown("Assembly", "Assemblies", true, true).Expression(OutputExpression.Alternating);
            Map(x => x.FolderID).FolderSelect("Folder", true, Mediakiwi.Data.FolderType.Administration_Or_List).Expression(OutputExpression.Alternating);

            Map(x => x.ClassName).Dropdown("Class", "Classes", true).Expression(OutputExpression.Alternating);

            Map(x => x.Icon).TextField("Icon", 50).Expression(OutputExpression.Alternating);
            Map(x => x.GUID).TextField("GUID", 36, true).Expression(OutputExpression.Alternating);
            Map(x => x.IsInherited).Checkbox("Is inherited").Expression(OutputExpression.Alternating);
            Map(x => x.IsVisible).Checkbox("Is visible").Expression(OutputExpression.Alternating);

           
            Map<ComponentListForm>(x => x.Section1, this).Section("Search grid");
            Map(x => x.Option_Search_MaxResult).TextField("Maximum pages", 3, true).Expression(OutputExpression.Alternating);
            Map(x => x.Option_Search_MaxResultPerPage).TextField("Result per page", 3, true).Expression(OutputExpression.Alternating);

            Map<ComponentListForm>(x => x.Section2, this).Section("Labels");

            Map(x => x.Label_NewRecord).TextField("Label 'new'", 50).Expression(OutputExpression.Alternating);
            Map(x => x.Label_Save).TextField("Label 'save'", 50).Expression(OutputExpression.Alternating);
            Map(x => x.Label_Search).TextField("Label 'search'", 50).Expression(OutputExpression.Alternating);
            Map(x => x.Label_Saved).TextField("Label 'saved'", 50).Expression(OutputExpression.Alternating);

            Map<ComponentListForm>(x => x.Section3, this).Section("Options");

            Map(x => x.Option_CanCreate).Checkbox("Create").Expression(OutputExpression.Alternating);
            Map(x => x.Option_CanSave).Checkbox("Save").Expression(OutputExpression.Alternating);
            Map(x => x.Option_CanDelete).Checkbox("Delete").Expression(OutputExpression.Alternating);
            Map(x => x.Option_CanSaveAndAddNew).Checkbox("Save and new").Expression(OutputExpression.Alternating);

            Map(x => x.Option_HasExportXLS).Checkbox("Export (XLS)").Expression(OutputExpression.Alternating);
            Map(x => x.Option_HasExportColumnTitlesXLS).Checkbox("Export columns").Expression(OutputExpression.Alternating);

            Map(x => x.Option_OpenInEditMode).Checkbox("Direct edit").Expression(OutputExpression.Alternating);
            Map(x => x.Option_HasShowAll).Checkbox("Show all").Expression(OutputExpression.Alternating);
            Map(x => x.Option_PostBackSearch).Checkbox("Postback search").Expression(OutputExpression.Alternating);
            Map(x => x.Option_AfterSaveListView).Checkbox("Save to view").Expression(OutputExpression.Alternating);
            Map(x => x.Option_SearchAsync).Checkbox("Async search").Expression(OutputExpression.Alternating);
            Map(x => x.Option_FormAsync).Checkbox("Async form").Expression(OutputExpression.Alternating);
            Map(x => x.Option_LayerResult).Checkbox("Open in layer").Expression(OutputExpression.Alternating);
            Map(x => x.Option_HideBreadCrumbs).Checkbox("Hide breadcrumbs").Expression(OutputExpression.Alternating);
            Map(x => x.Option_HideNavigation).Checkbox("Hide navigation").Expression(OutputExpression.Alternating);
            Map(x => x.Option_HasDataReport).Checkbox("Has datareport").Expression(OutputExpression.Alternating);
        }

        public string Section0 { get; set; }
        public string Section1 { get; set; }
        public string Section2 { get; set; }
        public string Section3 { get; set; }

        private ListItemCollection m_Sites;
        /// <summary>
        /// Gets the sites.
        /// </summary>
        /// <value>The sites.</value>
        public ListItemCollection Sites
        {
            get
            {
                if (m_Sites != null)
                    return m_Sites;

                m_Sites = new ListItemCollection();
                ListItem li;

                m_Sites.Add(new ListItem("", ""));

                foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll(false))
                {
                    li = new ListItem(site.Name, site.ID.ToString());
                    m_Sites.Add(li);
                }
                return m_Sites;
            }
        }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        /// <value>The assemblies.</value>
        public ListItemCollection Assemblies
        {
            get
            {
                ListItemCollection col = new ListItemCollection();
                ListItem li;

                string exclude = "aspose,cuteeditor,ephtmltopdf,wim.syncfusion,winnovative.webchart,app_,netspell,intersystems,nunit,syncfusion,system,htmlagility,microsoft,newtonsoft";
                string[] list = exclude.Split(',');

                var file = Assembly.GetExecutingAssembly().ManifestModule.Name;
                var folder = Assembly.GetExecutingAssembly().Location;
                var directory = folder.Replace(file, string.Empty);

                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(directory);
                try
                {
                    foreach (System.IO.FileInfo info in dir.GetFiles("*.dll"))
                    {
                        bool skip = false;
                        foreach (string tmp in list)
                        {
                            if (info.Name.StartsWith(tmp, StringComparison.OrdinalIgnoreCase))
                            {
                                skip = true;
                                break;
                            }
                        }
                        if (skip) continue;
                        li = new ListItem(info.Name);
                        col.Add(li);
                    }
                }
                catch(Exception ex)
                {
                    throw ex;
                }
                return col;
            }
        }

        /// <summary>
        /// Gets the classes.
        /// </summary>
        /// <value>The classes.</value>
        public ListItemCollection Classes
        {
            get
            {
                

                ListItemCollection col = new ListItemCollection();
               
                if (String.IsNullOrEmpty(Instance.AssemblyName)) return col;

                var file = Assembly.GetExecutingAssembly().ManifestModule.Name;
                var folder = Assembly.GetExecutingAssembly().Location;
                var directory = folder.Replace(file, string.Empty);

                string assemblyLoadName = string.Concat(directory, Instance.AssemblyName);

                System.Reflection.Assembly assem = System.Reflection.Assembly.LoadFrom(assemblyLoadName);

                SortedList<string, string> list = new SortedList<string, string>();

                try
                {
                    foreach (Type type in assem.GetTypes())
                    {
                        if (type.BaseType == null) continue;
                        Type innerType = type.BaseType;
                        while (innerType != null)
                        {
                            if (innerType == typeof(Sushi.Mediakiwi.Framework.ComponentListTemplate)) break;
                            innerType = innerType.BaseType;
                        }
                        if (innerType != typeof(Sushi.Mediakiwi.Framework.ComponentListTemplate)) continue;

                        //  [20090410(MM): Exception introduced to avoid Generic selection. This can(should) be stripped in later versions]
                        if (type.FullName == "Wim.Templates.Templates.UI.GenericList"
                            && Instance != null
                            && Instance.ClassName != "Wim.Templates.Templates.UI.GenericList")
                        {
                            continue;
                        }

                        list.Add(type.FullName, type.FullName);
                    }

                    IEnumerator<KeyValuePair<string, string>> ienum = list.GetEnumerator();
                    while (ienum.MoveNext())
                    {
                        col.Add(new ListItem(ienum.Current.Value, ienum.Current.Key));
                    }
                    return col;
                }
                catch (System.Reflection.ReflectionTypeLoadException ex)
                {
                    string messages = "";
                    foreach (Exception ex2 in ex.LoaderExceptions)
                        messages += ex2.Message;
                    throw new Exception(messages, ex);
                }
            }
        }
    }
}
