using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;

namespace Sushi.Mediakiwi.AppCentre.Data.Test
{
    /// <summary>
    /// 
    /// </summary>
    [DatabaseTable("cat_DemoList")]
    public class AllElement : DatabaseEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AllElement"/> class.
        /// </summary>
        public AllElement()
        {
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static AllElement SelectOne(int key)
        {
            return (AllElement)new AllElement()._SelectOne(key);
        }

        int m_ID;
        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>The ID.</value>
        [DatabaseColumn("Demo_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public int ID
        {
            get { return m_ID; }
            set { m_ID = value; }
        }

        string m_TextField;
        /// <summary>
        /// Gets or sets the text field.
        /// </summary>
        /// <value>The text field.</value>
        [DatabaseColumn("DemoTextField", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public string TextField
        {
            get { return m_TextField; }
            set { m_TextField = value; }
        }

        string m_TextArea;
        /// <summary>
        /// Gets or sets the text field.
        /// </summary>
        /// <value>The text field.</value>
        [DatabaseColumn("DemoTextArea", SqlDbType.VarChar, Length = 250, IsNullable = true)]
        public string TextArea
        {
            get { return m_TextArea; }
            set { m_TextArea = value; }
        }

        string m_RichText;
        /// <summary>
        /// Gets or sets the text field.
        /// </summary>
        /// <value>The text field.</value>
        [DatabaseColumn("DemoRichtext", SqlDbType.VarChar, Length = 500, IsNullable = true)]
        public string RichText
        {
            get { return m_RichText; }
            set { m_RichText = value; }
        }


        DateTime m_DateTimeOption;
        /// <summary>
        /// Gets or sets the text field.
        /// </summary>
        /// <value>The text field.</value>
        [DatabaseColumn("DemoDateTime", SqlDbType.DateTime, IsNullable = true)]
        public DateTime DateTimeOption
        {
            get { return m_DateTimeOption; }
            set { m_DateTimeOption = value; }
        }

        DateTime m_DateOption;
        /// <summary>
        /// Gets or sets the text field.
        /// </summary>
        /// <value>The text field.</value>
        [DatabaseColumn("DemoDate", SqlDbType.DateTime, IsNullable = true)]
        public DateTime DateOption
        {
            get { return m_DateOption; }
            set { m_DateOption = value; }
        }

        private int m_Dropdown;
        /// <summary>
        /// Gets or sets the dropdown option.
        /// </summary>
        /// <value>The dropdown option.</value>
        [DatabaseColumn("DemoDropdown", SqlDbType.Int, IsNullable = true)]
        public int DropdownOption
        {
            get { return m_Dropdown; }
            set { m_Dropdown = value; }
        }

        private int m_ChoiceRadioOption;
        /// <summary>
        /// Gets or sets the choice radio option.
        /// </summary>
        /// <value>The choice radio option.</value>
        [DatabaseColumn("DemoRadio", SqlDbType.Int, IsNullable = true)]
        public int ChoiceRadioOption
        {
            get { return m_ChoiceRadioOption; }
            set { m_ChoiceRadioOption = value; }
        }

        private bool m_Choice_CheckboxOption;
        /// <summary>
        /// Gets or sets a value indicating whether [choice_ checkbox option].
        /// </summary>
        /// <value>
        /// 	<c>true</c> if [choice_ checkbox option]; otherwise, <c>false</c>.
        /// </value>
        [DatabaseColumn("DemoCheckbox", SqlDbType.Bit, IsNullable = true)]
        public bool Choice_CheckboxOption
        {
            get { return m_Choice_CheckboxOption; }
            set { m_Choice_CheckboxOption = value; }
        }

        string m_SingleSubListSelect;
        /// <summary>
        /// Gets or sets the single sub list select.
        /// </summary>
        /// <value>The single sub list select.</value>
        [DatabaseColumn("DemoSubListSelect", SqlDbType.Text, IsNullable = true)]
        public string SingleSubListSelect
        {
            get { return m_SingleSubListSelect; }
            set { m_SingleSubListSelect = value; }
        }

        string m_SubListSelect;
        /// <summary>
        /// Gets or sets the single sub list select.
        /// </summary>
        /// <value>The single sub list select.</value>
        [DatabaseColumn("DemoSubListSelect2", SqlDbType.Text, IsNullable = true)]
        public string SubListSelect
        {
            get { return m_SubListSelect; }
            set { m_SubListSelect = value; }
        }

        private string m_ListItemSelect;
        /// <summary>
        /// Gets or sets the list item select.
        /// </summary>
        /// <value>The list item select.</value>
        [DatabaseColumn("DemoListItemSelect", SqlDbType.VarChar, Length = 50, IsNullable = true)]
        public string ListItemSelect
        {
            get { return m_ListItemSelect; }
            set { m_ListItemSelect = value; }
        }

        private int m_PageSelect;
        /// <summary>
        /// Gets or sets the page select option.
        /// </summary>
        /// <value>The page select option.</value>
        [DatabaseColumn("DemoPageSelect", SqlDbType.Int, IsNullable = true)]
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
        [DatabaseColumn("DemoFolderSelect", SqlDbType.Int, IsNullable = true)]
        public int FolderSelect
        {
            get { return m_FolderSelect; }
            set { m_FolderSelect = value; }
        }

        private int m_Hyperlink;
        /// <summary>
        /// Gets or sets the hyperlink.
        /// </summary>
        /// <value>The hyperlink.</value>
        [DatabaseColumn("DemoLink", SqlDbType.Int, IsNullable = true)]
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
        [DatabaseColumn("DemoDocument", SqlDbType.Int, IsNullable = true)]
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
        [DatabaseColumn("DemoImage", SqlDbType.Int, IsNullable = true)]
        public int Image
        {
            get { return m_Image; }
            set { m_Image = value; }
        }
    }
}
