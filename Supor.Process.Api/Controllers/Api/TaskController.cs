using ESign.Entity.Result;
using ESign.Services;
using ESign.Services.Interfaces;
using NLog;
using Supor.Process.Common.Extensions;
using Supor.Process.Domain.Interfaces;
using Supor.Process.Entity.Entity;
using Supor.Process.Entity.InputDto;
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
        private readonly IESignService _eSignService;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="taskDomain"></param>
        public TaskController(ILogger logger, ITaskDomain taskDomain, IESignService eSignService)
        {
            _logger = logger;
            _taskDomain = taskDomain;
            _eSignService = eSignService;
        }


        /// <summary>
        /// 提交任务
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Task/Send")]
        public async Task<ApiResult<string>> Send(TaskDto taskDto)
        {
            _logger.Info($"Task.Send");

            var task = taskDto.MapTo<TaskDto, TaskEntity>();
            var json = task.ToJson();
            var entity = json.FromJson<TaskEntity>();

            var data = await _taskDomain.Send();

            var result = ApiResult<string>.Success(data, data);
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 测试异常写入
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Task/Test")]
        public async Task<SignUrl> Test(string fileName)
        {
            var result = await _eSignService.Send(fileName);
            return await Task.FromResult(result);
        }
    }
}