using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class CurrencyOption
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Value { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public decimal? WireTransferFee { get; set; }
        public decimal? CreditCardFee { get; set; }
        public bool? Locked { get; set; }
    }
}