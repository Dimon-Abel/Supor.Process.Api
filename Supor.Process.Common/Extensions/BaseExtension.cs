using Autofac;
using AutoMapper;
using Newtonsoft.Json;

namespace Supor.Process.Common.Extensions
{
    public static class BaseExtension
    {
        private static IMapper _mapper;

        public static TO MapTo<T, TO>(this T obj) where T : new()
        {
            if (_mapper == null)
            {
                _mapper = ServiceLocator.Container.Resolve<IMapper>();
            }

            return _mapper.Map<TO>(obj);
        }

        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T FromJson<T> (this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
    }
}
