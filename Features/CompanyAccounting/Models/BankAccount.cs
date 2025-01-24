using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class BankAccount
    {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public string Name { get; set; }
        public bool Locked { get; set; }

        public bool? IsDefaultCCAccount { get; set; }
        public string QuickBooksId { get; set; }
        
    }
}
