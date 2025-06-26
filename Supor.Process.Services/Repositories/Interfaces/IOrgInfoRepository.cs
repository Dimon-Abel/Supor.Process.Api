using Supor.Process.Entity.Entity;
using System.Collections.Generic;

namespace Supor.Process.Services.Repositories
{
    public interface IOrgInfoRepository : IRepositoryBase<IOrgInfo>
    {
        List<IOrgInfo> GetOrgInfosByOrgCode(string orgCode, string sourceName);

        List<IOrgInfo> GetOrgInfosByOrgName(string orgName, string sourceName);
    }
}
