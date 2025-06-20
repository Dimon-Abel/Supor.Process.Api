using Autofac;
using AutoMapper;

namespace Supor.Process.Common.Extensions
{
    public static class BaseExtension
    {
        public static TO MapTo<T, TO>(this T obj) where T : new()
        {
            var mapper = ServiceLocator.Container.Resolve<IMapper>();
            return mapper.Map<TO>(obj);
        }
    }
}
