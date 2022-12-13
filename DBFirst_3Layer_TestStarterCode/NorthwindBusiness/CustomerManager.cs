using NorthwindData;
using NorthwindData.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NorthwindBusiness {
    public class CustomerManager {
        private ICustomerService _service;
        public CustomerManager() {
            _service = new CustomerService();
        }

        public CustomerManager( ICustomerService service ) {
            if ( service == null ) {
                throw new ArgumentException( "ICustomerService object cannot be null" );
            }
            _service = service;
        }

        public Customer SelectedCustomer { get; set; }

        public void SetSelectedCustomer( object selectedItem ) {
            SelectedCustomer = ( Customer ) selectedItem;
        }

        public List<Customer> RetrieveAll() {
            return _service.GetCustomerList();
        }

        public void Create( string customerId, string contactName, string companyName, string city = null ) {

            var newCust = new Customer() { CustomerId = customerId, ContactName = contactName, CompanyName = companyName, City = city };
            _service.CreateCustomer(
            new Customer() {
                CustomerId = customerId,
                ContactName = contactName,
                CompanyName = companyName,
            }
            );
        }

        public bool Update( string customerId, string contactName, string country, string city, string postcode ) {
            var customer = _service.GetCustomerById( customerId );
            if ( customer == null ) {
                Debug.WriteLine( $"Customer {customerId} not found" );
                return false;
            }
            customer.ContactName = contactName;
            customer.City = city;
            customer.PostalCode = postcode;
            customer.Country = country;
            try {
                _service.SaveCustomerChanges();
                SelectedCustomer = customer;
            }
            catch ( Exception e ) {
                Debug.WriteLine( $"Error updating {customerId}" );
                return false;
            }
            return true;
        }

        public void Delete( string customerId ) {
            var cust = _service.GetCustomerById( customerId );
            _service.RemoveCustomer( cust );
        }
    }
}
