using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using System.Net;
using System.Web;
using System.IO;
using Newtonsoft.Json;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("ShippingTypes")]
    public class ShippingTypesController : Controller {
        private readonly AppDBContext _context;

        public ShippingTypesController(AppDBContext context) {
            _context = context;
        }

        // GET: ShippingTypes
        [HttpGet]
        public IEnumerable<ShippingType> GetShippingTypes() {
            return _context.ShippingTypes;
        }

        // GET: ShippingTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShippingType([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrderStatusOption = await _context.ShippingTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (salesOrderStatusOption == null) {
                return NotFound();
            }

            return Ok(salesOrderStatusOption);
        }

        // PUT: ShippingTypes/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShippingType([FromRoute] int id, [FromBody] ShippingType salesOrderStatusOption) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != salesOrderStatusOption.Id) {
                return BadRequest();
            }

            _context.Entry(salesOrderStatusOption).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ShippingTypeExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: ShippingTypes
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostShippingType([FromBody] ShippingType salesOrderStatusOption) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.ShippingTypes.Add(salesOrderStatusOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShippingType", new { id = salesOrderStatusOption.Id }, salesOrderStatusOption);
        }

        // DELETE: ShippingTypes/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingType([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrderStatusOption = await _context.ShippingTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (salesOrderStatusOption == null) {
                return NotFound();
            }

            _context.ShippingTypes.Remove(salesOrderStatusOption);
            await _context.SaveChangesAsync();

            return Ok(salesOrderStatusOption);
        }

        private bool ShippingTypeExists(int id) {
            return _context.ShippingTypes.Any(e => e.Id == id);
        }
    }
}