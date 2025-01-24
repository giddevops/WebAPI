using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.Extensions.Configuration;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("WorkLogItems")]
    public class WorkLogItemsController : Controller {
        private readonly AppDBContext _context;
        private IConfiguration _configuration;

        public WorkLogItemsController(AppDBContext context, IConfiguration config) {
            _context = context;
            _configuration = config;
        }


        // GET: WorkLogItems
        [HttpGet]
        public ListResult GetWorkLogItems(
            [FromQuery] int? inventoryItemId = null,
            [FromQuery] bool countOnly = false,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from workLogItem in _context.WorkLogItems
                        select workLogItem;

            if (inventoryItemId != null) {
                query = query.Where(ii => ii.InventoryItemId == inventoryItemId);
            }
            // if(hideRelated){
            //     query = query.Where(ii => ii.ChildRelatedWorkLogItems.Contains() )
            // }

            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }

            if (countOnly == true) {
                return new ListResult {
                    Items = null,
                    Count = query.Count()
                };
            }

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }


        // GET: WorkLogItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkLogItem([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var workLogItem = await _context.WorkLogItems
                .SingleOrDefaultAsync(item => item.Id == id);

            if (workLogItem == null) {
                return NotFound();
            }

            return Ok(workLogItem);
        }

        // PUT: WorkLogItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkLogItem([FromRoute] int? id, [FromBody] WorkLogItem workLogItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != workLogItem.Id) {
                return BadRequest();
            }

            using (var transaction = _context.Database.BeginTransaction()) {
                _context.Entry(workLogItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                transaction.Commit();
            }

            return NoContent();
        }

        // POST: WorkLogItems
        [HttpPost]
        public async Task<IActionResult> PostWorkLogItem([FromBody] WorkLogItem workLogItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            workLogItem.CreatedAt = DateTime.UtcNow;
            workLogItem.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            if(workLogItem.PerformedById == null)
                workLogItem.PerformedById = workLogItem.CreatedById;

            _context.WorkLogItems.Add(workLogItem);

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkLogItem", new { id = workLogItem.Id }, workLogItem);
        }

        // DELETE: WorkLogItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkLogItem([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var workLogItem = await _context.WorkLogItems
                .SingleOrDefaultAsync(m => m.Id == id);
            if (workLogItem == null) {
                return NotFound();
            }

            _context.WorkLogItems.Remove(workLogItem);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return Ok(workLogItem);
        }
        

        private bool WorkLogItemExists(int? id) {
            return _context.WorkLogItems.Any(e => e.Id == id);
        }
    }
}