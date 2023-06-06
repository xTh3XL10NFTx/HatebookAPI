namespace Hatebook.IRepository
{
    public interface IUnitOfWork : IDisposable  
    {

        IGenericRepository<DbIdentityExtention> Identity { get; }
        Task Save();

    }
}
