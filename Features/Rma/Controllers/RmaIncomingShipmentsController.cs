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
    [Route("RmaIncomingShipments")]
    public class RmaIncomingShipmentsController : Controller
    {
        private readonly AppDBContext _context;

        public RmaIncomingShipmentsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: RmaIncomingShipments?rmaId=&incomingShipmentId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="rmaId"></param>
        /// <param name="incomingShipmentId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetRmaIncomingShipmentById([FromQuery] int? rmaId, [FromQuery] int? incomingShipmentId)
        {
            var rmaIncomingShipment = await _context.RmaIncomingShipments
                .Include(q => q.IncomingShipment)
                    .ThenInclude(i => i.InventoryItems)
                        .ThenInclude(ii => ii.InventoryItem)
                            .ThenInclude(ii => ii.Product)
                .Include(q => q.Rma)
                .SingleOrDefaultAsync(m => m.IncomingShipmentId == incomingShipmentId && m.RmaId == rmaId);



            if (rmaIncomingShipment == null)
            {
                return NotFound();
            }

            return Ok(rmaIncomingShipment);
        }

        // GET: RmaIncomingShipments
        [HttpGet]
        public IActionResult GetRmaIncomingShipments(
            [FromQuery] int? rmaId, [FromQuery] int? incomingShipmentId
        // [FromQuery] int skip = 0,
        // [FromQuery] int perPage = 10
        )
        {
            var query = from rmaIncomingShipment in _context.RmaIncomingShipments select rmaIncomingShipment;

            if (rmaId != null)
            {
                query = query.Where(pois => pois.RmaId == rmaId);
            }

            query = query
                .Include(q => q.IncomingShipment)
                    .ThenInclude(i => i.InventoryItems)
                        .ThenInclude(ii => ii.InventoryItem)
                            .ThenInclude(ii => ii.Product)
                .Include(q => q.Rma);

            return Ok(query);
        }

        // PUT: RmaIncomingShipments?incomingShipmentId=&rmaId=
        [HttpPut]
        public async Task<IActionResult> PutRmaIncomingShipment([FromQuery] int? incomingShipmentId, [FromQuery] int? rmaId, [FromBody] RmaIncomingShipment rmaIncomingShipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(rmaIncomingShipment).State = EntityState.Modified;

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

        // POST: RmaIncomingShipments
        [HttpPost]
        public async Task<IActionResult> PostRmaIncomingShipment([FromBody] RmaIncomingShipment rmaIncomingShipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // //If the lead "Position" is a string, need to add it to the list of rmaIncomingShipmentRelationshipTypes and then set that relationshipt Id as the relationship type for the rma and incomingShipment
            // if (!string.IsNullOrWhiteSpace(rmaIncomingShipment.RelationshipTypeString))
            // {
            //     var existingRelationship = await _context.RmaIncomingShipmentRelationshipTypes.FirstOrDefaultAsync(cc => cc.Value == rmaIncomingShipment.RelationshipTypeString);
            //     if(existingRelationship != null){
            //         rmaIncomingShipment.RmaIncomingShipmentRelationshipTypeId = existingRelationship.Id;
            //     }else{
            //         var newRmaIncomingShipmentRelationshipType = new RmaIncomingShipmentRelationshipType{
            //             Value = rmaIncomingShipment.RelationshipTypeString
            //         };
            //         _context.RmaIncomingShipmentRelationshipTypes.Add(newRmaIncomingShipmentRelationshipType);
            //         await _context.SaveChangesAsync();
            //         rmaIncomingShipment.RmaIncomingShipmentRelationshipTypeId = newRmaIncomingShipmentRelationshipType.Id;
            //     }
            // }

            _context.RmaIncomingShipments.Add(rmaIncomingShipment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RmaIncomingShipmentExists(rmaIncomingShipment.RmaId, rmaIncomingShipment.IncomingShipmentId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            // //need to return all related properties
            // var query = from pois in _context.RmaIncomingShipments select pois;
            // query = query
            //     .Include(q => q.IncomingShipment)
            //         .ThenInclude(i => i.InventoryItems)
            //             .ThenInclude(ii => ii.InventoryItem)
            //                 .ThenInclude(ii => ii.Product)
            //     .Include(q => q.Rma);
            

            return CreatedAtAction("GetRmaIncomingShipment", new { id = rmaIncomingShipment.IncomingShipmentId }, rmaIncomingShipment);
        }

        // DELETE: RmaIncomingShipments?rmaId=&incomingShipmentid=
        [HttpDelete]
        public async Task<IActionResult> DeleteRmaIncomingShipment([FromQuery] int? rmaId, [FromQuery] int? incomingShipmentId)
        {
            if (rmaId == null)
            {
                return BadRequest(new
                {
                    Error = "rmaId querystring parameter is required"
                });
            }
            if (incomingShipmentId == null)
            {
                return BadRequest(new
                {
                    Error = "rmaId querystring parameter is required"
                });
            }

            var rmaIncomingShipment = await _context.RmaIncomingShipments.FirstOrDefaultAsync(m => m.IncomingShipmentId == incomingShipmentId && m.RmaId == rmaId);

            if (rmaIncomingShipment == null)
            {
                return NotFound();
            }

            _context.RmaIncomingShipments.Remove(rmaIncomingShipment);
            await _context.SaveChangesAsync();

            return Ok(rmaIncomingShipment);
        }

        private bool RmaIncomingShipmentExists(int? rmaId, int? incomingShipmentId)
        {
            return _context.RmaIncomingShipments.Any(e => e.IncomingShipmentId == incomingShipmentId && e.RmaId == rmaId);
        }
    }
}