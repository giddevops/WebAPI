using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Features.Controllers
{
    [Produces("application/json")]
    [Route("CustomerTypes")]
    [Authorize]
    public class CustomerTypesController : Controller
    {
        private readonly AppDBContext _context;

        public CustomerTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CustomerTypes
        [HttpGet]
        public IEnumerable<CustomerType> GetCustomerTypes()
        {
            return _context.CustomerTypes;
        }

        // GET: CustomerTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomerType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerType = await _context.CustomerTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (customerType == null)
            {
                return NotFound();
            }

            return Ok(customerType);
        }

        // PUT: CustomerTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutCustomerType([FromRoute] int id, [FromBody] CustomerType customerType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != customerType.Id)
            {
                return BadRequest();
            }

            _context.Entry(customerType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerTypeExists(id))
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

        // POST: CustomerTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostCustomerType([FromBody] CustomerType customerType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CustomerTypes.Add(customerType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomerType", new { id = customerType.Id }, customerType);
        }

        // DELETE: CustomerTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteCustomerType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customerType = await _context.CustomerTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (customerType == null)
            {
                return NotFound();
            }

            _context.CustomerTypes.Remove(customerType);
            await _context.SaveChangesAsync();

            return Ok(customerType);
        }

        private bool CustomerTypeExists(int id)
        {
            return _context.CustomerTypes.Any(e => e.Id == id);
        }
    }
}