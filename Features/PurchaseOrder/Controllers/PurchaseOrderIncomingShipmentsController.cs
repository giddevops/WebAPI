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
    [Route("PurchaseOrderIncomingShipments")]
    public class PurchaseOrderIncomingShipmentsController : Controller
    {
        private readonly AppDBContext _context;

        public PurchaseOrderIncomingShipmentsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrderIncomingShipments?purchaseOrderId=&incomingShipmentId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="purchaseOrderId"></param>
        /// <param name="incomingShipmentId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetPurchaseOrderIncomingShipmentById([FromQuery] int? purchaseOrderId, [FromQuery] int? incomingShipmentId)
        {
            var purchaseOrderIncomingShipment = await _context.PurchaseOrderIncomingShipments
                .Include(q => q.IncomingShipment)
                    .ThenInclude(i => i.InventoryItems)
                        .ThenInclude(ii => ii.InventoryItem)
                            .ThenInclude(ii => ii.Product)
                .Include(q => q.PurchaseOrder)
                .SingleOrDefaultAsync(m => m.IncomingShipmentId == incomingShipmentId && m.PurchaseOrderId == purchaseOrderId);



            if (purchaseOrderIncomingShipment == null)
            {
                return NotFound();
            }

            return Ok(purchaseOrderIncomingShipment);
        }

        // GET: PurchaseOrderIncomingShipments
        [HttpGet]
        public IActionResult GetPurchaseOrderIncomingShipments(
            [FromQuery] int? purchaseOrderId, [FromQuery] int? incomingShipmentId
        // [FromQuery] int skip = 0,
        // [FromQuery] int perPage = 10
        )
        {
            var query = from purchaseOrderIncomingShipment in _context.PurchaseOrderIncomingShipments select purchaseOrderIncomingShipment;

            if (purchaseOrderId != null)
            {
                query = query.Where(pois => pois.PurchaseOrderId == purchaseOrderId);
            }

            query = query
                .Include(q => q.IncomingShipment)
                    .ThenInclude(i => i.InventoryItems)
                        .ThenInclude(ii => ii.InventoryItem)
                            .ThenInclude(ii => ii.Product)
                .Include(q => q.PurchaseOrder);

            return Ok(query);
        }

        // PUT: PurchaseOrderIncomingShipments?incomingShipmentId=&purchaseOrderId=
        [HttpPut]
        public async Task<IActionResult> PutPurchaseOrderIncomingShipment([FromQuery] int? incomingShipmentId, [FromQuery] int? purchaseOrderId, [FromBody] PurchaseOrderIncomingShipment purchaseOrderIncomingShipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(purchaseOrderIncomingShipment).State = EntityState.Modified;

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

        // POST: PurchaseOrderIncomingShipments
        [HttpPost]
        public async Task<IActionResult> PostPurchaseOrderIncomingShipment([FromBody] PurchaseOrderIncomingShipment purchaseOrderIncomingShipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // //If the lead "Position" is a string, need to add it to the list of purchaseOrderIncomingShipmentRelationshipTypes and then set that relationshipt Id as the relationship type for the purchaseOrder and incomingShipment
            // if (!string.IsNullOrWhiteSpace(purchaseOrderIncomingShipment.RelationshipTypeString))
            // {
            //     var existingRelationship = await _context.PurchaseOrderIncomingShipmentRelationshipTypes.FirstOrDefaultAsync(cc => cc.Value == purchaseOrderIncomingShipment.RelationshipTypeString);
            //     if(existingRelationship != null){
            //         purchaseOrderIncomingShipment.PurchaseOrderIncomingShipmentRelationshipTypeId = existingRelationship.Id;
            //     }else{
            //         var newPurchaseOrderIncomingShipmentRelationshipType = new PurchaseOrderIncomingShipmentRelationshipType{
            //             Value = purchaseOrderIncomingShipment.RelationshipTypeString
            //         };
            //         _context.PurchaseOrderIncomingShipmentRelationshipTypes.Add(newPurchaseOrderIncomingShipmentRelationshipType);
            //         await _context.SaveChangesAsync();
            //         purchaseOrderIncomingShipment.PurchaseOrderIncomingShipmentRelationshipTypeId = newPurchaseOrderIncomingShipmentRelationshipType.Id;
            //     }
            // }

            _context.PurchaseOrderIncomingShipments.Add(purchaseOrderIncomingShipment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PurchaseOrderIncomingShipmentExists(purchaseOrderIncomingShipment.PurchaseOrderId, purchaseOrderIncomingShipment.IncomingShipmentId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            // //need to return all related properties
            // var query = from pois in _context.PurchaseOrderIncomingShipments select pois;
            // query = query
            //     .Include(q => q.IncomingShipment)
            //         .ThenInclude(i => i.InventoryItems)
            //             .ThenInclude(ii => ii.InventoryItem)
            //                 .ThenInclude(ii => ii.Product)
            //     .Include(q => q.PurchaseOrder);
            

            return CreatedAtAction("GetPurchaseOrderIncomingShipment", new { id = purchaseOrderIncomingShipment.IncomingShipmentId }, purchaseOrderIncomingShipment);
        }

        // DELETE: PurchaseOrderIncomingShipments?purchaseOrderId=&incomingShipmentid=
        [HttpDelete]
        public async Task<IActionResult> DeletePurchaseOrderIncomingShipment([FromQuery] int? purchaseOrderId, [FromQuery] int? incomingShipmentId)
        {
            if (purchaseOrderId == null)
            {
                return BadRequest(new
                {
                    Error = "purchaseOrderId querystring parameter is required"
                });
            }
            if (incomingShipmentId == null)
            {
                return BadRequest(new
                {
                    Error = "purchaseOrderId querystring parameter is required"
                });
            }

            var purchaseOrderIncomingShipment = await _context.PurchaseOrderIncomingShipments.FirstOrDefaultAsync(m => m.IncomingShipmentId == incomingShipmentId && m.PurchaseOrderId == purchaseOrderId);

            if (purchaseOrderIncomingShipment == null)
            {
                return NotFound();
            }

            _context.PurchaseOrderIncomingShipments.Remove(purchaseOrderIncomingShipment);
            await _context.SaveChangesAsync();

            return Ok(purchaseOrderIncomingShipment);
        }

        private bool PurchaseOrderIncomingShipmentExists(int? purchaseOrderId, int? incomingShipmentId)
        {
            return _context.PurchaseOrderIncomingShipments.Any(e => e.IncomingShipmentId == incomingShipmentId && e.PurchaseOrderId == purchaseOrderId);
        }
    }
}