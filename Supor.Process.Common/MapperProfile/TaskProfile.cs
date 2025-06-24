using AutoMapper;
using Supor.Process.Entity.Entity;
using Supor.Process.Entity.InputDto;

namespace Supor.Process.Common.MapperProfile
{
    public class TaskProfile: Profile
    {
        public TaskProfile()
        {
            //CreateMap<ApiTaskEntity, TaskDto>()
            //    .ForMember(t => t.Id, d => d.MapFrom(x => x.Id))
            //    .ForMember(t => t.TaskName, d => d.MapFrom(x => x.TaskName));
        }
    }
}
