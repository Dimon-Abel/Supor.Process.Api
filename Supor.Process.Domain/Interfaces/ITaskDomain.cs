using System.Threading.Tasks;

namespace Supor.Process.Domain.Interfaces
{
    public interface ITaskDomain
    {
        Task<string> Send();
    }
}
