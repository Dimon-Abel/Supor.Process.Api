using Supor.Process.Entity.Entity;
using Supor.Process.Services.Dapper;

namespace Supor.Process.Services.Repositories
{
    public class I_OSYS_WF_WORKITEMSRepository : BaseRepository<I_OSYS_WF_WORKITEMS>, II_OSYS_WF_WORKITEMSRepository
    {
        public I_OSYS_WF_WORKITEMSRepository(IDapperExecutor dapperExecutor) : base(dapperExecutor)
        {
        }
    }
}
