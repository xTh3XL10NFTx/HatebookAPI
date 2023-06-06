namespace Hatebook.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<DbIdentityExtention> Identity { get; }
        IGenericRepository<GroupsModel> Groups { get; }
        IGenericRepository<UsersInGroups> UsersInGroups { get; }
        IGenericRepository<GroupAdmins> Admins { get; }
        IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
        Task Save();
    }
}
