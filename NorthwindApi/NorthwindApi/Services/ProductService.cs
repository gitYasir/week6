using Microsoft.EntityFrameworkCore;
using NorthwindApi.Models;



namespace NorthwindAPI.Services {
    public class ProductService : IService<Product> {
        private readonly NorthwindContext _context;
        public ProductService( NorthwindContext context ) {
            _context = context;
        }

        public async Task AddAsync( Product entity ) {
            _context.Products.Add( entity );
            await SaveChangesAsync();
        }

        public async Task AddRangeAsync( IEnumerable<Product> entities ) {
            _context.Products.AddRange( entities );
            await SaveChangesAsync();
        }

        public async Task<Product?> FindAsync( int id ) {
            return await _context.Products.FindAsync( id );
        }

        public DbSet<Product> GetAll() {
            return _context.Products;
        }
        public async Task RemoveAsync( Product entity ) {
            _context.Products.Remove( entity );
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync() {
            await _context.SaveChangesAsync();
        }
    }
}