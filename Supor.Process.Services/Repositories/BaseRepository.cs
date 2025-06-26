using Supor.Process.Common.SentenceGenerate;
using Supor.Process.Services.Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Supor.Process.Services.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : class
    {
        protected readonly string _selectSql;
        protected readonly string _insertSql;
        protected readonly string _updateSql;
        protected readonly IDapperExecutor _dapperExecutor;

        public BaseRepository(IDapperExecutor dapperExecutor)
        {
            _selectSql = SqlGenerate.GenerateSelect<T>();
            _insertSql = SqlGenerate.GenerateInsert<T>();
            _updateSql = SqlGenerate.GenerateUpdate<T>();
            _dapperExecutor = dapperExecutor;
        }

        public virtual async Task<bool> AddAsync(params T[] insert)
        {
            return (await _dapperExecutor.ExecuteAsync(_insertSql, insert)) > 0;
        }

        public virtual async Task<IEnumerable<T>> QueryAsync(string condition, object param = null)
        {
            return await _dapperExecutor.QueryAsync<T>($"{_selectSql}{condition}", param);
        }

        public virtual async Task<bool> UpdateAsync(params T[] update)
        {
            return (await _dapperExecutor.ExecuteAsync(_updateSql, update)) > 0;
        }

        public virtual async Task<int> InsertTranAsync(IDbConnection connection, IDbTransaction transaction, params T[] insert)
        {
            return await connection.ExecuteAsync(_insertSql, insert, transaction);
        }

        public virtual async Task<int> UpdateTranAsync(IDbConnection connection, IDbTransaction transaction, params T[] update)
        {
            return await connection.ExecuteAsync(_updateSql, update, transaction);
        }
    }
}
