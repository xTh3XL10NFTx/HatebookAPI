using Hatebook.IRepository;
using System.Diagnostics.Metrics;

namespace Hatebook.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IGenericRepository<DbIdentityExtention> _Identity;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }
        public IGenericRepository<DbIdentityExtention> Identity => _Identity ??= new GenericRepository<DbIdentityExtention>(_context);
       
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
