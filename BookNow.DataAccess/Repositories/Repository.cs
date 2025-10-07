using BookNow.DataAccess.Data;
using BookNow.Models;
using BookNow.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookNow.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            this.dbSet = _db.Set<T>();
        }

        //public T Get(
        //    Expression<Func<T, bool>> filter,
        //    string? includeProperties = null,
        //    bool tracked = false)
        //{
        //    IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

        //    query = query.Where(filter);

        //    if (!string.IsNullOrEmpty(includeProperties))
        //    {
        //        foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        //        {
        //            query = query.Include(includeProp);
        //        }
        //    }

        //    return query.FirstOrDefault();
        //}
        public async Task<T?> GetAsync(
          Expression<Func<T, bool>> filter,
          string? includeProperties = null,
          bool tracked = false)
        {
            IQueryable<T> query = tracked ? dbSet : dbSet.AsNoTracking();

            query = query.Where(filter);

            // [Robustness: Clean LINQ Inclusion]
            query = ApplyIncludes(query, includeProperties);

            // [Performance] Execute query asynchronously
            return await query.FirstOrDefaultAsync();
        }

        public IEnumerable<T> GetAll(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }

            return query.ToList();
        }

        //public void Add(T entity)
        //{
        //    dbSet.Add(entity);
        //}
        public async Task AddAsync(T entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            dbSet.Update(entity);
        }

        public void Remove(T entity)
        {
            dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
        //public void AddRange(IEnumerable<T> entities)
        //{
        //    dbSet.AddRange(entities);
        //}
     
        // ------------------ NEW: Private Helper Method (C# Clean Code) ------------------
        // Helper method to consolidate the string splitting and Include logic
        // This method shows good SRP (Single Responsibility Principle)
        private IQueryable<T> ApplyIncludes(IQueryable<T> query, string? includeProperties)
        {
            if (!string.IsNullOrEmpty(includeProperties))
            {
                // [C# Showcase: String SplitOptions]
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp.Trim()); // Add Trim() for robustness
                }
            }
            return query;
        }

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;

            if (filter != null)
                query = query.Where(filter);

            // [Robustness: Clean LINQ Inclusion]
            query = ApplyIncludes(query, includeProperties);

            // Note: We leave this as AsNoTracking() is for Get/FirstOrDefault

            if (orderBy != null)
            {
                // [Performance] Execute ordered query asynchronously
                return await orderBy(query).ToListAsync();
            }

            // [Performance] Execute query asynchronously
            return await query.ToListAsync();
        }


        // Note on existing Sync methods:
        // You can now clean up the old 'Get' and 'GetAll' methods by calling ApplyIncludes(query, includeProperties);
        // This is a safe refactor. The rest of the original sync methods should remain unchanged for minimal risk.
    }
}

