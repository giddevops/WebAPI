using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class EmailTemplateType
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Value { get; set; }
        public bool? Locked { get; set; }
    }
}
