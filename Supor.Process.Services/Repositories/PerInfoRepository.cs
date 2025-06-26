using Dapper;
using KFLibrary.Log;
using Supor.Process.Entity.Entity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Supor.Process.Services.Repositories
{
    public class PerInfoRepository: BaseSuporRepository<HandlerPerInfo>, IPerInfoRepository
    {
        public List<HandlerPerInfo> GetPerInfoByAD(string[] perADs, string sourceName)
        {
            List<HandlerPerInfo> HandlerPerInfos = null;
            string sql = @"select Per_AD,Per_Name,Per_Code,Per_Email,Org_Path,Pos_OrgCode,Pos_OrgName,Pos_DeptCode,Pos_DeptName,Pos_ResCenterCode,Pos_ResCenterName,Pos_BUCode,Pos_BUName,Org_Property 
                               from dbo.V_ORG_PositionPersonOrg where Per_Status=1 and PP_Proportion='100' and Per_AD in @perADs";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    HandlerPerInfos = conn.Query<HandlerPerInfo>(sql, new { perADs = perADs }).ToList();
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("GetPerInfoByAD", "Source=" + sourceName + "，查询人员信息错误: " + ex.ToString());
                }
            }
            return HandlerPerInfos;
        }

        public List<ApplicantInfo> GetUserInfos(string userName, string userNo, string userAd, string posCode, string deptCode, string orgCode, string buCode, string sourceName)
        {
            List<ApplicantInfo> UserInfos = null;
            string sql = @"select top 1 Pos_Code AS Sys_PosCode
  ,Pos_FullName as SYSTEM_APPLYJOBNAME
  ,Pos_ResCenterCode as Sys_ResCenterCode
  ,Pos_ResCenterName as Sys_ResCenterName
  ,Pos_BUCode as Sys_BuCode
  ,Per_Name as SYSTEM_APPLYUSERNAME
  ,Per_Email as SYSTEM_APPLYEMAIL
  ,Per_Code as Sys_UserCode
  ,Per_AD as SYSTEM_APPLYUSERID
  ,Per_Mobile as SYSTEM_APPLYPHONE
  ,Org_Code as SYSTEM_APPLYDEPARTMENTID
  ,Org_FullName as SYSTEM_APPLYDEPARTMENT
  ,Org_Path as SYSTEM_APPLYDEPTLINE
  ,Per_CompanyCode as Sys_CompanyCode
  ,Per_CompanyName as SYSTEM_APPLYCOMPANY
  ,Pos_DeptCode as Sys_DeptCode
  ,Pos_DeptName as Sys_DeptName
  ,Pos_OrgCode as Sys_OrgCode
  ,Pos_OrgName as Sys_OrgName
  ,Per_CostCode as Sys_CostCode
  ,Per_CostName as Sys_CostName
  ,'' as SYSTEM_APPLYUSER_SUPERID
  ,'' as SYSTEM_APPLYUSER_MANAGERID
  ,Org_Property
  from dbo.V_ORG_PositionPersonOrg";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, orgCode = orgCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, deptCode = deptCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_OrgCode=@orgCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_Code=@posCode and Pos_BUCode=@buCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_OrgCode=@orgCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_DeptCode=@deptCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and PP_Proportion=100 and Pos_BUCode=@buCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_DeptCode=@deptCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, orgCode = orgCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, deptCode = deptCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_OrgCode=@orgCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_Code=@posCode and Pos_BUCode=@buCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_OrgCode=@orgCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_DeptCode=@deptCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and PP_Proportion=100 and Pos_BUCode=@buCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_DeptCode=@deptCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, orgCode = orgCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, deptCode = deptCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_OrgCode=@orgCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_Code=@posCode and Pos_BUCode=@buCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_OrgCode=@orgCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_DeptCode=@deptCode and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and PP_Proportion=100 and Pos_BUCode=@buCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_DeptCode=@deptCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_OrgCode=@orgCode and Per_Status='1' order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_Code=@posCode and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and PP_Proportion=100 and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and PP_Proportion=100 and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and PP_Proportion=100 and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userAd))
                    {
                        sql += @" where Per_AD=@userAd and PP_Proportion=100 and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userAd = userAd }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo))
                    {
                        sql += @" where Per_Code=@userNo and PP_Proportion=100 and Per_Status='1'";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("GetUserInfos", "Source=" + sourceName + "，sql=" + sql + "，查询人员信息错误: " + ex.ToString());
                }
            }
            return UserInfos;
        }

        public List<ApplicantInfo> GetUserInfosIncludedDimission(string userName, string userNo, string userAd, string posCode, string deptCode, string orgCode, string buCode, string sourceName)
        {
            List<ApplicantInfo> UserInfos = null;
            string sql = @"select top 1 Pos_Code AS Sys_PosCode
  ,Pos_FullName as SYSTEM_APPLYJOBNAME
  ,Pos_ResCenterCode as Sys_ResCenterCode
  ,Pos_ResCenterName as Sys_ResCenterName
  ,Pos_BUCode as Sys_BuCode
  ,Per_Name as SYSTEM_APPLYUSERNAME
  ,Per_Email as SYSTEM_APPLYEMAIL
  ,Per_Code as Sys_UserCode
  ,Per_AD as SYSTEM_APPLYUSERID
  ,Per_Mobile as SYSTEM_APPLYPHONE
  ,Org_Code as SYSTEM_APPLYDEPARTMENTID
  ,Org_FullName as SYSTEM_APPLYDEPARTMENT
  ,Org_Path as SYSTEM_APPLYDEPTLINE
  ,Per_CompanyCode as Sys_CompanyCode
  ,Per_CompanyName as SYSTEM_APPLYCOMPANY
  ,Pos_DeptCode as Sys_DeptCode
  ,Pos_DeptName as Sys_DeptName
  ,Pos_OrgCode as Sys_OrgCode
  ,Pos_OrgName as Sys_OrgName
  ,Per_CostCode as Sys_CostCode
  ,Per_CostName as Sys_CostName
  ,'' as SYSTEM_APPLYUSER_SUPERID
  ,'' as SYSTEM_APPLYUSER_MANAGERID
  ,Org_Property
  from dbo.V_ORG_PositionPersonOrg";

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, orgCode = orgCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, deptCode = deptCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_OrgCode=@orgCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_Code=@posCode and Pos_BUCode=@buCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_OrgCode=@orgCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_DeptCode=@deptCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and PP_Proportion=100 and Pos_BUCode=@buCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_DeptCode=@deptCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd, posCode = posCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, orgCode = orgCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, deptCode = deptCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_OrgCode=@orgCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_Code=@posCode and Pos_BUCode=@buCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_OrgCode=@orgCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_DeptCode=@deptCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and PP_Proportion=100 and Pos_BUCode=@buCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_DeptCode=@deptCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd) && !string.IsNullOrEmpty(posCode))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and PP_Proportion=100 and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd, posCode = posCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, orgCode = orgCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, deptCode = deptCode, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_OrgCode=@orgCode and Pos_DeptCode=@deptCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_Code=@posCode and Pos_BUCode=@buCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_OrgCode=@orgCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_DeptCode=@deptCode and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_DeptCode=@deptCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, deptCode = deptCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_BUCode=@buCode and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, orgCode = orgCode, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(deptCode) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_DeptCode=@deptCode and Pos_OrgCode=@orgCode order by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, deptCode = deptCode, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(buCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and PP_Proportion=100 and Pos_BUCode=@buCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, buCode = buCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(deptCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_DeptCode=@deptCode by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, deptCode = deptCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(orgCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_OrgCode=@orgCode by PP_RelationTime asc";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, orgCode = orgCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(posCode))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and Pos_Code=@posCode";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo, posCode = posCode }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo) && !string.IsNullOrEmpty(userAd))
                    {
                        sql += @" where Per_Code=@userNo and Per_AD=@userAd and PP_Proportion=100";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo, userAd = userAd }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userAd))
                    {
                        sql += @" where Per_Name=@userName and Per_AD=@userAd and PP_Proportion=100";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userAd = userAd }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(userNo))
                    {
                        sql += @" where Per_Name=@userName and Per_Code=@userNo and PP_Proportion=100";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userName = userName, userNo = userNo }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userAd))
                    {
                        sql += @" where Per_AD=@userAd and PP_Proportion=100";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userAd = userAd }).ToList();
                    }
                    else if (!string.IsNullOrEmpty(userNo))
                    {
                        sql += @" where Per_Code=@userNo and PP_Proportion=100";
                        UserInfos = conn.Query<ApplicantInfo>(sql, new { userNo = userNo }).ToList();
                    }
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("GetUserInfos", "Source=" + sourceName + "，查询人员信息错误: " + ex.ToString());
                }
            }
            return UserInfos;
        }

        public List<HandlerPerInfo> GetPerInfo(string[] perCodes)
        {
            List<HandlerPerInfo> HandlerPerInfos = null;
            string sql = @"select Per_AD,Per_Name,Per_Code,Per_Email,Org_Path,Pos_OrgCode,Pos_OrgName,Pos_DeptCode,Pos_DeptName,Pos_ResCenterCode,Pos_ResCenterName,Pos_BUCode,Pos_BUName,Pos_FullName,Pos_Code,Org_Property,Org_ParentName,Org_ParentCode 
                               from [dbo].[V_ORG_PositionPersonOrg] where Per_Code in @perCodes and PP_Proportion='100' and Per_Status='1'";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    HandlerPerInfos = conn.Query<HandlerPerInfo>(sql, new { perCodes = perCodes }).ToList();
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("GetPerInfo", "Source=SES，查询人员信息错误: " + ex.ToString());
                }
            }
            return HandlerPerInfos;
        }

        public List<HandlerPerInfo> GetPerInfoIncludeSeparated(string[] perCodes)
        {
            List<HandlerPerInfo> HandlerPerInfos = null;
            string sql = @"select Per_AD,Per_Name,Per_Code,Per_Email,Org_Path,Pos_OrgCode,Pos_OrgName,Pos_DeptCode,Pos_DeptName,Pos_ResCenterCode,Pos_ResCenterName,Pos_BUCode,Pos_BUName
                               from [dbo].[V_ORG_PositionPersonOrg] where Per_Code in @perCodes and PP_Proportion='100'";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    HandlerPerInfos = conn.Query<HandlerPerInfo>(sql, new { perCodes = perCodes }).ToList();
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("GetPerInfoIncludeSeparated", "Source=SES，查询人员信息错误: " + ex.ToString());
                }
            }
            return HandlerPerInfos;
        }

        public List<HandlerPerInfo> GetMarketingAreasInfoByOrgFullName(string orgFullName, string appointedOrgCode, string sourceName)
        {
            List<HandlerPerInfo> HandlerPerInfos = null;
            string sql = @"select Org_Code as Pos_OrgCode,Org_FullName as Pos_OrgName,Org_Property from [dbo].[Org_Organization] 
                 where (Org_ParentCode=@appointedOrgCode or Org_ParentCode in (select Org_Code from [dbo].[Org_Organization] where Org_ParentCode=@appointedOrgCode)) 
                 and Org_FullName=@orgFullName";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    HandlerPerInfos = conn.Query<HandlerPerInfo>(sql, new { orgFullName = orgFullName, appointedOrgCode = appointedOrgCode }).ToList();
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("GetMarketingAreasInfoByOrgFullName", "Source=" + sourceName + "，查询人员信息错误: " + ex.ToString());
                }
            }
            return HandlerPerInfos;
        }

        //依据传入的岗位编码获取其直接上级人员信息
        public List<HandlerPerInfo> GetPosParentInfoByPosCode(string posCode, string sourceName)
        {
            List<HandlerPerInfo> HandlerPerInfos = null;
            string sql = @"select Per_AD,Pos_Code from BPM_Trans.dbo.V_ORG_PositionPersonOrg where Pos_Code=(select Pos_ParentCode from BPM_Trans.dbo.Org_Position where Pos_Code=@posCode) and Per_Status=1 order by PP_RelationTime";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["BPM_Trans"].ConnectionString))
            {
                try
                {
                    conn.Open();
                    HandlerPerInfos = conn.Query<HandlerPerInfo>(sql, new { posCode = posCode }).ToList();
                }
                catch (Exception ex)
                {
                    LoggorHelper.WriteLog("GetPosParentInfoByPosCode", "Source=" + sourceName + ",查询人员信息错误: " + ex.ToString());
                }
            }
            return HandlerPerInfos;
        }
    }
}