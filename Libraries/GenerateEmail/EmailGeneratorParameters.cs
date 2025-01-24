using System.Collections.Generic;

namespace GidIndustrial.Gideon.WebApi.Libraries
{
    public class EmailGeneratorParameters
    {
        public string To { get; set; }
        public string From { get; set; }
        public string FromName { get; set; }
        public string Subject { get; set; }
        public string HtmlContent { get; set; }
        public string CC { get; set; }
        public string Bcc { get; set; }

        
        public List<GidIndustrial.Gideon.WebApi.Models.Attachment> Attachments { get; set; }

        public dynamic getErrorMessage()
        {
            if (From == null)
            {
                return new
                {
                    Error = "From is required"
                };
            }
            else if (To == null)
            {
                return new
                {
                    Error = "To is required"
                };
            }
            else if (Subject == null)
            {
                return new
                {
                    Error = "Subject is required"
                };
            }
            else if (HtmlContent == null)
            {
                return new
                {
                    Error = "HtmlContent is required"
                };
            }
            return null;
        }
    }
}
