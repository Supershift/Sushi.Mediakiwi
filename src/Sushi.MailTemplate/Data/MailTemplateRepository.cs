using Sushi.Mediakiwi.MicroORM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sushi.MailTemplate.Data
{
    public class MailTemplateRepository
    {
        private readonly Connector<MailTemplate> _connector;
        private readonly MailTemplateListRepository _mailTemplateListRepository;

        public MailTemplateRepository(Connector<MailTemplate> connector, MailTemplateListRepository mailTemplateListRepository)
        {
            _connector = connector;
            _mailTemplateListRepository = mailTemplateListRepository;
        }

        /// <summary>
        /// Fetch a single mail template by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<MailTemplate> FetchSingleAsync(int id)
        {
            var query = _connector.CreateQuery();
            query.Add(x => x.ID, id);
            var result = await _connector.FetchSingleAsync(query);
            return result;
        }

        /// <summary>
        /// Retrieves all MailTemplates that matches the specified identifiers
        /// </summary>
        /// <param name="identifiers">Collection of identifiers</param>
        /// <param name="onlyPublished"></param>
        /// <returns></returns>
        public async Task<List<MailTemplate>> FetchAllByIdentifiersAsync(IEnumerable<string> identifiers, bool onlyPublished = true)
        {
            var query = _connector.CreateQuery();
            query.Add(x => x.Identifier, identifiers, ComparisonOperator.In);

            if (onlyPublished)
            {
                query.Add(x => x.IsPublished, onlyPublished);
            }

            var result = await _connector.FetchAllAsync(query);
            return result;
        }

        internal async Task<MailTemplate> FetchSingleAsync(string identifier, int versionMajor)
        {
            var query = _connector.CreateQuery();
            query.Add(x => x.VersionMajor, versionMajor);
            query.Add(x => x.Identifier, identifier);

            var result = await _connector.FetchSingleAsync(query);
            return result;
        }

        /// <summary>
        /// Returns a single Mailtemplate, based on the supplied Identifier.
        /// The latest version will be returned in case there are multiple
        /// </summary>
        /// <param name="identifier">The mailtemplate identifier</param>
        /// <param name="onlyPublished">Only include published mailtemplates</param>
        /// <returns></returns>
        internal async Task<MailTemplate> FetchSingleByIdentifierAsync(string identifier, bool onlyPublished)
        {
            var query = _connector.CreateQuery();
            query.Add(x => x.Identifier, identifier);
            if (onlyPublished)
            {
                query.Add(x => x.IsPublished, true);
                query.Add(x => x.VersionMinor, 0);
            }

            query.AddOrder(x => x.VersionMajor, SortOrder.DESC);
            var result = await _connector.FetchSingleAsync(query);
            return result;
        }

        internal async Task<List<MailTemplate>> FetchAllByIdentifierAsync(string identifier)
        {
            var query = _connector.CreateQuery();
            query.Add(x => x.Identifier, identifier);

            var result = await _connector.FetchAllAsync(query);
            return result;
        }

        /// <summary>
        /// Saves the current mail template to the database with checks on the validity of placeholders in the body and subject.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userDisplayname"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task<int> SaveAsync(MailTemplate mailTemplate, int userID, string userDisplayname, string userEmail)
        {
            if (Logic.Helper.IsValidTemplate(mailTemplate.Body, mailTemplate.Subject))
            {
                mailTemplate.Body = Logic.Helper.ReplaceLegacyUnsubscribe(mailTemplate.Body);
                mailTemplate.Subject = Logic.Helper.ReplaceLegacyUnsubscribe(mailTemplate.Subject);

                var currentTemplateInDatabase = await _mailTemplateListRepository.FetchSingleAsync(mailTemplate.ID);
                var result = await _mailTemplateListRepository.FetchSingleByIdentifierAsync(mailTemplate.Identifier);

                if (result != null && result.ID != mailTemplate.ID)
                {
                    return 0;
                }

                if (currentTemplateInDatabase != null && currentTemplateInDatabase.Identifier != mailTemplate.Identifier)
                {
                    // user changed the identifier, special case. All versions need to be updated
                    var mailTemplatesToUpdate = await FetchAllByIdentifierAsync(currentTemplateInDatabase.Identifier);

                    foreach (var template in mailTemplatesToUpdate)
                    {
                        template.Identifier = mailTemplate.Identifier;
                        await _connector.SaveAsync(mailTemplate);
                    }
                }

                if (mailTemplate.ID > 0)
                {
                    // existing template, save as new minor version
                    mailTemplate.VersionMinor++;
                    mailTemplate.ID = 0;
                    mailTemplate.UserID = userID;
                    mailTemplate.UserName = $"{userDisplayname} ({userEmail})";
                    mailTemplate.DateLastUpdated = DateTime.UtcNow;
                    // saving template, not publishing, new minor, so not published
                    mailTemplate.IsPublished = false;
                }
                else
                {
                    // new, not archived, first minor version
                    mailTemplate.IsPublished = false;
                    mailTemplate.IsArchived = false;
                    mailTemplate.VersionMajor = 0;
                    mailTemplate.VersionMinor = 1;
                    mailTemplate.DateLastUpdated = DateTime.UtcNow;
                    mailTemplate.DateCreated = DateTime.UtcNow;
                    mailTemplate.GUID = Guid.NewGuid();
                }

                await _connector.SaveAsync(mailTemplate);

                return mailTemplate.ID;
            }

            return 0;
        }

        /// <summary>
        /// Revert the current mail template to the previous major version.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userDisplayname"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task<bool> RevertAsync(MailTemplate mailTemplate, int userID, string userDisplayname, string userEmail)
        {
            var result = await FetchSingleAsync(mailTemplate.Identifier, mailTemplate.VersionMajor);

            if (result != null && result.ID != mailTemplate.ID)
            {
                // unpublish current version
                result.IsPublished = false;
                await _connector.SaveAsync(result);

                // create new version from old published template and publish directly
                // this way the edit is saved as well
                result.ID = 0;
                result.IsPublished = true;
                result.VersionMajor++;
                result.UserID = userID;
                result.UserName = $"{userDisplayname} ({userEmail})";
                await _connector.SaveAsync(result);
            }

            return true;
        }

        /// <summary>
        /// Publishes the current mail template by making it a major version.
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userDisplayname"></param>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        public async Task<bool> PublishAsync(MailTemplate mailTemplate, int userID, string userDisplayname, string userEmail)
        {
            if (Logic.Helper.IsValidTemplate(mailTemplate.Body, mailTemplate.Subject))
            {
                // unpublish all previous
                var query = _connector.CreateQuery();
                query.AddParameter("@identifier", System.Data.SqlDbType.NVarChar, mailTemplate.Identifier);

                await _connector.ExecuteNonQueryAsync($@"UPDATE wim_MailTemplates SET MailTemplate_IsPublished = 0 WHERE MailTemplate_Identifier = @identifier", query);

                // create new version
                mailTemplate.VersionMajor++;
                mailTemplate.VersionMinor = 0;
                mailTemplate.IsPublished = true;
                mailTemplate.IsArchived = false;
                mailTemplate.UserID = userID;
                mailTemplate.UserName = $"{userDisplayname} ({userEmail})";
                mailTemplate.ID = 0;
                mailTemplate.DateLastUpdated = DateTime.UtcNow;
                mailTemplate.DateCreated = DateTime.UtcNow;
                await _connector.SaveAsync(mailTemplate);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes mail templates that are identified by their id.
        /// </summary>
        /// <param name="mailTemplateIDs"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(List<int> mailTemplateIDs)
        {
            if (mailTemplateIDs?.Count > 0)
            {
                var joinedMailTemplateIDs = string.Join(",", mailTemplateIDs);
                await _connector.ExecuteNonQueryAsync($@"UPDATE wim_MailTemplates SET MailTemplate_IsArchived = 1 WHERE MailTemplate_Key IN ({joinedMailTemplateIDs})");

                return true;
            }
            return false;
        }
    }
}