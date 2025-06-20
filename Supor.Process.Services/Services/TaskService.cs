using Supor.Process.Entity.InputDto;
using Supor.Process.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supor.Process.Services.Services
{
    /// <summary>
    /// 任务服务
    /// </summary>
    public partial class TaskService : ITaskService
    {
        public async Task<string> Send(TaskDto taskDto)
        {
            return "send task";
        }
    }
}
