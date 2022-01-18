using Sushi.Mediakiwi.Data.Interfaces;
using Sushi.Mediakiwi.Data.MicroORM;
using Sushi.MicroORM.Mapping;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Countries of the world. Can be used for country references within webpages and or business logic pages.
    /// </summary>
    [DataMap(typeof(CountryMap))]
    public class Country : ICountry
    {
        public class CountryMap : DataMap<Country>
        {
            public CountryMap()
            {
                Table("wim_Countries");
                Id(x => x.ID, "Country_Key");
                Map(x => x.GUID, "Country_Guid");
                Map(x => x.Country_EN, "Country_Name_EN").Length(100);
                Map(x => x.Country_NL, "Country_Name_NL").Length(100);
                Map(x => x.IsActive, "Country_IsActive");
            }
        }

        #region properties
        /// <summary>
        /// Uniqe identifier of the Country
        /// </summary>
        public virtual int ID { get; set; }

        private Guid m_GUID;

        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        public virtual Guid GUID
        {
            get
            {
                if (this.m_GUID == Guid.Empty) this.m_GUID = Guid.NewGuid();
                return m_GUID;
            }
            set { m_GUID = value; }
        }

        /// <summary>
        /// Displayname of the Country (English)
        /// </summary>
        public virtual string Country_EN { get; set; }

        /// <summary>
        /// Displayname of the Country (Dutch)
        /// </summary>
        public virtual string Country_NL { get; set; }

        /// <summary>
        /// Is this country active (visible)?
        /// </summary>
        public bool IsActive { get; set; }

        #endregion properties

        /// <summary>
        /// Select a Country based on its primary key
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static ICountry SelectOne(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            return connector.FetchSingle(ID);
        }

        /// <summary>
        /// Select a Country based on its primary key
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static async Task<ICountry> SelectOneAsync(int ID)
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            return await connector.FetchSingleAsync(ID);
        }

        /// <summary>
        /// Get a Country based on its unique key
        /// </summary>
        /// <param name="countryGUID">The country GUID.</param>
        /// <returns></returns>
        public static ICountry SelectOne(Guid countryGUID)
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, countryGUID);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Get a Country based on its unique key
        /// </summary>
        /// <param name="countryGUID">The country GUID.</param>
        /// <returns></returns>
        public static async Task<ICountry> SelectOneAsync(Guid countryGUID)
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.GUID, countryGUID);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="englishName">Name of the english.</param>
        /// <returns></returns>
        public static ICountry SelectOne(string englishName)
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Country_EN, englishName);
            return connector.FetchSingle(filter);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="englishName">Name of the english.</param>
        /// <returns></returns>
        public static async Task<ICountry> SelectOneAsync(string englishName)
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            var filter = connector.CreateQuery();
            filter.Add(x => x.Country_EN, englishName);
            return await connector.FetchSingleAsync(filter);
        }

        /// <summary>
        /// Select all active Countries
        /// </summary>
        public static ICountry[] SelectAll()
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            var filter = connector.CreateQuery();
            return connector.FetchAll(filter).ToArray();
        }

        /// <summary>
        /// Select all active Countries
        /// </summary>
        public static async Task<ICountry[]> SelectAllAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            var filter = connector.CreateQuery();
            var aresult = await connector.FetchAllAsync(filter);
            return aresult.ToArray();
        }

        /// <summary>
        /// Select all active Countries
        /// </summary>
        /// <param name="languageSortOrder">Sort the output based on the language. Current options are "en" and "nl"</param>
        /// <returns></returns>
        public static ICountry[] SelectAll(string languageSortOrder)
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            var filter = connector.CreateQuery();

            if (languageSortOrder.Equals("nl", StringComparison.CurrentCultureIgnoreCase))
                return connector.FetchAll(filter).OrderBy(x => x.Country_NL).ToArray();
            return connector.FetchAll(filter).OrderBy(x => x.Country_EN).ToArray();
        }

        /// <summary>
        /// Select all active Countries
        /// </summary>
        /// <param name="languageSortOrder">Sort the output based on the language. Current options are "en" and "nl"</param>
        /// <returns></returns>
        public static async Task<ICountry[]> SelectAllAsync(string languageSortOrder)
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            var filter = connector.CreateQuery();

            var aresult = await connector.FetchAllAsync(filter);

            if (languageSortOrder.Equals("nl", StringComparison.CurrentCultureIgnoreCase))
                return aresult.OrderBy(x => x.Country_NL).ToArray();
            return aresult.OrderBy(x => x.Country_EN).ToArray();
        }

        /// <summary>
        /// Select all Countries based on a
        /// </summary>
        /// <param name="onlyReturnActiveCountries">if set to <c>true</c> [only return active countries].</param>
        /// <param name="languageSortOrder">Sort the output based on the language. Current options are "en" and "nl"</param>
        /// <returns></returns>
        public static ICountry[] SelectAll(bool onlyReturnActiveCountries, string languageSortOrder)
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            var filter = connector.CreateQuery();

            if (onlyReturnActiveCountries)
                filter.Add(x => x.IsActive, true);

            if (languageSortOrder.Equals("nl", StringComparison.CurrentCultureIgnoreCase))
                return connector.FetchAll(filter).OrderBy(x => x.Country_NL).ToArray();
            return connector.FetchAll(filter).OrderBy(x => x.Country_EN).ToArray();
        }

        /// <summary>
        /// Select all Countries based on a
        /// </summary>
        /// <param name="onlyReturnActiveCountries">if set to <c>true</c> [only return active countries].</param>
        /// <param name="languageSortOrder">Sort the output based on the language. Current options are "en" and "nl"</param>
        /// <returns></returns>
        public static async Task<ICountry[]> SelectAllAsync(bool onlyReturnActiveCountries, string languageSortOrder)
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            var filter = connector.CreateQuery();

            if (onlyReturnActiveCountries)
                filter.Add(x => x.IsActive, true);

            var aresult = await connector.FetchAllAsync(filter);

            if (languageSortOrder.Equals("nl", StringComparison.CurrentCultureIgnoreCase))
                return aresult.OrderBy(x => x.Country_NL).ToArray();
            return aresult.OrderBy(x => x.Country_EN).ToArray();
        }

        public void Save()
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            connector.Save(this);
        }

        public async Task SaveAsync()
        {
            var connector = ConnectorFactory.CreateConnector<Country>();
            await connector.SaveAsync(this);
        }
    }
}