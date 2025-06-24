using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using KFLibrary.Configuration;
using System.Threading.Tasks;


namespace Supor.Process.Services.Dapper
{
    public class DapperExecutor : IDapperExecutor
    {
        /// <inheritdoc />
        public async Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync(sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync<T>(sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<T> QueryFirstAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryFirstAsync<T>(sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<T> QuerySingleAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QuerySingleAsync<T>(sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QuerySingleOrDefaultAsync<T>(sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<object>> QueryAsync(Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync(type, sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<object> QueryFirstAsync(Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryFirstAsync(type, sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<object> QueryFirstOrDefaultAsync(Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryFirstOrDefaultAsync(type, sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<object> QuerySingleAsync(Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QuerySingleAsync(type, sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<object> QuerySingleOrDefaultAsync(Type type, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QuerySingleOrDefaultAsync(type, sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TReturn>(string sql, Func<TFirst, TSecond, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TReturn>(string sql, Func<TFirst, TSecond, TThird, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReturn>> QueryAsync<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn>(string sql, Func<TFirst, TSecond, TThird, TFourth, TFifth, TSixth, TSeventh, TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync(sql, map, param, transaction, buffered, splitOn, commandTimeout);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TReturn>> QueryAsync<TReturn>(string sql, Type[] types, Func<object[], TReturn> map, object param = null, IDbTransaction transaction = null, bool buffered = true, string splitOn = "Id", int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.QueryAsync(sql, types, map, param, transaction, buffered, splitOn, commandTimeout);
            }
        }

        /// <inheritdoc />
        public async Task<object> ExecuteScalarAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.ExecuteScalarAsync(sql, param, transaction, commandTimeout, null);
            }
        }

        /// <inheritdoc />
        public async Task<T> ExecuteScalarAsync<T>(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName="busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.ExecuteScalarAsync<T>(sql, param, transaction, commandTimeout, null);
            }
        }

        public async Task<int> ExecuteAsync(string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, string dbName = "busDB")
        {
            using (var connection = new SqlConnection(GetConnectionString(dbName)))
            {
                await connection.OpenAsync();

                return await connection.ExecuteAsync(sql, param, transaction, commandTimeout, null);
            }
        }


        /// <summary>
        /// 获取链接字符串
        /// </summary>
        /// <param name="isMaster"></param>
        /// <returns></returns>
        private string GetConnectionString(string dbName = "busDB")
        {
            return Configer.ConnectionStrings[dbName].ConnectionString;
        }
    }
}
