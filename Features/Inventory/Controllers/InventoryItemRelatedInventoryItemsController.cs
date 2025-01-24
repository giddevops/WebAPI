using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("InventoryItemRelatedInventoryItems")]
    public class InventoryItemRelatedInventoryItemsController : Controller {
        private readonly AppDBContext _context;

        public InventoryItemRelatedInventoryItemsController(AppDBContext context) {
            _context = context;
        }


        // GET: InventoryItemRelatedInventoryItems?parentInventoryItemId=&childInventoryItemId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="parentInventoryItemId"></param>
        /// <param name="childInventoryItemId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetInventoryItemRelatedInventoryItemById([FromQuery] int? parentInventoryItemId, [FromQuery] int? childInventoryItemId) {

            var inventoryItemRelatedInventoryItem = await _context.InventoryItemRelatedInventoryItems.SingleOrDefaultAsync(m => m.ChildInventoryItemId == childInventoryItemId && m.ParentInventoryItemId == parentInventoryItemId);

            if (inventoryItemRelatedInventoryItem == null) {
                return NotFound();
            }

            return Ok(inventoryItemRelatedInventoryItem);
        }

        // GET: InventoryItemRelatedInventoryItems
        /// <summary>
        /// Fetches a list of parentInventoryItem childInventoryItems
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<InventoryItemRelatedInventoryItem> GetInventoryItemRelatedInventoryItems([FromQuery] int? parentInventoryItemId, [FromQuery] int? childInventoryItemId) {
            return _context.InventoryItemRelatedInventoryItems;
        }

        // PUT: InventoryItemRelatedInventoryItems?childInventoryItemId=&parentInventoryItemId=
        [HttpPut]
        public async Task<IActionResult> PutInventoryItemRelatedInventoryItem([FromQuery] int? childInventoryItemId, [FromQuery] int? parentInventoryItemId, [FromBody] InventoryItemRelatedInventoryItem inventoryItemRelatedInventoryItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(inventoryItemRelatedInventoryItem).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: InventoryItemRelatedInventoryItems
        [HttpPost]
        public async Task<IActionResult> PostInventoryItemRelatedInventoryItem([FromBody] InventoryItemRelatedInventoryItem inventoryItemRelatedInventoryItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            // //If the lead "Position" is a string, need to add it to the list of inventoryItemRelatedInventoryItemRelationshipTypes and then set that relationshipt Id as the relationship type for the parentInventoryItem and childInventoryItem
            // if (!string.IsNullOrWhiteSpace(inventoryItemRelatedInventoryItem.RelationshipTypeString))
            // {
            //     var existingRelationship = await _context.InventoryItemRelatedInventoryItemRelationshipTypes.FirstOrDefaultAsync(cc => cc.Value == inventoryItemRelatedInventoryItem.RelationshipTypeString);
            //     if(existingRelationship != null){
            //         inventoryItemRelatedInventoryItem.InventoryItemRelatedInventoryItemRelationshipTypeId = existingRelationship.Id;
            //     }else{
            //         var newInventoryItemRelatedInventoryItemRelationshipType = new InventoryItemRelatedInventoryItemRelationshipType{
            //             Value = inventoryItemRelatedInventoryItem.RelationshipTypeString
            //         };
            //         _context.InventoryItemRelatedInventoryItemRelationshipTypes.Add(newInventoryItemRelatedInventoryItemRelationshipType);
            //         await _context.SaveChangesAsync();
            //         inventoryItemRelatedInventoryItem.InventoryItemRelatedInventoryItemRelationshipTypeId = newInventoryItemRelatedInventoryItemRelationshipType.Id;
            //     }
            // }

            // set status to 
            var childInventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(item => item.Id == inventoryItemRelatedInventoryItem.ChildInventoryItemId);
            childInventoryItem.InventoryItemStatusOptionId = InventoryItemStatusOption.Committed;

            _context.InventoryItemRelatedInventoryItems.Add(inventoryItemRelatedInventoryItem);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (InventoryItemRelatedInventoryItemExists(inventoryItemRelatedInventoryItem.ParentInventoryItemId, inventoryItemRelatedInventoryItem.ChildInventoryItemId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            return CreatedAtAction("GetInventoryItemRelatedInventoryItem", new { id = inventoryItemRelatedInventoryItem.ChildInventoryItemId }, inventoryItemRelatedInventoryItem);
        }

        // DELETE: InventoryItemRelatedInventoryItems?parentInventoryItemId=&childInventoryItemid=
        [HttpDelete]
        public async Task<IActionResult> DeleteInventoryItemRelatedInventoryItem([FromQuery] int parentInventoryItemId, [FromQuery] int childInventoryItemId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var inventoryItemRelatedInventoryItem = await _context.InventoryItemRelatedInventoryItems.SingleOrDefaultAsync(m => m.ChildInventoryItemId == childInventoryItemId && m.ParentInventoryItemId == parentInventoryItemId);
            if (inventoryItemRelatedInventoryItem == null) {
                return NotFound();
            }

            var childInventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(item => item.Id == inventoryItemRelatedInventoryItem.ChildInventoryItemId);
            if(childInventoryItem.GidSubLocationOptionId == InventoryItemStatusOption.Committed)
                childInventoryItem.InventoryItemStatusOptionId = InventoryItemStatusOption.Available;

            _context.InventoryItemRelatedInventoryItems.Remove(inventoryItemRelatedInventoryItem);
            await _context.SaveChangesAsync();


            return Ok(inventoryItemRelatedInventoryItem);
        }

        private bool InventoryItemRelatedInventoryItemExists(int? parentInventoryItemId, int? childInventoryItemId) {
            return _context.InventoryItemRelatedInventoryItems.Any(e => e.ChildInventoryItemId == childInventoryItemId && e.ParentInventoryItemId == parentInventoryItemId);
        }
    }
}