using Sushi.Mediakiwi.MicroORM;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.MailTemplate.Data
{
    public class MailTemplateListRepository
    {
        private readonly Connector<MailTemplateList> _connector;

        public MailTemplateListRepository(Connector<MailTemplateList> connector)
        {
            _connector = connector;
        }

        /// <summary>
        /// Fetch all mail templates
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<List<MailTemplateList>> FetchAllAsync(string name = "")
        {
            var query = _connector.CreateQuery();

            query.AddOrder(x => x.Name);

            if (!string.IsNullOrEmpty(name))
            {
                string searchLike = $"%{name}%";
                query.AddSql($"(MailTemplate_Name LIKE @searchLike OR MailTemplate_Identifier LIKE @searchLike)");
                query.AddParameter("@searchLike", System.Data.SqlDbType.NVarChar, searchLike);
            }

            var result = await _connector.FetchAllAsync(query);

            return result;
        }

        /// <summary>
        /// Fetch one mail template by identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public async Task<MailTemplateList> FetchSingleByIdentifierAsync(string identifier)
        {
            var query = _connector.CreateQuery();
            query.Add(x => x.Identifier, identifier, ComparisonOperator.Like);
            var result = await _connector.FetchSingleAsync(query);
            return result;
        }

        /// <summary>
        /// Fetch one mail template by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MailTemplateList> FetchSingleAsync(int id)
        {
            var query = _connector.CreateQuery();
            query.Add(x => x.ID, id);

            var result = await _connector.FetchSingleAsync(query);
            return result;
        }
    }
}