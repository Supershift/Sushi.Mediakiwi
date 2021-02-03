using System;

namespace Sushi.Mediakiwi.Data.Statistics.Parsers
{
    public interface IVisitorClickParser
    {
        VisitorLog Log(IVisitorClick entity);
        void Save(IVisitorClick entity);
        IVisitorClick[] SelectAll(DateTime from, DateTime to);
        IVisitorClick SelectOne(int ID);
    }
}