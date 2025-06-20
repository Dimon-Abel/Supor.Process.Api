using Supor.Process.Domain.Interfaces;
using Supor.Process.Services.Interfaces;
using System.Threading.Tasks;

namespace Supor.Process.Domain.Abstract
{
    public partial class TaskDomain : ITaskDomain
    {
        private readonly ITaskService _taskService;

        public TaskDomain(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<string> Send()
        {
            return await _taskService.Send(null);
        }
    }
}
