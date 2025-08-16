using ESign.Entity;
using ESign.Entity.Request;
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
        public async Task<SignUrl> Test(SendRequest  request)
        {
            var result = await _eSignService.Send(request);
            return await Task.FromResult(result);
        }

        /// <summary>
        /// 回调接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Task/Callback")]
        public async Task Callback(SignCallbackRequest request)
        {
            //action固定为：SIGN_FLOW_COMPLETE，此类型当整个签署流程完结，触发该类型的回调通知
            if (request != null && request.action == "SIGN_FLOW_COMPLETE")
            {
                if (request.signFlowStatus == SignFlowStatus.Complete)
                {
                    await _eSignService.Callback(request.signFlowId);
                }
            }
        }
    }
}