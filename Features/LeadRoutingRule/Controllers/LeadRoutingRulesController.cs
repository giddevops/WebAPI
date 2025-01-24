using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;

namespace WebApi.Features.Controllers
{
    [Produces("application/json")]
    [Route("LeadRoutingRules")]
    public class LeadRoutingRulesController : Controller
    {
        private readonly AppDBContext _context;

        public LeadRoutingRulesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: LeadRoutingRules
        [HttpGet]
        [RequirePermission("EditUsers")]
        public IEnumerable<LeadRoutingRule> GetLeadRoutingRules()
        {
            return _context.LeadRoutingRules;
        }

        // GET: LeadRoutingRules/5
        [HttpGet("{id}")]
        [RequirePermission("EditUsers")]
        public async Task<IActionResult> GetLeadRoutingRule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LeadRoutingRule = await _context.LeadRoutingRules
                .Include(item => item.ProductTypes)
                .Include(item => item.LineItemServiceTypes)
                .Include(item => item.LeadWebsites)
                .Include(item => item.Countries)
                .Include(item => item.CompanyNames)
                // .Include(item => item.ProductTypeIncludeOptionId)
                // .Include(item => item.CountryIncludeOptionId)
                // .Include(item => item.LeadWebsiteIncludeOptionId)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (LeadRoutingRule == null)
            {
                return NotFound();
            }

            return Ok(LeadRoutingRule);
        }


        // POST: LeadRoutingRules
        [HttpPost]
        [RequirePermission("EditUsers")]
        public async Task<IActionResult> PostLeadRoutingRule([FromBody] LeadRoutingRule LeadRoutingRule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LeadRoutingRules.Add(LeadRoutingRule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeadRoutingRule", new { id = LeadRoutingRule.Id }, LeadRoutingRule);
        }

        // PUT: LeadRoutingRules/5
        [HttpPut("{id}")]
        [RequirePermission("EditUsers")]
        public async Task<IActionResult> PutLeadRoutingRule([FromRoute] int id, [FromBody] LeadRoutingRule LeadRoutingRule)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != LeadRoutingRule.Id)
            {
                return BadRequest();
            }
            
            var dbLeadRoutingRule = await _context.LeadRoutingRules.AsNoTracking()
                .Include(item => item.ProductTypes)
                .Include(item => item.Countries)
                .Include(item => item.LineItemServiceTypes)
                .Include(item => item.LeadWebsites)
                .Include(item => item.CompanyNames)
                .SingleOrDefaultAsync(c => c.Id == id);

            //add/modify/remove associated phone numbers
            if (dbLeadRoutingRule.ProductTypes != null)
            {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldProductTypes = dbLeadRoutingRule.ProductTypes.Select(p => p.ProductTypeId).ToList();
                List<int?> newProductTypes = LeadRoutingRule.ProductTypes.Where(p => p.ProductTypeId != null).Select(p => p.ProductTypeId).ToList();

                //Mark new items as added
                var added = LeadRoutingRule.ProductTypes.Where(p => !oldProductTypes.Any(oldProductTypeId => oldProductTypeId == p.ProductTypeId)).ToList();

                var modified = LeadRoutingRule.ProductTypes.Where(p => oldProductTypes.Any(oldProductTypeId => oldProductTypeId == p.ProductTypeId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                var deleted = dbLeadRoutingRule.ProductTypes.Where(p => !newProductTypes.Any(newProductTypeId => newProductTypeId == p.ProductTypeId)).ToList();

                added.ForEach(item =>
                { 
                    _context.LeadRoutingRuleProductTypes.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item =>
                {
                    _context.LeadRoutingRuleProductTypes.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item =>
                {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.LeadRoutingRuleProductTypes.Remove(item);
                });

            }

            //add/modify/remove associated countries
            if (dbLeadRoutingRule.Countries != null)
            {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldCountries = dbLeadRoutingRule.Countries.Select(p => p.CountryId).ToList();
                List<int?> newCountries = LeadRoutingRule.Countries.Where(p => p.CountryId != null).Select(p => p.CountryId).ToList();

                //Mark new items as added
                var added = LeadRoutingRule.Countries.Where(p => !oldCountries.Any(oldCountryId => oldCountryId == p.CountryId)).ToList();

                //mark all items without an id as modified
                var modified = LeadRoutingRule.Countries.Where(p => oldCountries.Any(oldCountryId => oldCountryId == p.CountryId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                var deleted = dbLeadRoutingRule.Countries.Where(p => !newCountries.Any(newCountryId => newCountryId == p.CountryId)).ToList();

                added.ForEach(item =>
                {
                    _context.LeadRoutingRuleCountries.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item =>
                {
                    _context.LeadRoutingRuleCountries.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item =>
                {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.LeadRoutingRuleCountries.Remove(item);
                });

            }
            //add/modify/remove associated service types
            if (dbLeadRoutingRule.LineItemServiceTypes != null)
            {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldLineItemServiceTypes = dbLeadRoutingRule.LineItemServiceTypes.Select(p => p.LineItemServiceTypeId).ToList();
                List<int?> newLineItemServiceTypes = LeadRoutingRule.LineItemServiceTypes.Where(p => p.LineItemServiceTypeId != null).Select(p => p.LineItemServiceTypeId).ToList();

                //Mark new items as added
                var added = LeadRoutingRule.LineItemServiceTypes.Where(p => !oldLineItemServiceTypes.Any(oldLineItemServiceTypeId => oldLineItemServiceTypeId == p.LineItemServiceTypeId)).ToList();

                //mark all items without an id as modified
                var modified = LeadRoutingRule.LineItemServiceTypes.Where(p => oldLineItemServiceTypes.Any(oldLineItemServiceTypeId => oldLineItemServiceTypeId == p.LineItemServiceTypeId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                var deleted = dbLeadRoutingRule.LineItemServiceTypes.Where(p => !newLineItemServiceTypes.Any(newLineItemServiceTypeId => newLineItemServiceTypeId == p.LineItemServiceTypeId)).ToList();

                added.ForEach(item =>
                {
                    _context.LeadRoutingRuleLineItemServiceTypes.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item =>
                {
                    _context.LeadRoutingRuleLineItemServiceTypes.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item =>
                {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.LeadRoutingRuleLineItemServiceTypes.Remove(item);
                });

            }
            //add/modify/remove associated websites
            if (dbLeadRoutingRule.LeadWebsites != null)
            {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldLeadWebsites = dbLeadRoutingRule.LeadWebsites.Select(p => p.LeadWebsiteId).ToList();
                List<int?> newLeadWebsites = LeadRoutingRule.LeadWebsites.Where(p => p.LeadWebsiteId != null).Select(p => p.LeadWebsiteId).ToList();

                //Mark new items as added
                var added = LeadRoutingRule.LeadWebsites.Where(p => !oldLeadWebsites.Any(oldLeadWebsiteId => oldLeadWebsiteId == p.LeadWebsiteId)).ToList();

                //mark all items without an id as modified
                var modified = LeadRoutingRule.LeadWebsites.Where(p => oldLeadWebsites.Any(oldLeadWebsiteId => oldLeadWebsiteId == p.LeadWebsiteId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                var deleted = dbLeadRoutingRule.LeadWebsites.Where(p => !newLeadWebsites.Any(newLeadWebsiteId => newLeadWebsiteId == p.LeadWebsiteId)).ToList();

                added.ForEach(item =>
                {
                    _context.LeadRoutingRuleLeadWebsites.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item =>
                {
                    _context.LeadRoutingRuleLeadWebsites.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item =>
                {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.LeadRoutingRuleLeadWebsites.Remove(item);
                });

            }

            //add/modify/remove associated Company names
            if (dbLeadRoutingRule.CompanyNames != null)
            {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldCompanyNames = dbLeadRoutingRule.CompanyNames.Select(p => p.Id).ToList();
                List<int?> newCompanyNames = LeadRoutingRule.CompanyNames.Where(p => p.Id != null).Select(p => p.Id).ToList();

                //Mark new items as added
                var added = LeadRoutingRule.CompanyNames.Where(p => !oldCompanyNames.Any(oldCompanyNameId => oldCompanyNameId == p.Id)).ToList();

                //mark all items without an id as modified
                var modified = LeadRoutingRule.CompanyNames.Where(p => oldCompanyNames.Any(oldCompanyNameId => oldCompanyNameId == p.Id)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                var deleted = dbLeadRoutingRule.CompanyNames.Where(p => !newCompanyNames.Any(newCompanyNameId => newCompanyNameId == p.Id)).ToList();

                added.ForEach(item =>
                {
                    _context.LeadRoutingRuleCompanyNames.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item =>
                {
                    _context.LeadRoutingRuleCompanyNames.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item =>
                {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.LeadRoutingRuleCompanyNames.Remove(item);
                });

            }

            _context.Entry(LeadRoutingRule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeadRoutingRuleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: LeadRoutingRules/5
        [HttpDelete("{id}")]
        [RequirePermission("EditUsers")]
        public async Task<IActionResult> DeleteLeadRoutingRule([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LeadRoutingRule = await _context.LeadRoutingRules.SingleOrDefaultAsync(m => m.Id == id);
            if (LeadRoutingRule == null)
            {
                return NotFound();
            }

            _context.LeadRoutingRules.Remove(LeadRoutingRule);
            await _context.SaveChangesAsync();

            return Ok(LeadRoutingRule);
        }

        private bool LeadRoutingRuleExists(int id)
        {
            return _context.LeadRoutingRules.Any(e => e.Id == id);
        }
    }
}