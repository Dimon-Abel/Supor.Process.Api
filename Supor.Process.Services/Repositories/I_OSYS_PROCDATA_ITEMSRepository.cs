using Supor.Process.Entity.Entity;
using Supor.Process.Services.Dapper;

namespace Supor.Process.Services.Repositories
{
    public class I_OSYS_PROCDATA_ITEMSRepository : BaseRepository<I_OSYS_PROCDATA_ITEMS>, II_OSYS_PROCDATA_ITEMSRepository
    {
        public I_OSYS_PROCDATA_ITEMSRepository(IDapperExecutor dapperExecutor): base(dapperExecutor)
        {
        }
    }
}
