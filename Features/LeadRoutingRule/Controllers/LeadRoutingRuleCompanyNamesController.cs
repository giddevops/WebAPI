using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;

namespace WebApi.Features.Controllers {

    [Produces("application/json")]
    [Route("LeadRoutingRuleCompanyNames")]
    public class LeadRoutingRuleCompanyNamesController : Controller {
        private readonly AppDBContext _context;

        public LeadRoutingRuleCompanyNamesController(AppDBContext context) {
            _context = context;
        }

        // GET: LeadRoutingRuleCompanyNames
        [HttpGet]
        public IEnumerable<LeadRoutingRuleCompanyName> GetLeadRoutingRuleCompanyNames() {
            return _context.LeadRoutingRuleCompanyNames;
        }

        // GET: LeadRoutingRuleCompanyNames/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadRoutingRuleCompanyName([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var leadRoutingRuleCompanyName = await _context.LeadRoutingRuleCompanyNames.SingleOrDefaultAsync(m => m.Id == id);

            if (leadRoutingRuleCompanyName == null) {
                return NotFound();
            }

            return Ok(leadRoutingRuleCompanyName);
        }

        // PUT: LeadRoutingRuleCompanyNames/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeadRoutingRuleCompanyName([FromRoute] int? id, [FromBody] LeadRoutingRuleCompanyName leadRoutingRuleCompanyName) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != leadRoutingRuleCompanyName.Id) {
                return BadRequest();
            }

            _context.Entry(leadRoutingRuleCompanyName).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: LeadRoutingRuleCompanyNames
        [HttpPost]
        public async Task<IActionResult> PostLeadRoutingRuleCompanyName([FromBody] LeadRoutingRuleCompanyName leadRoutingRuleCompanyName) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.LeadRoutingRuleCompanyNames.Add(leadRoutingRuleCompanyName);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeadRoutingRuleCompanyName", new { id = leadRoutingRuleCompanyName.Id }, leadRoutingRuleCompanyName);
        }

        // DELETE: LeadRoutingRuleCompanyNames/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeadRoutingRuleCompanyName([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var leadRoutingRuleCompanyName = await _context.LeadRoutingRuleCompanyNames.SingleOrDefaultAsync(m => m.Id == id);
            if (leadRoutingRuleCompanyName == null) {
                return NotFound();
            }
            _context.Entry(leadRoutingRuleCompanyName).State = EntityState.Deleted;
            return Ok(leadRoutingRuleCompanyName);
        }

        private bool LeadRoutingRuleCompanyNameExists(int? id) {
            return _context.LeadRoutingRuleCompanyNames.Any(e => e.Id == id);
        }
    }
}