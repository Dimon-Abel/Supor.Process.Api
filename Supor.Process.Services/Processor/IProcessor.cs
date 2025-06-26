using Supor.Process.Entity.Entity;
using Supor.Process.Entity.InputDto;
using System.Threading.Tasks;

namespace Supor.Process.Services.Processor
{
    /// <summary>
    /// 流程处理器
    /// </summary>
    public interface IProcessor
    {
        /// <summary>
        /// 获取处理器标签
        /// </summary>
        /// <returns></returns>
        string GetTag();

        /// <summary>
        /// 创建任务数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<(I_OSYS_PROCDATA_ITEMS, I_OSYS_PROC_INSTS, I_OSYS_WF_WORKITEMS)> SendTaskAsync(TaskDto dto, object processData);
    }
}
