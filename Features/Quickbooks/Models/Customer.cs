using System;
using System.Collections.Generic;

namespace QuickBooks.Models {
    public class Address {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string Line2 { get; set; }
        public string Line3 { get; set; }
        public string City { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }

        public Address(GidIndustrial.Gideon.WebApi.Models.Address addr){
            if(addr == null){
                return;
            }
            this.Line1 = addr.Address1;
            this.Line2 = addr.Address2;
            this.Line3 = addr.Address3;
            this.City = addr.City;
            this.CountrySubDivisionCode = addr.State;
            this.PostalCode = addr.ZipPostalCode;
        }
    }

    public class PrimaryPhone {
        public string FreeFormNumber { get; set; }
    }

    public class PrimaryEmailAddr {
        public string Address { get; set; }
    }

    public class Customer {
        // public bool Taxable { get; set; }
        public Address BillAddr { get; set; }
        public Address ShipAddr { get; set; }
        public bool Job { get; set; }
        public bool BillWithParent { get; set; }
        public double Balance { get; set; }
        public double BalanceWithJobs { get; set; }
        public string PreferredDeliveryMethod { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public QuickBooksMetaData MetaData { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string FullyQualifiedName { get; set; }
        public string CompanyName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public PrimaryPhone PrimaryPhone { get; set; }
        public PrimaryEmailAddr PrimaryEmailAddr { get; set; }
    }
    public class CustomerResponse {
        public Customer Customer { get; set; }
    }
    public class CustomerQueryResponseContainer {
        public CustomerQueryResponse QueryResponse { get; set; }
    }
    public class CustomerQueryResponse {
        public List<Customer> Customer { get; set; }
    }
}