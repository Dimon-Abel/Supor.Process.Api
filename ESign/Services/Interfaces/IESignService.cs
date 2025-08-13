using System.Threading.Tasks;
using ESign.Entity.Result;

namespace ESign.Services.Interfaces
{
    public interface IESignService
    {
        Task<SignUrl> Send(string fileName, string title = "签署文件");
        Task Callback(string signFlowId);
    }
}
