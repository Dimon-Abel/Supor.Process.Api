using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Supor.Process.Services
{
    public interface IRepository<T>
    {
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<IEnumerable<T>> QueryAsync(string condition, object param = null);

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="insert"></param>
        /// <returns></returns>
        Task<bool> AddAsync(params T[] insert);

        /// <summary>
        /// 更新数据 （null值不更新）
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(params T[] update);

        /// <summary>
        /// 事务写入
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="insert"></param>
        /// <returns></returns>
        Task<int> InsertTranAsync(IDbConnection connection, IDbTransaction transaction, params T[] insert);
        
        /// <summary>
        /// 事务更新
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        Task<int> UpdateTranAsync(IDbConnection connection, IDbTransaction transaction, params T[] update);
    }
}
