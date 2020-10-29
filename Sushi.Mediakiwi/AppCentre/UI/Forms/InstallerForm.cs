using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Data;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.UI
{
    public class InstallerForm
    {
        System.Web.HttpContext _context;
        IInstaller _entity;

        public InstallerForm(IInstaller entity, System.Web.HttpContext context)
        {
            _context = context;
            _entity = entity;

            if (_entity == null)
                _entity = Sushi.Mediakiwi.Data.Environment.GetInstance<IInstaller>();

            Installer = string.Concat(_entity.Assembly, ",", _entity.ClassName);
        }

        public virtual IInstaller ToEntity()
        {
            var info = Installer.Split(',');
            _entity.Assembly = info[0];
            _entity.ClassName = info[1];
            _entity.FolderID = Folder;

            var attrib = GetAttribute(_entity.Assembly, _entity.ClassName);
            if (attrib != null)
            {
                _entity.Name = attrib.Name;
                _entity.GUID = attrib.ID;
                _entity.Version = attrib.Version;
                _entity.Description = attrib.Description;
            }
            return _entity;
        }

        InstallableAttribute GetAttribute(string assembly, string className)
        {
            var instance = (IInstallable)Wim.Utility.CreateInstance(assembly, className);
            var attributes = instance.GetType().GetCustomAttributes(typeof(InstallableAttribute), true);
            if (attributes.Length > 0)
            {
                return (InstallableAttribute)attributes[0];
            }
            return null;
        }

        /// <summary>
        /// Get a list of installers
        /// </summary>
        public ListItemCollection Installers
        {
            get
            {
                var col = new ListItemCollection();
                SortedList<string, string> list = new SortedList<string, string>();

                var current = Sushi.Mediakiwi.Data.Installer.SelectAll();

                var exclude = ("aspose,cuteeditor,ephtmltopdf,wim.syncfusion,winnovative.webchart,app_,netspell,intersystems,nunit,syncfusion").Split(',');
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(_context.Server.MapPath(string.Concat(_context.Request.ApplicationPath, "/bin/")));
                foreach (System.IO.FileInfo info in dir.GetFiles("*.dll"))
                {
                    bool skip = false;
                    foreach (string tmp in exclude)
                    {
                        if (info.Name.StartsWith(tmp, StringComparison.OrdinalIgnoreCase))
                        {
                            skip = true;
                            break;
                        }
                    }
                    if (skip) continue;

                    string assemblyLoadName = _context.Server.MapPath(string.Concat(_context.Request.ApplicationPath, "/bin/", info.Name));

                    if (String.IsNullOrEmpty(info.Name))
                        continue;

                    System.Reflection.Assembly assem = System.Reflection.Assembly.LoadFrom(assemblyLoadName);

                    try
                    {
                        foreach (Type type in assem.GetTypes())
                        {
                            var t = type.GetInterface("IInstallable");
                            if (t == null)
                                continue;

                            var exist = (from item in current where item.Assembly == info.Name && item.ClassName == type.FullName select item).Count();
                            if (exist > 0)
                                continue;

                            var attribute = GetAttribute(info.Name, type.FullName);
                            if (attribute != null)
                            {
                                string key = string.Concat(info.Name, ",", type.FullName);
                                string val = string.Format("<b>{0}</b> - {1}", attribute.Name, attribute.Description);
                                list.Add(key, val);
                            }
                        }
                    }
                    catch (System.Reflection.ReflectionTypeLoadException)
                    {
                        //string messages = "";
                        //foreach (Exception ex2 in ex.LoaderExceptions)
                        //    messages += ex2.Message;
                        //throw new Exception(messages, ex);
                    }
                }
                IEnumerator<KeyValuePair<string, string>> ienum = list.GetEnumerator();
                while (ienum.MoveNext())
                    col.Add(new ListItem(ienum.Current.Value, ienum.Current.Key));

                return col;
            }
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Installer", "Installers", true)]
        public string Installer { get; set; }

        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Folder", "Folders", true)]
        public int Folder { get; set; }

        /// <summary>
        /// Gets the search sites.
        /// </summary>
        /// <value>The search sites.</value>
        public ListItemCollection Folders
        {
            get
            {
                var folders = new ListItemCollection();
                ListItem li;
                folders.Add(new ListItem("", ""));

                foreach (Sushi.Mediakiwi.Data.Site site in Sushi.Mediakiwi.Data.Site.SelectAll())
                {
                    if (site.MasterID.GetValueOrDefault() > 0) continue;
                    foreach (var folder in Sushi.Mediakiwi.Data.Folder.SelectAll(FolderType.Administration_Or_List, site.ID))
                    {
                        string key = folder.ID.ToString();
                        string val = string.Format("{0} {1}", site.Name, folder.CompletePath);
                        li = new ListItem(val, key);
                        folders.Add(li);
                    }
                }
                return folders;
            }
        }

        [Sushi.Mediakiwi.Framework.ContentListItem.DataList()]
        public Sushi.Mediakiwi.Data.DataList ItemList { get; set; }


    }
}
