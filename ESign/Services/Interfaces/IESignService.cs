using System.Threading.Tasks;
using ESign.Entity.Request;
using ESign.Entity.Result;

namespace ESign.Services.Interfaces
{
    public interface IESignService
    {
        Task<SignUrl> Send(SendRequest request);
        Task Callback(string signFlowId);
    }
}
