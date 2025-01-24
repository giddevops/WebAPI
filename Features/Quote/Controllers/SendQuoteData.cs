using System;
using GidIndustrial.Gideon.WebApi.Models;
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers
{
    public class SendQuoteData
    {
        public EmailGeneratorParameters EmailParameters { get; set; }
        public Quote Quote { get; set; }
    }

    public class CreateAutoQuoteData
    {
        public string PersonName { get; set; }
        public string CompanyName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Price { get; set; }
        public string Manufacturer { get; set; }
        public string PartNumber { get; set; }
        public string Category { get; set; }
        public int Quantity { get; set; }
        public int QuoteTime { get; set; }
    }
}