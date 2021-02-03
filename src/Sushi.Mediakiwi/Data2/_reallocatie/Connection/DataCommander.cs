using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlTypes;
using System.Data.SqlClient;
//using InterSystems.Data.CacheClient;
//using InterSystems.Data.CacheTypes;

namespace Sushi.Mediakiwi.Data.Connection
{
    /// <summary>
    /// Represents the generic data commander. Supported connection types are SQLServer, ODBC and OLEDB
    /// </summary>
    public class DataCommander : IDisposable
    {
        private Regex m_parameterCleanup = new Regex(@"@.*? ", RegexOptions.IgnoreCase);
        private IDbCommand m_Command;
        private IDbDataParameter m_Parameter;
        internal string m_Parameterlist;
        private bool m_Disposed;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose( true );
            GC.SuppressFinalize( this );
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose( bool disposing )
        {
            if( !this.m_Disposed )
            {
                if( disposing )
                {
                    if ( !this.Connect.IsPartOfTransaction )
                        this.Close();
                }
            }
            this.m_Disposed = true;         
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the
        /// <see cref="SqlCommander"/> is reclaimed by garbage collection.
        /// </summary>
        ~DataCommander()      
        {
            Dispose( false );
        }

        private DataConnect _connect;
        /// <summary>
        /// Gets or sets the connect.
        /// </summary>
        /// <value>The connect.</value>
        public DataConnect Connect
        {
            set { this._connect = value; }
            get { return this._connect; }
        }

        /// <summary>
        /// The database connectionString
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return this.Connect.ConnectionString; }
            set { this.Connect.ConnectionString = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is part of transaction.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is part of transaction; otherwise, <c>false</c>.
        /// </value>
        public bool IsPartOfTransaction
        {
            get { return this.Connect.IsPartOfTransaction; }
            set { this.Connect.IsPartOfTransaction = value; }
        }

        private string m_Text;
        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        /// <value>The text.</value>
        public string Text
        {
            get { return this.m_Text; }
            set 
            { 
                this.m_Text = CleanUpSql(value); 
            }
        }

        private CommandType m_CommandType;
        /// <summary>
        /// Gets or sets the commandtype.
        /// </summary>
        /// <value>The commandtype.</value>
        public CommandType Commandtype
        {
            get { return this.m_CommandType; }
            set { this.m_CommandType = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommander"/> class.
        /// </summary>
        /// <param name="connect">The connect.</param>
        public DataCommander(DataConnect connect)
        {
            this.Commandtype = CommandType.Text;
            this.ConnectionType = connect.ConnectionType;
            this.Connect = connect;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommander"/> class.
        /// </summary>
        public DataCommander() : this(string.Empty) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommander"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public DataCommander( string connection ) : this( connection, string.Empty, CommandType.Text ) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommander"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="Text">The SQL text.</param>
        /// <param name="type">The type.</param>
        public DataCommander(string connection, string Text, CommandType type)
        {
            this.m_ConnectionType = Sushi.Mediakiwi.Data.Common.DatabaseConnectionType;

            if ( this.Connect == null )
                this.Connect = new DataConnect(this.m_ConnectionType);

            this.m_Parameterlist = string.Empty;
            this.Text = CleanUpSql(Text);
            this.Commandtype = type;
            this.ConnectionString = connection;
        }

        /// <summary>
        /// Cleans up SQL.
        /// </summary>
        /// <param name="sqlText">The SQL text.</param>
        /// <returns></returns>
        private string CleanUpSql(string sqlText)
        {
            sqlText = Regex.Replace(sqlText, "\n", "");
            sqlText = Regex.Replace(sqlText, "(  *)", " ");
            sqlText = Regex.Replace(sqlText, "( , )", " ,");
            sqlText = Regex.Replace(sqlText, "( = )", " =");

            if (this.ConnectionType != DataConnectionType.SqlServer)
                sqlText = m_parameterCleanup.Replace(string.Concat(sqlText, " "), "? ").Trim();
            
            return sqlText;
        }

        /// <summary>
        /// Set database environment, like Connection and Command.
        /// </summary>
        private void SetEnvironment()
        {
            if ( this.Connect.Connection == null )
                this.Connect.Open();

            if ( this.m_Command != null )
            {
                if ( this.m_Command.CommandText == null || this.m_Command.CommandText != this.Text )
                {
                    this.m_Command.CommandText = this.Text;
                }
                return;
            }

            switch (ConnectionType)
            {
                case Sushi.Mediakiwi.Data.DataConnectionType.SqlServer:
                    this.m_Command = new SqlCommand();
                    break;
                case Sushi.Mediakiwi.Data.DataConnectionType.Odbc:
                    this.m_Command = new OdbcCommand();
                    break;
                case Sushi.Mediakiwi.Data.DataConnectionType.OleDb:
                    this.m_Command = new OleDbCommand();
                    break;
                //case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache:
                //    this.m_Command = new CacheCommand();
                //    break;
            }

            this.m_Command.Connection = this.Connect.Connection;
            this.m_Command.CommandText = this.Text;
            this.m_Command.CommandType = this.Commandtype;

            if ( this.Connect.IsPartOfTransaction )
            {
                this.m_Command.Transaction = this.Connect.Transaction;
            }
        }


        /// <summary>
        /// Determines whether [is item blank value] [the specified itemvalue].
        /// </summary>
        /// <param name="itemvalue">The itemvalue.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is item blank value] [the specified itemvalue]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsItemBlankValue(object itemvalue, DbType type)
        {
            switch (type)
            {
                case DbType.String:
                case DbType.StringFixedLength:
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                    if (itemvalue.ToString() == string.Empty) return true;
                    break;
                case DbType.Int32:
                    if (itemvalue.ToString() == int.MinValue.ToString()) return true;
                    break;
                case DbType.Int16:
                    if (itemvalue.ToString() == int.MinValue.ToString()) return true;
                    break;
                case DbType.Decimal:
                    if (itemvalue.ToString() == Decimal.MinValue.ToString()) return true;
                    break;
                case DbType.DateTime:
                    if (itemvalue.ToString() == DateTime.MinValue.ToString()) return true;
                    break;

            }
            return false;
        }

        /// <summary>
        /// Determines whether [is item blank value] [the specified itemvalue].
        /// </summary>
        /// <param name="itemvalue">The itemvalue.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is item blank value] [the specified itemvalue]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsItemBlankValue(object itemvalue, SqlDbType type)
        {
            switch (type)
            {
                case SqlDbType.NVarChar:
                case SqlDbType.VarChar:
                    if (itemvalue.ToString() == string.Empty) return true;
                    break;
                case SqlDbType.Int:
                    if (itemvalue.ToString() == int.MinValue.ToString()) return true;
                    break;
                case SqlDbType.TinyInt:
                    if (itemvalue.ToString() == int.MinValue.ToString()) return true;
                    break;
                case SqlDbType.Decimal:
                    if (itemvalue.ToString() == Decimal.MinValue.ToString()) return true;
                    break;
                case SqlDbType.DateTime:
                    if (itemvalue.ToString() == DateTime.MinValue.ToString()) return true;
                    break;

            }
            return false;
        }

        Sushi.Mediakiwi.Data.DataConnectionType m_ConnectionType;
        /// <summary>
        /// Gets the type of the connection.
        /// </summary>
        /// <value>The type of the connection.</value>
        public Sushi.Mediakiwi.Data.DataConnectionType ConnectionType
        {
            get { return m_ConnectionType; }
            set
            {
                m_ConnectionType = value;
                if (this.Connect != null)
                    this.Connect.ConnectionType = value;
            }
        }

        #region Parameter addition
        /// <summary>
        /// Set Parameter as output value
        /// </summary>
        public void SetParameterOutput(string name, DbType type)
        {
            this.SetParameter( name, null, type, 0, ParameterDirection.Output );
        }
        
        /// <summary>
        /// Set Parameter as output value
        /// </summary>
        public void SetParameterOutput(string name, DbType type, int length)
        {
            this.SetParameter( name, null, type, length, ParameterDirection.Output );
        }

        /// <summary>
        /// Set Parameter as output value
        /// </summary>
        public void SetParameterOutput(string name, DbType type, int length, byte scale)
        {
            this.SetParameter( name, null, type, length, scale, ParameterDirection.Output );
        }

        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterInput(string name, DbType type)
        {
            this.SetParameter( name, null, type, 0, ParameterDirection.Input );
        }

        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterInput(string name, object itemvalue, DbType type)
        {
            this.SetParameter( name, itemvalue, type, 0, ParameterDirection.Input );
        }

        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterValidationInput(string name, object itemvalue, DbType type)
        {
            if ( IsItemBlankValue(itemvalue, type) ) 
                this.SetParameterInput( name, null, type );
            else
                this.SetParameterInput( name, itemvalue, type );
        }


        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterInput(string name, object itemvalue, DbType type, int length)
        {
            this.SetParameter( name, itemvalue, type, length, ParameterDirection.Input );
        }

        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterValidationInput(string name, object itemvalue, DbType type, int length)
        {
            if ( IsItemBlankValue(itemvalue, type) ) return;
            this.SetParameter( name, itemvalue, type, length, ParameterDirection.Input );
        }

        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterInput(string name, object itemvalue, DbType type, int length, byte scale)
        {
            this.SetParameter( name, itemvalue, type, length, scale, ParameterDirection.Input );
        }
        /// <summary>
        /// Set Sql parameter
        /// </summary>
        public void SetParameter(string name, object itemvalue, DbType type, int length, ParameterDirection direction)
        {
            this.SetParameter( name, itemvalue, type, length, 0, direction );
        }
        #endregion

        #region SQLDbType Parameter addition
        /// <summary>
        /// Set Parameter as output value
        /// </summary>
        public void SetParameterOutput(string name, SqlDbType type)
        {
            this.SetParameter(name, null, type, 0, ParameterDirection.Output);
        }

        /// <summary>
        /// Set Parameter as output value
        /// </summary>
        public void SetParameterOutput(string name, SqlDbType type, int length)
        {
            this.SetParameter(name, null, type, length, ParameterDirection.Output);
        }

        /// <summary>
        /// Set Parameter as output value
        /// </summary>
        public void SetParameterOutput(string name, SqlDbType type, int length, byte scale)
        {
            this.SetParameter(name, null, Convert(type), length, scale, ParameterDirection.Output);
        }

        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterInput(string name, SqlDbType type)
        {
            this.SetParameter(name, null, type, 0, ParameterDirection.Input);
        }

        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterInput(string name, object itemvalue, SqlDbType type)
        {
            this.SetParameter(name, itemvalue, type, 0, ParameterDirection.Input);
        }

        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterValidationInput(string name, object itemvalue, SqlDbType type)
        {
            if (IsItemBlankValue(itemvalue, type))
                this.SetParameterInput(name, null, type);
            else
                this.SetParameterInput(name, itemvalue, type);
        }


        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterInput(string name, object itemvalue, SqlDbType type, int length)
        {
            this.SetParameter(name, itemvalue, type, length, ParameterDirection.Input);
        }

        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterValidationInput(string name, object itemvalue, SqlDbType type, int length)
        {
            if (IsItemBlankValue(itemvalue, type)) return;
            this.SetParameter(name, itemvalue, type, length, ParameterDirection.Input);
        }

        /// <summary>
        /// Set Parameter as input value
        /// </summary>
        public void SetParameterInput(string name, object itemvalue, SqlDbType type, int length, byte scale)
        {
            this.SetParameter(name, itemvalue, Convert(type), length, scale, ParameterDirection.Input);
        }
        /// <summary>
        /// Set Sql parameter
        /// </summary>
        public void SetParameter(string name, object itemvalue, SqlDbType type, int length, ParameterDirection direction)
        {
            this.SetParameter(name, itemvalue, Convert(type), length, 0, direction);
        }
        #endregion

        /// <summary>
        /// Converts the specified type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        DbType Convert(SqlDbType type)
        {
            switch (type)
            {
                case SqlDbType.Bit: return DbType.Boolean;
            }

            SqlParameter tmp = new SqlParameter();
            tmp.SqlDbType = type;
            return tmp.DbType;
        }

        /// <summary>
        /// Sets the parameter.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="itemvalue">The itemvalue.</param>
        /// <param name="type">The type.</param>
        /// <param name="length">The length.</param>
        /// <param name="scale">The scale.</param>
        /// <param name="direction">The direction.</param>
        public void SetParameter( string name, object itemvalue, DbType type, int length, byte scale, ParameterDirection direction )
        {
            this.SetEnvironment();

            if (this.m_Command.Parameters.Contains(name)) return;

            switch (ConnectionType)
            {
                case Sushi.Mediakiwi.Data.DataConnectionType.SqlServer:
                    this.m_Parameter = new SqlParameter();
                    break;
                case Sushi.Mediakiwi.Data.DataConnectionType.Odbc:
                    this.m_Parameter = new OdbcParameter();
                    break;
                case Sushi.Mediakiwi.Data.DataConnectionType.OleDb:
                    this.m_Parameter = new OleDbParameter();
                    break;
                //case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache:
                //    this.m_Parameter = new CacheParameter();
                //    break;
            }
            this.m_Parameter.ParameterName = name;

            //  Exceptions
            if (type == DbType.Xml)
            {
                type = DbType.String;
                if (itemvalue != null)
                    itemvalue = itemvalue.ToString();
            }
            if (type == DbType.Guid)
            {
                type = DbType.StringFixedLength;
                if (itemvalue != null) 
                    itemvalue = itemvalue.ToString();
            }
            if (type == DbType.StringFixedLength || type == DbType.String || type == DbType.AnsiString || type == DbType.AnsiStringFixedLength)
            {
                if (itemvalue != null)
                    itemvalue = itemvalue.ToString();
            }
            
            this.m_Parameter.DbType = type;
            this.m_Parameter.Size = length;
            this.m_Parameter.Direction = direction;
            this.m_Parameter.Scale = scale;

            if ( itemvalue == null || itemvalue.ToString() == string.Empty )
                this.m_Parameter.Value    = DBNull.Value;
            else
            {
                this.m_Parameter.Value    = itemvalue;

                //  Verify the SqlTypes exception
                if ( itemvalue.GetType().Namespace.ToLower() == "system.data" )
                {
                    if ( itemvalue.ToString().ToLower() == "null" )
                        this.m_Parameter.Value = DBNull.Value;
                }
            }
            this.m_Command.Parameters.Add(this.m_Parameter);
            

            this.m_Parameterlist += string.Format( "<br/>{0} = '{1}' ({2}) ", name, itemvalue, type.ToString() );
        }

        /// <summary>
        /// Get the return value (object) of a specific parameter
        /// </summary>
        public object GetParameter( string name )
        {
            object param = this.m_Command.Parameters[name];
            switch (ConnectionType)
            {
                case Sushi.Mediakiwi.Data.DataConnectionType.SqlServer:
                    return ((SqlParameter)param).Value;
                case Sushi.Mediakiwi.Data.DataConnectionType.Odbc:
                    return ((OdbcParameter)param).Value;
                case Sushi.Mediakiwi.Data.DataConnectionType.OleDb:
                    return ((OleDbParameter)param).Value;
                //case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache:
                //    return ((CacheParameter)param).Value;
            }
            throw new Exception("No defined connection type");
        }

        /// <summary>
        /// Get the return value (string) of a specific parameter
        /// </summary>
        public SqlString GetParameterString( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return SqlString.Null;

            return param.ToString();
        }
        
        /// <summary>
        /// Get the return value (string) of a specific parameter
        /// </summary>
        public string GetParamString( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return string.Empty;
            return param.ToString();
        }

        /// <summary>
        /// Get the return value (string) of a specific parameter
        /// </summary>
        public string GetReaderString( string name )
        {
            int ordinal = m_reader.GetOrdinal(name);
            if ( m_reader.IsDBNull(ordinal) )
                return string.Empty;
         
            return m_reader.GetString(ordinal);
        }

        /// <summary>
        /// Get the return value (GUID) of a specific parameter
        /// </summary>
        public SqlGuid GetParameterGuid( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return SqlGuid.Null;

            return SqlGuid.Parse( param.ToString() );
        }

        /// <summary>
        /// Get the return value (int) of a specific parameter
        /// </summary>
        public SqlInt32 GetParameterInt( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return SqlInt32.Null;

            string f = param.ToString();
            return SqlInt32.Parse( param.ToString() );
        }

        /// <summary>
        /// Get the return value (int) of a specific parameter
        /// </summary>
        public int GetParamInt( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return 0;

            return int.Parse( param.ToString() );
        }

        /// <summary>
        /// Get the return value (int) of a specific parameter
        /// </summary>
        public int GetReaderInt( string name )
        {
            int ordinal = m_reader.GetOrdinal(name);
            if ( m_reader.IsDBNull(ordinal) )
                return 0;
         
            try
            {
                return m_reader.GetInt32(ordinal);
            }
            catch( Exception ex )
            {
                throw new Exception( string.Format("[GetReaderInt: '{0}'] - {1}", name, ex.Message ) );
            }
        }

        /// <summary>
        /// Get the return value (decimal) of a specific parameter
        /// </summary>
        public SqlDecimal GetParameterDecimal( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return SqlDecimal.Null;

            return SqlDecimal.Parse( param.ToString() );
        }

        /// <summary>
        /// Get the return value (decimal) of a specific parameter
        /// </summary>
        public Decimal GetParamDecimal( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return 0;

            return Decimal.Parse( param.ToString() );
        }

        /// <summary>
        /// Get the return value (decimal) of a specific parameter
        /// </summary>
        public Decimal GetReaderDecimal( string name )
        {
            int ordinal = m_reader.GetOrdinal(name);
            if ( m_reader.IsDBNull(ordinal) )
                return 0;
         
            return m_reader.GetDecimal(ordinal);
        }
        /// <summary>
        /// Get the return value (bool) of a specific parameter
        /// </summary>
        public SqlBoolean GetParameterBool( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return SqlBoolean.Null;

            return SqlBoolean.Parse( param.ToString() );
        }

        /// <summary>
        /// Get the return value (bool) of a specific parameter
        /// </summary>
        public bool GetParamBool( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return false;

            return Boolean.Parse( param.ToString() );
        }

        /// <summary>
        /// Get the return value (bool) of a specific parameter
        /// </summary>
        public bool GetReaderBool( string name )
        {
            int ordinal = m_reader.GetOrdinal(name);
            if ( m_reader.IsDBNull(ordinal) )
                return false;
         
            return m_reader.GetBoolean(ordinal);
        }
        
        /// <summary>
        /// Get the return value (DateTime) of a specific parameter
        /// </summary>
        public SqlDateTime GetParameterDateTime( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return SqlDateTime.Null;

            return SqlDateTime.Parse( param.ToString() );
        }
        
        /// <summary>
        /// Get the return value (DateTime) of a specific parameter
        /// </summary>
        public DateTime GetParamDateTime( string name )
        {
            object param = this.GetParameter( name );
            if ( param == System.DBNull.Value )
                return DateTime.MinValue;

            return DateTime.Parse( param.ToString() );
        }

        /// <summary>
        /// Clears the parameters.
        /// </summary>
        /// <returns></returns>
        public bool ClearParameters()
        {
            if ( this.m_Command == null )
                return true;

            this.m_Command.Parameters.Clear();
            if ( this.m_Command.Parameters.Count == 0 )
                return true;
            return false;
        }

        private IDataReader m_reader;
        /// <summary>
        /// Gets the exec reader.
        /// </summary>
        /// <value>The exec reader.</value>
        public IDataReader ExecReader
        {
            get
            {
                this.SetEnvironment();
                try
                {
                    
                    switch (ConnectionType)
                    {
                        case Sushi.Mediakiwi.Data.DataConnectionType.SqlServer:
                            m_reader = ((SqlCommand)this.m_Command).ExecuteReader();
                            break;
                        case Sushi.Mediakiwi.Data.DataConnectionType.Odbc:
                            m_reader = ((OdbcCommand)this.m_Command).ExecuteReader();
                            break;
                        case Sushi.Mediakiwi.Data.DataConnectionType.OleDb:
                            m_reader = ((OleDbCommand)this.m_Command).ExecuteReader();
                            break;
                        //case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache:
                        //    m_reader = ((CacheCommand)this.m_Command).ExecuteReader();
                        //    break;
                    }

                    return m_reader;
                }
                catch ( Exception ex )
                {
                    this.Close();
                    throw new Exception( this.GetErrorText( ex.Message ) );
                }
            }
        }

        /// <summary>
        /// Execs the reader command.
        /// </summary>
        /// <param name="behavior">The behavior.</param>
        /// <returns></returns>
        public IDataReader ExecReaderCommand( CommandBehavior behavior )
        {
            this.SetEnvironment();
            try
            {
                switch (ConnectionType)
                {
                    case Sushi.Mediakiwi.Data.DataConnectionType.SqlServer:
                        m_reader = ((SqlCommand)this.m_Command).ExecuteReader(behavior);
                        break;
                    case Sushi.Mediakiwi.Data.DataConnectionType.Odbc:
                        m_reader = ((OdbcCommand)this.m_Command).ExecuteReader(behavior);
                        break;
                    case Sushi.Mediakiwi.Data.DataConnectionType.OleDb:
                        m_reader = ((OleDbCommand)this.m_Command).ExecuteReader(behavior);
                        break;
                    //case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache:
                    //    m_reader = ((CacheCommand)this.m_Command).ExecuteReader(behavior);
                    //    break;
                }
                return m_reader;
            }
            catch ( Exception ex )
            {
                this.Close();
                throw new Exception(this.GetErrorText(ex.Message));
            }
        }


        /// <summary>
        /// Execute the SqlCommand non query.
        /// </summary>
        /// <returns></returns>
        public int ExecNonQuery()
        {
            this.SetEnvironment();
            try
            {
                return this.m_Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                this.Close();
                throw new Exception(this.GetErrorText(ex.Message));
            }
        } 


        /// <summary>
        /// Execute the SqlCommand non query.
        /// </summary>
        /// <param name="throwErrorOnException">if set to <c>true</c> [throw error on exception].</param>
        /// <param name="hasError">if set to <c>true</c> [has error].</param>
        /// <returns></returns>
        public int ExecNonQuery(bool throwErrorOnException, out bool hasError)
        {
            hasError = false;
            this.SetEnvironment();
            try
            {
                return this.m_Command.ExecuteNonQuery();
            }
            catch ( Exception ex )
            {
                this.Close();
                hasError = true;

                if (throwErrorOnException)
                    throw new Exception(this.GetErrorText(ex.Message));
                return 0;
            }
        }   

        /// <summary>
        /// Execute the SqlCommand scalar.
        /// </summary>
        public object ExecScalar(bool throwErrorOnException, object onErrorValue)
        {
            this.SetEnvironment();
            try
            {
                return this.m_Command.ExecuteScalar();
            }
            catch ( Exception ex )
            {
                this.Close();
                if (throwErrorOnException)
                    throw new Exception(this.GetErrorText(ex.Message));
                return onErrorValue;
            }
        }

        /// <summary>
        /// Execs the scalar.
        /// </summary>
        /// <returns></returns>
        public object ExecScalar()
        {
            this.SetEnvironment();
            try
            {
                return this.m_Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                this.Close();
                throw new Exception(this.GetErrorText(ex.Message));
            }
        }

        /// <summary>
        /// Execute the SqlCommand DataAdapter and return the generated dataset.
        /// </summary>
        public DataSet ExecAdapter
        {
            get
            {
                this.SetEnvironment();
                try
                {
                    DataSet ds = new DataSet();

                    switch (ConnectionType)
                    {
                        case Sushi.Mediakiwi.Data.DataConnectionType.SqlServer:
                            SqlDataAdapter sqlAdapter = new SqlDataAdapter(((SqlCommand)this.m_Command));
                            sqlAdapter.Fill(ds);
                            break;
                        case Sushi.Mediakiwi.Data.DataConnectionType.Odbc:
                            OdbcDataAdapter odbcAdapter = new OdbcDataAdapter(((OdbcCommand)this.m_Command));
                            odbcAdapter.Fill(ds);
                            break;
                        case Sushi.Mediakiwi.Data.DataConnectionType.OleDb:
                            OleDbDataAdapter oledbAdapter = new OleDbDataAdapter(((OleDbCommand)this.m_Command));
                            oledbAdapter.Fill(ds);
                            break;
                        //case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache:
                        //    CacheDataAdapter cacheAdapter = new CacheDataAdapter(((CacheCommand)this.m_Command));
                        //    cacheAdapter.Fill(ds);
                        //    break;
                    }
                    return ds;
                }
                catch ( Exception ex )
                {
                    this.Close();
                    throw new Exception(this.GetErrorText(ex.Message));
                }
            }
        } 

        /// <summary>
        /// Execute the SqlCommand DataAdapter and return the generated dataset with the according XML schema.
        /// </summary>
        public DataTable ExecAdapterFill( DataTable dt )
        {
            this.SetEnvironment();
            try
            {
                AddTrace("----", "----------------------------------------------------------------------------------: DataTable");
                switch (ConnectionType)
                {
                    case Sushi.Mediakiwi.Data.DataConnectionType.SqlServer:
                        SqlDataAdapter sqlAdapter = new SqlDataAdapter(((SqlCommand)this.m_Command));
                        sqlAdapter.Fill(dt);
                        break;
                    case Sushi.Mediakiwi.Data.DataConnectionType.Odbc:
                        OdbcDataAdapter odbcAdapter = new OdbcDataAdapter(((OdbcCommand)this.m_Command));
                        odbcAdapter.Fill(dt);
                        break;
                    case Sushi.Mediakiwi.Data.DataConnectionType.OleDb:
                        OleDbDataAdapter oledbAdapter = new OleDbDataAdapter(((OleDbCommand)this.m_Command));
                        oledbAdapter.Fill(dt);
                        break;
                    //case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache:
                    //    CacheDataAdapter cacheAdapter = new CacheDataAdapter(((CacheCommand)this.m_Command));
                    //    cacheAdapter.Fill(dt);
                    //    break;
                }
                AddTrace("DataTable", this.Text);
                return dt;
            }
            catch ( Exception ex )
            {
                this.Close();
                throw new Exception(this.GetErrorText(ex.Message));
            }
        }

        void AddTrace(string category, string message)
        {
            if (System.Web.HttpContext.Current == null || !System.Web.HttpContext.Current.Trace.IsEnabled) return;
            System.Web.HttpContext.Current.Trace.Write(category, message);
        }

        /// <summary>
        /// Execute the SqlCommand DataAdapter and return the generated dataset with the according XML schema.
        /// </summary>
        public DataSet ExecAdapterFill( DataSet ds )
        {
            this.SetEnvironment();
            try
            {
                AddTrace("----", "----------------------------------------------------------------------------------: DataSet");
                switch (ConnectionType)
                {
                    case Sushi.Mediakiwi.Data.DataConnectionType.SqlServer:
                        SqlDataAdapter sqlAdapter = new SqlDataAdapter(((SqlCommand)this.m_Command));
                        sqlAdapter.Fill(ds);
                        break;
                    case Sushi.Mediakiwi.Data.DataConnectionType.Odbc:
                        OdbcDataAdapter odbcAdapter = new OdbcDataAdapter(((OdbcCommand)this.m_Command));
                        odbcAdapter.Fill(ds);
                        break;
                    case Sushi.Mediakiwi.Data.DataConnectionType.OleDb:
                        OleDbDataAdapter oledbAdapter = new OleDbDataAdapter(((OleDbCommand)this.m_Command));
                        oledbAdapter.Fill(ds);
                        break;
                    //case Sushi.Mediakiwi.Data.DataConnectionType.InterSystemsCache:
                    //    CacheDataAdapter cacheAdapter = new CacheDataAdapter(((CacheCommand)this.m_Command));
                    //    cacheAdapter.Fill(ds);
                    //    break;
                }
                AddTrace("DATASET", this.Text);
                return ds;
            }
            catch ( Exception ex )
            {
                this.Close();
                throw new Exception(this.GetErrorText(ex.Message));
            }
        } 

        /// <summary>
        /// Execute the SqlCommand DataAdapter and return the generated first datatable from the dataset.
        /// </summary>
        public DataTable ExecAdapterTable
        {
            get
            {
                this.SetEnvironment();
                try
                {
                    DataSet ds = new DataSet();
                    ExecAdapterFill(ds);
                    if (ds.Tables.Count > 0)
                        return ds.Tables[0];
                    else
                        return null;
                }
                catch (Exception ex)
                {
                    this.Close();
                    throw new Exception(this.GetErrorText(ex.Message));
                }
            }
        }   

        /// <summary>
        /// Closes this connection instance.
        /// </summary>
        public void Close()
        {
            //  First validate if Connection exist
            if ( this.Connect.Connection == null )
                return;
            //  Validate if the connection is closed
            if ( this.Connect.Connection.State != ConnectionState.Closed )
                this.Connect.Connection.Close();       
        }

        /// <summary>
        /// Gets the error text.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        private string GetErrorText(string error)
        {
            Wim.Utilities.CacheItemManager.FlushAll();

            return string.Format("Error while executing<br/>{0} ({3})<br/>{1}<br/><br/><b>{2}</b>", 
                this.Text, 
                this.m_Parameterlist,
                error, this.ConnectionType.ToString());
        }
    }
}
