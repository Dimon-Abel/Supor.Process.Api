using Supor.Process.Domain.Interfaces;
using System.Threading.Tasks;
using System.Web.Http;

namespace Supor.Process.Api.Controllers.Api
{
    /// <summary>
    /// 任务管理
    /// </summary>
    public class TaskController : ApiController
    {
        private readonly ITaskDomain _taskDomain;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="taskDomain"></param>
        public TaskController(ITaskDomain taskDomain)
        {
            _taskDomain = taskDomain;
        }

        /// <summary>
        /// 提交任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ApiResult<string>> Send()
        {
            _ = await _taskDomain.Send();

            var result = ApiResult<string>.Success(null, null);
            return await Task.FromResult(result);
        }
    }
}