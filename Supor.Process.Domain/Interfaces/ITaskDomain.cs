using Supor.Process.Entity.InputDto;
using Supor.Process.Entity.OutDto;
using System.Threading.Tasks;

namespace Supor.Process.Domain.Interfaces
{
    public interface ITaskDomain
    {
        /// <summary>
        /// 提交任务
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<TaskOutDto> Send(TaskDto dto);
    }
}
