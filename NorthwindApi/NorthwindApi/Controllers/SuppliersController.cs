using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorthwindApi.Models;
using NorthwindAPI.Models.DTO;
using NorthwindAPI.Services;

namespace NorthwindApi.Controllers {
    [Route( "api/[controller]" )]
    [ApiController]
    public class SuppliersController : ControllerBase {
        private readonly IService<Supplier> _suppliers;
        private readonly IService<Product> _products;
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController( IService<Supplier> suppliers, IService<Product> products, ILogger<SuppliersController> logger ) {
            _suppliers = suppliers;
            _products = products;
            _logger = logger;
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SupplierDTO>>> GetSuppliers() {
            var suppliers = await _suppliers.GetAll()
                .Include( s => s.Products )
                .Select( x => Utils.SupplierToDTO( x ) )
                .ToListAsync();
            return suppliers;
        }

        // GET: api/Suppliers/5
        [HttpGet( "{id}" )]
        public async Task<ActionResult<SupplierDTO>> GetSupplier( int id ) {
            var supplier = await _suppliers.GetAll()
               .Include( s => s.Products )
               .Where( w => w.SupplierId == id )
               .Select( s => Utils.SupplierToDTO( s ) )
               .FirstOrDefaultAsync();

            if ( supplier == null ) {
                _logger.LogWarning( $"Supplier with id:{id} not found" );
                return NotFound();
            }
            _logger.LogInformation( $"Supplier with id:{id} found" );
            return supplier;
        }

        [HttpGet( "{id}/products" )]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetSupplierWithProducts( int id ) {
            if ( !SupplierExists( id ) ) {
                return NotFound();
            }

            return await _products.GetAll()
                .Where( p => p.SupplierId == id )
                .Select( p => Utils.ProductToDTO( p ) )
                .ToListAsync();

        }

        // PUT: api/Suppliers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut( "{id}" )]
        public async Task<IActionResult> PutSupplier( int id, SupplierDTO supplierDto ) {
            //The id in the URI has to match the URI in the JSON request body we send

            if ( id != supplierDto.SupplierId ) {
                return BadRequest();
            }

            Supplier supplier = await _suppliers.FindAsync( id );

            //Null-coalescing oeprator returns the value of it's left hand 
            //operand if it isn't null.
            supplier.CompanyName = supplierDto.CompanyName ?? supplier.CompanyName;
            supplier.ContactName = supplierDto.ContactName ?? supplier.ContactName;
            supplier.ContactTitle = supplierDto.ContactTitle ?? supplier.ContactTitle;
            supplier.Country = supplierDto.Country ?? supplier.Country;

            try {
                await _suppliers.SaveChangesAsync();
            }
            catch ( DbUpdateConcurrencyException ) {
                if ( !SupplierExists( id ) ) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Suppliers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<SupplierDTO>> PostSupplier( SupplierDTO supplierDto ) {
            List<Product> products = new List<Product>();

            supplierDto.Products
                .ToList()
                .ForEach( x => products.Add( new Product() {
                    ProductName = x.ProductName,
                    UnitPrice = x.UnitPrice
                } ) );
            await _products.AddRangeAsync( products );
            await _suppliers.SaveChangesAsync();

            Supplier supplier = new Supplier {
                SupplierId = supplierDto.SupplierId,
                CompanyName = supplierDto.CompanyName,
                ContactName = supplierDto.ContactName,
                ContactTitle = supplierDto.ContactTitle,
                Country = supplierDto.Country,
                Products = products
            };
            await _suppliers.AddAsync( supplier );
            await _suppliers.SaveChangesAsync();

            // Update IDs DTO
            supplierDto = await _suppliers.GetAll()
                .Where( s => s.SupplierId == supplier.SupplierId )
                .Include( x => x.Products )
                .Select( x => Utils.SupplierToDTO( x ) )
                .FirstAsync();

            return CreatedAtAction(
                nameof( GetSupplier ),
                new { id = supplier.SupplierId },
                supplierDto );
        }

        // DELETE: api/Suppliers/5
        [HttpDelete( "{id}" )]
        public async Task<IActionResult> DeleteSupplier( int id ) {
            var supplier = await _suppliers.FindAsync( id );
            if ( supplier == null ) {
                return NotFound();
            }
            // Severing the relationship
            foreach ( var prod in _products.GetAll().Where( p => p.SupplierId == id ) ) {
                prod.Supplier = null;
            }
            _suppliers.RemoveAsync( supplier );
            await _suppliers.SaveChangesAsync();

            return NoContent();
        }

        private bool SupplierExists( int id ) {
            return _suppliers.FindAsync( id ) != null;
        }
    }
}
