using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApi.DAL;
using WebApi.DAL.Entities;
using WebAPI.DAL.Interfaces;

namespace WebAPI.DAL.Repositories
{
    public class Repository<T> : IRepository<T> where T : Entity
    {
        private readonly StarWarsDbContext _dbContext;
        public Repository(StarWarsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public long Add(T entity)
        {
            var entityEntry = _dbContext.Add(entity);
            _dbContext.SaveChanges();
            return entityEntry.Entity.Id;
        }

        public void Delete(T entity)
        {
            _dbContext.Remove(entity);
            _dbContext.SaveChanges();
        }

        public IList<T> GetAll() => _dbContext.Set<T>().ToList();

        public T GetById(long Id) => _dbContext.Set<T>().Find(Id);

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }

        public IQueryable<T> GetDbSet() => _dbContext.Set<T>();


        #region Async methods

        public async Task<long> AddAsync(T entity)
        {
            var entityEntry = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entityEntry.Entity.Id;
        }

        public Task DeleteAsync(T entity)
        {
            _dbContext.Remove(entity);
            return _dbContext.SaveChangesAsync();
        }

        public async Task<IList<T>> GetAllAsync() => await _dbContext.Set<T>().ToListAsync();

        public ValueTask<T> GetByIdAsync(long Id) => _dbContext.Set<T>().FindAsync(Id);

        public async Task UpdateAsync(T entity)
        {
            var local = _dbContext.Set<T>()
                .Local.FirstOrDefault(entry => entry.Id.Equals(entity.Id));

            // check if local is not null 
            if (local != null)
            {
                // detach
                _dbContext.Entry(local).State = EntityState.Detached;
            }

            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        #endregion
    }
}
