using System;
using GidIndustrial.Gideon.WebApi.Models;
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers
{
    public class SendRmaData
    {
        public EmailGeneratorParameters EmailParameters { get; set; }
        public Rma Rma { get; set; }
    }
}