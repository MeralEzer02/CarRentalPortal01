using System.Linq.Expressions;
using CarRentalPortal01.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CarRentalPortal01.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly CarRentalDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(CarRentalDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();          
        }
        public T? GetById(int id)
        {
            return _dbSet.Find(id);
        }
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }
        public IEnumerable<T> Find(Expression<Func<T, bool>> expression)
        {
            return _dbSet.Where(expression);
        }
        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query.ToList();
        }
        public IEnumerable<T> GetAll(params string[] includes)
        {
            IQueryable<T> query = _dbSet;

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query.ToList();
        }
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }
        public void AddRange(IEnumerable<T> entities)
        {
            _dbSet.AddRange(entities);
        }
        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbSet.RemoveRange(entities);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
