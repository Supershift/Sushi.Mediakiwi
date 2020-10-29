using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Sushi.Mediakiwi.Data.DalReflection;
using Sushi.Mediakiwi.Framework;

namespace Sushi.Mediakiwi.Data.Statistics.Parsers
{
    /// <summary>
    /// 
    /// </summary>
    public class VisitorClickParser : IVisitorClickParser
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


        /// Selects the one.
        /// </summary>
        /// <param name="ID">The ID.</param>
        /// <returns></returns>
        public virtual IVisitorClick SelectOne(int ID)
        {
            return DataParser.SelectOne<IVisitorClick>(ID, false);
        }

        /// <summary>
        /// Selects all.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        public virtual IVisitorClick[] SelectAll(DateTime from, DateTime to)
        {
            List<DatabaseDataValueColumn> list = new List<DatabaseDataValueColumn>();
            DataRequest data = new DataRequest();
            data.AddWhere("VisitorClick_Created BETWEEN @D1 AND @D2");
            data.AddParam("D1", from, SqlDbType.DateTime);
            data.AddParam("D2", to, SqlDbType.DateTime);

            return DataParser.SelectAll<IVisitorClick>(data).ToArray();
        }

        /// <summary>
        /// Gets the log.
        /// </summary>
        /// <value>The log.</value>
        public virtual VisitorLog Log(IVisitorClick entity)
        {
            return VisitorLog.SelectOne(entity.VisitorLogID);
        }

        public virtual void Save(IVisitorClick entity)
        {
            if (entity.ProfileID > 0)
            {

                string sql = string.Format("update wim_VisitorLogs set VisitorLog_Profile_Key = {0}, VisitorLog_Pageview = VisitorLog_Pageview +1 where VisitorLog_Key = {1}"
                    , entity.ProfileID, entity.VisitorLogID);
                DataParser.Execute(sql);
            }
            else
            {
                string sql = string.Format("update wim_VisitorLogs set VisitorLog_Pageview = VisitorLog_Pageview +1 where VisitorLog_Key = {0}", entity.VisitorLogID);
                DataParser.Execute(sql);
            }
            DataParser.Save<IVisitorClick>(entity);
        }
    }
}
