using Supor.Process.Entity.InputDto;
using System.Threading.Tasks;

namespace Supor.Process.Services.Interfaces
{
    public interface ITaskService
    {
        Task<string> Send(TaskDto taskDto);
    }
}
