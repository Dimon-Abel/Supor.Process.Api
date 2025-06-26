using Client.Library.API;
using Client.Library;
using Entity;
using Logic;
using Supor.Process.Common.Extensions;
using Supor.Process.Entity.Entity;
using Supor.Process.Entity.Enums;
using Supor.Process.Entity.InputDto;
using Supor.Utility.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Supor.Utility.Data;
using Supor.Utility.Config;
using System.Data;
using Supor.Utility.Email;
using System.Net.Mail;
using System.Text;
using Supor.Process.Services.Repositories;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Supor.Process.Services.Services;
using NLog;

namespace Supor.Process.Services.Processor
{
    public abstract class BaseProcessor : IProcessor
    {
        protected ILogger _logger;

        protected static TaskAPI TaskAPI = new TaskAPI();

        protected WorkFlowAPISoapClient soap = new WorkFlowAPISoapClient();

        protected readonly II_OSYS_PROCDATA_ITEMSRepository _i_OSYS_PROCDATA_ITEMSRepository;

        protected readonly II_OSYS_PROC_INSTSRepository _i_OSYS_PROC_INSTSRepository;

        protected readonly II_OSYS_WF_WORKITEMSRepository _i_OSYS_WF_WORKITEMSRepository;

        protected readonly IPerInfoService _perInfoService;

        protected readonly IProcessItemsService _processItemsService;

        protected readonly IOrgInfoService _orgInfoService;

        public BaseProcessor(ILogger logger, II_OSYS_PROCDATA_ITEMSRepository i_OSYS_PROCDATA_ITEMSRepository,
            II_OSYS_PROC_INSTSRepository i_OSYS_PROC_INSTSRepository,
            II_OSYS_WF_WORKITEMSRepository i_OSYS_WF_WORKITEMSRepository,
            IPerInfoService perInfoService,
            IProcessItemsService processItemsService,
            IOrgInfoService orgInfoService)
        {
            _logger = logger;
            _i_OSYS_PROCDATA_ITEMSRepository = i_OSYS_PROCDATA_ITEMSRepository;
            _i_OSYS_PROC_INSTSRepository = i_OSYS_PROC_INSTSRepository;
            _i_OSYS_WF_WORKITEMSRepository = i_OSYS_WF_WORKITEMSRepository;
            _perInfoService = perInfoService;
            _processItemsService = processItemsService;
            _orgInfoService = orgInfoService;
        }

        public virtual async Task<(I_OSYS_PROCDATA_ITEMS, I_OSYS_PROC_INSTS, I_OSYS_WF_WORKITEMS)> SendTask(TaskDto dto, object processData)
        {
            string sourceName = string.Empty, appNo = string.Empty;
            string processName = string.Empty, procInstId = string.Empty, summary = string.Empty;
            string procDefId = string.Empty, workItemId = string.Empty, wCreatTime = string.Empty;
            string createUserId = string.Empty, userId = string.Empty, userName = string.Empty, orgName = string.Empty, userEmail = string.Empty;
            bool hResult = false, isPublicPerAD = false;

            var json = processData.ToJson().FromJson<ProcessDataDto>();
            Enum.TryParse<EnumTaskStatus>(json.Status, out var status);


            var task = status == EnumTaskStatus.Submit
                    ? Create_I_OSYS_PROCDATA_ITEMS(dto, json)
                    : await Get_I_OSYS_PROCDATA_ITEMS(json.Guid);

            var ins = status == EnumTaskStatus.Submit
                ? Create_I_OSYS_PROC_INSTS(dto, json)
                : await Get_I_OSYS_PROC_INSTS(task.ProcInstId);

            var workitem = status == EnumTaskStatus.Submit
                ? Create_I_OSYS_WF_WORKITEMS(dto, json)
                : await Get_I_OSYS_WF_WORKITEMS(task.ProcInstId);

            try
            {
                var wpscl = new SYS_Workflow_ProcessStepConfigLogic();//获取表单配置
                var StepConfig = new List<SYS_Workflow_ProcessStepConfigEntity>();//步骤配置

                #region 判断是否来源于公共账号
                string ads = Configs.AppSettings("PublicPerADs");
                if (!string.IsNullOrWhiteSpace(ads))
                {
                    var arr = ads.Split(';');
                    for (int j = 0; j < arr.Length; j++)
                    {
                        if (arr[j].Split(':')[0].ToString().Equals(task.ProcessName) && arr[j].Split(':')[1].ToString().Replace("/", "\\").Equals(task.CreateUserID.Replace("/", "\\")))
                        {
                            isPublicPerAD = true;
                            break;
                        }
                    }
                }

                #endregion

                // 转换成页面提交数据结构
                Dictionary<string, object> formData = ConvertToFormData(dto, isPublicPerAD);

                task.ProcessData = formData.ToJson();

                _logger.Info($"formData: {formData.ToJson()}");

                if (task.Status == EnumTaskStatus.Submit)
                {
                    var TaskAPI = new TaskAPI();
                    TaskEntity te = new TaskEntity();
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                    DefEntity defModel = soap.GetLastProcDefsBySqlWhere(string.Format(" and DEF_NAME='{0}' ", dto.ProcessName));
                    if (defModel != null)
                    {
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                        te = soap.GetSingleTask(defModel.DefId);
                        TaskAPI.LoadByTask(te);
                        TaskAPI.AUDITSTATUS = "1";
                        TaskAPI.Device = "PC";


                        List<string> nextSteps = new List<string>();//节点信息
                        var stepConfig = wpscl.GetStepConfigListByPn(dto.ProcessName, nextSteps);
                        if (stepConfig.Any())
                        {
                            foreach (var ent in stepConfig)
                            {
                                if (!string.IsNullOrWhiteSpace(ent.ProcessConfig))
                                {
                                    Dictionary<string, string> dicConfig = SettingHelper.DeSetting(ent.ProcessConfig);
                                    if (dicConfig.ContainsKey("prefix") && dicConfig.ContainsKey("nolength"))
                                    {
                                        appNo = AppNoHelper.GetIdentity(dicConfig["prefix"] + DateTime.Now.ToString("yyyyMMdd"), null, Convert.ToInt32(dicConfig["nolength"]), true);
                                        task.ProcApplyNo = appNo;
                                    }

                                    if (dicConfig.ContainsKey("process_summarys"))
                                    {
                                        Hashtable htValues = new Hashtable();
                                        List<string> values = new List<string>();
                                        List<string> selItems = Regex.Split(dicConfig["process_summarys"].Replace("\r\n", "").Trim(','), ",", RegexOptions.IgnoreCase).ToList();
                                        foreach (var item in selItems)
                                        {
                                            string title = item.Split('=')[0];
                                            string code = item.Split('=')[1];

                                            string value = formData.GetMainDataValue(code);
                                            //在审批节点由于控件隐藏了，value是获取不到的；此则在原摘要中获取value
                                            if (string.IsNullOrWhiteSpace(value) && htValues.ContainsKey(title))
                                                value = htValues[title].ToString();
                                            values.Add(string.Format("{0}:{1}", title, value));
                                        }
                                        values.Add(string.Format("单号:{0}", task.ProcApplyNo));
                                        summary = string.Join("^", values);
                                        TaskAPI.SUMMARY = summary;
                                    }
                                }
                            }
                        }

                        IProcess p = ProcessManager.GetProcessHandler(task.ProcessName);
                        if (p != null)
                        {
                            if (!string.IsNullOrEmpty(te.STEPID))
                            {
                                hResult = p.SubmitTask_Before(TaskAPI, formData);  //调用提交前处理程序
                            }
                        }
                        else
                        {
                            KFLibrary.Log.LoggorHelper.WriteLog("流程处理器为空");
                            hResult = true;
                        }

                        if (hResult)
                        {
                            summary = TaskAPI.SUMMARY;
                            //NextSteps = soap.GetNextSteps(processName, defModel.DefId, procInstId, te.STEPID, te.STEPLABEL.Split('#')[0], ListVars.ToArray()).ToList(); //获取下一节点信息
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                            nextSteps = soap.GetNextSteps(task.ProcessName, defModel.DefId, task.ProcInstId, te.STEPID, te.STEPLABEL.Split('#')[0], TaskAPI.ListVars.ToArray()).ToList();

                            //先走判断是否需要走老版本逻辑---再取数据库逻辑
                            //读取指定流程所有节点的步骤配置
                            var pfm = ProcessFormManager.Instance().GetFormVersion(task.ProcessName, task.ProcInstId, TaskAPI.STARTTIME_P);
                            if (pfm != new Models.FormConfig())
                            {
                                var stepList = pfm.StepConfigs.Where(a => nextSteps.Contains(a.StepName)).Select(m => m.StepConfig).ToList();
                                if (stepList.Count() > 0)
                                {
                                    stepConfig = stepList;
                                }
                                else stepConfig = wpscl.GetStepConfigListByPn(task.ProcessName, nextSteps);
                            }
                            if (stepConfig.Count > 0)
                            {
                                foreach (var ent in stepConfig)
                                {
                                    KFLibrary.Log.LoggorHelper.WriteLog("正在查找节点的处理人：" + ent.ProcessName + "," + ent.StepName + ",type=" + ent.RecipientType + ",typevalue:" + ent.Recipient);
                                    if (!string.IsNullOrWhiteSpace(ent.Recipient))
                                    {
                                        try
                                        {
                                            IUserFinder userFinder = UserFinderManager.GetUserFinder(); //获取用户查找器实现自动找人功能
                                            if (userFinder != null)
                                            {
                                                var config = new StepUserConfig()
                                                {
                                                    ProcessName = ent.ProcessName,
                                                    Incident = task.ProcInstId,
                                                    StepName = ent.StepName,
                                                    TypeValue = ent.Recipient,
                                                    TypeValueId = ent.Recipient_Value,
                                                    ConfigType = ent.RecipientType,
                                                    UserAttrName = ent.RecipientVariName,
                                                    FormData = formData
                                                };

                                                List<string> userIds = userFinder.GetStepUsers(config);

                                                if (userIds.Count > 0)
                                                {
                                                    if (userIds[0].Contains("{"))
                                                    {
                                                        var uids = ""; var posids = "";
                                                        foreach (var item in userIds)
                                                        {
                                                            var uflist = KFLibrary.Utils.DataToJSON.FromJson<List<UserFinder>>(item);
                                                            uids += string.Join("~", uflist.Select(a => a.AD.Replace("/", "\\"))) + "~";
                                                            posids += string.Join("~", uflist.Select(a => a.Pos_code)) + "~";
                                                        }
                                                        uids = uids.TrimEnd('~');
                                                        posids = posids.TrimEnd('~');
                                                        KFLibrary.Log.LoggorHelper.WriteLog(uids + "," + posids);
                                                        if (ent.StepName.Contains("AgileWork"))
                                                        {
                                                            if (TaskAPI.ListVars.FirstOrDefault(a => a.Key == "UserPosIDForLoop") != null)
                                                            {
                                                                TaskAPI.ListVars.FirstOrDefault(a => a.Key == "UserPosIDForLoop").Value = posids;
                                                            }
                                                            else
                                                            {
                                                                TaskAPI.ListVars.Add(new Params() { Key = "UserPosIDForLoop", Value = posids });
                                                            }
                                                            this["UserPosIDForLoop"] = posids.TrimEnd('~');
                                                        }
                                                        this["__STEPUSERID__" + ent.RecipientVariName] = uids.TrimEnd('~');

                                                    }
                                                    else
                                                    {
                                                        this["__STEPUSERID__" + ent.RecipientVariName] = string.Join("~", userIds);
                                                    }
                                                }
                                                else
                                                {
                                                    this["__STEPUSERID__" + ent.RecipientVariName] = "";
                                                }
                                            }
                                            else
                                            {
                                                KFLibrary.Log.LoggorHelper.WriteLog("警告：没有用户查找器，流程无法自动找人！！！！");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception("用户查找器出错！--关联信息：" + ex);
                                        }
                                    }
                                }

                                //创建新实例信息
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                workItemId = soap.SendTask(task.CreateUserID.Replace("/", "\\"), defModel.DefId, summary, TaskAPI.ListVars.ToArray(), false, ref procInstId);

                                #region 回传任务信息填写

                                task.ProcInstId = procInstId;

                                ins.PROC_INST_ID = procInstId;
                                ins.PROC_APPLYNO = task.ProcApplyNo;
                                ins.PROC_INST_NAME = summary;

                                workitem.WORK_ITEM_ID = workItemId;

                                #endregion


                                if (!string.IsNullOrWhiteSpace(workItemId) && !string.IsNullOrWhiteSpace(task.ProcInstId))
                                {
                                    this["SYSTEM_INCIDENT"] = task.ProcInstId;
                                    KFLibrary.Log.LoggorHelper.WriteLog(summary + "已经创建.ProcInstId:" + task.ProcInstId + ",workItemId:" + workItemId);
                                    try
                                    {
                                        DataCenter dc = new DataCenter("BPM_Trans");
                                        #region 业务表数据插入
                                        bool insertFlag = BusDataToDB(task, json, processData, te);

                                        if (!insertFlag)
                                            throw new Exception("事务存储业务数据写入出错！--关联信息：" + summary + "___" + task.ProcInstId);
                                        #endregion

                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                        string mes = soap.SendTask(task.CreateUserID.Replace("/", "\\"), workItemId, summary, TaskAPI.ListVars.ToArray(), false, ref procInstId);

                                        if (string.IsNullOrWhiteSpace(mes))
                                        {
                                            KFLibrary.Log.LoggorHelper.WriteLog(summary + "已经驱动成功.ProcInstId:" + task.ProcInstId + ",workItemId:" + workItemId);
                                            if (!isPublicPerAD)
                                            {
                                                new BaseData().InsertSign(task.ProcessName, task.ProcInstId, workItemId, te.STEPLABEL, json.System_ApplyUserID, json.System_ApplyUserName, json.System_ApplyEmail, json.Sys_OrgName, DateTime.Now.ToString(), "自动在" + task.SysSource + "中发起流程", json.Status);
                                            }
                                            new BaseData().UpdateSubmitStatus(json.Guid, "1", "1", ((int)status).ToString(), task.ProcInstId, task.ProcApplyNo);
                                            new BaseData().DeleteMtBusinessData(json.Guid);
                                        }
                                        else
                                        {
                                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                            soap.SuspendProcInst(task.ProcInstId);
                                            new BaseData().UpdateSubmitStatus(json.Guid, "1", "0", ((int)status).ToString(), task.ProcInstId, task.ProcApplyNo); //launchstatus,issubmit
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("调用 AutoCreateProcess 方法出错：" + ex);
                                    }
                                }
                                else
                                    throw new Exception("流程创建失败！--关联信息：" + summary);
                            }
                            else
                                throw new Exception("自动创建流程失败，未获取到可用的流程配置和步骤配置信息！--关联信息：" + task.GUID);
                        }
                        else
                            throw new Exception("自动创建流程失败，未能正常加载流程处理程序！--关联信息：" + task.GUID);
                    }
                    else
                        throw new Exception("自动创建流程失败，未获取到可用的流程模板信息！--关联信息：" + task.GUID);
                }
                else if (status == EnumTaskStatus.Agree || (status == EnumTaskStatus.Submit && task.IsSubmit == "0" && task.LaunchStatus == EnumLanchStatus.Faild))//重新发起流程
                {
                    DataTable procdt = new BaseData().GetWorkItemId(task.ProcInstId);
                    if (procdt != null && procdt.Rows.Count == 1)
                    {
                        procDefId = procdt.Rows[0]["PROC_DEF_ID"].ToString();
                        workItemId = procdt.Rows[0]["WORK_ITEM_ID"].ToString();
                        wCreatTime = procdt.Rows[0]["CREATED_DATE"].ToString();

                        TaskAPI = new TaskAPI();
                        TaskEntity te = new TaskEntity();
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                        te = soap.GetSingleTask(workItemId);
                        TaskAPI.LoadByTask(te);
                        TaskAPI.AUDITSTATUS = "1";
                        TaskAPI.Device = "PC";
                        summary = TaskAPI.SUMMARY;
                        appNo = TaskAPI.SUMMARY.Substring(TaskAPI.SUMMARY.LastIndexOf(":") + 1);
                        this["AuditStatus"] = "1";
                        this["SYSTEM_INCIDENT"] = task.ProcInstId;

                        List<string> NextSteps = new List<string>();//节点信息

                        #region 解析摘要配置并设置摘要
                        NextSteps.Add("-");//获取流程配置中的摘要和单号配置信息
                                           //先走判断是否需要走老版本逻辑---再取数据库逻辑
                                           //读取指定流程所有节点的步骤配置
                        var pfm = ProcessFormManager.Instance().GetFormVersion(task.ProcessName, task.ProcInstId, TaskAPI.STARTTIME_P);
                        if (pfm != new Models.FormConfig())
                        {
                            var stepList = pfm.StepConfigs.Where(a => NextSteps.Contains(a.StepName)).Select(m => m.StepConfig).ToList();
                            if (stepList.Count() > 0)
                            {
                                StepConfig = stepList;
                            }
                            else StepConfig = wpscl.GetStepConfigListByPn(task.ProcessName, NextSteps);
                        }
                        if (StepConfig.Count > 0)
                        {
                            foreach (var ent in StepConfig)
                            {
                                if (!string.IsNullOrWhiteSpace(ent.ProcessConfig)) //解析单号规则并配置单号
                                {
                                    Dictionary<string, string> dicConfig = SettingHelper.DeSetting(ent.ProcessConfig);
                                    if (dicConfig.ContainsKey("process_summarys"))
                                    {
                                        Hashtable htValues = new Hashtable();
                                        List<string> values = new List<string>();
                                        List<string> selItems = Regex.Split(dicConfig["process_summarys"].Replace("\r\n", "").Trim(','), ",", RegexOptions.IgnoreCase).ToList();
                                        foreach (var item in selItems)
                                        {
                                            string title = item.Split('=')[0];
                                            string code = item.Split('=')[1];

                                            string value = formData.GetMainDataValue(code);
                                            //在审批节点由于控件隐藏了，value是获取不到的；此则在原摘要中获取value
                                            if (string.IsNullOrWhiteSpace(value) && htValues.ContainsKey(title))
                                                value = htValues[title].ToString();
                                            values.Add(string.Format("{0}:{1}", title, value));
                                        }
                                        values.Add(string.Format("单号:{0}", appNo));
                                        summary = string.Join("^", values);
                                        TaskAPI.SUMMARY = summary;
                                    }
                                }
                            }
                        }
                        #endregion

                        //加载流程处理程序
                        IProcess p = ProcessManager.GetProcessHandler(processName);
                        if (p != null)
                        {
                            if (!string.IsNullOrEmpty(te.STEPID))
                            {
                                hResult = p.SubmitTask_Before(TaskAPI, formData);  //调用提交前处理程序
                            }
                        }
                        else
                        {
                            KFLibrary.Log.LoggorHelper.WriteLog("流程处理器为空");
                            hResult = true;
                        }

                        if (hResult)
                        {
                            summary = TaskAPI.SUMMARY;
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                            NextSteps = soap.GetNextSteps(processName, workItemId, procInstId, te.STEPID, te.STEPLABEL.Split('#')[0], TaskAPI.ListVars.ToArray()).ToList(); //获取下一节点信息
                            if (pfm != new Models.FormConfig())
                            {
                                var stepList = pfm.StepConfigs.Where(a => NextSteps.Contains(a.StepName)).Select(m => m.StepConfig).ToList();
                                if (stepList.Count() > 0)
                                {
                                    StepConfig = stepList;
                                }
                                else StepConfig = wpscl.GetStepConfigListByPn(task.ProcessName, NextSteps);
                            }
                            if (StepConfig.Count > 0)
                            {
                                foreach (var ent in StepConfig)
                                {
                                    KFLibrary.Log.LoggorHelper.WriteLog("正在查找节点的处理人：" + ent.ProcessName + "," + ent.StepName + ",type=" + ent.RecipientType + ",typevalue:" + ent.Recipient);
                                    if (!string.IsNullOrWhiteSpace(ent.Recipient))
                                    {
                                        try
                                        {
                                            IUserFinder userFinder = UserFinderManager.GetUserFinder(); //获取用户查找器实现自动找人功能
                                            if (userFinder != null)
                                            {
                                                var config = new StepUserConfig()
                                                {
                                                    ProcessName = ent.ProcessName,
                                                    Incident = procInstId,
                                                    StepName = ent.StepName,
                                                    TypeValue = ent.Recipient,
                                                    TypeValueId = ent.Recipient_Value,
                                                    ConfigType = ent.RecipientType,
                                                    UserAttrName = ent.RecipientVariName,
                                                    FormData = formData
                                                };

                                                List<string> userIds = userFinder.GetStepUsers(config);

                                                if (userIds.Count > 0)
                                                {
                                                    if (userIds[0].Contains("{"))
                                                    {
                                                        var uids = ""; var posids = "";
                                                        foreach (var item in userIds)
                                                        {
                                                            var uflist = KFLibrary.Utils.DataToJSON.FromJson<List<UserFinder>>(item);
                                                            uids += string.Join("~", uflist.Select(a => a.AD.Replace("/", "\\"))) + "~";
                                                            posids += string.Join("~", uflist.Select(a => a.Pos_code)) + "~";
                                                        }
                                                        uids = uids.TrimEnd('~');
                                                        posids = posids.TrimEnd('~');
                                                        KFLibrary.Log.LoggorHelper.WriteLog(uids + "," + posids);
                                                        if (ent.StepName.Contains("AgileWork"))
                                                        {
                                                            if (TaskAPI.ListVars.FirstOrDefault(a => a.Key == "UserPosIDForLoop") != null)
                                                            {
                                                                TaskAPI.ListVars.FirstOrDefault(a => a.Key == "UserPosIDForLoop").Value = posids;
                                                            }
                                                            else
                                                            {
                                                                TaskAPI.ListVars.Add(new Params() { Key = "UserPosIDForLoop", Value = posids });
                                                            }
                                                            this["UserPosIDForLoop"] = posids.TrimEnd('~');
                                                        }
                                                        this["__STEPUSERID__" + ent.RecipientVariName] = uids.TrimEnd('~');

                                                    }
                                                    else
                                                    {
                                                        this["__STEPUSERID__" + ent.RecipientVariName] = string.Join("~", userIds);
                                                    }
                                                }
                                                else
                                                {
                                                    this["__STEPUSERID__" + ent.RecipientVariName] = "";
                                                }
                                            }
                                            else
                                            {
                                                KFLibrary.Log.LoggorHelper.WriteLog("警告：没有用户查找器，流程无法自动找人！！！！");
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            throw new Exception("用户查找器出错！--关联信息：" + ex);
                                        }
                                    }
                                }

                                try
                                {
                                    DataCenter dc = new DataCenter("BPM_Trans");
                                    #region 业务表数据插入
                                    bool insertFlag = BusDataToDB(task, json, processData, te);

                                    if (!insertFlag)
                                        throw new Exception("事务存储业务数据写入出错！--关联信息：" + summary + "___" + procInstId);
                                    #endregion

                                    //依据流程实例号重新启动流程
                                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                    soap.ResumeProcInst(procInstId);
                                    KFLibrary.Log.LoggorHelper.WriteLog(processName + "ResumeProcInst成功。关联信息：" + procInstId);

                                    string mes = string.Empty;
                                    if (createUserId == userId)
                                    {
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                        mes = soap.SendTask(userId.Replace("/", "\\"), workItemId, summary, TaskAPI.ListVars.ToArray(), false, ref procInstId);
                                    }
                                    else
                                    {
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                        if (soap.AssignTask(workItemId, createUserId.Replace("/", "\\")))
                                        {
                                            KFLibrary.Log.LoggorHelper.WriteLog(summary + "已经指派完成.ProcInstId:" + procInstId + ",workItemId:" + workItemId);
                                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                            mes = soap.SendTask(createUserId.Replace("/", "\\"), workItemId, summary, TaskAPI.ListVars.ToArray(), false, ref procInstId);
                                        }
                                        else
                                        {
                                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                            soap.SuspendProcInst(procInstId);
                                        }
                                    }
                                    if (string.IsNullOrWhiteSpace(mes))
                                    {
                                        KFLibrary.Log.LoggorHelper.WriteLog(summary + "已经激活.ProcInstId:" + procInstId + ",workItemId:" + workItemId);
                                        if (createUserId == userId && !isPublicPerAD)
                                            new BaseData().InsertSign(processName, procInstId, workItemId, te.STEPLABEL, userId, userName, userEmail, orgName, wCreatTime, "自动在" + sourceName + "中重新发起流程", ((int)status).ToString());
                                        else
                                        {
                                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                            UserInfoEntity u = soap.GetUserInfo(createUserId);
                                            if (u != null)
                                                new BaseData().InsertSign(processName, procInstId, workItemId, te.STEPLABEL, createUserId, u.StrUserFullName, u.StrEmail, u.StrDepartment, wCreatTime, userName + "委托给" + u.StrUserFullName + "自动在" + sourceName + "中重新发起流程", ((int)status).ToString());
                                        }
                                        new BaseData().UpdateSubmitStatus(json.Guid, "1", "1", ((int)status).ToString(), procInstId, appNo);//launchstatus,issubmit
                                        new BaseData().DeleteMtBusinessData(json.Guid);
                                    }
                                    else
                                    {
                                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                        soap.SuspendProcInst(procInstId);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("调用 AutoCreateProcess 方法出错：" + ex);
                                }
                            }
                            else
                                throw new Exception("重新发起流程失败，未获取到可用的流程配置和步骤配置信息！--关联信息：" + json.Guid);
                        }
                        else
                            throw new Exception("重新发起流程失败，未能正常加载流程处理程序！--关联信息：" + json.Guid);
                    }
                    else
                        throw new Exception("重新发起流程失败，未获取到激活的任务信息！--关联信息：" + summary + "___" + procInstId);
                }
                else if (status == EnumTaskStatus.DisAgree) //终止流程
                {
                    DataTable procdt = new BaseData().GetWorkItemId(procInstId);
                    if (procdt != null)
                    {
                        workItemId = procdt.Rows[0]["WORK_ITEM_ID"].ToString();
                        wCreatTime = procdt.Rows[0]["CREATED_DATE"].ToString();

                        TaskAPI = new TaskAPI();
                        TaskEntity te = new TaskEntity();
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                        te = soap.GetSingleTask(workItemId);
                        TaskAPI.LoadByTask(te);
                        TaskAPI.AUDITSTATUS = "终止";
                        summary = TaskAPI.SUMMARY;

                        //依据流程实例号重新启动流程
                        ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                        soap.ResumeProcInst(procInstId);
                        KFLibrary.Log.LoggorHelper.WriteLog(processName + "ResumeProcInst成功。关联信息：" + procInstId);

                        bool cancelflag = false;
                        if (createUserId == userId)
                        {
                            //依据创建人账号、流程实例号去驱动取消流程。若流程驱动失败，则继续暂停流程
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                            cancelflag = soap.SetProcessStall(createUserId.Replace("/", "\\"), procInstId);
                            KFLibrary.Log.LoggorHelper.WriteLog(processName + "取消" + cancelflag + ".ProcInstId:" + procInstId);
                        }
                        else if (soap.AssignTask(workItemId, createUserId.Replace("/", "\\")))
                        {
                            //依据创建人账号、流程实例号去驱动取消流程。若流程驱动失败，则继续暂停流程
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                            cancelflag = soap.SetProcessStall(createUserId.Replace("/", "\\"), procInstId);
                            KFLibrary.Log.LoggorHelper.WriteLog(processName + "取消" + cancelflag + ".ProcInstId:" + procInstId);
                        }
                        if (cancelflag)
                        {
                            if (createUserId == userId && !isPublicPerAD)
                                new BaseData().InsertSign(processName, procInstId, workItemId, procdt.Rows[0]["DISPLAY_NAME"].ToString() + "#" + procdt.Rows[0]["NAME"].ToString(), userId, userName, userEmail, orgName, wCreatTime, "流程在" + sourceName + "中被终止", ((int)status).ToString());
                            else
                            {
                                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                                UserInfoEntity u = soap.GetUserInfo(createUserId);
                                if (u != null)
                                    new BaseData().InsertSign(processName, procInstId, workItemId, procdt.Rows[0]["DISPLAY_NAME"].ToString() + "#" + procdt.Rows[0]["NAME"].ToString(), createUserId, u.StrUserFullName, u.StrEmail, u.StrDepartment, wCreatTime, userName + "委托给" + u.StrUserFullName + "自动在" + sourceName + "中终止流程", ((int)status).ToString());
                            }
                            new BaseData().UpdateSubmitStatus(json.Guid, "1", "1", ((int)status).ToString(), procInstId, appNo);
                            new BaseData().DeleteMtBusinessData(json.Guid);

                            IProcess p = ProcessManager.GetProcessHandler(processName);
                            if (p != null)
                            {
                                p.SubmitTask_After(TaskAPI, formData);  //调用提交前处理程序
                            }
                            else
                            {
                                KFLibrary.Log.LoggorHelper.WriteLog("流程处理器为空");
                            }
                        }
                        else
                        {
                            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                            bool suspendflag = soap.SuspendProcInst(procInstId);
                            KFLibrary.Log.LoggorHelper.WriteLog(processName + "SuspendProcInst" + suspendflag + "。关联信息：" + procInstId);
                        }
                    }
                    else
                        throw new Exception("终止流程失败，未获取到激活的任务信息！--关联信息：" + summary + "___" + procInstId);
                }

            }
            catch (Exception ex)
            {
                KFLibrary.Log.LoggorHelper.WriteLog("调用 Processor.AutoCreateProcess 方法出错：" + ex);
                SendEmail.Send(new MailAddress(Configs.AppSettings("AdminEmail"), "系统管理员", Encoding.GetEncoding(936)), Configs.AppSettings("ToEmail"), "【" + summary + "】流程发起失败", "【" + summary + "】流程调用 Processor.AutoCreateProcess 方法出错：" + ex.ToString(), "");
                throw new Exception("调用 Processor.AutoCreateProcess 方法出错：" + ex);
            }

            return (task, ins, workitem);
        }

        public virtual bool BusDataToDB(I_OSYS_PROCDATA_ITEMS task, ProcessDataDto dto, object processData, TaskEntity te)
        {
            DataCenter dc = new DataCenter("BPM_Trans");
            return dc.ExecuteNonQuery((tran) =>
            {
                try
                {
                    int res = 0;
                    KFLibrary.Log.LoggorHelper.WriteLog(task.ProcApplyNo + "开始插入业务表数据。关联信息：" + task.ProcInstId);
                    Dictionary<string, object> formData = (Dictionary<string, object>)processData;//获取流程信息
                    object[] objMain = processData as object[];//主数据
                    //主数据
                    res += new BaseData().SaveBussinessMainData(dto.Guid, objMain, task.ProcInstId, task.ProcApplyNo, task.Status.ToString(), tran);
                    KFLibrary.Log.LoggorHelper.WriteLog(task.ProcApplyNo + "插入业务主表数据成功。关联信息：" + task.ProcInstId);

                    //明细
                    if (formData.ContainsKey("Details"))
                    {
                        object[] objsub = formData["Details"] as object[];
                        res += new BaseData().SaveBussinessSubData(dto.Guid, objsub, dto.Status, tran);
                        KFLibrary.Log.LoggorHelper.WriteLog(task.ProcApplyNo + "插入业务明细表数据成功。关联信息：" + task.ProcInstId);
                    }

                    //附件明细
                    object[] objattachsub = formData["AttachmentDetails"] as object[];
                    KFLibrary.Log.LoggorHelper.WriteLog(task.ProcApplyNo + "开始插入业务附件明细表数据。关联信息：" + task.ProcInstId);
                    res += new BaseData().InsertAttchment(task.ProcessName, objattachsub, dto.System_ApplyUserID, dto.System_ApplyUserName, task.GUID, te.STEPLABEL, tran);
                    KFLibrary.Log.LoggorHelper.WriteLog(task.ProcApplyNo + "插入业务附件明细表数据成功。关联信息：" + task.ProcInstId);

                    //流程实例表数据插入
                    KFLibrary.Log.LoggorHelper.WriteLog(task.ProcApplyNo + "开始插入业务流程实例表数据。关联信息：" + task.ProcInstId);
                    res += new BaseData().SaveProcInstsInfo(task.ProcInstId, tran);
                    KFLibrary.Log.LoggorHelper.WriteLog(task.ProcApplyNo + "插入业务流程实例表数据成功。关联信息：" + task.ProcInstId);
                }
                catch (Exception insertex)
                {
                    task.IsError = insertex.Message;

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | (SecurityProtocolType)0x300 | (SecurityProtocolType)0xC00;
                    soap.SetProcessStall(task.CreateUserID, task.ProcInstId); // 自动取消流程
                    throw insertex;
                }
                return true;
            });

        }

        public virtual string GetTag()
        {
            return "Base";
        }



        /// <summary>
        /// 获取或设置电子表格中变量的值
        /// </summary>
        /// <param name="strVarName"></param>
        /// <returns></returns>
        public object this[string strVarName]
        {
            get
            {
                return TaskAPI.ListVars.Find(delegate (Params v) { return v.Key == strVarName; }).Value;
            }
            set
            {
                bool flag = TaskAPI.ListVars.Exists(delegate (Params v) { return v.Key == strVarName; });
                var _list = new List<object>();
                if (value.GetType().IsArray)
                {
                    _list = (value as object[]).ToList();
                    if (_list.Count == 1)
                        _list.Add(""); //如果为数组且长度为1则添加一个空的
                }
                object objVal = value.GetType().IsArray ? (string.Join("#,#", _list)) : value; //多值时分隔传递

                if (!flag)
                {
                    TaskAPI.ListVars.Add(new Params() { Key = strVarName, Value = objVal });
                }
                else
                {
                    TaskAPI.ListVars.Find(delegate (Params v) { return v.Key == strVarName; }).Value = objVal;
                }
            }
        }



        #region private

        public Dictionary<string, object> ConvertToFormData(TaskDto dto, bool isPublicPerAD)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            var jsonData = dto.ProcessData.ToJson().FromJson<Dictionary<string, object>>();
            var status = jsonData["Status"].ToString();
            var sourceName = dto.SourceName;
            var processName = dto.ProcessName;
            var key = jsonData["Guid"]?.ToString();


            result.Add("SourceName", sourceName);
            result.Add("CreateUserID", dto.CreateUserID.Replace("\\", "/"));
            result.Add("Status", status);


            //获取流程信息中流程名称和对应的DM_GUID
            DataTable pmdt = new DMM_processmodelLogic().SelectProcessModelListByProcess(processName);

            if (pmdt != null && pmdt.Rows.Count > 0)
            {
                int i = 0;
                List<DetailModel> _subModel = new List<DetailModel>();
                foreach (DataRow dr in pmdt.Rows)
                {
                    if (dr["MODELCODE"].ToString() != "PROC_ATTACHMENT")
                    {
                        if (dr["ISDATALIST"].ToString() == "0") //主表添加头部字段
                        {
                            JObject _data = new JObject();

                            foreach (var item in jsonData)
                            {
                                if (item.Key.ToUpper() == "SYSTEM_APPLYUSERID")
                                {
                                    _data.Add(new JProperty("SYSTEM_APPLYUSERID", jsonData["System_ApplyUserID"].ToString().Replace("\\", "/")));
                                }
                                else if (item.Key.ToUpper() == "SYSTEM_APPLYUSERNAME")
                                {
                                    _data.Add(new JProperty("SYSTEM_APPLYUSERNAME", item.Value));
                                }
                                else if (item.Key.ToLower() != "status")
                                {
                                    if (item.Value.GetType().ToString() != "Array")
                                        _data.Add(new JProperty(item.Key, item.Value));
                                }
                            }

                            #region 封装系统字段

                            _data.Add(new JProperty("SYSTEM_PROCESSNAME", processName));

                            if (jsonData["Status"].ToString() == "0")
                            {
                                _data.Add(new JProperty("SYSTEM_INCIDENT", "0"));
                                _data.Add(new JProperty("SYSTEM_APPLYNO", ""));
                                _data.Add(new JProperty("SYSTEM_APPLYDATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
                            }
                            else
                            {
                                var processItemModel = _processItemsService.GetInfobyJsonKeyAndProcessName(key, processName, sourceName);
                                _data.Add(new JProperty("SYSTEM_INCIDENT", processItemModel.ProcInstId));
                                _data.Add(new JProperty("SYSTEM_APPLYNO", processItemModel.ProcApplyNo));
                            }
                            List<ApplicantInfo> perInfoList = new List<ApplicantInfo>();
                            if (!isPublicPerAD)
                            {
                                perInfoList = _perInfoService.GetUserInfos(jsonData["System_ApplyUserName"].ToString(), jsonData["Sys_UserCode"].ToString(), jsonData["System_ApplyUserID"].ToString().Replace('/', '\\'), jsonData["Sys_PosCode"].ToString(), "", jsonData["Sys_OrgCode"].ToString(), jsonData["Sys_BuCode"].ToString(), sourceName);
                            }
                            else
                            {
                                perInfoList = _perInfoService.GetUserInfos("", jsonData["Actual_EmpNo"].ToString(), "", "", "", "", "", sourceName);
                            }
                            if (perInfoList.Count == 0 && status == "1")
                            {
                                if (!isPublicPerAD)
                                {
                                    perInfoList = _perInfoService.GetUserInfosIncludedDimission(jsonData["System_ApplyUserName"].ToString(), jsonData["Sys_UserCode"].ToString(), jsonData["System_ApplyUserID"].ToString().Replace('/', '\\'), jsonData["Sys_PosCode"].ToString(), "", jsonData["Sys_OrgCode"].ToString(), jsonData["Sys_BuCode"].ToString(), sourceName);
                                }
                                else
                                {
                                    perInfoList = _perInfoService.GetUserInfosIncludedDimission("", jsonData["Actual_EmpNo"].ToString(), "", "", "", "", "", sourceName);
                                }
                                foreach (ApplicantInfo item in perInfoList)
                                {
                                    _data["Sys_PosCode"] = item.Sys_PosCode;
                                    _data["Sys_BuCode"] = item.Sys_BuCode;
                                    _data["SYSTEM_APPLYUSERID"] = string.IsNullOrWhiteSpace(item.SYSTEM_APPLYUSERID) ? "" : item.SYSTEM_APPLYUSERID.Replace("\\", "/");
                                    _data["SYSTEM_APPLYUSERNAME"] = item.SYSTEM_APPLYUSERNAME;
                                    _data["Sys_UserCode"] = item.Sys_UserCode;
                                    _data["Sys_OrgCode"] = item.Sys_OrgCode;
                                }
                            }
                            else
                            {
                                foreach (ApplicantInfo item in perInfoList)
                                {
                                    _data.Add(new JProperty("SYSTEM_APPLYCOMPANY", item.SYSTEM_APPLYCOMPANY));
                                    _data.Add(new JProperty("SYSTEM_APPLYDEPARTMENT", item.SYSTEM_APPLYDEPARTMENT));
                                    _data.Add(new JProperty("SYSTEM_APPLYDEPARTMENTID", item.SYSTEM_APPLYDEPARTMENTID));
                                    _data.Add(new JProperty("SYSTEM_APPLYDEPTLINE", item.SYSTEM_APPLYDEPTLINE));
                                    _data.Add(new JProperty("SYSTEM_APPLYEMAIL", item.SYSTEM_APPLYEMAIL));
                                    _data.Add(new JProperty("SYSTEM_APPLYJOBNAME", item.SYSTEM_APPLYJOBNAME));
                                    _data.Add(new JProperty("SYSTEM_APPLYPHONE", item.SYSTEM_APPLYPHONE));
                                    _data.Add(new JProperty("SYSTEM_APPLYUSER_MANAGERID", item.SYSTEM_APPLYUSER_MANAGERID));
                                    _data.Add(new JProperty("SYSTEM_APPLYUSER_SUPERID", item.SYSTEM_APPLYUSER_SUPERID));
                                    _data.Add(new JProperty("Sys_CompanyCode", item.Sys_CompanyCode));
                                    _data.Add(new JProperty("Sys_CostCode", item.Sys_CostCode));
                                    _data.Add(new JProperty("Sys_CostName", item.Sys_CostName));
                                    _data.Add(new JProperty("Sys_DeptCode", item.Sys_DeptCode));
                                    _data.Add(new JProperty("Sys_DeptName", item.Sys_DeptName));
                                    _data.Add(new JProperty("Sys_OrgName", item.Sys_OrgName));
                                    _data.Add(new JProperty("Sys_ResCenterCode", item.Sys_ResCenterCode));
                                    _data.Add(new JProperty("Sys_ResCenterName", item.Sys_ResCenterName));
                                    _data.Add(new JProperty("Sys_IsActive", "1"));
                                    _data.Add(new JProperty("Sys_AppStatus", "21"));
                                    _data.Add(new JProperty("SYSTEM_TITLE", "系统自动发起"));
                                    _data["Sys_PosCode"] = item.Sys_PosCode;
                                    _data["Sys_BuCode"] = item.Sys_BuCode;
                                    _data["SYSTEM_APPLYUSERID"] = string.IsNullOrWhiteSpace(item.SYSTEM_APPLYUSERID) ? "" : item.SYSTEM_APPLYUSERID.Replace("\\", "/");
                                    _data["SYSTEM_APPLYUSERNAME"] = item.SYSTEM_APPLYUSERNAME;
                                    _data["Sys_UserCode"] = item.Sys_UserCode;
                                    _data["Sys_OrgCode"] = item.Sys_OrgCode;
                                }
                            }

                            #endregion

                            JArray _inserta = JArray.Parse("[" + _data.ToString() + "]");
                            MainModel _model = new MainModel()
                            {
                                TableId = dr["DM_GUID"].ToString(),
                                TableName = dr["MODELCODE"].ToString(),
                                Data = _inserta.ToList()
                            };
                            List<MainModel> _listModel = new List<MainModel>();
                            _listModel.Add(_model);
                            result.Add("main", _listModel);
                        }
                        else
                        {
                            i = i + 1;
                            JArray _insert = JArray.Parse(jsonData["Details" + i].ToString());
                            DetailModel _model = new DetailModel()
                            {
                                TableId = dr["DM_GUID"].ToString(),
                                TableName = dr["MODELCODE"].ToString(),
                                insert = _insert.ToList()
                            };
                            List<DetailModel> _listModel = new List<DetailModel>();
                            _subModel.Add(_model);
                            _listModel.Add(_model);
                            if (i > 1 && _subModel.Count() > 1)
                            {
                                _listModel = _subModel;
                                result.Remove("sub");
                            }
                            result.Add("sub", _listModel);
                        }
                    }
                    else
                    {
                        JArray _inserta = JArray.Parse(jsonData["AttachmentDetails"].ToString());
                        DetailModel _model = new DetailModel()
                        {
                            TableId = dr["DM_GUID"].ToString(),
                            TableName = dr["MODELCODE"].ToString(),
                            insert = _inserta.ToList()
                        };
                        List<DetailModel> _listModel = new List<DetailModel>();
                        _listModel.Add(_model);
                        result.Add("attachsub", _listModel);
                    }
                }
            }
            else
            {
                throw new Exception("未获取到【" + processName + "】的流程配置相关信息");
            }


            return result;
        }

        private I_OSYS_PROCDATA_ITEMS Create_I_OSYS_PROCDATA_ITEMS(TaskDto task, ProcessDataDto json)
        {
            return new I_OSYS_PROCDATA_ITEMS()
            {
                GUID = json.Guid ?? Guid.NewGuid().ToString("N"),
                ProcessName = task.ProcessName,
                SysSource = task.SourceName,
                JsonData = json.ToJson(),
                Status = EnumTaskStatus.Submit,
                LaunchStatus = EnumLanchStatus.Success,
                ProcInstId = "0",
                CreateTime = task.CreateDate.HasValue ? task.CreateDate.Value.ToString() : DateTime.Now.ToString(),
                CreateUserID = task.CreateUserID,
                LastUpdateTime = task.CreateDate.HasValue ? task.CreateDate.Value.ToString() : DateTime.Now.ToString(),

            };
        }

        private I_OSYS_PROC_INSTS Create_I_OSYS_PROC_INSTS(TaskDto task, ProcessDataDto json)
        {
            return new I_OSYS_PROC_INSTS()
            {
                PROC_INST_ID = "0",
                STARTED_DATE = task.CreateDate.HasValue ? task.CreateDate.Value : DateTime.Now
            };
        }
        private I_OSYS_WF_WORKITEMS Create_I_OSYS_WF_WORKITEMS(TaskDto task, ProcessDataDto json)
        {
            return new I_OSYS_WF_WORKITEMS()
            {

            };
        }

        private async Task<I_OSYS_PROCDATA_ITEMS> Get_I_OSYS_PROCDATA_ITEMS(string guid)
        {
            return (await _i_OSYS_PROCDATA_ITEMSRepository.QueryAsync(" and guid=@guid", new { guid })).FirstOrDefault();
        }

        private async Task<I_OSYS_PROC_INSTS> Get_I_OSYS_PROC_INSTS(string insId)
        {
            return (await _i_OSYS_PROC_INSTSRepository.QueryAsync(" and PROC_INST_ID=@PROC_INST_ID", new { PROC_INST_ID = insId })).FirstOrDefault();
        }

        private async Task<I_OSYS_WF_WORKITEMS> Get_I_OSYS_WF_WORKITEMS(string insId)
        {
            return (await _i_OSYS_WF_WORKITEMSRepository.QueryAsync(" and PROC_INST_ID=@PROC_INST_ID", new { PROC_INST_ID = insId })).FirstOrDefault();
        }

        #endregion

        #region protected model

        protected class UserFinder
        {
            public string Pos_code { get; set; }
            public string AD { get; set; }
        }

        protected class DetailModel
        {

            public string TableId { get; set; }
            public string TableName { get; set; }
            public object insert { get; set; }
        }

        public class MainModel
        {
            public string TableId { get; set; }
            public string TableName { get; set; }
            public object Data { get; set; }
        }

        #endregion
    }
}
