using System;
using System.Configuration;
using System.Linq;
using System.Reflection;
using Autofac;
using ESign.Entity;
using ESign.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ESign
{
    public static class ESignExtenstions
    {
        public static ContainerBuilder AutoFac_RegisterESign(this ContainerBuilder builder)
        {
            var assembly = Assembly.Load("ESign");

            builder.RegisterAssemblyTypes(assembly)
                    .Where(t => t.Name.EndsWith("Service") || t.Name.EndsWith("ApiProxy"))
                   .AsImplementedInterfaces()
                   .InstancePerLifetimeScope();

            ESignOption options = new ESignOption();
            options.ESignFileServer = ConfigurationManager.AppSettings[nameof(options.ESignFileServer)];
            options.ESignOrgName = ConfigurationManager.AppSettings[nameof(options.ESignOrgName)];
            options.ESignUrl = ConfigurationManager.AppSettings[nameof(options.ESignUrl)];
            options.AppId = ConfigurationManager.AppSettings[nameof(options.AppId)];
            options.AppSecret = ConfigurationManager.AppSettings[nameof(options.AppSecret)];
            options.UploadFile = ConfigurationManager.AppSettings[nameof(options.UploadFile)];
            options.Keyword = ConfigurationManager.AppSettings[nameof(options.Keyword)];
            options.PsnAccount = ConfigurationManager.AppSettings[nameof(options.PsnAccount)];
            options.UploadUrl = ConfigurationManager.AppSettings[nameof(options.UploadUrl)];

            options.AutoStart = Convert.ToBoolean(ConfigurationManager.AppSettings[nameof(options.AutoStart)]);
            options.AutoFinish = Convert.ToBoolean(ConfigurationManager.AppSettings[nameof(options.AutoFinish)]);
            options.NoticeTypes = ConfigurationManager.AppSettings[nameof(options.NoticeTypes)];
            options.RedirectUrl = ConfigurationManager.AppSettings[nameof(options.RedirectUrl)];

            options.SignRedirectUrl = ConfigurationManager.AppSettings[nameof(options.SignRedirectUrl)];
            options.NeedLogin = Convert.ToBoolean(ConfigurationManager.AppSettings[nameof(options.NeedLogin)]);
            options.UrlType = Convert.ToInt32(ConfigurationManager.AppSettings[nameof(options.UrlType)]);
            options.ClientType = ConfigurationManager.AppSettings[nameof(options.ClientType)];
            options.RedirectDelayTime = Convert.ToInt32(ConfigurationManager.AppSettings[nameof(options.RedirectDelayTime)]);

            builder.RegisterInstance(options).As<ESignOption>().SingleInstance();

            return builder;
        }

        public static T GetResult<T>(this HttpRespResult response)
        {
            return response.HttpStatusCode == 200
               ? JsonConvert.DeserializeObject<T>(response.RespData.ToString())
               : throw new Exception(response.HttpStatusCodeMsg);
        }
    }
}
