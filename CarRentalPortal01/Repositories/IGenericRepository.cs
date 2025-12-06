using System.Linq.Expressions;

namespace CarRentalPortal01.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        T? GetById(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
        IEnumerable<T> GetAll(params string[] includes);

        IEnumerable<T> Find(Expression<Func<T, bool>> expression);

        void Add(T entity);
        void AddRange(IEnumerable<T> entities);

        void Update(T entity);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        void Save();
    }
}