using Client.Library.API;
using NLog;
using Supor.Process.Entity.Entity;
using Supor.Process.Entity.InputDto;
using Supor.Process.Services.Repositories;
using Supor.Process.Services.Services;
using Supor.Utility.Data;
using System;
using System.Collections.Generic;
using System.Net;

namespace Supor.Process.Services.Processor
{
    public class SECProcessor : BaseProcessor
    {
        public SECProcessor(ILogger logger, II_OSYS_PROCDATA_ITEMSRepository i_OSYS_PROCDATA_ITEMSRepository,
            II_OSYS_PROC_INSTSRepository i_OSYS_PROC_INSTSRepository,
            II_OSYS_WF_WORKITEMSRepository i_OSYS_WF_WORKITEMSRepository,
            IPerInfoService perInfoService, IProcessItemsService processItemsService, IOrgInfoService orgInfoService)
            : base(logger, i_OSYS_PROCDATA_ITEMSRepository, i_OSYS_PROC_INSTSRepository, i_OSYS_WF_WORKITEMSRepository,
                  perInfoService, processItemsService, orgInfoService)
        {
        }

        public override string GetTag()
        {
            return "SEC";
        }

        public override bool SubmitBusDataToDB(TaskDto dto, ProcessDataDto processDataDto, Dictionary<string, object> formData, TaskEntity te, string status, string appNo, string procInstId)
        {
            DataCenter dc = new DataCenter("BPM_Trans");
            return dc.ExecuteNonQuery((tran) =>
            {
                try
                {
                    object[] objMain = formData["main"] as object[];

                    int res = 0;
                    KFLibrary.Log.LoggorHelper.WriteLog(appNo + "开始插入业务表数据。关联信息：" + procInstId);

                    //流程实例表数据插入
                    KFLibrary.Log.LoggorHelper.WriteLog(appNo + "开始插入业务流程实例表数据。关联信息：" + procInstId);
                    res += new BaseData().SaveProcInstsInfo(procInstId, tran);
                    KFLibrary.Log.LoggorHelper.WriteLog(appNo + "插入业务流程实例表数据成功。关联信息：" + procInstId);
                }
                catch (Exception insertex)
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                    soap.SetProcessStall(dto.CreateUserID, procInstId); // 自动取消流程
                    throw insertex;
                }

                return true;
            });
        }

        public override bool UpdateBusDataToDB(TaskDto dto, ProcessDataDto processDataDto, Dictionary<string, object> formData, TaskEntity te, string status, string appNo, string procInstId)
        {
            return true;
        }
    }
}
