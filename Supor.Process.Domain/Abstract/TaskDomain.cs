using NLog;
using Supor.Process.Common.Validtors;
using Supor.Process.Domain.Interfaces;
using Supor.Process.Entity.InputDto;
using Supor.Process.Entity.OutDto;
using Supor.Process.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Supor.Process.Common.Extensions;

namespace Supor.Process.Domain.Abstract
{
    public partial class TaskDomain : ITaskDomain
    {
        private readonly ILogger _logger;

        private readonly ITaskService _taskService;

        private readonly IProcessVaildtor _vaildtor;

        public TaskDomain(ILogger logger ,ITaskService taskService, IProcessVaildtor vaildtor)
        {
            _logger = logger;
            _taskService = taskService;
            _vaildtor = vaildtor;
        }

        public async Task<TaskOutDto> Send(TaskDto dto)
        {
            var result = new TaskOutDto();

            // 验证提交数据
            if(ValidTask(dto))
            {
                foreach (var item in dto.ProcessData)
                {
                    // 验证业务数据
                    if (!_vaildtor.FieldValid(dto.ProcessData, out var message))
                    {
                        throw new Exception(message);
                    }
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

            if(dto.ProcessData == null)
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
