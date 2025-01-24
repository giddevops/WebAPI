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
    [Route("LineItemConditionTypes")]
    public class LineItemConditionTypesController : Controller
    {
        private readonly AppDBContext _context;

        public LineItemConditionTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: LineItemConditionTypes
        [HttpGet]
        public IEnumerable<LineItemConditionType> GetLineItemConditionTypes()
        {
            return _context.LineItemConditionTypes;
        }

        // GET: LineItemConditionTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLineItemConditionType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LineItemConditionType = await _context.LineItemConditionTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (LineItemConditionType == null)
            {
                return NotFound();
            }

            return Ok(LineItemConditionType);
        }

        // PUT: LineItemConditionTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutLineItemConditionType([FromRoute] int id, [FromBody] LineItemConditionType LineItemConditionType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != LineItemConditionType.Id)
            {
                return BadRequest();
            }

            _context.Entry(LineItemConditionType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineItemConditionTypeExists(id))
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

        // POST: LineItemConditionTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostLineItemConditionType([FromBody] LineItemConditionType LineItemConditionType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LineItemConditionTypes.Add(LineItemConditionType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLineItemConditionType", new { id = LineItemConditionType.Id }, LineItemConditionType);
        }

        // DELETE: LineItemConditionTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteLineItemConditionType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LineItemConditionType = await _context.LineItemConditionTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (LineItemConditionType == null)
            {
                return NotFound();
            }

            _context.LineItemConditionTypes.Remove(LineItemConditionType);
            await _context.SaveChangesAsync();

            return Ok(LineItemConditionType);
        }

        private bool LineItemConditionTypeExists(int id)
        {
            return _context.LineItemConditionTypes.Any(e => e.Id == id);
        }
    }
}