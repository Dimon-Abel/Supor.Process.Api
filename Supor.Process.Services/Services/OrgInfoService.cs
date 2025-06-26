using Supor.Process.Entity.Entity;
using Supor.Process.Services.Repositories;
using System.Collections.Generic;

namespace Supor.Process.Services.Services
{
    public class OrgInfoService: IOrgInfoService
    {
        public IOrgInfoRepository _orgInfoRepository;
        public OrgInfoService(IOrgInfoRepository orgInfoRepository)
        {
            _orgInfoRepository = orgInfoRepository;
        }

        public List<IOrgInfo> GetOrgInfosByOrgCode(string orgCode, string sourceName)
        {
            return _orgInfoRepository.GetOrgInfosByOrgCode(orgCode, sourceName);
        }

        public List<IOrgInfo> GetOrgInfosByOrgName(string orgName, string sourceName)
        {
            return _orgInfoRepository.GetOrgInfosByOrgName(orgName, sourceName);
        }
    }
}