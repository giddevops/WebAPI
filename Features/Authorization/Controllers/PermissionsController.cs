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
    [Route("Permissions")]
    public class PermissionsController : Controller
    {
        private readonly AppDBContext _context;

        public PermissionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Permissions
        [HttpGet]
        [RequirePermission("ManagePermissions")]
        public ListResult GetPermissions(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ){
            IQueryable<Permission> query = from permission in _context.Permissions select permission;

            query = query.OrderByDescending(l => l.Name);

            return new ListResult{
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: Permissions/5
        [HttpGet("{id}")]
        [RequirePermission("ManagePermissions")]
        public async Task<IActionResult> GetPermission([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var permission = await _context.Permissions.SingleOrDefaultAsync(m => m.Id == id);

            if (permission == null)
            {
                return NotFound();
            }

            return Ok(permission);
        }

        // PUT: Permissions/5
        [HttpPut("{id}")]
        [RequirePermission("ManagePermissions")]
        public async Task<IActionResult> PutPermission([FromRoute] int id, [FromBody] Permission permission)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != permission.Id)
            {
                return BadRequest();
            }

            _context.Entry(permission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PermissionExists(id))
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

        // // POST: Permissions
        // [HttpPost]
        // public async Task<IActionResult> PostPermission([FromBody] Permission permission)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     _context.Permissions.Add(permission);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetPermission", new { id = permission.Id }, permission);
        // }

        // // DELETE: Permissions/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeletePermission([FromRoute] int id)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     var permission = await _context.Permissions.SingleOrDefaultAsync(m => m.Id == id);
        //     if (permission == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.Permissions.Remove(permission);
        //     await _context.SaveChangesAsync();

        //     return Ok(permission);
        // }

        [RequirePermission("ManagePermissions")]
        private bool PermissionExists(int id)
        {
            return _context.Permissions.Any(e => e.Id == id);
        }
    }
}