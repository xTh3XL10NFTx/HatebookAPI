using Hatebook.IRepository;
using System.Diagnostics.Metrics;

namespace Hatebook.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenericRepository<DbIdentityExtention> _Identity;
        private IGenericRepository<GroupsModel> _Groups;
        private IGenericRepository<UsersInGroups> _UsersInGroups;
        private IGenericRepository<GroupAdmins> _Admins;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public IGenericRepository<DbIdentityExtention> Identity => _Identity ??= new GenericRepository<DbIdentityExtention>(_context);
        public IGenericRepository<GroupsModel> Groups => _Groups ??= new GenericRepository<GroupsModel>(_context);
        public IGenericRepository<UsersInGroups> UsersInGroups => _UsersInGroups ??= new GenericRepository<UsersInGroups>(_context);
        public IGenericRepository<GroupAdmins> Admins => _Admins ??= new GenericRepository<GroupAdmins>(_context);
        public IGenericRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            // Create and return a repository instance based on the entity type
            return new GenericRepository<TEntity>(_context);
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }

        public async Task Save()
        {
            await _context.SaveChangesAsync();
        }
    }
}
