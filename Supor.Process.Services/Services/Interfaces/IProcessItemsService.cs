using Supor.Process.Entity.Entity;

namespace Supor.Process.Services.Services
{
    public interface IProcessItemsService
    {
        IOSysProcItems GetInfobyJsonKeyAndProcessName(string jsonKey, string processName, string sourceName);

        bool InsertInfo(IOSysProcItems items, string sourceName);

        bool UpdateInfo(IOSysProcItems items, string sourceName);

        bool UpdateStatus(IOSysProcItems items, string sourceName);
    }
}
