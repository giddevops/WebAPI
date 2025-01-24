using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class LeadRoutingRuleCompanyName {
        public int? Id { get; set; }
        public LeadRoutingRule LeadRoutingRule { get; set; }
        public int? LeadRoutingRuleId { get; set; }

        public string CompanyName { get; set; }
    }
}