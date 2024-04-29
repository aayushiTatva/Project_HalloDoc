using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly HalloDocContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(HalloDocContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        /*public virtual async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter, HalloDocContext _context)
            => await _context.FirstOrDefaultAsync(filter);

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter)
        {
            return GetFirstOrDefaultAsync(filter, _context);
        }*/

        /*public T GetFirstOrDefault(Expression<Func<T, bool>> filter, HalloDocContext _context)
        {
            T model = _context.FirstOrDefault(filter);
            return model;
        }*/
        public async Task AddAsync(T entity)
        {
            _context.Add(entity);
            await _context.SaveChangesAsync();
        }
        public T Add(T model)
        {
            _context.Add(model);
            _context.SaveChanges();

            return model;
        }
        public async Task UpdateAsync(T entity)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
        public T Update(T model)
        {
            _context.Update(model);
            _context.SaveChanges();

            return model;
        }
        public async Task RemoveAsync(T entity)
        {
            _context.Remove(entity);
            await _context.SaveChangesAsync();
        }
        public T Remove(T model)
        {
            _context.Remove(model);
            _context.SaveChanges();

            return model;
        }

        public Physician GetPhysicianById(int id)
        {
            return _context.Physicians.FirstOrDefault(e => e.Physicianid == id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }
    }
}