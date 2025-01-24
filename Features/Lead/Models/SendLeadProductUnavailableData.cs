using System;
using GidIndustrial.Gideon.WebApi.Models;
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers
{
    public class SendLeadProductUnavailableData
    {
        public EmailGeneratorParameters EmailParameters { get; set; }
        public Lead Lead { get; set; }
    }
}