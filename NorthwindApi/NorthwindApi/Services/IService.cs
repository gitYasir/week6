using Microsoft.EntityFrameworkCore;

namespace NorthwindAPI.Services {
    public interface IService<T> where T : class {
        DbSet<T> GetAll();
        Task<T?> FindAsync( int id );
        Task AddAsync( T entity );
        Task AddRangeAsync( IEnumerable<T> entities );
        Task RemoveAsync( T entity );
        Task SaveChangesAsync();
    }
}