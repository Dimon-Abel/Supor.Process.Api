using Supor.Process.Common.Extensions;
using Supor.Process.Entity.InputDto;
using System.Reflection;

namespace Supor.Process.Common.Validtors
{
    public class BaseProcesssVaildator: IProcessVaildtor
    {
        public virtual bool FieldValid(object jsonData, out string message)
        {
            message = string.Empty;

            var data = jsonData.ToJson().FromJson<ProcessDataDto>();
            var type = data.GetType();
            var properties = type.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(data)?.ToString();
                if (value.IsNullOrWhiteSpace())
                {
                    message = $"ProcessData.{propertyInfo.Name}不能为空。";
                    return false;
                }
            }

            return true;

        }
    }
}
