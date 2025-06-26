using Supor.Process.Entity.Entity;
using Supor.Process.Services.Dapper;

namespace Supor.Process.Services.Repositories
{
    public class I_OSYS_PROC_INSTSRepository : BaseRepository<I_OSYS_PROC_INSTS>, II_OSYS_PROC_INSTSRepository
    {
        public I_OSYS_PROC_INSTSRepository(IDapperExecutor dapperExecutor) : base(dapperExecutor)
        {
        }
    }
}
