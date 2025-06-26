using Supor.Utility.Data;
using Supor.Utility.Log;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Supor.Process.Services.Processor
{
    public class BaseData
    {
        BaseDal dal = new BaseDal("BPM_Trans");
        BaseDal dalAP = new BaseDal("bpmDB");
        BaseDal dalConfig = new BaseDal("dbConnect");

        #region 公共查询类
        /// <summary>
        /// 根据实例号获取WorkItemId
        /// </summary>
        /// <param name="ProcInstID"></param>
        /// <returns></returns>
        public DataTable GetWorkItemId(string ProcInstID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select wmw.WORK_ITEM_ID,wmw.CREATED_DATE,wmw.PROC_DEF_ID,wai.NAME,wai.DISPLAY_NAME,wpi.STARTED_DATE");
            strSql.Append(" from WF_MANUAL_WORKITEMS wmw inner join WF_ACTIVITY_INSTS wai on wmw.ACTIVITY_INST_ID = wai.ID");
            strSql.Append(" inner join WF_PROC_INSTS wpi on wai.PROC_INST_ID = wpi.PROC_INST_ID");
            strSql.AppendFormat(" where wai.PROC_INST_ID ='{0}' ", ProcInstID);
            strSql.Append(" and wai.TOKEN_POS = 3  and wmw.STATUS in('Assigned','New','Overdue')");
            return dalAP.SelectCommand(strSql.ToString(), null);
        }

        #endregion

        #region 公共处理类
        public bool InsertSign(string process, string procInstID, string taskId, string stepname, string username, string userfullname, string usermail, string department, string WICreateDate, string remark, string status)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" INSERT INTO [SYS_SignInfo]");
            strSql.Append(" (guid,process,incident,stepname,username,userfullname,usermail,department");
            strSql.Append(" ,opinion,sign,signdate,remarks,others,UpdDate,Status)");
            strSql.Append("  values");
            strSql.Append(" (@guid,@process,@incident,@stepname,@username,@userfullname,@usermail,@department");
            strSql.Append(" ,@opinion,@sign,@signdate,@remarks,@others,@UpdDate,@Status)");

            Hashtable htList = new Hashtable();

            htList.Add("guid", Guid.NewGuid().ToString());
            htList.Add("process", process);
            htList.Add("incident", procInstID);
            htList.Add("stepname", stepname);
            htList.Add("username", username);
            htList.Add("userfullname", userfullname);
            htList.Add("usermail", usermail);
            htList.Add("department", department);
            if (status == "0" || status == "1")
            {
                htList.Add("opinion", "提交");
                htList.Add("sign", "提交");
            }
            else if (status == "2")
            {
                htList.Add("opinion", "终止");
                htList.Add("sign", "终止");
            }
            htList.Add("signdate", DateTime.Now.ToString());
            htList.Add("remarks", remark);
            htList.Add("others", taskId);
            htList.Add("UpdDate", WICreateDate);
            htList.Add("Status", "20");
            return dal.InsertCommand(strSql.ToString(), htList) > 0;
        }

        /// <summary>
        /// 事务保存主表数据
        /// </summary>
        /// <returns></returns>
        public int SaveBussinessMainData(string guid, object[] objMainDatas, string incident, string appno, string status, SqlTransaction trans)
        {
            int res = 0;
            foreach (object objMain in objMainDatas)
            {
                Dictionary<string, object> dicTable = (Dictionary<string, object>)objMain;
                if (dicTable != null)
                {
                    string strTableName = dicTable["TableName"].ToString();
                    try
                    {
                        object[] objDataSource = (object[])dicTable["Data"];
                        if (status == "0")
                        {
                            if (InsertData(strTableName, guid, objDataSource, incident, appno, trans) > 0) res++;
                            else throw new Exception("新增主表数据【" + strTableName + "】出错");
                        }
                        else
                        {
                            if (UpdateData(strTableName, guid, objDataSource, incident, appno, trans) > 0) res++;
                            else throw new Exception("更新主表数据【" + strTableName + "】出错");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("保存主表数据【" + strTableName + "】出错：" + ex.Message);
                        throw new Exception("保存主表数据【" + strTableName + "】出错：" + ex.Message);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 事务保存子表数据
        /// </summary>
        /// <returns></returns>
        public int SaveBussinessSubData(string guid, object[] objSubDatas, string status, SqlTransaction trans)
        {
            int res = 0;
            if (objSubDatas == null) return 0;

            foreach (object o in objSubDatas)
            {
                Dictionary<string, object> dicSub = o as Dictionary<string, object>; //tablename,records
                if (dicSub != null)
                {
                    string strTableName = dicSub["TableName"].ToString();
                    object[] objInsertRow = (object[])dicSub["insert"];  //records
                    try
                    {
                        if (status == "0")
                        {
                            if (objInsertRow != null && objInsertRow.Length > 0)
                            {
                                if (InsertData(strTableName, guid, objInsertRow, "-1", "", trans) > 0) res++;
                                else throw new Exception("事务新增明细数据【" + strTableName + "】出错");
                            }
                        }
                        else if (status == "1")
                        {
                            if (DeleteData(strTableName, guid, trans) > 0)
                            {
                                if (objInsertRow != null && objInsertRow.Length > 0)
                                {
                                    if (InsertData(strTableName, guid, objInsertRow, "-1", "", trans) > 0) res++;
                                    else throw new Exception("事务新增明细数据【" + strTableName + "】出错");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLog("事务保存明细数据【" + strTableName + "】出错：" + ex);
                        throw new Exception("事务保存明细数据【" + strTableName + "】出错：" + ex);
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 事务新增数据
        /// </summary>
        /// <returns></returns>
        private int InsertData(string strTableName, string guid, object[] objDatas, string procInstId, string appNo, SqlTransaction trans)
        {
            //获取表字段配置集合
            //List<VMProcessColumn> fields = !string.IsNullOrEmpty(tableId) ? ConfigManager.GetColumns(tableId) : new List<VMProcessColumn>();

            int res = 0;
            string strSQLInsert = "";
            foreach (object obj in objDatas)
            {
                StringBuilder sbInsertSQL = new StringBuilder("");
                StringBuilder sbColumns = new StringBuilder("");
                StringBuilder sbValues = new StringBuilder(""); //业务字段的值
                Hashtable paras = new Hashtable();  //参数化值 用于处理单个字段内容过长，导致写入不成功

                Dictionary<string, object> dicMain = obj as Dictionary<string, object>; //列名，值
                try
                {
                    if (dicMain != null)
                    {
                        foreach (string strKey in dicMain.Keys)
                        {
                            var v = dicMain[strKey] == null ? null : dicMain[strKey].ToString().Replace("'", "''");
                            switch (strKey.Trim().ToLower())
                            {
                                case "system_incident":
                                    sbColumns.AppendLine("," + strKey.ToString());
                                    sbValues.Append(string.Format(",'{0}'", procInstId));
                                    break;
                                case "system_applyno":
                                    sbColumns.AppendLine("," + strKey.ToString());
                                    sbValues.Append(string.Format(",'{0}'", appNo));
                                    break;
                                case "guid":
                                    if (procInstId != "-1")
                                    {
                                        sbColumns.AppendLine("," + strKey.ToString());
                                        sbValues.Append(string.Format(",'{0}'", guid));
                                    }
                                    break;
                                case "fk_guid":
                                    if (procInstId == "-1")
                                    {
                                        sbColumns.AppendLine("," + strKey.ToString());
                                        sbValues.Append(string.Format(",'{0}'", guid));
                                    }
                                    break;
                                default:
                                    sbColumns.AppendLine("," + strKey.ToString());
                                    if (v != null && v.Length >= 2000)
                                    {
                                        //长度大于2000的改用参数化写入
                                        sbValues.Append(",:" + strKey);
                                        paras.Add(strKey, dicMain[strKey].ToString());
                                    }
                                    else
                                    {
                                        sbValues.Append(",'" + v + "'");
                                    }
                                    break;
                            }
                        }
                        strSQLInsert = string.Format("Insert into {0}({1}) values ({2})", strTableName, sbColumns.ToString().TrimStart(','), sbValues.ToString().TrimStart(','));
                        //Logger.WriteLog("事务保存明细数据【" + strTableName + "】：" + strSQLInsert);
                        res = dal.InsertCommand(strSQLInsert, paras, trans);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("事务新增表单数据出错：SQL=>\r\n" + strSQLInsert + "\r\n--" + ex); ;
                }
            }
            return res;
        }

        /// <summary>
        /// 数据更新
        /// </summary>
        /// <param name="objDatas"></param>
        /// <returns></returns>
        private int UpdateData(string strTableName, string guid, object[] objDatas, string procInstId, string appNo, SqlTransaction trans)
        {
            //获取表字段配置集合
            //List<VMProcessColumn> fields = !string.IsNullOrEmpty(tableId) ? ConfigManager.GetColumns(tableId) : new List<VMProcessColumn>();

            int res = 0;
            foreach (object obj in objDatas)
            {
                string strSQLUpdate = "";
                StringBuilder sbValues = new StringBuilder(""); //业务字段的值
                Dictionary<string, object> dicMain = obj as Dictionary<string, object>; //列名，值
                Hashtable paras = new Hashtable();  //参数化值 用于处理单个字段内容过长，导致写入不成功
                try
                {
                    if (dicMain != null)
                    {
                        foreach (string strKey in dicMain.Keys)
                        {
                            var v = dicMain[strKey] == null ? null : dicMain[strKey].ToString().Replace("'", "''");
                            switch (strKey.Trim().ToLower())
                            {
                                case "guid":
                                    sbValues.Append(string.Format(",{0}='{1}'", strKey, guid));
                                    break;
                                default:
                                    if (v != null && v.Length >= 2000)
                                    {
                                        //长度大于2000的改用参数化写入
                                        sbValues.Append(string.Format(",{0}={1}", strKey, ":" + strKey));
                                        paras.Add(strKey, dicMain[strKey].ToString());
                                    }
                                    else
                                    {
                                        sbValues.Append(string.Format(",{0}='{1}'", strKey, v));
                                    }
                                    break;
                            }
                        }
                        if (!string.IsNullOrEmpty(sbValues.ToString().Trim()))
                        {
                            strSQLUpdate = string.Format("Update {0} Set {1} Where SYSTEM_INCIDENT='{2}'", strTableName, sbValues.ToString().TrimStart(','), procInstId);
                            res = dal.UpdateCommand(strSQLUpdate, paras, trans);
                        }
                    }
                }
                catch (Exception ex)
                {
                    //KFLibrary.Log.LoggorHelper.WriteLog("更新表单数据出错：SQL=>\r\n" + strSQLUpdate + "\r\n--" + ee.Message);
                    throw new Exception("事务更新表单数据出错：SQL=>\r\n" + strSQLUpdate + "\r\n--" + ex);
                }
            }

            return res;
        }

        /// <summary>
        /// 事务删除数据
        /// </summary>
        /// <returns></returns>
        protected int DeleteData(string strTableName, string guid, SqlTransaction trans)
        {
            int res = 0;
            string strSQL = "";
            try
            {
                strSQL = string.Format("select * from {0} where FK_Guid='{1}'", strTableName, guid);
                var scount = dal.SelectCommand(strSQL, null, trans).Rows.Count;
                KFLibrary.Log.LoggorHelper.WriteLog("DeleteData" + scount);
                if (scount > 0)
                {
                    strSQL = string.Format("Delete {0} where FK_Guid='{1}'", strTableName, guid);
                    var count = dal.UpdateCommand(strSQL, null, trans);
                    if (count > 0) res += count;
                    else throw new Exception("事务删除表单数据出错");
                }
                else
                    res = 1;
            }
            catch (Exception ex)
            {
                throw new Exception("事务删除表单数据出错：SQL=>\r\n" + strSQL + "\r\n--" + ex);
            }
            return res;
        }

        /// <summary>
        /// 流程实例表数据更新
        /// </summary>
        /// <returns></returns>
        public int SaveProcInstsInfo(string incident, SqlTransaction trans)
        {
            string strSql = @"Insert into BPM_Trans.[dbo].[SYS_WF_PROC_INSTS]
(PROC_INST_ID,PROC_APPLYNO,PROC_INST_NAME,DEF_ID,DEF_NAME,PROC_INST_STATUS,STARTED_DATE,COMPLETED_DATE
,PROC_INITIATOR_AD,PROC_INITIATOR_NAME,PROC_INITIATOR_PERCODE,PROC_INITIATOR_ORGPATH
,PROC_INITIATOR_ORGCODE,PROC_INITIATOR_ORGNAME,PROC_INITIATOR_DEPTCODE,PROC_INITIATOR_DEPTNAME
,PROC_INITIATOR_RESCENTERCODE,PROC_INITIATOR_RESCENTERNAME,PROC_INITIATOR_BUCODE,PROC_INITIATOR_BUNAME)
select wpi.PROC_INST_ID,right(wpi.PROC_INST_NAME,16) as PROC_APPLYNO,wpi.PROC_INST_NAME,wpi.DEF_ID
,wpi.DEF_NAME,wpi.STATUS as PROC_INST_STATUS,wpi.STARTED_DATE
,CASE wpi.STATUS WHEN 'Cancelled' THEN wpi.LAST_MODIFIED_DATE ELSE wpi.COMPLETED_DATE END AS COMPLETED_DATE
,wpi.PROC_INITIATOR as PROC_INITIATOR_AD,per.Per_Name as PROC_INITIATOR_NAME
,per.Per_Code as PROC_INITIATOR_PERCODE,per.Org_Path as PROC_INITIATOR_ORGPATH
,per.Pos_OrgCode as PROC_INITIATOR_ORGCODE,per.Pos_OrgName as PROC_INITIATOR_ORGNAME
,per.Pos_DeptCode as PROC_INITIATOR_DEPTCODE,per.Pos_DeptName as PROC_INITIATOR_DEPTNAME
,per.Pos_ResCenterCode as PROC_INITIATOR_RESCENTERCODE,per.Pos_ResCenterName as PROC_INITIATOR_RESCENTERNAME
,per.Pos_BUCode as PROC_INITIATOR_BUCODE,per.Pos_BUName as PROC_INITIATOR_BUNAME
from AP50.[dbo].[WF_PROC_INSTS] wpi with(nolock)
left join BPM_Trans.[dbo].[V_ORG_PositionPersonOrg] per with(nolock) on wpi.PROC_INITIATOR= per.Per_AD and per.PP_Proportion='100'
where wpi.PROC_INST_ID=@incident";
            Hashtable htList = new Hashtable();
            htList.Add("incident", incident);
            return dal.InsertCommand(strSql.ToString(), htList, trans);
        }

        /// <summary>
        /// 附件信息更新
        /// </summary>
        /// <returns></returns>
        public int InsertAttchment(string processName, object[] objDatas, string userId, string userName, string guid, string stepName, SqlTransaction trans)
        {
            int res = 0;
            foreach (object obj in objDatas)
            {
                Dictionary<string, object> dicSub = obj as Dictionary<string, object>; //列名，值

                if (dicSub != null)
                {
                    string strTableName = dicSub["TableName"].ToString();
                    object[] objInsertRow = (object[])dicSub["insert"]; //records
                    if (objInsertRow.Length>0)
                    {
                        string strSQL = string.Format("Delete {0} where FK_Guid='{1}'", strTableName, guid);
                        var count = dal.UpdateCommand(strSQL, null, trans);
                        if (count >= 0)
                        {
                            for (var j = 0; j < objInsertRow.Length; j++)
                            {
                                Dictionary<string, object> diciSub = objInsertRow[j] as Dictionary<string, object>; //列名，值

                                StringBuilder strSql = new StringBuilder();
                                strSql.Append("INSERT INTO PROC_ATTACHMENT (ATTACHMENT_FJ");
                                strSql.Append(" , UPLOADER, UPLOADERDATE");
                                strSql.Append(" , EXT03, EXT04, EXT05, EXT06, EXT07, EXT08");
                                strSql.Append(" , FK_GUID, SYSTEM_ROWNUM)");
                                strSql.Append(" SELECT '<a target=\"_blank\" href=\"'+@Path+'\">'+@Name+'</a>'");
                                strSql.Append(" , @UserName, @UploadDate");
                                strSql.Append(" , @UploadDate,@StepName,@Name,@Path,@processName,@UserId");
                                strSql.Append(" , @FK_GUID, @SYSTEM_ROWNUM");

                                Hashtable htList = new Hashtable();
                                htList.Add("processName", processName);
                                htList.Add("UserId", userId.Replace('\\', '/'));
                                htList.Add("UserName", userName);
                                htList.Add("UploadDate", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                htList.Add("FK_GUID", guid);
                                htList.Add("Path", diciSub["Path"].ToString());
                                htList.Add("Name", diciSub["Name"].ToString());
                                htList.Add("StepName", stepName);
                                htList.Add("SYSTEM_ROWNUM", diciSub["SYSTEM_ROWNUM"].ToString());

                                res += dal.InsertCommand(strSql.ToString(), htList, trans);
                            }
                        }
                    }                    
                }
            }
            return res;
        }

        /// <summary>
        /// 中间表更新
        /// </summary>
        /// <returns></returns>
        public int UpdateSubmitStatus(string guid, string launchStatus, string isSubmit, string status, string procInstId, string procApplyNo)
        {
            string strSql = string.Empty;
            if (status == "0")
                strSql = @"update I_OSYS_PROC_ITEMS set LaunchStatus=@LaunchStatus,IsSubmit=@IsSubmit,ProcInstId=@ProcInstId,ProcApplyNo=@procApplyNo,LastUpdateTime=GETDATE() where GUID=@GUID";
            else
                strSql = @"update I_OSYS_PROC_ITEMS set LaunchStatus=@LaunchStatus,IsSubmit=@IsSubmit,LastUpdateTime=GETDATE() where GUID=@GUID";
            Hashtable htList = new Hashtable();
            htList.Add("LaunchStatus", launchStatus);
            htList.Add("IsSubmit", isSubmit);
            htList.Add("GUID", guid);
            if (status == "0")
            {
                htList.Add("ProcInstId", procInstId);
                htList.Add("ProcApplyNo", procApplyNo);
            }
            return dal.UpdateCommand(strSql.ToString(), htList);
        }

        /// <summary>
        /// 删除MT_BUSINESS中间表
        /// </summary>
        /// <returns></returns>
        public int DeleteMtBusinessData(string guid)
        {
            int res = 0;
            string strSQL = "";
            try
            {
                strSQL = string.Format("select * from MT_BUSINESS where GUID='{0}'", guid);
                var scount = dal.SelectCommand(strSQL, null).Rows.Count;
                KFLibrary.Log.LoggorHelper.WriteLog("DeleteMtBusinessData" + scount);
                if (scount > 0)
                {
                    strSQL = string.Format("Delete MT_BUSINESS where GUID='{0}'", guid);
                    var count = dal.UpdateCommand(strSQL, null);
                    if (count > 0) res += count;
                    else throw new Exception("删除MT_BUSINESS中间表数据出错");
                }
                else
                    res = 1;
            }
            catch (Exception ex)
            {
                throw new Exception("删除MT_BUSINESS中间表数据出错：SQL=>\r\n" + strSQL + "\r\n--" + ex);
            }
            return res;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public enum DataModel
        {
            Insert, Update
        }
    }
}