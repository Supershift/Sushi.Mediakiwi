using Sushi.Mediakiwi.MicroORM.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sushi.MailTemplate.Data
{
    /// <summary>
    /// MailTemplate data entity
    /// </summary>
    [DataMap(typeof(MailTemplateMap))]
    public class MailTemplate
    {
        /// <summary>
        /// Sushi.MicroORM Mapper class
        /// </summary>
        public class MailTemplateMap : DataMap<MailTemplate>
        {
            /// <summary>
            /// Sushi.MicroORM Mapper ctor
            /// </summary>
            public MailTemplateMap()
            {
                Table("wim_MailTemplates");
                Id(x => x.ID, "MailTemplate_Key");
                Map(x => x.Name, "MailTemplate_Name").Length(50); // DBType System.String
                Map(x => x.Description, "MailTemplate_Description").Length(255); // DBType System.String
                Map(x => x.Identifier, "MailTemplate_Identifier").Length(50); // DBType System.String
                Map(x => x.DefaultSenderEmail, "MailTemplate_DefaultSenderEmail").Length(255); // DBType System.String
                Map(x => x.DefaultSenderName, "MailTemplate_DefaultSenderName").Length(50); // DBType System.String
                Map(x => x.BCCReceivers, "MailTemplate_BCCReceivers"); // DBType System.String
                Map(x => x.Subject, "MailTemplate_Subject").Length(512); // DBType System.String
                Map(x => x.Body, "MailTemplate_Body"); // DBType System.String
                Map(x => x.DateCreated, "MailTemplate_DateCreated"); // DBType System.DateTime
                Map(x => x.DateLastUpdated, "MailTemplate_DateLastUpdated"); // DBType System.DateTime
                Map(x => x.IsArchived, "MailTemplate_IsArchived"); // DBType System.Boolean
                Map(x => x.IsPublished, "MailTemplate_IsPublished"); // DBType System.Boolean
                Map(x => x.VersionMajor, "MailTemplate_VersionMajor"); // DBType System.Int32
                Map(x => x.VersionMinor, "MailTemplate_VersionMinor"); // DBType System.Int32
                Map(x => x.UserID, "MailTemplate_UserID"); // DBType System.Int32
                Map(x => x.UserName, "MailTemplate_UserName").Length(512); // DBType System.String
                Map(x => x.GUID, "MailTemplate_GUID"); // DBType System.Guid

            }
        }

        /// <summary>
        /// The ID of the current mail template
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of the current mail template, which does not have to unique.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the current mail template.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The identifier of the current mail template, which is unique.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// The e-mail address of the sender of the e-mail
        /// </summary>
        public string DefaultSenderEmail { get; set; }

        /// <summary>
        /// The name of the sender of the e-mail
        /// </summary>
        public string DefaultSenderName { get; set; }

        /// <summary>
        /// The bcc receivers, as ";"-separated list, like info@supershift.com;support@supershift.nl
        /// </summary>
        public string BCCReceivers { get; set; }

        /// <summary>
        /// The e-mail subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The e-mail body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// The date of creation of the current mail template.
        /// </summary>
        public DateTime? DateCreated { get; set; }

        /// <summary>
        /// The date of the last modification of the current mail template.
        /// </summary>
        public DateTime? DateLastUpdated { get; set; }

        /// <summary>
        /// Returns if the current mail template is archived.
        /// </summary>
        public bool? IsArchived { get; set; }

        /// <summary>
        /// Returns if the current mail template is published.
        /// </summary>
        public bool? IsPublished { get; set; }

        /// <summary>
        /// The major version like 2.x
        /// </summary>
        public int VersionMajor { get; set; }

        /// <summary>
        /// The minor verion like x.1
        /// </summary>
        public int VersionMinor { get; set; }

        /// <summary>
        /// The UserID of the creator
        /// </summary>
        public int? UserID { get; set; }

        /// <summary>
        /// The Username of the creator
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// The GUID of the current mail template
        /// </summary>
        public Guid GUID { get; set; }

        /// <summary>
        /// HasPublishedVersion property is true if the mail template has at least one major version
        /// </summary>
        public bool HasPublishedVersion
        {
            get
            {
                return VersionMajor > 0;
            }
        }

        

        /// <summary>
        /// Add placeholders to this list, like template.PlaceholderList.Add("HOTEL", "Sunny Beach Hotel");
        /// </summary>
        public PlaceholderList PlaceholderList { get; set; } = new PlaceholderList();

        /// <summary>
        /// Add placeholder groups to this list, like template.PlaceholderGroupList.Add("rooms");
        /// template.PlaceholderGroupList.AddNewRow();
        /// template.PlaceholderGroupList.AddNewRowItem("NAME", "Cus Tomer");
        /// </summary>
        public PlaceholderGroupList PlaceholderGroupList { get; set; } = new PlaceholderGroupList();

        /// <summary>
        /// Optional sections are useful for instance to create invitation mail templates with optional blocks for rooms, sunbeds and express checkins. 
        /// Leaving a section out has as result that the section is removed from the e-mail.
        /// </summary>
        public List<string> OptionalSections { get; set; } = [];
    }

    /// <summary>
    /// PlaceholderGroupList class, with methods to add items
    /// </summary>
    public class PlaceholderGroupList
    {
        internal Dictionary<string, Entities.PlaceholderGroup> PlaceholderGroupDictionary { get; set; } = [];
        
        /// <summary>
        /// Add a group to the placeholder groups, like template.PlaceholderGroupList.Add("rooms");
        /// </summary>
        /// <param name="groupName"></param>
        public void Add(string groupName)
        {
            PlaceholderGroupDictionary.Add(groupName, new Entities.PlaceholderGroup(groupName));
        }

        /// <summary>
        /// Add a new row to the placeholder group, so it can hold items, like template.PlaceholderGroupList.AddNewRow();
        /// </summary>
        public void AddNewRow()
        {
            PlaceholderGroupDictionary.Last().Value.AddNewRow();
        }

        /// <summary>
        /// Add a new item to the row of a placeholder group, like template.PlaceholderGroupList.AddNewRowItem("NAME", "Cus Tomer");
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AddNewRowItem(string name, string value)
        {
            PlaceholderGroupDictionary.Last().Value.AddNewRowItem(name, value);
        }
    }

    /// <summary>
    /// PlaceholderList class
    /// </summary>
    public class PlaceholderList
    {
        internal Dictionary<string, Entities.Placeholder> PlaceholderDictionary { get; set; } = [];
        
        /// <summary>
        /// Add name-value pairs to the placeholder list, like template.PlaceholderList.Add("NAME", "Cus Tomer");
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void Add(string name, string value)
        {
            PlaceholderDictionary.Add(name, new Entities.Placeholder(name, value));
        }
    }
}