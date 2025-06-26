namespace Supor.Process.Services.Repositories
{
    public abstract class BaseSuporRepository<T>
    {
        public BaseSuporRepository()
        {
            //conn = DbConnection.GetSqlConnection();
        }
        public int Insert(T entity, string sql)
        {
            return 0;
        }

        public T Query(int Id, string sql)
        {
            return default(T);
        }

        public int Update(T entity, string sql)
        {
            return 0;
        }
    }
}