using System;

namespace GidIndustrial.Gideon.WebApi.Models {
    // public string Id { get; set; }
    public class ViewObjectNameOption {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public string Value { get; set; }
    }

}