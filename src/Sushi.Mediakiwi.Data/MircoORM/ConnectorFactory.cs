using Sushi.Mediakiwi.Data.Caching;
using Sushi.Mediakiwi.MicroORM;
using Sushi.Mediakiwi.MicroORM.Mapping;

namespace Sushi.Mediakiwi.Data.MicroORM
{
    public static class ConnectorFactory
    {
        public static CachedConnector<T> CreateConnector<T>(DataMap<T> map = null) where T : new()
        {
            //create connector
            var result = new CachedConnector<T>(map)
            {
                FetchSingleMode = FetchSingleMode.ReturnNewObjectWhenNotFound
            };
            //can add configuration here like portal/connection string mapping

            return result;
        }

        
    }
}
