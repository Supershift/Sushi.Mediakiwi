using System;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface ICountryParser
    {
        //void Clear();
        void Save(ICountry entity);
        ICountry[] SelectAll();
        ICountry[] SelectAll(string languageSortOrder);
        ICountry[] SelectAll(bool onlyReturnActiveCountries, string languageSortOrder);
        ICountry SelectOne(string englishName);
        ICountry SelectOne(int ID);
        ICountry SelectOne(Guid countryGUID);
    }
}