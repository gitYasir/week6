using NorthwindBusiness;
using NorthwindData;
using NUnit.Framework;
using System.Linq;

namespace NorthwindTests {
    public class CustomerTests {
        CustomerManager _customerManager;
        [SetUp]
        public void Setup() {
            _customerManager = new CustomerManager();
            // remove test entry in DB if present
            using ( var db = new NorthwindContext() ) {
                var selectedCustomers =
                from c in db.Customers
                where c.CustomerId == "YI"
                select c;

                db.Customers.RemoveRange( selectedCustomers );
                db.SaveChanges();
            }
        }

        [Test]
        public void WhenANewCustomerCreated_Create_ReturnsIncreaseTheNumberOfCustemersInTheCustomersTableBy1() {
            using ( var db = new NorthwindContext() ) {
                var numberOfCustomersBefore = db.Customers.Count();
                _customerManager.Create( "YI", "Yasir Ibrahim", "Sparta Global" );
                var numberOfCustomersAfter = db.Customers.Count();

                Assert.That( numberOfCustomersBefore + 1, Is.EqualTo( numberOfCustomersAfter ) );
            }
        }

        [Test]
        public void WhenANewCustomerIsCreated_TheirDetailsInTheCustomersTableShouldBeCorrect() {
            using ( var db = new NorthwindContext() ) {
                _customerManager.Create( "YI", "Yasir Ibrahim", "Sparta Global" );
                var selectedCustomer = db.Customers.Find( "YI" );
                Assert.That( selectedCustomer.ContactName, Is.EqualTo( "Yasir Ibrahim" ) );
                Assert.That( selectedCustomer.CompanyName, Is.EqualTo( "Sparta Global" ) );
            }
        }

        [Test]
        public void WhenACustomersDetailsAreChanged_Update_ReturnsTheDatabaseShouldBeUpdated() {
            using ( var db = new NorthwindContext() ) {
                _customerManager.Create( "YI", "Yasir Ibrahim", "Sparta Global" );
                _customerManager.Update( "YI", "Yasir Ibrahim", "UK", "Birmingham", "B23" );
                var selectedCustomers = db.Customers.Find( "YI" );
                Assert.That( selectedCustomers.Country, Is.EqualTo( "UK" ) );
                Assert.That( selectedCustomers.City, Is.EqualTo( "Birmingham" ) );
            }
        }

        [Test]
        public void WhenACustomerIsUpdated_Update_ReturnsSelectedCustomerIsUpdated() {
            using ( var db = new NorthwindContext() ) {
                _customerManager.Create( "YI", "Yasir Ibrahim", "Sparta Global" );
                _customerManager.Update( "YI", "Yasir Ibrahim", "UK", "Birmingham", "B23" );
                var findCustomer = _customerManager.SelectedCustomer;
                Assert.That( findCustomer.Country, Is.EqualTo( "UK" ) );
            }
        }

        [Test]
        public void WhenACustomerIsNotInTheDatabase_Update_ReturnsFalse() {
            using ( var db = new NorthwindContext() ) {
                var findCustomer = _customerManager.Update( "YI", "Yasir Ibrahim", "UK", "Birmingham", "B23" );
                Assert.That( findCustomer, Is.EqualTo( false ) );
            }
        }

        [Test]
        public void WhenACustomerIsRemoved_Delete_ReturnsTheNumberOfCustomersDecreasesBy1() {
            using ( var db = new NorthwindContext() ) {
                _customerManager.Create( "YI", "Yasir Ibrahim", "Sparta Global" );
                var numberOfCustomerseBefore = db.Customers.Count();
                _customerManager.Delete( "YI" );
                var numberOfCustomersAfter = db.Customers.Count();
                Assert.That( numberOfCustomerseBefore - 1, Is.EqualTo( numberOfCustomersAfter ) );
            }
        }

        [Test]
        public void WhenACustomerIsRemoved_Delete_ReturnsTheyAreNoLongerInTheDatabase() {
            using ( var db = new NorthwindContext() ) {
                _customerManager.Create( "YI", "Yasir Ibrahim", "Sparta Global" );
                _customerManager.Delete( "YI" );
                var findCustomer = db.Customers.Find( "YI" );
                Assert.That( findCustomer, Is.EqualTo( null ) );
            }
        }

        [TearDown]
        public void TearDown() {
            using ( var db = new NorthwindContext() ) {
                var selectedCustomers =
                from c in db.Customers
                where c.CustomerId == "YI"
                select c;

                db.Customers.RemoveRange( selectedCustomers );
                db.SaveChanges();
            }
        }
    }
}