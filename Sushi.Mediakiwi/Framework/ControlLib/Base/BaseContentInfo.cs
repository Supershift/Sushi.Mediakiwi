using System;
using System.ComponentModel;
using System.Data;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Sushi.Mediakiwi.Framework.ControlLib.Base
{
    /// <summary>
    /// 
    /// </summary>
    [DefaultProperty("Title")]
    public class ContentInfo : System.Web.UI.Control
    {
        string m_Fieldname;
        /// <summary>
        /// Gets or sets the fieldname.
        /// </summary>
        /// <value>The fieldname.</value>
        public string Fieldname
        {
            get {
                // CB; Fieldname no longer required for LIST based contentInfo items
                if (String.IsNullOrEmpty(m_Fieldname))
                    m_Fieldname = this.ID;
                return m_Fieldname; 
            }
            set { m_Fieldname = value; }
        }

        string m_List;
        /// <summary>
        /// Gets or sets the list.
        /// </summary>
        /// <value>The list.</value>
        public string List
        {
            get { return m_List; }
            set { m_List = value; }
        }

        Sushi.Mediakiwi.Framework.Templates.GenericInstance m_gi;
        Sushi.Mediakiwi.Framework.Templates.GenericsInstance m_gi2;
        Sushi.Mediakiwi.Framework.Templates.SimpleGenericsInstance m_gi3;
        Sushi.Mediakiwi.Data.IComponentList m_clist;
        Sushi.Mediakiwi.Data.CustomDataItem m_data;
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns></returns>
        protected Sushi.Mediakiwi.Data.CustomDataItem GetInstance()
        {
            if (m_data == null)
            {
                if (HasReflection)
                {
                    int listID = Wim.Utility.ConvertToInt(this.List);
                    if (listID == 0)
                        throw new Exception("The ListID is not appplied correct. It should be a numeric value");

                    m_clist = Sushi.Mediakiwi.Data.ComponentList.SelectOneByReference(listID);

                    if (m_clist.IsNewInstance)
                    {
                        Sushi.Mediakiwi.Data.Catalog cat = Sushi.Mediakiwi.Data.Catalog.SelectOne("cat_Settings");
                        if (cat.IsNewInstance)
                        {
                            cat.Title = "Settings";
                            cat.Table = "cat_Settings";
                            cat.IsActive = true;
                            cat.ColumnPrefix = "Setting";
                            cat.HasCatalogBaseStructure = true;
                            cat.HasSortOrder = true;
                            cat.Save();
                            cat.ValidateSqlTableCreation();
                        }

                        m_clist.Name = string.Concat("Custom list ", listID);
                        m_clist.SingleItemName = m_clist.Name;
                        m_clist.ReferenceID = listID;
                        m_clist.ClassName = "Wim.Templates.Templates.UI.GenericsList";
                        m_clist.AssemblyName = "Sushi.Mediakiwi.Framework.dll";
                        m_clist.SiteID = this.CurrentSite.ID;
                        m_clist.IsSingleInstance = true;
                        m_clist.CatalogID = cat.ID;
                        m_clist.FolderID = Sushi.Mediakiwi.Data.Folder.SelectOneBySite(this.CurrentSite.ID, Sushi.Mediakiwi.Data.FolderType.List).ID;
                        m_clist.IsVisible = true;
                        m_clist.Save();

                        if (m_clist.IsNewInstance)
                            throw new Exception("The ListID could not be mapped on a ComponentList; are you sure you are using the Reference ID from the list?");
                    }

                    if (!m_clist.IsSingleInstance)
                        throw new Exception(string.Format("The list '{0}' is not of kind Single Instance.", m_clist.Name));

                    if (m_clist.ClassName == "Wim.Templates.Templates.UI.GenericList")
                    {
                        m_gi = Sushi.Mediakiwi.Framework.Templates.GenericInstance.SelectSingleInstance(m_clist.ID, this.CurrentSite.ID);
                        if (m_gi.IsNewInstance)
                            throw new Exception(string.Format("Please apply [Wim.Templates.Templates.UI.GenericsList] for list {0}", m_clist.Name));

                        if (m_gi.Data == null)
                        {
                            m_gi.Data = new Sushi.Mediakiwi.Data.CustomData();
                            m_gi.Data.Apply(Fieldname, "");
                            m_gi.Save();

                            m_data = new Sushi.Mediakiwi.Data.CustomDataItem();
                            //m_data.IsNull = true;
                            return m_data;
                        }
                        m_data = m_gi.Data[this.Fieldname];
                    }
                    else if (m_clist.ClassName == "Wim.Templates.Templates.UI.GenericsList")
                    {
                        m_gi2 = Sushi.Mediakiwi.Framework.Templates.GenericsInstance.SelectSingleInstance(m_clist.ID, this.CurrentSite.ID);
                        if (m_gi2.IsNewInstance)
                        {
                            //  Try to create it
                            m_gi2.SiteID = this.CurrentSite.ID;
                            m_gi2.ListID = m_clist.ID;
                            m_gi2.Save();

                            m_gi2 = Sushi.Mediakiwi.Framework.Templates.GenericsInstance.SelectSingleInstance(m_clist.ID, this.CurrentSite.ID);
                            
                            if (m_gi2.IsNewInstance)
                                throw new Exception("There is no single instance in the requested site!");
                        }
                        if (m_gi2.Data == null)
                        {
                            m_gi2.Data = new Sushi.Mediakiwi.Data.CustomData();
                            m_gi2.Data.Apply(Fieldname, "");
                            m_gi2.Save();

                            m_data = new Sushi.Mediakiwi.Data.CustomDataItem();
                            //m_data.IsNull = true;
                            return m_data;
                        }
                        m_data = m_gi2.Data[this.Fieldname];
                    }
                    else if (m_clist.ClassName == "Wim.Templates.Templates.UI.SimpleGenericsList")
                    {
                        m_gi3 = Sushi.Mediakiwi.Framework.Templates.SimpleGenericsInstance.SelectSingleInstance(m_clist.ID, this.CurrentSite.ID);
                        if (m_gi3.IsNewInstance)
                        {
                            //  Try to create it
                            m_gi3.ListID = m_clist.ID;
                            m_gi3.SiteID = this.CurrentSite.ID;
                            m_gi3.Save();

                            m_gi3 = Sushi.Mediakiwi.Framework.Templates.SimpleGenericsInstance.SelectSingleInstance(m_clist.ID, this.CurrentSite.ID);

                            if (m_gi3.IsNewInstance)
                                throw new Exception("There is no single instance in the requested site!");
                        }
                        if (m_gi3.Data == null)
                        {
                            m_gi3.Data = new Sushi.Mediakiwi.Data.CustomData();
                            m_gi3.Data.Apply(Fieldname, "");
                            m_gi3.Save();

                            m_data = new Sushi.Mediakiwi.Data.CustomDataItem();
                            //m_data.IsNull = true;
                            return m_data;
                        }
                        m_data = m_gi3.Data[this.Fieldname];
                    }
                }
            }
            return m_data;
        }

        /// <summary>
        /// Applies the data.
        /// </summary>
        /// <param name="maxlength">The maxlength.</param>
        /// <param name="data">The data.</param>
        /// <param name="type">The type.</param>
        protected void ApplyData(int? maxlength, string data, int type)
        {
            Sushi.Mediakiwi.Data.Property option = Sushi.Mediakiwi.Data.Property.SelectOne(m_clist.ID, this.Fieldname, null);
            if (option.IsNewInstance)
            {
                if (string.IsNullOrEmpty(this.Title))
                    throw new Exception(string.Format("The Fieldname '{1}' does not existing on list '{0}'. If you want this to be automatically be added also apply a Title property.", m_clist.Name, this.Fieldname));

                option.ListID = m_clist.ID;
                option.Title = this.Title;
                option.FieldName = this.Fieldname;
                option.TypeID = type;
                option.InteractiveHelp = this.HelpText;
                option.Mandatory = this.Mandatory ? "1" : "0";
                
                if (maxlength.HasValue)
                    option.MaxValueLength = maxlength.Value.ToString();

                option.MetaData = new MetaData();
                Wim.Utility.ReflectProperty(option, option.MetaData);
                option.Data = Wim.Utility.GetSerialized(option.MetaData);

                option.Save();

                if (m_gi2 == null)
                {
                    if (m_gi3 == null)
                    {
                        m_gi.Data.Apply(this.Fieldname, data);
                        m_gi.Save();
                    }
                    else
                    {
                        m_gi3.Data.Apply(this.Fieldname, data);
                        m_gi3.Save();
                    }
                }
                else
                {
                    m_gi2.Data.Apply(this.Fieldname, data);
                    m_gi2.Save();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has reflection.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance has reflection; otherwise, <c>false</c>.
        /// </value>
        internal bool HasReflection
        {
            get {
                // CB; Removed the check on fieldname 
                if ( !string.IsNullOrEmpty(this.List))
                    return true;
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ContentInfo() {}

        bool m_Mandatory;
        /// <summary>
        /// Mandatory field?
        /// </summary>
        public bool Mandatory
        {
            get { return m_Mandatory; }
            set { m_Mandatory = value; }
        }

        string m_HelpText;
        /// <summary>
        /// Interactive help text
        /// </summary>
        public string HelpText
        {
            get { return m_HelpText; }
            set { m_HelpText = value; }
        }

        internal string InnerText
        {
            get {
                if ((HasControls()) && Controls != null && Controls.Count > 0 && (Controls[0] is LiteralControl))
                    return ((LiteralControl)Controls[0]).Text;
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="candidate"></param>
        /// <returns></returns>
        protected bool FixApplyFormat(ControlCollection collection, string candidate)
        {
            foreach (Control c in collection)
            {
                LiteralControl lit = c as LiteralControl;
                if (lit != null)
                {
                    if (lit.Text.Contains("{0}"))
                    {
                        lit.Text = string.Format(lit.Text, string.Concat(candidate, "</a>"));
                        return true;
                    }
                }
                if (c.HasControls())
                    return FixApplyFormat(c.Controls, candidate);
            }
            return false;
        }

        string m_Title;
        /// <summary>
        /// Interactive help text
        /// </summary>
        public string Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        Sushi.Mediakiwi.Data.Site m_Site;
        /// <summary>
        /// Gets information about the container that hosts the current control when rendered on a design surface.
        /// </summary>
        /// <value></value>
        /// <returns>An <see cref="T:System.ComponentModel.ISite"/> that contains information about the container that the control is hosted in.</returns>
        /// <exception cref="T:System.InvalidOperationException">The control is a <see cref="T:System.Web.UI.WebControls.Substitution"/> control.</exception>
        internal Sushi.Mediakiwi.Data.Site CurrentSite
        {
            set { m_Site = value; }
            get
            {
                if (m_Site == null)
                    m_Site = HttpContext.Current.Items["Wim.Site"] as Sushi.Mediakiwi.Data.Site;

                return m_Site;
            }
        }
    }
}

