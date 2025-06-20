using NLog;
using Supor.Process.Domain.Interfaces;
using Supor.Process.Services.Interfaces;
using System.Threading.Tasks;

namespace Supor.Process.Domain.Abstract
{
    public partial class TaskDomain : ITaskDomain
    {
        private readonly ILogger _logger;

        private readonly ITaskService _taskService;

        public TaskDomain(ILogger logger ,ITaskService taskService)
        {
            _logger = logger;
            _taskService = taskService;
        }

        public async Task<string> Send()
        {
            _logger.Debug($"TaskDomain.send");
            return await _taskService.Send(null);
        }
    }
}
