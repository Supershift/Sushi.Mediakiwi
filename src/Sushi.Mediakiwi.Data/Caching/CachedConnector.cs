using Sushi.MicroORM;
using Sushi.MicroORM.Mapping;
using Sushi.MicroORM.Supporting;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sushi.Mediakiwi.Data.Caching
{
    public class CachedConnector<T> : Connector<T> where T : new()
    {
        public CachedConnector() : base()
        {
            Cache = Configuration.CacheManagerProvider?.Invoke();
        }

        public CachedConnector(ICacheManager cache) : base()
        {
            Cache = cache;
        }

        public CachedConnector(DataMap<T> map) : base(map)
        {
            Cache = Configuration.CacheManagerProvider?.Invoke(); 
        }

        public CachedConnector(string connectionString, DataMap<T> map) : base(connectionString, map)
        {
            Cache = Configuration.CacheManagerProvider?.Invoke(); 
        }

        public CachedConnector(string connenectionString, DataMap<T> map, ICacheManager cache) : base(connenectionString, map)
        {
            Cache = cache;
        }

        public static DateTime? LastFlush { get; set; }

        /// <summary>
        /// Gets or sets a value indicating if cache must be used if a SELECT statement is executed. 
        /// A flush of cache for all objects with type <typeparamref name="T"/> is always performed, regardless of this setting.
        /// </summary>
        public bool UseCacheOnSelect { get; set; } = true;

        /// <summary>
        /// Gets a caching object implementing <see cref="ICacheManager"/>.
        /// </summary>
        public ICacheManager Cache { get; } 
        
        /// <summary>
        /// Gets the name of the region to use in the cache. Current implementation is <see cref="Type.FullName"/> of <typeparamref name="T"/>.
        /// </summary>
        public string CacheRegion { get; } = typeof(T).FullName;
        
        /// <summary>
        /// Generates a unique value for the sql statement by generating a hash of the statements SQL text and a hash of the statement's parameters name and value.
        /// </summary>        
        /// <param name="statement"></param>
        /// <returns></returns>
        public string GenerateKey(SqlStatement<T> statement)
        {
            //hash the query and parameters
            var query = statement.GenerateSqlStatement();
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                //hash query
                var hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(query));
                var secondKeyPart = Convert.ToBase64String(hashedBytes);

                //hash the parameters
                var sb = new StringBuilder();
                foreach (var parameter in statement.Parameters)
                {
                    sb.Append(parameter.Name).Append(parameter.Value);
                }
                string parameters = sb.ToString();
                hashedBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(parameters));
                var thirdKeyPart = Convert.ToBase64String(hashedBytes);

                //build the key
                return $"{secondKeyPart}_{thirdKeyPart}";
            }
        }

        public override SqlStatementResult<TResult> ExecuteSqlStatement<TResult>(SqlStatement<T> statement)
        {
            if (typeof(T) == typeof(EnvironmentVersion))
            {
                // Never cache EnvironmentVersion
                return base.ExecuteSqlStatement<TResult>(statement);
            }

            SqlStatementResult<TResult> result = null;

            //does this operation interact with the cache?
            switch (statement.DMLStatement)
            {
                case DMLStatementType.Delete:
                case DMLStatementType.Insert:
                case DMLStatementType.Update:
                case DMLStatementType.InsertOrUpdate:
                    //perform the operation
                    result = base.ExecuteSqlStatement<TResult>(statement);

                    //force a cache flush for all cached objects that have a matching first part of the key (= all objects for same type)
                    Cache?.FlushRegion(CacheRegion);

                    // Update the environment
                    EnvironmentVersion.SetUpdated();

                    break;
                case DMLStatementType.Select:
                    if (UseCacheOnSelect)
                    {
                        // See if the cache needs to be flushed
                        if (!LastFlush.HasValue)
                        {
                            LastFlush = DateTime.UtcNow;
                        }
                        else
                        {
                            if (Configuration.EnvironmentUpdated.HasValue && LastFlush.Value < Configuration.EnvironmentUpdated.Value)
                            {
                                Cache?.FlushRegion(CacheRegion);
                                LastFlush = DateTime.UtcNow;
                            }
                        }

                        //check if in cache, return if found
                        string key = GenerateKey(statement);
                        result = Cache?.Get<SqlStatementResult<TResult>>(CacheRegion, key);

                        //if not, perform the query and cache result
                        if (result == null)
                        {
                            result = base.ExecuteSqlStatement<TResult>(statement);
                            Cache?.Add(CacheRegion, key, result);
                        }
                    }
                    else
                        result = base.ExecuteSqlStatement<TResult>(statement);
                    break;
                default:
                    result = base.ExecuteSqlStatement<TResult>(statement);
                    break;
            }

            return result;
        }

        public async override Task<SqlStatementResult<TResult>> ExecuteSqlStatementAsync<TResult>(SqlStatement<T> statement, CancellationToken cancellationToken)
        {
            if (typeof(T) == typeof(EnvironmentVersion))
            {
                // Never cache EnvironmentVersion
                return await base.ExecuteSqlStatementAsync<TResult>(statement, cancellationToken).ConfigureAwait(false);
            }

            SqlStatementResult<TResult> result = null;

            //does this operation interact with the cache?
            switch (statement.DMLStatement)
            {
                case DMLStatementType.Delete:
                case DMLStatementType.Insert:
                case DMLStatementType.Update:
                case DMLStatementType.InsertOrUpdate:
                    //perform the operation
                    result = await base.ExecuteSqlStatementAsync<TResult>(statement, cancellationToken).ConfigureAwait(false);

                    //force a cache flush for all cached objects that have a matching first part of the key (= all objects for same type)
                    Cache?.FlushRegion(CacheRegion);

                    // Update the environment
                    EnvironmentVersion.SetUpdated();

                    break;
                case DMLStatementType.Select:
                    if (UseCacheOnSelect)
                    {
                        // See if the cache needs to be flushed
                        if (!LastFlush.HasValue)
                        {
                            LastFlush = DateTime.UtcNow;
                        }
                        else
                        {
                            if (Configuration.EnvironmentUpdated.HasValue && LastFlush.Value < Configuration.EnvironmentUpdated.Value)
                            {
                                Cache?.FlushRegion(CacheRegion);
                                LastFlush = DateTime.UtcNow;
                            }
                        }

                        //check if in cache, return if found
                        string key = GenerateKey(statement);
                        result = Cache?.Get<SqlStatementResult<TResult>>(CacheRegion, key);

                        //if not, perform the query and cache result
                        if (result == null)
                        {
                            result = await base.ExecuteSqlStatementAsync<TResult>(statement, cancellationToken).ConfigureAwait(false);
                            Cache?.Add(CacheRegion, key, result);
                        }
                    }
                    else
                        result = await base.ExecuteSqlStatementAsync<TResult>(statement, cancellationToken).ConfigureAwait(false);
                    break;
                default:
                    result = await base.ExecuteSqlStatementAsync<TResult>(statement, cancellationToken).ConfigureAwait(false);
                    break;
            }

            return result;
        }
    }
}
