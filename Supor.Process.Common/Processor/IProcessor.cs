using Supor.Process.Entity.Entity;
using Supor.Process.Entity.InputDto;
using System.Collections.Generic;

namespace Supor.Process.Common.Processor
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
        ApiTaskEntity SendTask(TaskDto dto, object processData);
    }
}
