using NLog;
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
        private readonly ILogger _logger;

        public TaskService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<string> Send(TaskDto taskDto)
        {
            _logger.Debug($"TaskService.send");
            return await Task.FromResult("send task");
        }
    }
}
