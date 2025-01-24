using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text.RegularExpressions;
using GidIndustrial.Gideon.WebApi.Libraries;
using GidIndustrial.Gideon.WebApi;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Checkout")]
    [AllowAnonymous]
    public class CeckoutController : Controller {
        private readonly AppDBContext _context;
        private IConfiguration configuration;

        public CeckoutController(AppDBContext context, IConfiguration config) {
            _context = context;
            configuration = config;
        }

        public IActionResult Index() {
            return View("~/Features/Checkout/Views/Index.cshtml");
        }
    }
}