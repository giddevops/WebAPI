using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class InventoryItemStatusOption
    {
        public static int Inbound = 1;
        public static int Available = 2;
        public static int Committed = 3;
        public static int Shipped = 4;

        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public string Value { get; set; }
        public bool Locked { get; set; }
        
    }
}
