using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("ListingsContainers")]
    public class ListingsContainersController : Controller {
        private readonly AppDBContext _context;

        public ListingsContainersController(AppDBContext context) {
            _context = context;
        }

        // GET: ListingsContainers
        [HttpGet]
        public IEnumerable<ListingsContainer> GetListingsContainers() {
            return _context.ListingsContainers;
        }

        // GET: ListingsContainers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetListingsContainer([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var listingsContainer = await _context.ListingsContainers.SingleOrDefaultAsync(m => m.Id == id);

            if (listingsContainer == null) {
                return NotFound();
            }

            return Ok(listingsContainer);
        }

        // GET: ListingsContainers/5
        [HttpGet("{id}/Refresh")]
        public async Task<IActionResult> RefreshListingsContainer([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var listingsContainer = await _context.ListingsContainers.SingleOrDefaultAsync(m => m.Id == id);

            if (listingsContainer == null) {
                return NotFound();
            }

            return Ok(listingsContainer);
        }

        // PUT: ListingsContainers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutListingsContainer([FromRoute] int id, [FromBody] ListingsContainer listingsContainer) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != listingsContainer.Id) {
                return BadRequest();
            }

            // var dbListingsContainer = await _context.ListingsContainers.SingleOrDefaultAsync(m => m.Id == id);
            // if (dbListingsContainer == null) {
            //     return NotFound();
            // }


            _context.Entry(listingsContainer).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ListingsContainerExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // // POST: ListingsContainers
        // [HttpPost]
        // public async Task<IActionResult> PostListingsContainer([FromBody] ListingsContainer listingsContainer) {
        //     if (!ModelState.IsValid) {
        //         return BadRequest(ModelState);
        //     }

        //     listingsContainer.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
        //     listingsContainer.CreatedAt = DateTime.UtcNow;

        //     _context.ListingsContainers.Add(listingsContainer);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetListingsContainer", new { id = listingsContainer.Id }, listingsContainer);
        // }

        // DELETE: ListingsContainers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListingsContainer([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var listingsContainer = await _context.ListingsContainers.SingleOrDefaultAsync(m => m.Id == id);
            if (listingsContainer == null) {
                return NotFound();
            }

            _context.ListingsContainers.Remove(listingsContainer);
            await _context.SaveChangesAsync();

            return Ok(listingsContainer);
        }

        private bool ListingsContainerExists(int id) {
            return _context.ListingsContainers.Any(e => e.Id == id);
        }
    }
}