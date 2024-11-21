using Sushi.Mediakiwi.MicroORM.Mapping;

namespace Sushi.MailTemplate.Data
{
    /// <summary>
    /// DefaultValuePlaceholder data entity
    /// </summary>
    [DataMap(typeof(DefaultValuePlaceholderMap))]
    public class DefaultValuePlaceholder
    {
        /// <summary>
        /// Sushi.MicroORM Mapper class
        /// </summary>
        public class DefaultValuePlaceholderMap : DataMap<DefaultValuePlaceholder>
        {
            /// <summary>
            /// Sushi.MicroORM Mapper ctor
            /// </summary>
            public DefaultValuePlaceholderMap()
            {
                Table("wim_MailTemplateDefaultValuePlaceholders");
                Id(x => x.ID, "MailTemplateDefaultValuePlaceholder_Key");
                Map(x => x.MailTemplateID, "MailTemplateDefaultValuePlaceholder_MailTemplate_Key"); // DBType System.Int
                Map(x => x.Placeholder, "MailTemplateDefaultValuePlaceholder_Placeholder"); // DBType System.String
                Map(x => x.Value, "MailTemplateDefaultValuePlaceholder_Value"); // DBType System.String
            }
        }

        /// <summary>
        /// ID of the current default value
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// GUID  of the current default value
        /// </summary>
        public int MailTemplateID { get; set; }

        /// <summary>
        /// Placeholder of the current default value
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// Value of the current default value
        /// </summary>
        public string Value { get; set; }
    }
}