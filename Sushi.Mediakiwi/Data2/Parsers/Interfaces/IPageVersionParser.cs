using System.Collections.Generic;

namespace Sushi.Mediakiwi.Data.Parsers
{
    public interface IPageVersionParser
    {

        bool Save(IPageVersion entity);
        IPageVersion SelectOne(int ID);
        List<IPageVersion> SelectAllOfPage(int pageID);
    }
}