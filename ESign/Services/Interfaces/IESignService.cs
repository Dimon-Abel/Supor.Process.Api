using System.Threading.Tasks;
using ESign.Entity.Request;
using ESign.Entity.Result;

namespace ESign.Services.Interfaces
{
    public interface IESignService
    {
        /// <summary>
        /// 发起签署流程
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<SignUrl> Send(SendRequest request);

        /// <summary>
        /// 签署回调
        /// </summary>
        /// <param name="signFlowId">签署流程Id</param>
        /// <param name="fileSavePath">第三方文件传输地址</param>
        /// <returns></returns>
        Task Callback(string signFlowId, string fileSavePath = "");
    }
}
