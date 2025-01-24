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
    [Route("SalesOrderStatusOptions")]
    public class SalesOrderStatusOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public SalesOrderStatusOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: SalesOrderStatusOptions
        [HttpGet]
        public IEnumerable<SalesOrderStatusOption> GetSalesOrderStatusOptions()
        {
            return _context.SalesOrderStatusOptions;
        }

        // GET: SalesOrderStatusOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesOrderStatusOption = await _context.SalesOrderStatusOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (salesOrderStatusOption == null)
            {
                return NotFound();
            }

            return Ok(salesOrderStatusOption);
        }

        // PUT: SalesOrderStatusOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderStatusOption([FromRoute] int id, [FromBody] SalesOrderStatusOption salesOrderStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesOrderStatusOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(salesOrderStatusOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderStatusOptionExists(id))
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

        // POST: SalesOrderStatusOptions
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostSalesOrderStatusOption([FromBody] SalesOrderStatusOption salesOrderStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SalesOrderStatusOptions.Add(salesOrderStatusOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSalesOrderStatusOption", new { id = salesOrderStatusOption.Id }, salesOrderStatusOption);
        }

        // DELETE: SalesOrderStatusOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesOrderStatusOption = await _context.SalesOrderStatusOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (salesOrderStatusOption == null)
            {
                return NotFound();
            }

            _context.SalesOrderStatusOptions.Remove(salesOrderStatusOption);
            await _context.SaveChangesAsync();

            return Ok(salesOrderStatusOption);
        }

        private bool SalesOrderStatusOptionExists(int id)
        {
            return _context.SalesOrderStatusOptions.Any(e => e.Id == id);
        }
    }
}