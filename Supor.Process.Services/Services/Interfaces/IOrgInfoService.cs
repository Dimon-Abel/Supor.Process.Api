using Supor.Process.Entity.Entity;
using System.Collections.Generic;

namespace Supor.Process.Services.Services
{
    public interface IOrgInfoService
    {
        List<IOrgInfo> GetOrgInfosByOrgCode(string orgCode, string sourceName);

        List<IOrgInfo> GetOrgInfosByOrgName(string orgName, string sourceName);
    }
}
