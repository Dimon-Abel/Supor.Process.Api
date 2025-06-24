using Supor.Process.Entity.Entity;
using Supor.Process.Entity.InputDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Supor.Process.Common.Processor
{
    public class BPMProcessor : IProcessor<TaskDto>
    {
        public IEnumerable<TaskEntity> CreateTask(TaskDto dto)
        {
            return dto.ProcessData.Select(x =>
            {
                TaskEntity entity = new TaskEntity()
                {
                    GUID = Guid.NewGuid().ToString("N"),
                    ProcessName = dto.ProcessName,
                    
                }


            });
        }
    }
}
