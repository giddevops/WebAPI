using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class LineItemServiceType
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Value { get; set; }
        public bool Locked { get; set; }
        
        public List<LeadRoutingRuleLineItemServiceType> LeadRoutingRules { get; set; }

        public const int Sales = 1;
        public const int Repair = 2;

    }
}
