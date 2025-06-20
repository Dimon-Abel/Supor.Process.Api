using Autofac;
using Autofac.Integration.WebApi;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;

namespace Supor.Process.Api
{
    public static class DependencyExtenstions
    {
        /// <summary>
        /// Api注入服务
        /// </summary>
        public static void AutofacRegister(this ContainerBuilder builder)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());


            var container = builder.Build();

            GlobalConfiguration.Configuration.DependencyResolver =
                new AutofacWebApiDependencyResolver(container);
        }

        public static void RegisterDomain(this ContainerBuilder builder)
        {

        }
    }
}