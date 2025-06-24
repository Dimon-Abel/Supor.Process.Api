using NLog;
using Supor.Process.Entity.Entity;
using Supor.Process.Services.Dapper;
using Supor.Process.Services.Interfaces;
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

        public async Task<bool> AddAsync(params ApiTaskEntity[] entities)
        {

        }

        public async Task<bool> UpdateAsync(params ApiTaskEntity[] entities)
        {

        }
    }
}
