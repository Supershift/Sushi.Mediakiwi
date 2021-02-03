using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    /// <summary>
    /// Countries of the world. Can be used for country references within webpages and or business logic pages.
    /// </summary>
    public class Country : ICountry
    {
        static ICountryParser _Parser;
        static ICountryParser Parser
        {
            get
            {
                if (_Parser == null)
                    _Parser = Environment.GetInstance<ICountryParser>();
                return _Parser;
            }
        }

        ///// <summary>
        ///// Clears this instance.
        ///// </summary>
        //public static void Clear()
        //{
        //    Parser.Clear();
        //}

        #region REPLICATED TO Sushi.Mediakiwi.Data.Standard
        /// <summary>
        /// Select a Country based on its primary key
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public static ICountry SelectOne(int ID)
        {
            return Parser.SelectOne(ID);
        }

        /// <summary>
        /// Get a Country based on its unique key
        /// </summary>
        /// <param name="countryGUID">The country GUID.</param>
        /// <returns></returns>
        public static ICountry SelectOne(Guid countryGUID)
        {
            return Parser.SelectOne(countryGUID);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="englishName">Name of the english.</param>
        /// <returns></returns>
        public static ICountry SelectOne(string englishName)
        {
            return Parser.SelectOne(englishName);
        }

        /// <summary>
        /// Select all active Countries
        /// </summary>
        public static ICountry[] SelectAll()
        {
            return Parser.SelectAll(true, "en");
        }

        /// <summary>
        /// Select all active Countries
        /// </summary>
        /// <param name="languageSortOrder">Sort the output based on the language. Current options are "en" and "nl"</param>
        /// <returns></returns>
        public static ICountry[] SelectAll(string languageSortOrder)
        {
            return Parser.SelectAll(true, languageSortOrder);
        }

        /// <summary>
        /// Select all Countries based on a
        /// </summary>
        /// <param name="onlyReturnActiveCountries">if set to <c>true</c> [only return active countries].</param>
        /// <param name="languageSortOrder">Sort the output based on the language. Current options are "en" and "nl"</param>
        /// <returns></returns>
        public static ICountry[] SelectAll(bool onlyReturnActiveCountries, string languageSortOrder)
        {
            return Parser.SelectAll(onlyReturnActiveCountries, languageSortOrder);
        }

        /// <summary>
        /// Uniqe identifier of the Country
        /// </summary>
        [DatabaseColumn("Country_Key", SqlDbType.Int, IsPrimaryKey = true)]
        public virtual int ID { get; set; }

        private Guid m_GUID;
        /// <summary>
        /// Unique identifier used for import/export
        /// </summary>
        [DatabaseColumn("Country_Guid", SqlDbType.UniqueIdentifier, IsMigrationKey = true)]
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
        [DatabaseColumn("Country_Name_EN", SqlDbType.NVarChar, Length = 100)]
        public virtual string Country_EN { get; set; }

        string m_Country_NL;
        /// <summary>
        /// Displayname of the Country (Dutch)
        /// </summary>
        [DatabaseColumn("Country_Name_NL", SqlDbType.NVarChar, Length = 100)]
        public virtual string Country_NL { get; set; }

        /// <summary>
        /// Is this country active (visible)? 
        /// </summary>
        [DatabaseColumn("Country_IsActive", SqlDbType.Bit)]
        public bool IsActive { get; set; }


        public void Save()
        {
            Parser.Save(this);
        }
        #endregion MOVED TO Sushi.Mediakiwi.Data.Standard
    }
}