using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.DAL.Interfaces
{
    public interface IRepository<T>
    {
        long Add(T entity);

        T GetById(long Id);

        IList<T> GetAll();

        void Update(T entity);

        void Delete(T entity);

        IQueryable<T> GetDbSet();

        Task<long> AddAsync(T entity);

        ValueTask<T> GetByIdAsync(long Id);

        Task<IList<T>> GetAllAsync();

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);
    }
}
