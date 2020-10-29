using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Sushi.Mediakiwi.Data.DalReflection;
using System.Xml.Serialization;
using Sushi.Mediakiwi.Data.Parsers;

namespace Sushi.Mediakiwi.Data
{
    ///// <summary>
    ///// 
    ///// </summary>
    //[DatabaseTable("wim_DataFilters")]

    //public class DataFilter : IDataFilter           //WEG, OOK SQL CREATE SCRIPT
    //{
       
    //    static IDataFilterParser _Parser;
    //    static IDataFilterParser Parser
    //    {
    //        get
    //        {
    //            if (_Parser == null)
    //                _Parser = Environment.GetInstance<IDataFilterParser>();
    //            return _Parser;
    //        }
    //    }

    //    /* [MR:07-01-2020]
    //     * HIERVOOR DIENT NOG EEA GEFIXED TE WORDEN, DEZE WERKT MET COLUMN STRING REPLACEMENTS
    //     * IS WEL 1 OP 1 OVERGENOMEN, MAAR DAAR MOET NOG IETS VOOR GEBEUREN !!
    //     */

    //    #region REPLICATED TO Sushi.Mediakiwi.Data.Standard

    //    /// <summary>
    //    /// Initializes a new instance of the <see cref="DataFilter"/> class.
    //    /// </summary>
    //    public DataFilter()
    //    {
    //        this.SqlColumnPrefix = "DataFilter";
    //    }

    //    /// <summary>
    //    /// Clears this instance.
    //    /// </summary>
    //    public static void Clear()
    //    {
    //        Parser.Clear();
    //    }


    //    /// <summary>
    //    /// Selects one instance
    //    /// </summary>
    //    /// <param name="ID">The ID.</param>
    //    /// <returns></returns>
    //    public static IDataFilter SelectOne(int ID)
    //    {
    //        return Parser.SelectOne(ID);
    //    }

    //    /// <summary>
    //    /// Deletes all.
    //    /// </summary>
    //    /// <param name="propertyID">The property ID.</param>
    //    /// <returns></returns>
    //    public static bool DeleteAll(int propertyID)
    //    {
    //        return Parser.DeleteAll(propertyID);
    //    }

    //    public void Save()
    //    {
    //        Parser.Save(this);
    //    }

    //    /// <summary>
    //    /// Selects the one.
    //    /// </summary>
    //    /// <param name="propertyID">The property ID.</param>
    //    /// <param name="itemID">The item ID.</param>
    //    /// <returns></returns>
    //    public static IDataFilter SelectOne(int propertyID, int itemID)
    //    {
    //        return Parser.SelectOne(propertyID, itemID);
    //    }

    //    public void Delete()
    //    {
    //        Parser.Delete(this);
    //    }

    //    ///// <summary>
    //    ///// Selects all.
    //    ///// </summary>
    //    ///// <returns></returns>
    //    //public static DataFilter[] SelectAll(int ListID)
    //    //{
    //    //    List<DataFilter> list = new List<DataFilter>();
    //    //    //List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
    //    //    //where.Add(new DatabaseDataValueColumn("BasketProductLink_Basket_Key", SqlDbType.Int, basketId));

    //    //    foreach (object o in new DataFilter()._SelectAll()) list.Add((DataFilter)o);
    //    //    return list.ToArray();
    //    //}

    //    ///// <summary>
    //    ///// Selects all.
    //    ///// </summary>
    //    ///// <param name="productGroupID">The product group ID.</param>
    //    ///// <returns></returns>
    //    //public static Product[] SelectAll(int productGroupID)
    //    //{
    //    //    List<Product> list = new List<Product>();
    //    //    List<DatabaseDataValueColumn> where = new List<DatabaseDataValueColumn>();
    //    //    where.Add(new DatabaseDataValueColumn("Product_ProductType_Key", SqlDbType.Int, productGroupID));

    //    //    foreach (object o in new Product()._SelectAll(where)) list.Add((Product)o);
    //    //    return list.ToArray();
    //    //}


    //    #region Properties
    //    [XmlIgnore()]
    //    /// <summary>
    //    /// Gets or sets the SQL column prefix.
    //    /// </summary>
    //    /// <value>The SQL column prefix.</value>
    //    protected virtual string SqlColumnPrefix { get; set; }
    //    /// <summary>
    //    /// Gets or sets the product link id.
    //    /// </summary>
    //    /// <value>The product link id.</value>
    //    [DatabaseColumn("<SQL_COL>_Key", SqlDbType.Int, IsPrimaryKey = true, IsOnlyRead = false)]
    //    public int ID { get; set; }

    //    /// <summary>
    //    /// Gets or sets the property ID.
    //    /// </summary>
    //    /// <value>The property ID.</value>
    //    [DatabaseColumn("<SQL_COL>_Property_Key", SqlDbType.Int)]
    //    public int PropertyID { get; set; }

    //    /// <summary>
    //    /// Gets or sets the item ID.
    //    /// </summary>
    //    /// <value>The item ID.</value>
    //    [DatabaseColumn("<SQL_COL>_Item_Key", SqlDbType.Int)]
    //    public int ItemID { get; set; }

    //    string m_Type;
    //    /// <summary>
    //    /// Gets or sets the type.
    //    /// </summary>
    //    /// <value>The type.</value>
    //    [DatabaseColumn("<SQL_COL>_Type", SqlDbType.Char, Length = 1)]
    //    public string Type
    //    {
    //        get { return m_Type; }
    //        set { m_Type = value; }
    //    }


    //    private bool? m_FilterB;
    //    /// <summary>
    //    /// Gets or sets the filter B.
    //    /// </summary>
    //    /// <value>The filter B.</value>
    //    [DatabaseColumn("<SQL_COL>_B", SqlDbType.Bit, IsNullable = true)]
    //    public bool? FilterB
    //    {
    //        get { return m_FilterB; }
    //        set {m_Type = "B"; m_FilterB = value; }
    //    }

    //    private int? m_FilterI;
    //    /// <summary>
    //    /// Gets or sets the filter I.
    //    /// </summary>
    //    /// <value>The filter I.</value>
    //    [DatabaseColumn("<SQL_COL>_I", SqlDbType.Int, IsNullable = true)]
    //    public int? FilterI
    //    {
    //        get { return m_FilterI; }
    //        set { m_Type = "I"; m_FilterI = value; }
    //    }

    //    private decimal? m_FilterD;
    //    /// <summary>
    //    /// Gets or sets the filter D.
    //    /// </summary>
    //    /// <value>The filter D.</value>
    //    [DatabaseColumn("<SQL_COL>_D", SqlDbType.Int, IsNullable = true)]
    //    public decimal? FilterD
    //    {
    //        get { return m_FilterD; }
    //        set { m_Type = "D"; m_FilterD = value; }
    //    }

    //    private DateTime? m_FilterT;
    //    /// <summary>
    //    /// Gets or sets the filter T.
    //    /// </summary>
    //    /// <value>The filter T.</value>
    //    [DatabaseColumn("<SQL_COL>_T", SqlDbType.DateTime, IsNullable = true)]
    //    public DateTime? FilterT
    //    {
    //        get { return m_FilterT; }
    //        set { m_Type = "T"; m_FilterT = value; }
    //    }

    //    private string m_FilterC;
    //    /// <summary>
    //    /// Gets or sets the filter C.
    //    /// </summary>
    //    /// <value>The filter C.</value>
    //    [DatabaseColumn("<SQL_COL>_C", SqlDbType.NVarChar, Length = 255, IsNullable = true)]
    //    public string FilterC
    //    {
    //        get { return m_FilterC; }
    //        set { m_Type = "C"; m_FilterC = value; }
    //    }

    //    public bool IsNewInstance
    //    {
    //        get
    //        {
    //            return this.ID == 0;
    //        }
    //    }
    //    #endregion

    //    #endregion REPLICATED TO Sushi.Mediakiwi.Data.Standard
    //}
}
