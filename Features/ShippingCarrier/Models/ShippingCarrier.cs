using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class ShippingCarrier
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string TrackingNumberLink { get; set; }
        public int? AccountNumberLength { get; set; }
        public bool? HideFromCustomer { get; set; }

        public List<ShippingCarrierShippingMethod> ShippingMethods { get; set; }
        
    }
}