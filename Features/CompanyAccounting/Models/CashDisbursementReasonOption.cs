using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class CashDisbursementReasonOption
    {
        public const int Bill = 1;
        public const int Refund = 2;

        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public string Value { get; set; }
        public bool Locked { get; set; }   
    }
}