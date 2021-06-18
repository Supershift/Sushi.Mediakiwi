using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace Sushi.Mediakiwi.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public class Splitlist
    {
        private bool m_isDataTableType;
        private int m_RowCount;
        private int m_MaxLength;
        private int m_Length;
        private List<object> m_List;
        private List<DataTable> m_DataTableList;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public object this[int list]
        {
            get {
                if (m_isDataTableType)
                    return m_DataTableList[list];
                if (this.m_List == null || this.m_List.Count == 0)
                    return null;

                return (IList)this.m_List[list];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public System.Type ListType()
        {
            if (m_isDataTableType)
            {
                return this.m_DataTableList[0].GetType();
            }
            else
            {
                return this.m_List[0].GetType();
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxRowCount
        {
            get { return this.m_RowCount; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int MaxListCount
        {
            get { return this.m_MaxLength; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ItemCount
        {
            get { return this.m_Length; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ListCount
        {
            get {
                if (m_isDataTableType)
                    return m_DataTableList.Count;
                return this.m_List.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="rowCountPerList"></param>
        public Splitlist(IList list, int rowCountPerList) : this(list, rowCountPerList, -1) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="rowCountPerList"></param>
        /// <param name="maxListCount"></param>
        public Splitlist(IList list, int rowCountPerList, int maxListCount)
        {
            this.m_RowCount = rowCountPerList;
            this.m_MaxLength = (maxListCount == 0 ? -1 : maxListCount);
            this.m_List = new List<object>();
            this.InitiateSplit( list.GetEnumerator() );
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="rowCountPerList"></param>
        /// <param name="maxListCount"></param>
        public Splitlist(System.Array list, int rowCountPerList, int maxListCount)
        {
            this.m_RowCount = rowCountPerList;
            this.m_MaxLength = (maxListCount == 0 ? -1 : maxListCount);
            this.m_List = new List<object>();
            this.InitiateSplit(list.GetEnumerator());
        }

        #region Splitlist :: DataTable
        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="rowCountPerList"></param>
        public Splitlist(DataTable list, int rowCountPerList ) 
            : this( list, rowCountPerList, -1 ) {}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="list"></param>
        /// <param name="rowCountPerList"></param>
        /// <param name="maxListCount"></param>
        public Splitlist(DataTable list, int rowCountPerList, int maxListCount)
        {
            m_isDataTableType = true;
            this.m_RowCount = rowCountPerList;
            this.m_MaxLength = (maxListCount == 0 ? -1 : maxListCount);
            this.m_Length = list.Rows.Count;
            m_DataTableList = new List<DataTable>();

            if ( this.m_Length > 0 )
                this.InitiateSplit( list );
        }

        private void InitiateSplit(DataTable list)
        {
            bool hasAvailableCandidate = false;

            //  Copy datatable structure
            DataTable listItem = new DataTable();
            listItem = list.Clone();

            int rowcount = 0, totalcount = 0;
            foreach ( DataRow row in list.Rows )
            {
                rowcount++;
                totalcount++;
                //  Import the rows into the newly created subset
                listItem.ImportRow( row );
                hasAvailableCandidate = true;

                //  When the maximum row count is exceeded, add to the list and start over
                if ( rowcount == this.m_RowCount )
                {
                    rowcount = 0;
                    m_DataTableList.Add(listItem);
                    hasAvailableCandidate = false;

                    if (m_DataTableList.Count == this.m_MaxLength - 1)
                        break;

                    //  Copy datatable structure
                    listItem = new DataTable();
                    listItem = list.Clone();
                }
                
            }
            //  Add last items to new list
            if (hasAvailableCandidate)
                m_DataTableList.Add(listItem);

            this.m_Length = totalcount;
        }
        #endregion

        void InitiateSplit(System.Collections.IEnumerator list)
        {
            bool hasAvailableCandidate = false;

            List<object> listItem = new List<object>();
            int rowcount = 0, totalcount = 0;
            
            while ( list.MoveNext() )
            {
                rowcount++;
                totalcount++;
                listItem.Add( list.Current );
                hasAvailableCandidate = true;

                //  When the maximum row count is exceeded, add to the list and start over
                if ( rowcount == this.m_RowCount )
                {
                    rowcount = 0;
                    this.m_List.Add( listItem );
                    hasAvailableCandidate = false;
                        
                    if ( this.m_List.Count == this.m_MaxLength -1)
                        break;

                    listItem = new List<object>();
                }
            }
            //  Add last items to new list
            if (hasAvailableCandidate)
                this.m_List.Add( listItem );

            this.m_Length = totalcount;
        }
    }
}