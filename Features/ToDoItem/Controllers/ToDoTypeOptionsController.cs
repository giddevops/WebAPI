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
    [Route("ToDoTypeOptions")]
    public class ToDoTypeOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public ToDoTypeOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ToDoTypeOptions
        [HttpGet]
        public IEnumerable<ToDoTypeOption> GetToDoTypeOption()
        {
            return _context.ToDoTypeOptions;
        }

        // GET: ToDoTypeOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetToDoTypeOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ToDoTypeOption = await _context.ToDoTypeOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (ToDoTypeOption == null)
            {
                return NotFound();
            }

            return Ok(ToDoTypeOption);
        }

        // PUT: ToDoTypeOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutToDoTypeOption([FromRoute] int id, [FromBody] ToDoTypeOption ToDoTypeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ToDoTypeOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(ToDoTypeOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ToDoTypeOptionExists(id))
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

        // POST: ToDoTypeOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostToDoTypeOption([FromBody] ToDoTypeOption ToDoTypeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ToDoTypeOptions.Add(ToDoTypeOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetToDoTypeOption", new { id = ToDoTypeOption.Id }, ToDoTypeOption);
        }

        // DELETE: ToDoTypeOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteToDoTypeOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ToDoTypeOption = await _context.ToDoTypeOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (ToDoTypeOption == null)
            {
                return NotFound();
            }

            _context.ToDoTypeOptions.Remove(ToDoTypeOption);
            await _context.SaveChangesAsync();

            return Ok(ToDoTypeOption);
        }

        private bool ToDoTypeOptionExists(int id)
        {
            return _context.ToDoTypeOptions.Any(e => e.Id == id);
        }
    }
}