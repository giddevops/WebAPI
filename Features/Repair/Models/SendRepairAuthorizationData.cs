using System;
using GidIndustrial.Gideon.WebApi.Models;
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers
{
    public class SendRepairAuthorizationData
    {
        public EmailGeneratorParameters EmailParameters { get; set; }
        public Repair Repair { get; set; }
    }
}