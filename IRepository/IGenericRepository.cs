using System.Linq.Expressions;

namespace Hatebook.IRepository
{
    public interface IGenericRepository<T> where T : class
    {

        Task<IList<T>> GetAll(
            Expression<Func<T,bool>> expression = null,
            Func<IQueryable<T>,IOrderedQueryable<T>> orderby = null,
            List<string> include = null
            );

        Task<T> Get(Expression<Func<T, bool>> expression, List<string> include = null);
        Task<T> Insert(T entity);
        Task<T> InsertRange(IEnumerable<T> entities);
        Task<T> Delete(int id);
        void  DeleteRange(IEnumerable<T> entities);
        void Update(T entity);


        //Task<T> UpdateRange(IEnumerable<T> entities);

    }
}
