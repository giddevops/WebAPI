using System;
using GidIndustrial.Gideon.WebApi.Models;
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers
{
    public class SendPurchaseOrderData
    {
        public EmailGeneratorParameters EmailParameters { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}