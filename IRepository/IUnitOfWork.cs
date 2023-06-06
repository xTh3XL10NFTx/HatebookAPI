namespace Hatebook.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<DbIdentityExtention> Identity { get; }
        IGenericRepository<HatebookLogin> IdentityLogin { get; }
        Task Save();
    }
}
