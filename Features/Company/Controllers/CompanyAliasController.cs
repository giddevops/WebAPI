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
    [Route("CompanyAliases")]
    public class CompanyAliasController : Controller
    {
        private readonly AppDBContext _context;

        public CompanyAliasController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CompanyAliases
        [HttpGet]
        public IEnumerable<CompanyAlias> GetCompanyAliases()
        {
            return _context.CompanyAliases;
        }

        // GET: CompanyAliases/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyAlias([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyAlias = await _context.CompanyAliases.SingleOrDefaultAsync(m => m.Id == id);

            if (companyAlias == null)
            {
                return NotFound();
            }

            return Ok(companyAlias);
        }

        // PUT: CompanyAliases/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanyAlias([FromRoute] int? id, [FromBody] CompanyAlias companyAlias)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != companyAlias.Id)
            {
                return BadRequest();
            }

            _context.Entry(companyAlias).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyAliasExists(id))
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

        // POST: CompanyAlias
        [HttpPost]
        public async Task<IActionResult> PostCompanyAlias([FromBody] CompanyAlias companyAlias)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CompanyAliases.Add(companyAlias);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanyAlias", new { id = companyAlias.Id }, companyAlias);
        }

        // DELETE: CompanyAliases/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyAlias([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyAlias = await _context.CompanyAliases.SingleOrDefaultAsync(m => m.Id == id);
            if (companyAlias == null)
            {
                return NotFound();
            }

            _context.CompanyAliases.Remove(companyAlias);
            await _context.SaveChangesAsync();

            return Ok(companyAlias);
        }

        private bool CompanyAliasExists(int? id)
        {
            return _context.CompanyAliases.Any(e => e.Id == id);
        }
    }
}