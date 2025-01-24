using System;
using GidIndustrial.Gideon.WebApi.Models;
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers
{
    public class SendEmailData
    {
        public EmailGeneratorParameters EmailParameters { get; set; }
    }
}