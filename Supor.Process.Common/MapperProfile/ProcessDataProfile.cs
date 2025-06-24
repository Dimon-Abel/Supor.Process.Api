using AutoMapper;
using Supor.Process.Entity.InputDto;

namespace Supor.Process.Common.MapperProfile
{
    public class ProcessDataProfile: Profile
    {
        public ProcessDataProfile()
        {
            CreateMap<object, ProcessDataDto>();
        }

    }
}
