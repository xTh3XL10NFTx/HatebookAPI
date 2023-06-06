using Hatebook.IRepository;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Hatebook.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _db;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _db = _context.Set<T>();    

        }

        public async Task Delete(int id)
        {
            var entity = await _db.FindAsync(id);
            _db.Remove(entity); 
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _db.RemoveRange(entities);
        }

        public async Task<T> Get(Expression<Func<T, bool>> expression, List<string> include = null)
        {
            IQueryable<T> query = _db;
            if (include != null)
            {
                foreach (var includeProperty in include)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.AsNoTracking().FirstOrDefaultAsync(expression);
        }

        public async Task<IList<T>> GetAll(Expression<Func<T, bool>> expression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderby = null, List<string> include = null)
        {
            IQueryable<T> query = _db;

            if (expression != null)
            {
                query = query.Where(expression);
            }

            if (include != null)
            {
                foreach (var includeProperty in include)
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderby != null)
            {
                query = orderby(query);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task Insert(T entity)
        {
            await _db.AddAsync(entity);
        }

        public async Task InsertRange(IEnumerable<T> entities)
        {
            await _db.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _db.Attach(entity); 
            _context.Entry(entity).State = EntityState.Modified;
        }


        //
        //
        //
        Task<T> IGenericRepository<T>.Delete(int id)
        {
            throw new NotImplementedException();
        }

        Task<T> IGenericRepository<T>.Insert(T entity)
        {
            throw new NotImplementedException();
        }

        Task<T> IGenericRepository<T>.InsertRange(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }
    }
}
