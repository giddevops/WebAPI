using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using LeadAutomation.Firefly.Exchange.Helpers;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("ExchangeRates")]
    public class ExchangeRatesController : Controller {
        private readonly AppDBContext _context;

        public ExchangeRatesController(AppDBContext context) {
            _context = context;

        }

        [HttpGet]
        public IActionResult GetAllRates() {
            ExchangeRates.RefreshRates();
            return Ok(ExchangeRates.GetAllRates());
        }
    }
}