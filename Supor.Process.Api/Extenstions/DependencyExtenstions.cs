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
        /// 注册基础服务
        /// </summary>
        public static void AutofacRegister()
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            RegisterBySuffix(builder, "Supor.Process.Domain", "Domain");
            RegisterBySuffix(builder, "Supor.Process.Services", "Service");
            RegisterBySuffix(builder, "Supor.Process.Services",
                new[] { "Validtor", "Processor", "ProcessorFactory" });
            RegisterBySuffix(builder, "Supor.Process.Services", "Repository");
            RegisterBySuffix(builder, "Supor.Process.Services", "Executor");

            builder.Register(ctx => new MapperConfiguration(cfg =>
                cfg.AddProfiles(Assembly.Load("Supor.Process.Common")))
            ).AsSelf().SingleInstance();

            builder.Register(c => c.Resolve<MapperConfiguration>().CreateMapper())
                .As<IMapper>().SingleInstance();

            LogManager.Setup().LoadConfigurationFromFile("nlog.config");
            builder.Register(_ => LogManager.GetCurrentClassLogger())
                .As<ILogger>().SingleInstance();

            var container = builder.Build();
            ServiceLocator.SetContainer(container);
            GlobalConfiguration.Configuration.DependencyResolver =
                new AutofacWebApiDependencyResolver(container);
        }

        private static void RegisterBySuffix(
            ContainerBuilder builder,
            string assemblyName,
            params string[] suffixes)
        {
            builder.RegisterAssemblyTypes(Assembly.Load(assemblyName))
                .Where(t => suffixes.Any(s => t.Name.EndsWith(s)) && !t.IsAbstract)
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}