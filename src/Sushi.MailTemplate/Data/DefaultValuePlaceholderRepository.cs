using Sushi.Mediakiwi.MicroORM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.MailTemplate.Data
{
    public class DefaultValuePlaceholderRepository
    {
        private readonly Connector<DefaultValuePlaceholder> _connector;

        public DefaultValuePlaceholderRepository(Connector<DefaultValuePlaceholder> connector)
        {
            _connector = connector;
        }

        /// <summary>
        /// Saves the current default value
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveAsync(DefaultValuePlaceholder defaultValuePlaceholder)
        {
            await _connector.SaveAsync(defaultValuePlaceholder);
            return defaultValuePlaceholder.ID;
        }

        /// <summary>
        /// Deletes the current default value
        /// </summary>
        public async Task DeleteAsync(DefaultValuePlaceholder defaultValuePlaceholder)
        {
            await _connector.DeleteAsync(defaultValuePlaceholder);
        }


        /// <summary>
        /// Fetch all default values by mail template identifier
        /// </summary>
        /// <param name="mailTemplateIdentifier"></param>
        /// <returns></returns>
        public async Task<List<DefaultValuePlaceholder>> FetchAllByMailTemplateAsync(string mailTemplateIdentifier)
        {
            var query = _connector.CreateQuery();
            query.AddSql(@"EXISTS (SELECT NULL FROM wim_MailTemplates where MailTemplate_Identifier = @mailTemplateIdentifier)");
            query.AddParameter("@mailTemplateIdentifier", mailTemplateIdentifier);

            return await _connector.FetchAllAsync(query);
        }
    }
}