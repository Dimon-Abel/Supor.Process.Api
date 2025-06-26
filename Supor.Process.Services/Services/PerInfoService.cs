using Supor.Process.Entity.Entity;
using Supor.Process.Services.Repositories;
using System.Collections.Generic;

namespace Supor.Process.Services.Services
{
    public class PerInfoService: IPerInfoService
    {
        public IPerInfoRepository _perInfoRepository;
        public PerInfoService(IPerInfoRepository perInfoRepository)
        {
            _perInfoRepository = perInfoRepository;
        }

        public List<HandlerPerInfo> GetPerInfoByAD(string[] perADs, string sourceName)
        {
            return _perInfoRepository.GetPerInfoByAD(perADs, sourceName);
        }

        public List<ApplicantInfo> GetUserInfos(string userName, string userNo, string userAd, string posCode, string deptCode, string orgCode, string buCode, string sourceName)
        {
            return _perInfoRepository.GetUserInfos(userName, userNo, userAd, posCode, deptCode, orgCode, buCode, sourceName);
        }

        public List<ApplicantInfo> GetUserInfosIncludedDimission(string userName, string userNo, string userAd, string posCode, string deptCode, string orgCode, string buCode, string sourceName)
        {
            return _perInfoRepository.GetUserInfosIncludedDimission(userName, userNo, userAd, posCode, deptCode, orgCode, buCode, sourceName);
        }

        public List<HandlerPerInfo> GetPerInfo(string[] perCodes)
        {
            return _perInfoRepository.GetPerInfo(perCodes);
        }

        public List<HandlerPerInfo> GetPerInfoIncludeSeparated(string[] perCodes)
        {
            return _perInfoRepository.GetPerInfoIncludeSeparated(perCodes);
        }

        public List<HandlerPerInfo> GetMarketingAreasInfoByOrgFullName(string orgFullName, string appointedOrgCode, string sourceName)
        {
            return _perInfoRepository.GetMarketingAreasInfoByOrgFullName(orgFullName, appointedOrgCode, sourceName);
        }

        public List<HandlerPerInfo> GetPosParentInfoByPosCode(string posCode, string sourceName)
        {
            return _perInfoRepository.GetPosParentInfoByPosCode(posCode, sourceName);
        }
    }
}
