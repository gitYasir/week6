using Microsoft.EntityFrameworkCore;
using NorthwindApi.Models;



namespace NorthwindAPI.Services {
    public class SupplierService : IService<Supplier> {
        private NorthwindContext _context;

        public SupplierService( NorthwindContext context ) {
            _context = context;
        }

        public async Task AddAsync( Supplier entity ) {
            _context.Suppliers.Add( entity );
            await SaveChangesAsync();
        }

        public async Task AddRangeAsync( IEnumerable<Supplier> entities ) {
            _context.Suppliers.AddRange( entities );
            await SaveChangesAsync();
        }

        public async Task<Supplier?> FindAsync( int id ) {
            return await _context.Suppliers.FindAsync( id );
        }

        public DbSet<Supplier> GetAll() {
            return _context.Suppliers;
        }

        public async Task RemoveAsync( Supplier entity ) {
            _context.Suppliers.Remove( entity );
            await SaveChangesAsync();
        }

        public async Task SaveChangesAsync() {
            await _context.SaveChangesAsync();
        }
    }
}