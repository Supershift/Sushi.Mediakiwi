using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.AppCentre.Data.UI.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class AllElement : ComponentListTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AllElement"/> class.
        /// </summary>
        public AllElement()
        {
            wim.HasDataDependency = true;

            wim.ListDataDependendProperties.Add("Revenue");

            wim.CanContainSingleInstancePerDefinedList = true;
            //wim.ShouldActAsDataStore = true;

            this.ListLoad += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(AllElement_ListLoad);
            this.ListPreRender += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(AllElement_ListPreRender);
            this.ListSave += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(AllElement_ListSave);
            this.ListAction += new Sushi.Mediakiwi.Framework.ComponentActionEventHandler(AllElement_ListAction);
            this.ListDelete += new Sushi.Mediakiwi.Framework.ComponentListEventHandler(AllElement_ListDelete);
        }

        void AllElement_ListDelete(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {

        }

        void AllElement_ListPreRender(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            this.TextLine += string.Concat("ListPreRender<br/>");

            //wim.Notification.AddError("ER IS EEN FOUT OPGETREDEN!");
        }

        void AllElement_ListAction(object sender, Sushi.Mediakiwi.Framework.ComponentActionEventArgs e)
        {
            this.TextLine += string.Format("ListAction: {0}<br/>", e.PropertyName);
        }

        void AllElement_ListSave(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            

            this.TextLine = string.Concat("ListSave<br/>");

            m_implement = Data.Test.AllElement.SelectOne(1);

            Wim.Utility.ReflectProperty(this, m_implement);
            m_implement.Save();
        }

        Data.Test.AllElement m_implement;
        void AllElement_ListLoad(object sender, Sushi.Mediakiwi.Framework.ComponentListEventArgs e)
        {
            this.TextLine = string.Concat("ListLoad<br/>");
            
            m_implement = Data.Test.AllElement.SelectOne(1);

            Wim.Utility.ReflectProperty(m_implement, this);
        }

        private string m_TextLine;
        /// <summary>
        /// Gets or sets the text line.
        /// </summary>
        /// <value>The text line.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine("TextLine")]
        public string TextLine
        {
            get { return m_TextLine; }
            set { m_TextLine = value; }
        }

        private string m_TextField;
        /// <summary>
        /// Gets or sets the text field.
        /// </summary>
        /// <value>The text field.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextField("TextField", 50, true, "Helptext for TextField")]
        public string TextField
        {
            get { return m_TextField; }
            set { m_TextField = value; }
        }

        private string m_TextArea;
        /// <summary>
        /// Gets or sets the text area.
        /// </summary>
        /// <value>The text area.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextArea("TextArea", 250, true, "Helptext for TextArea")]
        public string TextArea
        {
            get { return m_TextArea; }
            set { m_TextArea = value; }
        }

        private string m_RichText;
        /// <summary>
        /// Gets or sets the rich text.
        /// </summary>
        /// <value>The rich text.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.RichText("RichText", 250, true, "Helptext for RichText")]
        public string RichText
        {
            get { return m_RichText; }
            set { m_RichText = value; }
        }

        private DateTime m_DateTime;
        /// <summary>
        /// Gets or sets the date time option.
        /// </summary>
        /// <value>The date time option.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.DateTime("DateTime", true, "Helptext for DateTime field")]
        public DateTime DateTimeOption
        {
            get { return m_DateTime; }
            set { m_DateTime = value; }
        }

        private DateTime m_Date;
        /// <summary>
        /// Gets or sets the date option.
        /// </summary>
        /// <value>The date option.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Date("Date", true, "Helptext for Date field")]
        public DateTime DateOption
        {
            get { return m_Date; }
            set { m_Date = value; }
        }

        private System.Web.HttpPostedFile m_PostedFile;
        [Sushi.Mediakiwi.Framework.ContentListItem.FileUpload("FileUpload", false, "Helptext for FileUpload")]
        public System.Web.HttpPostedFile PostedFile
        {
            get { return m_PostedFile; }
            set { m_PostedFile = value; }
        }

        private string m_Subtitle1 = "Choice fields";
        /// <summary>
        /// Gets or sets the subtitle1.
        /// </summary>
        /// <value>The subtitle1.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine(null, false, null)]
        public string Subtitle1
        {
            get { return m_Subtitle1; }
            set { m_Subtitle1 = value; }
        }

        private int m_Dropdown;
        /// <summary>
        /// Gets or sets the dropdown option.
        /// </summary>
        /// <value>The dropdown option.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Dropdown("Choice_Dropdown", "DropdownCollection", true, false)]
        public int DropdownOption
        {
            get { return m_Dropdown; }
            set { m_Dropdown = value; }
        }

        private string m_ChoiceRadioOption;
        /// <summary>
        /// Gets or sets the choice radio option.
        /// </summary>
        /// <value>The choice radio option.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Radio("Choice_Radio", "DropdownCollection", "Choice_Radio", true, false, "Helptext for Choice_Radio")]
        public string ChoiceRadioOption
        {
            get { return m_ChoiceRadioOption; }
            set { m_ChoiceRadioOption = value; }
        }

        private bool m_Choice_CheckboxOption;
        /// <summary>
        /// Gets or sets the choice_ checkbox option.
        /// </summary>
        /// <value>The choice_ checkbox option.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Choice_Checkbox("Choice_Checkbox", true, "Helptext for Choice_Checkbox")]
        public bool Choice_CheckboxOption
        {
            get { return m_Choice_CheckboxOption; }
            set { m_Choice_CheckboxOption = value; }
        }

        ListItemCollection m_DropdownCollection;
        /// <summary>
        /// Gets the dropdown collection.
        /// </summary>
        /// <value>The dropdown collection.</value>
        public ListItemCollection DropdownCollection
        {
            get
            {
                m_DropdownCollection = new ListItemCollection();
                m_DropdownCollection.Add(new ListItem("selecteer een optie", ""));
                m_DropdownCollection.Add(new ListItem("optie 1", "4"));
                m_DropdownCollection.Add(new ListItem("optie 2", "5"));
                m_DropdownCollection.Add(new ListItem("optie 3", "6"));
                return m_DropdownCollection;
            }
        }

        private bool m_Button;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AllElement"/> is button.
        /// </summary>
        /// <value><c>true</c> if button; otherwise, <c>false</c>.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Button("Button")]
        public bool Button
        {
            get { return m_Button; }
            set { m_Button = value; }
        }


        private string m_Subtitle2 = "List select fields";
        /// <summary>
        /// Gets or sets the subtitle2.
        /// </summary>
        /// <value>The subtitle2.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.TextLine(null)]
        public string Subtitle2
        {
            get { return m_Subtitle2; }
            set { m_Subtitle2 = value; }
        }

        Sushi.Mediakiwi.Data.SubList m_SingleSubListSelect;
        /// <summary>
        /// Gets or sets the single sub list select.
        /// </summary>
        /// <value>The single sub list select.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Sublist select (single)", "9870fa8f-d9b5-47ed-89ae-cf01b6d00591", true, false, "Helptext for SubListSelect", CanContainOneItem = true)]
        public Sushi.Mediakiwi.Data.SubList SingleSubListSelect
        {
            get { return m_SingleSubListSelect; }
            set { m_SingleSubListSelect = value; }
        }

        Sushi.Mediakiwi.Data.SubList m_SubListSelect;
        /// <summary>
        /// Gets or sets the single sub list select.
        /// </summary>
        /// <value>The single sub list select.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.SubListSelect("Sublist select", "9870fa8f-d9b5-47ed-89ae-cf01b6d00591", true, false, "Helptext for SubListSelect", CanContainOneItem = false)]
        public Sushi.Mediakiwi.Data.SubList SubListSelect
        {
            get { return m_SubListSelect; }
            set { m_SubListSelect = value; }
        }

        //private string[] m_ListItemSelect;
        ///// <summary>
        ///// Gets or sets the list item select.
        ///// </summary>
        ///// <value>The list item select.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.ListItemSelect("ListItemSelect", "DropdownCollection", true, "Helptext for Listitem select")]
        //public string[] ListItemSelect
        //{
        //    get { return m_ListItemSelect; }
        //    set { m_ListItemSelect = value; }
        //}

        private int m_PageSelect;
        /// <summary>
        /// Gets or sets the page select option.
        /// </summary>
        /// <value>The page select option.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.PageSelect("PageSelect", true, "Helptext for Page select")]
        public int PageSelectOption
        {
            get { return m_PageSelect; }
            set { m_PageSelect = value; }
        }

        private int m_FolderSelect;
        /// <summary>
        /// Gets or sets the folder select.
        /// </summary>
        /// <value>The folder select.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.FolderSelect("FolderSelect", true, "Helptext for Folder select")]
        public int FolderSelect
        {
            get { return m_FolderSelect; }
            set { m_FolderSelect = value; }
        }

        //Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry[] m_TimeSheetLine1;
        ///// <summary>
        ///// Gets or sets the single sub list select.
        ///// </summary>
        ///// <value>The single sub list select.</value>
        //[Sushi.Mediakiwi.Framework.ContentListItem.SpecialTypes.TimesheetLine("9870fa8f-d9b5-47ed-89ae-cf01b6d00591")]
        //public Sushi.Mediakiwi.Data.SpecialTypes.TimesheetEntry[] TimeSheetLine1
        //{
        //    get { return m_TimeSheetLine1; }
        //    set { m_TimeSheetLine1 = value; }
        //}

        private int m_Hyperlink;
        /// <summary>
        /// Gets or sets the hyperlink.
        /// </summary>
        /// <value>The hyperlink.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Hyperlink("Hyperlink", true, "Helptext for Hyperlink")]
        public int Hyperlink
        {
            get { return m_Hyperlink; }
            set { m_Hyperlink = value; }
        }

        private int m_Document;
        /// <summary>
        /// Gets or sets the document.
        /// </summary>
        /// <value>The document.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Binary_Document("Binary_Document", true, "Helptext for Binary_Document")]
        public int Document
        {
            get { return m_Document; }
            set { m_Document = value; }
        }

        private int m_Image;
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>The image.</value>
        [Sushi.Mediakiwi.Framework.ContentListItem.Binary_Image("Binary_Image", true, "Helptext for Binary_Image")]
        public int Image
        {
            get { return m_Image; }
            set { m_Image = value; }
        }
    }
}
