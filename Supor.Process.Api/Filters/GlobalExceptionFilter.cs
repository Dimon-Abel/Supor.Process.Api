using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Supor.Process.Api.Filters
{
    /// <summary>
    /// 全局异常捕获
    /// </summary>
    public class GlobalExceptionFilter : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            // 写异常日志

            base.OnException(filterContext);
        }
    }
}