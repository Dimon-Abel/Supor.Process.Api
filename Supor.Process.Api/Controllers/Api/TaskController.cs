using NLog;
using Supor.Process.Common.Extensions;
using Supor.Process.Domain.Interfaces;
using Supor.Process.Entity.Entity;
using Supor.Process.Entity.InputDto;
using System;
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
        /// <param name="taskDto">任务数据</param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Task/Send")]
        public async Task<ApiResult> Send(TaskDto taskDto)
        {
            _logger.Info($"Task.Send");

            try
            {
                var data = await _taskDomain.Send(taskDto);
                return await Task.FromResult(ApiResult.Success(data, "提交成功"));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(ApiResult.Faild(null, ex.Message));
            }
        }

        ///// <summary>
        ///// 异步提交任务
        ///// </summary>
        ///// <param name="taskDto">任务数据</param>
        ///// <returns></returns>
        //[HttpPost]
        //[Route("api/Task/SendAsync")]
        //public async Task<ApiResult<string>> SendAsync(TaskDto taskDto)
        //{

        //}

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