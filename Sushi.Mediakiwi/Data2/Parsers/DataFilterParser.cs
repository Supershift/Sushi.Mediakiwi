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
    // [MR:23-01-2020] Must not be used anymore
    //public class DataFilterParser : IDataFilterParser
    //{
    //    static ISqlEntityParser _DataParser;
    //    static ISqlEntityParser DataParser
    //    {
    //        get
    //        {
    //            if (_DataParser == null)
    //                _DataParser = Environment.GetInstance<ISqlEntityParser>();
    //            return _DataParser;
    //        }
    //    }

    //    /// <summary>
    //    /// Clears this instance.
    //    /// </summary>
    //    public virtual void Clear()
    //    {
    //        if (!Wim.CommonConfiguration.IS_LOAD_BALANCED) return;
    //        DataParser.Execute("truncate table wim_DataFilters");
    //    }

    //    /// <summary>
    //    /// Selects one instance
    //    /// </summary>
    //    /// <param name="ID">The ID.</param>
    //    /// <returns></returns>
    //    public virtual IDataFilter SelectOne(int ID)
    //    {
    //        return DataParser.SelectOne<IDataFilter>(ID, false);
    //    }

    //    ///TODO; Delete Alleen all op de WHERE laten werken, moet nu verplicht 1 entity meegeven
    //    /// <summary>
    //    /// Deletes all.
    //    /// </summary>
    //    /// <param name="propertyID">The property ID.</param>
    //    /// <returns></returns>
    //    public virtual bool DeleteAll(int propertyID)
    //    {
    //        List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
    //        where.Add(new DatabaseDataValueColumn("DataFilter_Property_Key", SqlDbType.Int, propertyID));

    //        //return DataParser.Delete<IDataFilter>(where);
    //        return false;
    //    }

    //    /// <summary>
    //    /// Selects the one.
    //    /// </summary>
    //    /// <param name="propertyID">The property ID.</param>
    //    /// <param name="itemID">The item ID.</param>
    //    /// <returns></returns>
    //    public virtual IDataFilter SelectOne(int propertyID, int itemID)
    //    {
    //        List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
    //        where.Add(new DatabaseDataValueColumn("DataFilter_Property_Key", SqlDbType.Int, propertyID));
    //        where.Add(new DatabaseDataValueColumn("DataFilter_Item_Key", SqlDbType.Int, itemID));

    //        return DataParser.SelectOne<IDataFilter>(where);
    //    }

    //    public virtual void Save(IDataFilter entity)
    //    {
    //        DataParser.Save<IDataFilter>(entity);
    //    }

    //    public void Delete(IDataFilter entity)
    //    {
    //        DataParser.Delete<IDataFilter>(entity);
    //    }
    //}
}