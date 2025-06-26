using Supor.Process.Entity.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Supor.Process.Services
{
    public interface ITaskService
    {
        /// <summary>
        /// 任务数据入库
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<bool> AddOrUpdateTaskAsync(IEnumerable<(I_OSYS_PROCDATA_ITEMS, I_OSYS_PROC_INSTS, I_OSYS_WF_WORKITEMS)> data);
    }
}
