using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("ToDoItems")]
    public class ToDoItemsController : Controller {
        private readonly AppDBContext _context;
        private IConfiguration configuration;

        public ToDoItemsController(AppDBContext context, IConfiguration config) {
            _context = context;
            configuration = config;
        }

        [HttpGet()]
        public async Task<IActionResult> GetToDoItems([FromQuery] int? quoteId, [FromQuery] int? leadId, [FromQuery] int? salesOrderId, [FromQuery] int? purchaseOrderId, [FromQuery] int? rmaId)
        {
            var query = from ToDoItem in _context.ToDoItems select ToDoItem;

            if (quoteId != null)
                query = query.Where(item => item.QuoteToDoItems.Any(i => i.QuoteId == quoteId));
            else if (leadId != null)
                query = query.Where(item => item.LeadToDoItems.Any(i => i.LeadId == leadId));
            else if (salesOrderId != null)
                query = query.Where(item => item.SalesOrderToDoItems.Any(i => i.SalesOrderId == salesOrderId));
            else if (purchaseOrderId != null)
                query = query.Where(item => item.PurchaseOrderToDoItems.Any(i => i.PurchaseOrderId == purchaseOrderId));
            else if (rmaId != null)
                query = query.Where(item => item.RMAToDoItems.Any(i => i.RMAId == rmaId));
            else
                return BadRequest("You need to specify leadId or quoteId or salesOrderId or purchaseOrderId");

            return Ok(await query.ToListAsync());
        }

        // GET: ToDoItem/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetToDoItem([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var ToDoItem = await _context.ToDoItems.SingleOrDefaultAsync(m => m.Id == id);

            if (ToDoItem == null) {
                return NotFound();
            }

            return Ok(ToDoItem);
        }

        // PUT: ToDoItem/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutToDoItem([FromRoute] int id, [FromBody] ToDoItem ToDoItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != ToDoItem.Id) {
                return BadRequest();
            }

            var oldItem = await _context.ToDoItems
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);
            if (oldItem == null) {
                return NotFound("That item was not found");
            }
            if (oldItem.CreatedById != GidIndustrial.Gideon.WebApi.Models.User.GetId(User)) {
                return BadRequest("You are not authorized to edit this because you didn't make it");
            }

            _context.Entry(ToDoItem).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ToDoItemExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: ToDoItem
        [HttpPost]
        public async Task<IActionResult> PostToDoItem([FromBody] ToDoItem ToDoItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ToDoItem.CreatedAt = DateTime.UtcNow;
            ToDoItem.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

            _context.ToDoItems.Add(ToDoItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetToDoItem", new { id = ToDoItem.Id }, ToDoItem);
        }

        // DELETE: ToDoItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteToDoItem([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var oldItem = await _context.ToDoItems.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if (oldItem == null) {
                return NotFound("That item was not found");
            }
            if (oldItem.CreatedById != GidIndustrial.Gideon.WebApi.Models.User.GetId(User)) {
                return BadRequest("You are not authorized to delete this because you didn't make it");
            }

            var ToDoItem = await _context.ToDoItems
                .SingleOrDefaultAsync(m => m.Id == id);
            if (ToDoItem == null) {
                return NotFound();
            }

            _context.ToDoItems.Remove(ToDoItem);
            await _context.SaveChangesAsync();

            return Ok(ToDoItem);
        }

        private bool ToDoItemExists(int id) {
            return _context.ToDoItems.Any(e => e.Id == id);
        }
    }
}