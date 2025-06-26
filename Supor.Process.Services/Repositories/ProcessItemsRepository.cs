using Dapper;
using KFLibrary.Log;
using Supor.Process.Entity.Entity;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Supor.Process.Services.Repositories
{
    public class ProcessItemsRepository : BaseSuporRepository<IOSysProcItems>, IProcessItemsRepository
    {
        public IOSysProcItems GetInfobyJsonKeyAndProcessName(string jsonKey, string processName, string sourceName)
        {
            IOSysProcItems procitems = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                try
                {
                    string sql = @"Select * from I_OSYS_PROC_ITEMS where JsonKey = @JsonKey and ProcessName=@ProcessName";
                    conn.Open();
                    procitems = conn.QueryFirstOrDefault<IOSysProcItems>(sql, new { JsonKey = jsonKey, ProcessName = processName });
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("GetInfobyJsonKey", "Source=" + sourceName + "，JsonKey:" + jsonKey + "ProcessName:" + processName + ",错误:" + ex.ToString());

                }
            }
            return procitems;
        }

        public bool InsertInfo(IOSysProcItems item, string sourceName)
        {
            bool result = false;
            string sql = @"Insert into I_OSYS_PROC_ITEMS(GUID,SysSource,InterfaceName,JsonData,JsonKey,Status,LaunchStatus,ProcessName,ProcInstId,ProcApplyNo
                           ,ProcessData,IsSubmit,CreateUserID,CreateTime,LastUpdateTime)
                                values(@GUID,@SysSource,@InterfaceName,@JsonData,@JsonKey,@Status,@LaunchStatus,@ProcessName,@ProcInstId,@ProcApplyNo,@ProcessData
                                        ,@IsSubmit,@CreateUserID,@CreateTime,GETDATE())";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                conn.Open();
                IDbTransaction transaction = conn.BeginTransaction();
                try
                {
                    if (item != null)
                    {
                        result = conn.Execute(sql, item, transaction) > 0;
                    }

                    if (result)
                    {
                        transaction.Commit();//事务提交
                    }
                    else
                    {
                        transaction.Rollback();//事务回滚
                    }
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("InsertInfo", "Source=" + sourceName + "，插入中间表失败，错误: " + ex.ToString());
                    transaction.Rollback();//事务回滚
                    result = false;
                }
            }
            return result;
        }

        public bool UpdateInfo(IOSysProcItems item, string sourceName)
        {
            bool result = false;
            string sql = @"update I_OSYS_PROC_ITEMS set JsonData=@JsonData,Status=@Status,ProcessData=@ProcessData,IsSubmit=@IsSubmit
                        ,CreateUserID=@CreateUserID,LastUpdateTime=GETDATE() where ProcessName=@ProcessName and JsonKey=@JsonKey";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    result = conn.Execute(sql, new { item.JsonData, item.Status, item.ProcessData, item.IsSubmit, item.CreateUserID, item.ProcessName, item.JsonKey }) > 0;
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("UpdateInfo", "Source=" + sourceName + "，修改中间表失败，错误: " + ex.ToString());
                }
            }
            return result;
        }
        /// <summary>
        /// 根据Guid修改中间表状态
        /// </summary>
        /// <param name="status"></param>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool UpdateStatus(IOSysProcItems item, string sourceName)
        {
            bool result = false;
            string sql = @"update I_OSYS_PROC_ITEMS set JsonData=@JsonData,Status=@Status,IsSubmit=@IsSubmit
                        ,CreateUserID=@CreateUserID,LastUpdateTime=GETDATE() where ProcessName=@ProcessName and JsonKey=@JsonKey";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    result = conn.Execute(sql, new { item.JsonData, item.Status, item.IsSubmit, item.CreateUserID, item.ProcessName, item.JsonKey }) > 0;
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("UpdateInfo", "Source=" + sourceName + "，修改中间表失败，错误: " + ex.ToString());
                }
            }
            return result;
        }
    }
}