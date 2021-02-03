using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using System.Data.SqlTypes;
using System.Data.SqlClient;

namespace Sushi.Mediakiwi.Data.Connection
{
    /// <summary>
    /// Represents the SQL Server connection wrapper.
    /// </summary>
    public class SqlConnect : IDisposable
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
        ~SqlConnect()
        {
            Dispose(false);
        }

        SqlConnection m_SqlConnect;
        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        public SqlConnection Connection
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

        SqlTransaction m_Transaction;
        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public SqlTransaction Transaction
        {
            get { return m_Transaction; }
            set { m_Transaction = value; }
        }

        string m_TransactionName;
        /// <summary>
        /// Gets or sets the name of the transaction.
        /// </summary>
        /// <value>The name of the transaction.</value>
        public string TransactionName
        {
            get { return m_TransactionName; }
            set { m_TransactionName = value; }
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

        /// <summary>
        /// Opens the specified connection.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public void Open(string connection)
        {
            if (Connection == null)
                this.m_SqlConnect = new SqlConnection(connection);

            //  First validate if Connection exist
            if (Connection == null)
                return;

            //  Validate if the connection is closed
            if (Connection.State == ConnectionState.Closed)
            {
                Connection.Open();
                if (this.IsPartOfTransaction)
                {
                    if (this.TransactionName == null)
                        this.Transaction = Connection.BeginTransaction(this.Isolation);
                    else
                        this.Transaction = Connection.BeginTransaction(this.Isolation, this.TransactionName);
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
