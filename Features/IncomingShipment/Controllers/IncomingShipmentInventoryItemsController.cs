using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using System.Data.SqlClient;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("IncomingShipmentInventoryItems")]
    public class IncomingShipmentInventoryItemsController : Controller {
        private readonly AppDBContext _context;

        public IncomingShipmentInventoryItemsController(AppDBContext context) {
            _context = context;
        }

        // GET: IncomingShipmentInventoryItems?incomingShipmentId=&inventoryItemId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="incomingShipmentId"></param>
        /// <param name="inventoryItemId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetIncomingShipmentInventoryItemById([FromQuery] int? incomingShipmentId, [FromQuery] int? inventoryItemId) {

            var incomingShipmentInventoryItem = await _context.IncomingShipmentInventoryItems.SingleOrDefaultAsync(m => m.InventoryItemId == inventoryItemId && m.IncomingShipmentId == incomingShipmentId);

            if (incomingShipmentInventoryItem == null) {
                return NotFound();
            }

            return Ok(incomingShipmentInventoryItem);
        }

        [HttpGet("ReceiveItem")]
        public async Task<IActionResult> ReceiveIncomingShipmentInventoryItemById([FromQuery] int? incomingShipmentId, [FromQuery] int? inventoryItemId, [FromQuery] string serialNumber, [FromQuery] int? gidSubLocationOptionId, [FromQuery] int? gidLocationOptionId, [FromQuery] int? inventoryItemStatusOptionId) {

            IncomingShipmentInventoryItem incomingShipmentInventoryItem = await _context.IncomingShipmentInventoryItems.SingleOrDefaultAsync(m => m.InventoryItemId == inventoryItemId && m.IncomingShipmentId == incomingShipmentId);

            if (incomingShipmentInventoryItem == null) {
                return NotFound();
            }
            InventoryItem inventoryItem = await _context.InventoryItems
                .Include(ii => ii.Product)
                .FirstOrDefaultAsync(ii => ii.Id == incomingShipmentInventoryItem.InventoryItemId);

            incomingShipmentInventoryItem.ReceivedAt = DateTime.UtcNow;
            incomingShipmentInventoryItem.ReceivedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            if (serialNumber != null && serialNumber != "") {
                inventoryItem.SerialNumber = serialNumber;
            } else if (inventoryItem.Product.Serialized) {
                return BadRequest(new {
                    Error = "This product is serialized and you didn't include a serial number for it"
                });
            } else {
                inventoryItem.GenerateSerialNumber();
            }
            inventoryItem.GidSubLocationOptionId = gidSubLocationOptionId;
            inventoryItem.GidLocationOptionId = gidLocationOptionId;
            inventoryItem.InventoryItemStatusOptionId = inventoryItemStatusOptionId ?? InventoryItemStatusOption.Available;
            _context.Entry(inventoryItem).State = EntityState.Modified;
            _context.Entry(incomingShipmentInventoryItem).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            incomingShipmentInventoryItem.InventoryItem = inventoryItem;
            return Ok(incomingShipmentInventoryItem);

        }

        [HttpGet("UnReceiveItem")]
        public async Task<IActionResult> UnReceiveIncomingShipmentInventoryItemById([FromQuery] int? incomingShipmentId, [FromQuery] int? inventoryItemId) {

            IncomingShipmentInventoryItem incomingShipmentInventoryItem = await _context.IncomingShipmentInventoryItems.SingleOrDefaultAsync(m => m.InventoryItemId == inventoryItemId && m.IncomingShipmentId == incomingShipmentId);

            if (incomingShipmentInventoryItem == null) {
                return NotFound();
            }
            InventoryItem inventoryItem = await _context.InventoryItems
                .Include(ii => ii.Product)
                .FirstOrDefaultAsync(ii => ii.Id == incomingShipmentInventoryItem.InventoryItemId);

            incomingShipmentInventoryItem.ReceivedAt = null;
            incomingShipmentInventoryItem.ReceivedById = null;
            _context.Entry(incomingShipmentInventoryItem).State = EntityState.Modified;

            inventoryItem.GidSubLocationOptionId = null;
            inventoryItem.InventoryItemStatusOptionId = InventoryItemStatusOption.Inbound;
            _context.Entry(inventoryItem).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            incomingShipmentInventoryItem.InventoryItem = inventoryItem;
            return Ok(incomingShipmentInventoryItem);

        }

        //since the incoming shipment id is one of the keys for this object, I don't think you can change it with EF core. There are 2 ways around this. First is to edit it with raw sql, and second is to select it, delete it, then insert it with new info.
        [HttpGet("ChangeIncomingShipment")]
        public async Task<IActionResult> ChangeIncomingShipment([FromQuery] int? incomingShipmentId, [FromQuery] int? inventoryItemId, [FromQuery] int? newIncomingShipmentId) {
            int numRowsAffected = await _context.Database.ExecuteSqlCommandAsync("UPDATE IncomingShipmentInventoryItem SET IncomingShipmentId=@newIncomingShipmentId WHERE IncomingShipmentId=@incomingShipmentId AND InventoryItemId=@inventoryItemId",
                new SqlParameter("@newIncomingShipmentId", newIncomingShipmentId),
                new SqlParameter("@incomingShipmentId", incomingShipmentId),
                new SqlParameter("@inventoryItemId", inventoryItemId)
            );
            //Get the
            IncomingShipmentInventoryItem newIncomingShipmentInventoryItem = await _context.IncomingShipmentInventoryItems.AsNoTracking().SingleOrDefaultAsync(m => m.InventoryItemId == inventoryItemId && m.IncomingShipmentId == newIncomingShipmentId);

            if (newIncomingShipmentInventoryItem == null) {
                return NotFound();
            }
            InventoryItem inventoryItem = await _context.InventoryItems
                .Include(ii => ii.Product)
                .FirstOrDefaultAsync(ii => ii.Id == newIncomingShipmentInventoryItem.InventoryItemId);

            // incomingShipmentInventoryItem.IncomingShipmentId = newIncomingShipmentId;
            // _context.Entry(incomingShipmentInventoryItem).State = EntityState.Modified;

            // await _context.SaveChangesAsync();
            newIncomingShipmentInventoryItem.InventoryItem = inventoryItem;
            return Ok(newIncomingShipmentInventoryItem);

        }

        // GET: IncomingShipmentInventoryItems
        /// <summary>
        /// Fetches a list of incomingShipment inventoryItems
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IEnumerable<IncomingShipmentInventoryItem> GetIncomingShipmentInventoryItems([FromQuery] int? incomingShipmentId, [FromQuery] int? inventoryItemId) {
            return _context.IncomingShipmentInventoryItems;
        }

        // PUT: IncomingShipmentInventoryItems?inventoryItemId=&incomingShipmentId=
        [HttpPut]
        public async Task<IActionResult> PutIncomingShipmentInventoryItem([FromQuery] int? inventoryItemId, [FromQuery] int? incomingShipmentId, [FromBody] IncomingShipmentInventoryItem incomingShipmentInventoryItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(incomingShipmentInventoryItem).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                {
                    throw;
                }
            }
            var item = await _context.IncomingShipmentInventoryItems.FirstOrDefaultAsync(i => i.InventoryItemId == inventoryItemId && i.IncomingShipmentId == incomingShipmentId);
            return Ok(item);
        }

        // POST: IncomingShipmentInventoryItems
        [HttpPost]
        public async Task<IActionResult> PostIncomingShipmentInventoryItem([FromBody] IncomingShipmentInventoryItem incomingShipmentInventoryItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.IncomingShipmentInventoryItems.Add(incomingShipmentInventoryItem);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (IncomingShipmentInventoryItemExists(incomingShipmentInventoryItem.IncomingShipmentId, incomingShipmentInventoryItem.InventoryItemId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            return CreatedAtAction("GetIncomingShipmentInventoryItem", new { id = incomingShipmentInventoryItem.InventoryItemId }, incomingShipmentInventoryItem);
        }

        // DELETE: IncomingShipmentInventoryItems?incomingShipmentId=&inventoryItemid=
        [HttpDelete]
        public async Task<IActionResult> DeleteIncomingShipmentInventoryItem([FromQuery] int incomingShipmentId, [FromQuery] int inventoryItemId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var incomingShipmentInventoryItem = await _context.IncomingShipmentInventoryItems.SingleOrDefaultAsync(m => m.InventoryItemId == inventoryItemId && m.IncomingShipmentId == incomingShipmentId);
            if (incomingShipmentInventoryItem == null) {
                return NotFound();
            }

            _context.IncomingShipmentInventoryItems.Remove(incomingShipmentInventoryItem);
            await _context.SaveChangesAsync();

            return Ok(incomingShipmentInventoryItem);
        }

        private bool IncomingShipmentInventoryItemExists(int? incomingShipmentId, int? inventoryItemId) {
            return _context.IncomingShipmentInventoryItems.Any(e => e.InventoryItemId == inventoryItemId && e.IncomingShipmentId == incomingShipmentId);
        }
    }
}