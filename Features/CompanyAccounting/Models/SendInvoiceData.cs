using System;
using GidIndustrial.Gideon.WebApi.Models;
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers
{
    public class SendInvoiceData
    {
        public EmailGeneratorParameters EmailParameters { get; set; }
        public Invoice Invoice { get; set; }
    }
}