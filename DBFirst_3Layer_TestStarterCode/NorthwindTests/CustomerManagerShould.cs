using Microsoft.EntityFrameworkCore;
using Moq;
using NorthwindBusiness;
using NorthwindData;
using NorthwindData.Services;
using NUnit.Framework;

namespace NorthwindTests {
    public class CustomerManagerShould {
        private CustomerManager _sut;

        [Test]
        public void BeAbleToBeConstructedUsingMoq() {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            // Act
            _sut = new CustomerManager( mockCustomerService.Object );
            // Assert
            Assert.That( _sut, Is.InstanceOf<CustomerManager>() );
        }

        [Test]
        public void WhenCalledWithValidId_Update_ReturnsTrue() {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            var originalCustomer = new Customer {
                CustomerId = "ROCK"
            };
            mockCustomerService.Setup(
                cs => cs.GetCustomerById( "ROCK" ) )
                    .Returns( originalCustomer );

            _sut = new CustomerManager( mockCustomerService.Object );

            // Act
            var result = _sut.Update(
                "ROCK", It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>() );

            // Assert
            Assert.That( result, Is.True );
        }

        [Test]
        public void WhenCalledWithValidIdAndValues_Update_CorrectlyChangesValues() {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();
            var originalCustomer = new Customer() {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };
            mockCustomerService.Setup(
                cs => cs.GetCustomerById( "ROCK" ) )
                    .Returns( originalCustomer );
            _sut = new CustomerManager( mockCustomerService.Object );

            // Act
            _sut.Update( "ROCK", "Rocky Raccoon", "UK", "Chester", null );

            // Assert
            Assert.That(
                _sut.SelectedCustomer.ContactName,
                Is.EqualTo( "Rocky Raccoon" ) );
            Assert.That(
                _sut.SelectedCustomer.CompanyName,
                Is.EqualTo( "Zoo UK" ) );
            Assert.That(
                _sut.SelectedCustomer.Country,
                Is.EqualTo( "UK" ) );
            Assert.That(
                _sut.SelectedCustomer.City,
                Is.EqualTo( "Chester" ) );
        }

        [Test]
        public void WhenCalledWithInvalidId_Update_ReturnsFalse() {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();

            mockCustomerService.Setup(
                cs => cs.GetCustomerById( "ROCK" ) )
                    .Returns( ( Customer ) null );
            _sut = new CustomerManager( mockCustomerService.Object );
            // Act
            var result = _sut.Update(
                "ROCK", It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>() );

            // Assert
            Assert.That( result, Is.False );
        }

        [Test]
        public void WhenCalledWithInvalidId_Update_DoesNotChangeTheSelectedCustomer() {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();

            mockCustomerService.Setup(
                cs => cs.GetCustomerById( "ROCK" ) )
                    .Returns( ( Customer ) null );

            var originalCustomer = new Customer() {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };

            _sut = new CustomerManager( mockCustomerService.Object );
            _sut.SelectedCustomer = originalCustomer;

            // Act
            _sut.Update( "ROCK", "Rocky Raccoon", "UK", "Chester", null );

            // Assert that SelectedCustomer is unchanged
            Assert.That(
                _sut.SelectedCustomer.ContactName,
                Is.EqualTo( "Rocky Raccoon" ) );
            Assert.That(
                _sut.SelectedCustomer.CompanyName,
                Is.EqualTo( "Zoo UK" ) );
            Assert.That(
                _sut.SelectedCustomer.Country,
                Is.EqualTo( null ) );
            Assert.That(
                _sut.SelectedCustomer.City,
                Is.EqualTo( "Telford" ) );
        }

        [Test]
        public void WhenADatabaseExceptionIsThrownWhileUpdating_Update_ReturnsFalse() {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();

            mockCustomerService.Setup(
                cs => cs.GetCustomerById(
                    It.IsAny<string>() ) )
                .Returns( new Customer() );

            mockCustomerService.Setup(
                cs => cs.SaveCustomerChanges() )
                .Throws<DbUpdateConcurrencyException>();

            _sut = new CustomerManager( mockCustomerService.Object );

            // Act
            var result = _sut.Update(
                "ROCK", It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>(), It.IsAny<string>() );

            // Assert
            Assert.That( result, Is.False );
        }

        [Test]
        public void WhenADatabaseExceptionIsThrownWhileUpdating_Update_DoesNotChangeSelectedCustomer() {
            // Arrange
            var mockCustomerService = new Mock<ICustomerService>();

            mockCustomerService.Setup(
                cs => cs.GetCustomerById(
                    It.IsAny<string>() ) )
                .Returns( new Customer() );

            mockCustomerService.Setup(
                cs => cs.SaveCustomerChanges() )
                .Throws<DbUpdateConcurrencyException>();

            var originalCustomer = new Customer() {
                CustomerId = "ROCK",
                ContactName = "Rocky Raccoon",
                CompanyName = "Zoo UK",
                City = "Telford"
            };

            _sut = new CustomerManager( mockCustomerService.Object );
            _sut.SelectedCustomer = originalCustomer;
            // Act
            _sut.Update( "ROCK", "Rocky Raccoon", "UK", "Chester", null );

            // Assert
            Assert.That( _sut.SelectedCustomer.ContactName, Is.EqualTo( "Rocky Raccoon" ) );
            Assert.That( _sut.SelectedCustomer.CompanyName, Is.EqualTo( "Zoo UK" ) );
            Assert.That( _sut.SelectedCustomer.Country, Is.EqualTo( null ) );
            Assert.That( _sut.SelectedCustomer.City, Is.EqualTo( "Telford" ) );
        }

        [Test]
        public void GivenAValidId_Delete_ShouldReturnTrue() {
            var mockObject = new Mock<ICustomerService>();
            var originalCustomer = new Customer {
                CustomerId = "MANDA"
            };
            mockObject.Setup( cs => cs.GetCustomerById( "MANDA" ) ).Returns( originalCustomer );

            _sut = new CustomerManager( mockObject.Object );
            var result = _sut.Delete( "MANDA" );
            Assert.That( result, Is.True );
        }

        [Test]
        public void GivenANullCustomer_Delete_ShouldReturnFalse() {
            var mockObject = new Mock<ICustomerService>();
            mockObject.Setup( cs => cs.GetCustomerById( It.IsAny<string>() ) ).Returns( ( Customer ) null );

            _sut = new CustomerManager( mockObject.Object );

            var result = _sut.Delete( It.IsAny<string>() );
            Assert.That( result, Is.False );

        }
    }
}