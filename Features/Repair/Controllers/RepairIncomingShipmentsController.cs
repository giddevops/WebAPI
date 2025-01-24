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
    [Route("RepairIncomingShipments")]
    public class RepairIncomingShipmentsController : Controller
    {
        private readonly AppDBContext _context;

        public RepairIncomingShipmentsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: RepairIncomingShipments?repairId=&incomingShipmentId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="repairId"></param>
        /// <param name="incomingShipmentId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetRepairIncomingShipmentById([FromQuery] int? repairId, [FromQuery] int? incomingShipmentId)
        {
            var repairIncomingShipment = await _context.RepairIncomingShipments
                .Include(q => q.IncomingShipment)
                    .ThenInclude(i => i.InventoryItems)
                        .ThenInclude(ii => ii.InventoryItem)
                            .ThenInclude(ii => ii.Product)
                .Include(q => q.Repair)
                .SingleOrDefaultAsync(m => m.IncomingShipmentId == incomingShipmentId && m.RepairId == repairId);



            if (repairIncomingShipment == null)
            {
                return NotFound();
            }

            return Ok(repairIncomingShipment);
        }

        // GET: RepairIncomingShipments
        [HttpGet]
        public IActionResult GetRepairIncomingShipments(
            [FromQuery] int? repairId, [FromQuery] int? incomingShipmentId
        // [FromQuery] int skip = 0,
        // [FromQuery] int perPage = 10
        )
        {
            var query = from repairIncomingShipment in _context.RepairIncomingShipments select repairIncomingShipment;

            if (repairId != null)
            {
                query = query.Where(pois => pois.RepairId == repairId);
            }

            query = query
                .Include(q => q.IncomingShipment)
                    .ThenInclude(i => i.InventoryItems)
                        .ThenInclude(ii => ii.InventoryItem)
                            .ThenInclude(ii => ii.Product)
                .Include(q => q.Repair);

            return Ok(query);
        }

        // PUT: RepairIncomingShipments?incomingShipmentId=&repairId=
        [HttpPut]
        public async Task<IActionResult> PutRepairIncomingShipment([FromQuery] int? incomingShipmentId, [FromQuery] int? repairId, [FromBody] RepairIncomingShipment repairIncomingShipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(repairIncomingShipment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: RepairIncomingShipments
        [HttpPost]
        public async Task<IActionResult> PostRepairIncomingShipment([FromBody] RepairIncomingShipment repairIncomingShipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RepairIncomingShipments.Add(repairIncomingShipment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RepairIncomingShipmentExists(repairIncomingShipment.RepairId, repairIncomingShipment.IncomingShipmentId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRepairIncomingShipment", new { id = repairIncomingShipment.IncomingShipmentId }, repairIncomingShipment);
        }

        // DELETE: RepairIncomingShipments?repairId=&incomingShipmentid=
        [HttpDelete]
        public async Task<IActionResult> DeleteRepairIncomingShipment([FromQuery] int? repairId, [FromQuery] int? incomingShipmentId)
        {
            if (repairId == null)
            {
                return BadRequest(new
                {
                    Error = "repairId querystring parameter is required"
                });
            }
            if (incomingShipmentId == null)
            {
                return BadRequest(new
                {
                    Error = "repairId querystring parameter is required"
                });
            }

            var repairIncomingShipment = await _context.RepairIncomingShipments.FirstOrDefaultAsync(m => m.IncomingShipmentId == incomingShipmentId && m.RepairId == repairId);

            if (repairIncomingShipment == null)
            {
                return NotFound();
            }

            _context.RepairIncomingShipments.Remove(repairIncomingShipment);
            await _context.SaveChangesAsync();

            return Ok(repairIncomingShipment);
        }

        private bool RepairIncomingShipmentExists(int? repairId, int? incomingShipmentId)
        {
            return _context.RepairIncomingShipments.Any(e => e.IncomingShipmentId == incomingShipmentId && e.RepairId == repairId);
        }
    }
}