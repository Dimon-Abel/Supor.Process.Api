using NLog;
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
        private readonly ILogger _logger;
        private readonly ITaskDomain _taskDomain;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="taskDomain"></param>
        public TaskController(ILogger logger, ITaskDomain taskDomain)
        {
            _logger = logger;
            _taskDomain = taskDomain;
        }

        /// <summary>
        /// 提交任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Task/Send")]
        public async Task<ApiResult<string>> Send()
        {
            _logger.Info($"Task.Send");
            var data = await _taskDomain.Send();

            var result = ApiResult<string>.Success(data, data);
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 测试异常写入
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Task/Test")]
        public async Task<string> Test(string a)
        {
            _logger.Error($"测试异常写入");

            if (!string.IsNullOrWhiteSpace(a))
            {
                throw new System.Exception("123123");
            }

            return await Task.FromResult(a);
        }
    }
}