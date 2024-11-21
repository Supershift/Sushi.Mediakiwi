using Sushi.Mediakiwi.MicroORM.Mapping;

namespace Sushi.MailTemplate.Data
{
    /// <summary>
    /// MailTemplate view data entity
    /// </summary>
    [DataMap(typeof(MailTemplateListMap))]
    public class MailTemplateList : MailTemplate
    {
        /// <summary>
        /// Sushi.MicroORM Mapper class
        /// </summary>
        public class MailTemplateListMap : DataMap<MailTemplateList>
        {
            /// <summary>
            /// Sushi.MicroORM Mapper ctor
            /// </summary>
            public MailTemplateListMap()
            {
                Table("vw_MailTemplates");
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
                Map(x => x.seqnum, "seqnum"); // DBType System.Int64

            }
        }
        /// <summary>
        /// Sequence number in the view
        /// </summary>
        public long? seqnum { get; set; }


        /// <summary>
        /// Internal property for Mediakiwi to see if the record is selected
        /// </summary>
        public bool _IsSelected { get; set; }
    }
}