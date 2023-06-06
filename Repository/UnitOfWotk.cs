using Hatebook.IRepository;

namespace Hatebook.Repository
{
    public class UnitOfWotk : IUnitOfWork
    {

        private readonly ApplicationDbContext _context;
        private IGenericRepository<DbIdentityExtention> _identity;

        public UnitOfWotk(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<DbIdentityExtention> Identity => _identity ??= new GenericRepository<DbIdentityExtention>(_context);

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
