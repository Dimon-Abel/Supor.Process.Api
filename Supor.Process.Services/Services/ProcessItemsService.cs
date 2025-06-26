using Supor.Process.Entity.Entity;
using Supor.Process.Services.Repositories;

namespace Supor.Process.Services.Services
{
    public class ProcessItemsService : IProcessItemsService
    {
        public IProcessItemsRepository _processItemsRepository;
        public ProcessItemsService(IProcessItemsRepository processItemsRepository)
        {
            _processItemsRepository = processItemsRepository;
        }

        public IOSysProcItems GetInfobyJsonKeyAndProcessName(string jsonKey, string processName, string sourceName)
        {
            return _processItemsRepository.GetInfobyJsonKeyAndProcessName(jsonKey, processName, sourceName);
        }

        public bool InsertInfo(IOSysProcItems item, string sourceName)
        {
            return _processItemsRepository.InsertInfo(item, sourceName);
        }

        public bool UpdateInfo(IOSysProcItems item, string sourceName)
        {
            return _processItemsRepository.UpdateInfo(item, sourceName);
        }

        public bool UpdateStatus(IOSysProcItems item, string sourceName)
        {
            return _processItemsRepository.UpdateStatus(item, sourceName);
        }
    }
}