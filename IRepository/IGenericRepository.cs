namespace Hatebook.IRepository
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetByIdAsync(int id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task Update(TEntity entity);
        Task Remove(TEntity entity);
    }
}
