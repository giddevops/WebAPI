//Users can filter the list of leads by various criteria. They can save those criteria as Lead Filters

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class RmaFilter{
        public int Id { get; set; }
        public int? CreatedById { get; set; }
        public System.DateTime CreatedAt { get; set; }

        public string Name { get; set; }
        public bool Public { get; set; }
        public string SearchCriteria { get; set; }
    }
}