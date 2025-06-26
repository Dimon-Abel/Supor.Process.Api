using Supor.Process.Entity.Entity;
using System.Collections.Generic;

namespace Supor.Process.Services.Services
{
    public interface IPerInfoService
    {
        List<HandlerPerInfo> GetPerInfoByAD(string[] perADs, string sourceName);

        List<ApplicantInfo> GetUserInfos(string userName, string userNo, string userAd, string posCode, string deptCode, string orgCode, string buCode, string sourceName);

        List<ApplicantInfo> GetUserInfosIncludedDimission(string userName, string userNo, string userAd, string posCode, string deptCode, string orgCode, string buCode, string sourceName);

        List<HandlerPerInfo> GetPerInfo(string[] perCodes);

        List<HandlerPerInfo> GetPerInfoIncludeSeparated(string[] perCodes);

        List<HandlerPerInfo> GetMarketingAreasInfoByOrgFullName(string orgFullName, string appointedOrgCode, string sourceName);

        List<HandlerPerInfo> GetPosParentInfoByPosCode(string posCode, string sourceName);
    }
}
