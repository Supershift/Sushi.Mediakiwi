using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Data;
using System.Data.OleDb;

namespace Sushi.Mediakiwi.Data.Connection
{
    /// <summary>
    /// Represents the OLEDB data command wrapper.
    /// </summary>
    public class OleDbCommander : IDisposable
    {
        private OleDbCommand m_Command;
        private OleDbParameter m_Parameter;
        private string m_Parameterlist;
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
        ~OleDbCommander()      
        {
            Dispose( false );
        }

        private OleDbConnect _connect;
        /// <summary>
        /// Gets or sets the connect.
        /// </summary>
        /// <value>The connect.</value>
        public OleDbConnect Connect
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

        private string _sqlText;
        /// <summary>
        /// Name of the stored procedure.
        /// </summary>
        /// <value>The SQL text.</value>
        public string SqlText
        {
            get { return this._sqlText; }
            set 
            { 
                this._sqlText = CleanUpSql(value); 
            }
        }

        private CommandType _commandType;
        /// <summary>
        /// Gets or sets the commandtype.
        /// </summary>
        /// <value>The commandtype.</value>
        public CommandType Commandtype
        {
            get { return this._commandType; }
            set { this._commandType = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommander"/> class.
        /// </summary>
        /// <param name="connect">The connect.</param>
        public OleDbCommander(OleDbConnect connect)
        {
            this.Commandtype = CommandType.Text;
            this.Connect = connect;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommander"/> class.
        /// </summary>
        public OleDbCommander() : this( string.Empty ) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommander"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public OleDbCommander( string connection ) : this( connection, string.Empty, CommandType.Text ) {}

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCommander"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="SqlText">The SQL text.</param>
        /// <param name="type">The type.</param>
        public OleDbCommander(string connection, string SqlText, CommandType type)
        {
            if ( this.Connect == null )
                this.Connect = new OleDbConnect();

            this.m_Parameterlist = string.Empty;
            this.SqlText = CleanUpSql(SqlText);
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
            return sqlText;
        }

        /// <summary>
        /// Set database environment, like Connection and Command.
        /// </summary>
        private void SetEnvironment()
        {
            if ( this.Connect.Connection == null )
            {
                this.Connect.Open();
            }

            if ( this.m_Command != null )
            {
                if ( this.m_Command.CommandText == null || this.m_Command.CommandText != this.SqlText )
                {
                    this.m_Command.CommandText = this.SqlText;
                }
                return;
            }

            this.m_Command = new OleDbCommand(this.SqlText, this.Connect.Connection);
            this.m_Command.CommandType = this.Commandtype;

            if ( this.Connect.IsPartOfTransaction )
            {
                this.m_Command.Transaction = this.Connect.Transaction;
            }
        }
        
        /// <summary>
        /// Set Sqlparameter as output value
        /// </summary>
        public void SetParameterOutput(string name, OleDbType type)
        {
            this.SetParameter( name, null, type, 0, ParameterDirection.Output );
        }
        
        /// <summary>
        /// Set Sqlparameter as output value
        /// </summary>
        public void SetParameterOutput(string name, OleDbType type, int length)
        {
            this.SetParameter( name, null, type, length, ParameterDirection.Output );
        }

        /// <summary>
        /// Set Sqlparameter as output value
        /// </summary>
        public void SetParameterOutput(string name, OleDbType type, int length, byte scale)
        {
            this.SetParameter( name, null, type, length, scale, ParameterDirection.Output );
        }

        /// <summary>
        /// Set Sqlparameter as input value
        /// </summary>
        public void SetParameterInput(string name, OleDbType type)
        {
            this.SetParameter( name, null, type, 0, ParameterDirection.Input );
        }

        /// <summary>
        /// Set Sqlparameter as input value
        /// </summary>
        public void SetParameterInput(string name, object itemvalue, OleDbType type)
        {
            this.SetParameter( name, itemvalue, type, 0, ParameterDirection.Input );
        }

        /// <summary>
        /// Set Sqlparameter as input value
        /// </summary>
        public void SetParameterValidationInput(string name, object itemvalue, OleDbType type)
        {
            if ( IsItemBlankValue(itemvalue, type) ) 
                this.SetParameterInput( name, null, type );
            else
                this.SetParameterInput( name, itemvalue, type );
        }

        /// <summary>
        /// Determines whether [is item blank value] [the specified itemvalue].
        /// </summary>
        /// <param name="itemvalue">The itemvalue.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// 	<c>true</c> if [is item blank value] [the specified itemvalue]; otherwise, <c>false</c>.
        /// </returns>
        private bool IsItemBlankValue(object itemvalue, OleDbType type)
        {
            switch( type )
            {
                case OleDbType.VarChar:
                    if ( itemvalue.ToString() == string.Empty ) return true;
                    break;
                case OleDbType.Integer:
                    if ( itemvalue.ToString() == int.MinValue.ToString() ) return true;
                    break;
                case OleDbType.TinyInt:
                    if ( itemvalue.ToString() == int.MinValue.ToString() ) return true;
                    break;
                case OleDbType.Decimal:
                    if ( itemvalue.ToString() == Decimal.MinValue.ToString() ) return true;
                    break;
                case OleDbType.DBDate:
                    if ( itemvalue.ToString() == DateTime.MinValue.ToString() ) return true;
                    break;

            }
            return false;
        }

        /// <summary>
        /// Set Sqlparameter as input value
        /// </summary>
        public void SetParameterInput(string name, object itemvalue, OleDbType type, int length)
        {
            this.SetParameter( name, itemvalue, type, length, ParameterDirection.Input );
        }

        /// <summary>
        /// Set Sqlparameter as input value
        /// </summary>
        public void SetParameterValidationInput(string name, object itemvalue, OleDbType type, int length)
        {
            if ( IsItemBlankValue(itemvalue, type) ) return;
            this.SetParameter( name, itemvalue, type, length, ParameterDirection.Input );
        }

        /// <summary>
        /// Set Sqlparameter as input value
        /// </summary>
        public void SetParameterInput(string name, object itemvalue, OleDbType type, int length, byte scale)
        {
            this.SetParameter( name, itemvalue, type, length, scale, ParameterDirection.Input );
        }
        /// <summary>
        /// Set Sql parameter
        /// </summary>
        public void SetParameter(string name, object itemvalue, OleDbType type, int length, ParameterDirection direction)
        {
            this.SetParameter( name, itemvalue, type, length, 0, direction );
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
        public void SetParameter(string name, object itemvalue, OleDbType type, int length, byte scale, ParameterDirection direction)
        {
            this.SetEnvironment();

            if (this.m_Command.Parameters.Contains(name)) return;
            this.m_Parameter = this.m_Command.Parameters.Add( name, type, length );
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

            this.m_Parameterlist += string.Format( "<br/>{0} = '{1}' ({2}) ", name, itemvalue, type.ToString() );
            
//            if ( this.SqlText != null || this._sqlText != string.Empty )
//                this._actualQuery = this._actualQuery.Replace( name, string.Format( "{0}", itemvalue ) );
        }

        /// <summary>
        /// Get the return value (object) of a specific parameter
        /// </summary>
        public object GetParameter( string name )
        {
            return this.m_Command.Parameters[name].Value;
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

        private OleDbDataReader m_reader;
        /// <summary>
        /// Gets the exec reader.
        /// </summary>
        /// <value>The exec reader.</value>
        public OleDbDataReader ExecReader
        {
            get
            {
                this.SetEnvironment();
                try
                {
                    m_reader = this.m_Command.ExecuteReader();
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
        public OleDbDataReader ExecReaderCommand(CommandBehavior behavior)
        {
            this.SetEnvironment();
            try
            {
                m_reader = this.m_Command.ExecuteReader( behavior );
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
        public int ExecNonQuery()
        {
            this.SetEnvironment();
            try
            {
                return this.m_Command.ExecuteNonQuery();
            }
            catch ( Exception ex )
            {
                this.Close();
                throw new Exception(this.GetErrorText(ex.Message));
            }
        }   

        /// <summary>
        /// Execute the SqlCommand scalar.
        /// </summary>
        public object ExecScalar()
        {
            this.SetEnvironment();
            try
            {
                return this.m_Command.ExecuteScalar();
            }
            catch ( Exception ex )
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
                    DataSet ds                  = new DataSet();
                    OleDbDataAdapter sqlAdapter = new OleDbDataAdapter(this.m_Command);
                    sqlAdapter.Fill( ds );
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
                OleDbDataAdapter sqlAdapter = new OleDbDataAdapter(this.m_Command);
                sqlAdapter.Fill( dt );
                return dt;
            }
            catch ( Exception ex )
            {
                this.Close();
                throw new Exception(this.GetErrorText(ex.Message));
            }
        } 


        /// <summary>
        /// Execute the SqlCommand DataAdapter and return the generated dataset with the according XML schema.
        /// </summary>
        public DataSet ExecAdapterFill( DataSet ds )
        {
            this.SetEnvironment();
            try
            {
                OleDbDataAdapter sqlAdapter = new OleDbDataAdapter(this.m_Command);
                sqlAdapter.Fill( ds );
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
                    OleDbDataAdapter sqlAdapter = new OleDbDataAdapter(this.m_Command);
                    sqlAdapter.Fill(ds);
                    return ds.Tables[0];
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
            return string.Format("Error while executing<br/>{0}<br/>{1}<br/><br/><b>{2}</b>", 
                this.SqlText, 
                this.m_Parameterlist,
                error);
        }
    }
}
