using Supor.Process.Entity.Entity;
using System.Threading.Tasks;

namespace Supor.Process.Services.Interfaces
{
    public interface ITaskService
    {
        /// <summary>
        /// 添加任务信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> AddAsync(params I_OSYS_PROCDATA_ITEMS[] entities);

        /// <summary>
        /// 更新任务信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(params I_OSYS_PROCDATA_ITEMS[] entities);

    }
}
