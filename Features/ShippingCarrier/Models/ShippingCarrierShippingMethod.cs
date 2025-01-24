using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class ShippingCarrierShippingMethod
    {
        public int? Id { get; set; }
        public int? ShippingCarrierId { get; set; }
        public string Name { get; set; }
        public int? SortPosition { get; set; }
    }
}