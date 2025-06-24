using Autofac;
using Autofac.Integration.WebApi;
using AutoMapper;
using NLog;
using Supor.Process.Common;
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

            builder.RegisterProcesssModule();

            builder.RegisterDataBase();

            builder.RegisterLog();

            builder.RegisterMapper();

            var container = builder.Build();

            ServiceLocator.SetContainer(container);
            GlobalConfiguration.Configuration.DependencyResolver =
                new AutofacWebApiDependencyResolver(container);
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

        /// <summary>
        /// 注入 AutoMapper
        /// </summary>
        /// <param name="builder"></param>
        private static void RegisterMapper(this ContainerBuilder builder)
        {
            var assembly = Assembly.Load("Supor.Process.Common");
            builder.Register(ctx =>
            {
                var config = new MapperConfiguration(cfg =>
                {
                    cfg.AddProfiles(assembly);
                });

                return config.CreateMapper();
            }).As<IMapper>().SingleInstance();
        }

        /// <summary>
        /// 注入流程校验器、流程处理器
        /// </summary>
        /// <param name="builder"></param>
        private static void RegisterProcesssModule(this ContainerBuilder builder)
        {
            var assembly = Assembly.Load("Supor.Process.Common");
            builder.RegisterAssemblyTypes(assembly)
                .Where(x => (x.Name.EndsWith("Validtor") || x.Name.EndsWith("Processor") || x.Name.EndsWith("ProcessorFactory")) && !x.IsAbstract)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }

        private static void RegisterDataBase(this ContainerBuilder builder)
        {
            var assembly = Assembly.Load("Supor.Process.Services");
            builder.RegisterAssemblyTypes(assembly)
               .Where(x => x.Name.EndsWith("Executor") && !x.IsAbstract)
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();
        }

        /// <summary>
        /// 注入 Log
        /// </summary>
        /// <param name="builder"></param>
        private static void RegisterLog(this ContainerBuilder builder)
        {
            LogManager.Setup().LoadConfigurationFromFile("nlog.config");

            builder.Register(x => LogManager.GetCurrentClassLogger())
            .As<ILogger>()
            .SingleInstance();
        }
    }
}