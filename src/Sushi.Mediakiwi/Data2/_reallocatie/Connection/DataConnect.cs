using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.Common;
using System.Data.SqlTypes;
using System.Data.SqlClient;
//using InterSystems.Data.CacheClient;
//using InterSystems.Data.CacheTypes;

namespace Sushi.Mediakiwi.Data.Connection
{
    /// <summary>
    /// Represents the generic connection wrapper. Supported connection types are SQLServer, ODBC and OLEDB
    /// </summary>
    public class DataConnect : IDisposable
    {
        bool m_Disposed = false;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.m_Disposed)
            {
                if (disposing)
                {
                    //  Dispose unmanaged objects
                    this.Close();
                }
            }
            this.m_Disposed = true;
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SqlConnect"/> is reclaimed by garbage collection.
        /// </summary>
        ~DataConnect()
        {
            Dispose(false);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConnect"/> class.
        /// </summary>
        public DataConnect()
        {
            this.m_ConnectionType = Sushi.Mediakiwi.Data.Common.DatabaseConnectionType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataConnect"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        public DataConnect(Sushi.Mediakiwi.Data.DataConnectionType type)
        {
            this.m_ConnectionType = type;
        }

        IDbConnection m_SqlConnect;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public IDbConnection Connection
        {
            get { return m_SqlConnect; }
        }

        string m_Connection;
        /// <summary>
        /// The database connectionString
        /// </summary>
        public string ConnectionString
        {
            get { return m_Connection; }
            set { m_Connection = value; }
        }

        bool m_IsPartOfTransaction;
        /// <summary>
        /// Gets or sets a value indicating whether this instance is part of transaction.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is part of transaction; otherwise, <c>false</c>.
        /// </value>
        public bool IsPartOfTransaction
        {
            get { return m_IsPartOfTransaction; }
            set { m_IsPartOfTransaction = value; }
        }

        IDbTransaction m_Transaction;
        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public IDbTransaction Transaction
        {
            get { return m_Transaction; }
            set { m_Transaction = value; }
        }

        IsolationLevel m_Isolation = IsolationLevel.ReadCommitted;
        /// <summary>
        /// Gets or sets the isolation (Default value is IsolationLevel.ReadCommitted).
        /// </summary>
        /// <value>The isolation.</value>
        public IsolationLevel Isolation
        {
            get { return m_Isolation; }
            set { m_Isolation = value; }
        }

        /// <summary>
        /// Opens this connection instance.
        /// </summary>
        public void Open()
        {
            this.Open(this.ConnectionString);
        }

        Sushi.Mediakiwi.Data.DataConnectionType m_ConnectionType;
        /// <summary>
        /// Gets the type of the connection.
        /// </summary>
        /// <value>The type of the connection.</value>
        public Sushi.Mediakiwi.Data.DataConnectionType ConnectionType
        {
            get { return m_ConnectionType; }
            set { m_ConnectionType = value; }
        }

        /// <summary>
        /// Opens the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void Open(string connection)
        {
            if (Connection == null)
            {
                switch (ConnectionType)
                {
                    case  Sushi.Mediakiwi.Data.DataConnectionType.SqlServer:
                        this.m_SqlConnect = new SqlConnection(connection);
                        break;
                    case Sushi.Mediakiwi.Data.DataConnectionType.Odbc:
                        this.m_SqlConnect = new OdbcConnection(connection);
                        break;
                    case Sushi.Mediakiwi.Data.DataConnectionType.OleDb:
                        this.m_SqlConnect = new OleDbConnection(connection);
                        break;
                    //case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache:
                    //    this.m_SqlConnect = new CacheConnection(connection);
                    //    break;
                }
            }

            //  First validate if Connection exist
            if (Connection == null)
                return;

            //  Validate if the connection is closed
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
                if (this.IsPartOfTransaction)
                {
                    this.Transaction = Connection.BeginTransaction(this.Isolation);
                }
            }
        }

        /// <summary>
        /// Closes this connection instance.
        /// </summary>
        public void Close()
        {
            //  First validate if Connection exist
            if (Connection == null)
                return;
            //  Validate if the connection is closed
            if (Connection.State != ConnectionState.Closed)
                Connection.Close();
        }
    }
}
