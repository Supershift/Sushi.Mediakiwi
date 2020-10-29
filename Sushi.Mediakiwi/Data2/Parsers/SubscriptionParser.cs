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
    public class SubscriptionParser : ISubscriptionParser
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

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public virtual void Clear()
        {
            if (!Wim.CommonConfiguration.IS_LOAD_BALANCED) return;
            DataParser.Execute("truncate table wim_Subscriptions");
        }

        public virtual void Save(ISubscription entity)
        {
            DataParser.Save<ISubscription>(entity);
        }

        public virtual void Delete(ISubscription entity)
        {
            DataParser.Delete<ISubscription>(entity);
        }

        /// <summary>
        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public virtual ISubscription SelectOne(int ID)
        {
            return DataParser.SelectOne<ISubscription>(ID, false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <returns></returns>
        public virtual ISubscription[] SelectAllActive()
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();

            where.Add(new DatabaseDataValueColumn("Subscription_Scheduled", SqlDbType.DateTime, null, DatabaseDataValueCompareType.Default));
            where.Add(new DatabaseDataValueColumn("Subscription_Scheduled", SqlDbType.DateTime, DateTime.Now, DatabaseDataValueCompareType.SmallerThen, DatabaseDataValueConnectType.Or));
            where.Add(new DatabaseDataValueColumn("Subscription_IsActive", SqlDbType.Bit, true));

            return DataParser.SelectAll<ISubscription>(where).ToArray();
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="listID">The list ID.</param>
        /// <param name="userID">The user ID.</param>
        /// <returns></returns>
        public virtual ISubscription[] SelectAll(int listID, int userID)
        {
            List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
            where.Add(new DatabaseDataValueColumn("Subscription_ComponentList_Key", SqlDbType.Int, listID));

            return DataParser.SelectAll<ISubscription>(where).ToArray();
        }

        public virtual ISubscription[] SelectAll()
        {
            return DataParser.SelectAll<ISubscription>().ToArray();
        }
    }
}