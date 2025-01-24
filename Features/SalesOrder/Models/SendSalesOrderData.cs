using System;
using GidIndustrial.Gideon.WebApi.Models;
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers
{
    public class SendSalesOrderData
    {
        public EmailGeneratorParameters EmailParameters { get; set; }
        public SalesOrder SalesOrder { get; set; }
    }
}