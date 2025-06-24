using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supor.Process.Services.Interfaces
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
        /// 更新数据
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(params T[] update);
    }
}
