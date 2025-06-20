using Autofac;
using AutoMapper;
using Newtonsoft.Json;

namespace Supor.Process.Common.Extensions
{
    public static class BaseExtension
    {
        public static TO MapTo<T, TO>(this T obj) where T : new()
        {
            var mapper = ServiceLocator.Container.Resolve<IMapper>();
            return mapper.Map<TO>(obj);
        }

        public static string ToJson<T>(this T obj) where T : new()
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T> (this string json) where T: class
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
