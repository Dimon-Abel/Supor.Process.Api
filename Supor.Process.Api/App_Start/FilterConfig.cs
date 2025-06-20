using Supor.Process.Api.Filters;
using System.Web;
using System.Web.Mvc;

namespace Supor.Process.Api
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalExceptionFilter());
        }
    }
}
