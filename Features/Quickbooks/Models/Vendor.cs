using System;
using System.Collections.Generic;

namespace QuickBooks.Models {

    public class WebAddr {
        public string URI { get; set; }
    }

    public class Vendor {
        public Address BillAddr { get; set; }
        public int Balance { get; set; }
        public string AcctNum { get; set; }
        public bool Vendor1099 { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public QuickBooksMetaData MetaData { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string CompanyName { get; set; }
        public string DisplayName { get; set; }
        public string PrintOnCheckName { get; set; }
        public bool Active { get; set; }
        public PrimaryPhone PrimaryPhone { get; set; }
        public PrimaryEmailAddr PrimaryEmailAddr { get; set; }
        public WebAddr WebAddr { get; set; }
    }

    public class VendorResponse {
        public Vendor Vendor { get; set; }
    }
    public class VendorQueryResponseContainer {
        public VendorQueryResponse QueryResponse { get; set; }
    }
    public class VendorQueryResponse {
        public List<Vendor> Vendor { get; set; }
    }
}