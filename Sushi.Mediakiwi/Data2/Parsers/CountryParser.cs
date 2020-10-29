using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public class CountryParser : ICountryParser
    {
        static ISqlEntityParser _DataParser;
        static ISqlEntityParser DataParser
        {
            get
            {
                if (_DataParser == null)
                    _DataParser = Environment.GetInstance<ISqlEntityParser>();
                return _DataParser;
            }
        }

        // [MR:23-01-2020] RISKY and gone
        ///// <summary>
        ///// Clears this instance.
        ///// </summary>
        //public virtual void Clear()     //WEG
        //{
        //    if (!Wim.CommonConfiguration.IS_LOAD_BALANCED) return;
        //    DataParser.Execute("truncate table wim_Countries");
        //}

        public virtual void Save(ICountry entity)
        {
            DataParser.Save<ICountry>(entity);
        }

        /// <summary>
        /// Select a Country based on its primary key
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public virtual ICountry SelectOne(int ID)
        {
            return DataParser.SelectOne<ICountry>(ID, false);
        }

        /// <summary>
        /// Get a Country based on its unique key
        /// </summary>
        /// <param name="countryGUID">The country GUID.</param>
        /// <returns></returns>
        public virtual ICountry SelectOne(Guid countryGUID)
        {
            return DataParser.SelectOne<ICountry>(countryGUID, false);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="englishName">Name of the english.</param>
        /// <returns></returns>
        public virtual ICountry SelectOne(string englishName)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Country_Name_EN", SqlDbType.NVarChar, englishName));
            return DataParser.SelectOne<ICountry>(where);
        }

        /// <summary>
        /// Select all active Countries
        /// </summary>
        public virtual ICountry[] SelectAll()
        {
            return SelectAll(true, "en");
        }

        /// <summary>
        /// Select all active Countries
        /// </summary>
        /// <param name="languageSortOrder">Sort the output based on the language. Current options are "en" and "nl"</param>
        /// <returns></returns>
        public virtual ICountry[] SelectAll(string languageSortOrder)
        {
            return SelectAll(true, languageSortOrder);
        }

        //TODO; SortOrder Implementeren
        /// <summary>
        /// Select all Countries based on a
        /// </summary>
        /// <param name="onlyReturnActiveCountries">if set to <c>true</c> [only return active countries].</param>
        /// <param name="languageSortOrder">Sort the output based on the language. Current options are "en" and "nl"</param>
        /// <returns></returns>
        public virtual ICountry[] SelectAll(bool onlyReturnActiveCountries, string languageSortOrder)
        {
            //if (languageSortOrder.Equals("en", StringComparison.InvariantCultureIgnoreCase))
            //    implement.SqlOrder = "Country_Name_EN";
            //if (languageSortOrder.Equals("nl", StringComparison.InvariantCultureIgnoreCase))
            //    implement.SqlOrder = "Country_Name_NL";

            List<ICountry> countries = new List<ICountry>();

            if (onlyReturnActiveCountries)
            {
                List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
                where.Add(new DatabaseDataValueColumn("Country_IsActive", SqlDbType.Bit, 1));   
                foreach (object o in DataParser.SelectAll<ICountry>(where).ToArray()) countries.Add((ICountry)o);
                return countries.ToArray();
            }
            foreach (object o in DataParser.SelectAll<ICountry>().ToArray()) countries.Add((ICountry)o);
            return countries.ToArray();
        }
    }
}