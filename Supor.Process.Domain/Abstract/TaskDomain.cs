using NLog;
using Supor.Process.Common.Validtors;
using Supor.Process.Domain.Interfaces;
using Supor.Process.Entity.InputDto;
using Supor.Process.Entity.OutDto;
using Supor.Process.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Supor.Process.Common.Extensions;
using Supor.Process.Common.Processor;
using System.Collections.Generic;
using Supor.Process.Entity.Entity;
using System.Linq;

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
            var result = new TaskOutDto();

            // 验证提交数据
            if (ValidTask(dto))
            {
                List<ApiTaskEntity> tasks = null;
                // 任务处理器
                var processor = _processorFactory.GetProcessor(dto.SourceName);

                foreach (var item in dto.ProcessData)
                {
                    // 验证业务数据
                    if (!_vaildtor.FieldValid(item, out var message))
                    {
                        throw new Exception(message);
                    }

                    if (processor != null)
                    {
                        tasks.Add(processor.SendTask(dto, item));
                    }
                    else
                    {
                        _logger.Error($"任务处理器未实现。");
                        throw new Exception("任务处理器未实现。");
                    }
                }

                if (tasks.Any())
                {

                }
            }

            result.Success = true;

            return await Task.FromResult(result);
        }

        private bool ValidTask(TaskDto dto)
        {
            if (dto.ProcessName.IsNullOrWhiteSpace())
            {
                throw new Exception($"流程名称不能为空。");
            }

            if (dto.SourceName.IsNullOrWhiteSpace())
            {
                throw new Exception($"数据来源不能为空。");
            }

            if (dto.CreateUserID.IsNullOrWhiteSpace())
            {
                throw new Exception($"用户ID不能为空。");
            }

            if (dto.ProcessData == null)
            {
                throw new Exception($"流程业务数据不能为空");
            }

            if (!dto.CreateDate.HasValue)
            {
                dto.CreateDate = DateTime.Now;
            }

            return true;
        }
    }
}
