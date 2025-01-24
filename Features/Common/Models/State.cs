using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class State
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Name { get; set; }
    }
}
