using NLog;
using Supor.Process.Common.Validtors;
using Supor.Process.Domain.Interfaces;
using Supor.Process.Entity.InputDto;
using Supor.Process.Entity.OutDto;
using System;
using System.Threading.Tasks;
using Supor.Process.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using Supor.Process.Services.Processor;
using Supor.Process.Services;

namespace Supor.Process.Domain.Abstract
{
    public partial class TaskDomain : ITaskDomain
    {
        private readonly ILogger _logger;

        private readonly ITaskService _taskService;

        private readonly IProcessVaildtor _vaildtor;

        private readonly IProcessorFactory _processorFactory;

        public TaskDomain(ILogger logger, ITaskService taskService, IProcessVaildtor vaildtor, IProcessorFactory processorFactory)
        {
            _logger = logger;
            _taskService = taskService;
            _vaildtor = vaildtor;
            _processorFactory = processorFactory;
        }


        public async Task<TaskOutDto> Send(TaskDto dto)
        {
            ValidTask(dto);
            var processor = _processorFactory.GetProcessor(dto.SourceName)
                ?? throw new Exception("任务处理器未实现");

            var result = new TaskOutDto { Success = true };
            var tasks = dto.ProcessData.Select(async item =>
            {
                if (!_vaildtor.FieldValid(item, out var msg)) throw new Exception(msg);
                return await processor.SendTask(dto, item);
            }).ToList();

            try
            {
                if (await _taskService.AddOrUpdateTaskAsync(await Task.WhenAll(tasks)))
                    tasks.ForEach(t => result.GuidInsId.Add(t.Result.Item1.GUID, t.Result.Item1.ProcInstId));
            }
            catch (Exception ex)
            {
                _logger.Error($"操作失败: {ex.Message}");
                throw ex;
            }
            return result;
        }

        private void ValidTask(TaskDto dto)
        {
            var checks = new Dictionary<Func<bool>, string>
            {
                [() => !dto.ProcessName.IsNullOrWhiteSpace()] = "流程名称不能为空",
                [() => !dto.SourceName.IsNullOrWhiteSpace()] = "数据来源不能为空",
                [() => !dto.CreateUserID.IsNullOrWhiteSpace()] = "用户ID不能为空",
                [() => dto.ProcessData != null] = "流程业务数据不能为空"
            };
            foreach (var check in checks.Where(c => !c.Key()))
                throw new Exception(check.Value);

            dto.CreateDate = dto.CreateDate.HasValue ? dto.CreateDate.Value : DateTime.Now;
        }
    }
}
