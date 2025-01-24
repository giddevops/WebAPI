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
    [Route("OutgoingShipmentBoxes")]
    public class OutgoingShipmentBoxesController : Controller
    {
        private readonly AppDBContext _context;

        public OutgoingShipmentBoxesController(AppDBContext context)
        {
            _context = context;
        }


        // GET: OutgoingShipmentBoxes
        [HttpGet]
        public ListResult GetOutgoingShipmentBoxes(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        )
        {
            var query = from outgoingShipmentBox in _context.OutgoingShipmentBoxes select outgoingShipmentBox;


            query = query
                .OrderByDescending(q => q.CreatedAt);

            return new ListResult
            {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }


        // GET: OutgoingShipmentBoxes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOutgoingShipmentBox([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var outgoingShipmentBox = await _context.OutgoingShipmentBoxes.SingleOrDefaultAsync(m => m.Id == id);

            if (outgoingShipmentBox == null)
            {
                return NotFound();
            }

            return Ok(outgoingShipmentBox);
        }

        // PUT: OutgoingShipmentBoxes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOutgoingShipmentBox([FromRoute] int? id, [FromBody] OutgoingShipmentBox outgoingShipmentBox)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != outgoingShipmentBox.Id)
            {
                return BadRequest();
            }

            _context.Entry(outgoingShipmentBox).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OutgoingShipmentBoxExists(id))
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

        // POST: OutgoingShipmentBoxes
        [HttpPost]
        public async Task<IActionResult> PostOutgoingShipmentBox([FromBody] OutgoingShipmentBox outgoingShipmentBox)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            outgoingShipmentBox.CreatedAt = DateTime.UtcNow;

            _context.OutgoingShipmentBoxes.Add(outgoingShipmentBox);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOutgoingShipmentBox", new { id = outgoingShipmentBox.Id }, outgoingShipmentBox);
        }

        // DELETE: OutgoingShipmentBoxes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOutgoingShipmentBox([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var outgoingShipmentBox = await _context.OutgoingShipmentBoxes.SingleOrDefaultAsync(m => m.Id == id);
            if (outgoingShipmentBox == null)
            {
                return NotFound();
            }

            _context.OutgoingShipmentBoxes.Remove(outgoingShipmentBox);
            await _context.SaveChangesAsync();

            return Ok(outgoingShipmentBox);
        }

        private bool OutgoingShipmentBoxExists(int? id)
        {
            return _context.OutgoingShipmentBoxes.Any(e => e.Id == id);
        }
    }
}