using HalloDocMVC.DBEntity.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HalloDocMVC.Repositories.Admin.Repository.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        public IQueryable<T> GetAll();
        /*Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> filter);

        public T GetFirstOrDefault(Expression<Func<T, bool>> filter);*/
        Task AddAsync(T entity);
        public T Add(T model);
        Task UpdateAsync(T entity);
        public T Update(T entity);
        Task RemoveAsync(T entity);
        public T Remove(T entity);
        public Physician GetPhysicianById(int id);

    }
}
