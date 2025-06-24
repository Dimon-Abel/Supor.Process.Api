using Supor.Process.Entity.Entity;
using Supor.Process.Services.Dapper;

namespace Supor.Process.Services.Repositories
{
    public class TaskRepository : BaseRepository<ApiTaskEntity>
    {
        public TaskRepository(IDapperExecutor dapperExecutor): base(dapperExecutor)
        {
        }
    }
}
