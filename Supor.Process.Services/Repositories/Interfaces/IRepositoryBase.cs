namespace Supor.Process.Services.Repositories
{
    public interface IRepositoryBase<T>
    {
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        int Insert(T entity, string sql);
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="sql">更新sql</param>
        /// <returns></returns>
        int Update(T entity, string sql);
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="Id">id</param>
        /// <param name="sql">查询sql</param>
        /// <returns></returns>
        T Query(int Id, string sql);
    }
}
