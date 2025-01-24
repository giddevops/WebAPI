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
    [Route("RequiredDeliveryTimeOptions")]
    public class RequiredDeliveryTimeOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public RequiredDeliveryTimeOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: RequiredDeliveryTimeOptions
        [HttpGet]
        public IEnumerable<RequiredDeliveryTimeOption> GetRequiredDeliveryTimeOptions()
        {
            return _context.RequiredDeliveryTimeOptions;
        }

        // GET: RequiredDeliveryTimeOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRequiredDeliveryTimeOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requiredDeliveryTimeOption = await _context.RequiredDeliveryTimeOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (requiredDeliveryTimeOption == null)
            {
                return NotFound();
            }

            return Ok(requiredDeliveryTimeOption);
        }

        // PUT: RequiredDeliveryTimeOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutRequiredDeliveryTimeOption([FromRoute] int id, [FromBody] RequiredDeliveryTimeOption requiredDeliveryTimeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != requiredDeliveryTimeOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(requiredDeliveryTimeOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RequiredDeliveryTimeOptionExists(id))
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

        // POST: RequiredDeliveryTimeOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostRequiredDeliveryTimeOption([FromBody] RequiredDeliveryTimeOption requiredDeliveryTimeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RequiredDeliveryTimeOptions.Add(requiredDeliveryTimeOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRequiredDeliveryTimeOption", new { id = requiredDeliveryTimeOption.Id }, requiredDeliveryTimeOption);
        }

        // DELETE: RequiredDeliveryTimeOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteRequiredDeliveryTimeOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var requiredDeliveryTimeOption = await _context.RequiredDeliveryTimeOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (requiredDeliveryTimeOption == null)
            {
                return NotFound();
            }

            _context.RequiredDeliveryTimeOptions.Remove(requiredDeliveryTimeOption);
            await _context.SaveChangesAsync();

            return Ok(requiredDeliveryTimeOption);
        }

        private bool RequiredDeliveryTimeOptionExists(int id)
        {
            return _context.RequiredDeliveryTimeOptions.Any(e => e.Id == id);
        }
    }
}