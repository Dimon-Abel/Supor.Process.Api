using Autofac;
using Autofac.Integration.WebApi;
using NLog;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace Supor.Process.Api
{
    /// <summary>
    /// 注入 服务
    /// </summary>
    public static class DependencyExtenstions
    {
        /// <summary>
        /// Api注入服务
        /// </summary>
        public static void AutofacRegister()
        {
            var builder = new ContainerBuilder();

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterDomain();
            builder.RegisterService();

            builder.RegisterLog();

            GlobalConfiguration.Configuration.DependencyResolver =
                new AutofacWebApiDependencyResolver(builder.Build());
        }

        /// <summary>
        /// 注入 Domain
        /// </summary>
        /// <param name="builder"></param>
        private static void RegisterDomain(this ContainerBuilder builder)
        {
            var assembly = Assembly.Load("Supor.Process.Domain");
            builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Domain"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
        }

        /// <summary>
        /// 注入 Service
        /// </summary>
        /// <param name="builder"></param>
        private static void RegisterService(this ContainerBuilder builder)
        {
            var assembly = Assembly.Load("Supor.Process.Services");
            builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Service"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();
        }

        private static void RegisterLog(this ContainerBuilder builder)
        {
            LogManager.Setup().LoadConfigurationFromFile("nlog.config");

            builder.Register(x => LogManager.GetCurrentClassLogger())
            .As<ILogger>()
            .SingleInstance();
        }
    }
}