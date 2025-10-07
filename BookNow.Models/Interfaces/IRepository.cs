using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.Models.Interfaces
{
   public interface IRepository<T> where T : class
    {
        //T Get(
        //    Expression<Func<T, bool>> filter,
        //    string? includeProperties = null,
        //    bool tracked = false);

        //IEnumerable<T> GetAll(
        //    Expression<Func<T, bool>>? filter = null,
        //    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        //    string? includeProperties = null);

        //void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);


        // ------------------ NEW: Asynchronous Methods (Focus on these) ------------------

        // [C# Showcase: Async/Task] Returns a Task that yields a single T (for query)
        Task<T?> GetAsync(
            Expression<Func<T, bool>> filter,
            string? includeProperties = null,
            bool tracked = false);

        // [C# Showcase: Async/Task] Returns a Task that yields a collection of T
        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null);

        // [Performance: Non-Blocking I/O]
        Task AddAsync(T entity);

        // [Performance: Non-Blocking I/O]
        Task AddRangeAsync(IEnumerable<T> entities);
    }
}
