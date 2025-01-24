//if you uncomment this, make sure you do some stuff to prevent sql injection!!


// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using GidIndustrial.Gideon.WebApi.Models;

// namespace WebApi.Features.Controllers
// {
//     [Produces("application/json")]
//     [Route("ViewFilters")]
//     public class ViewFiltersController : Controller
//     {
//         private readonly AppDBContext _context;

//         public ViewFiltersController(AppDBContext context)
//         {
//             _context = context;
//         }
        

//         // GET: ViewFilters
//         [HttpGet]
//         public IEnumerable<ViewFilter> GetViewFilters()
//         {
//             return _context.ViewFilters;
//         }

//         // GET: ViewFilters/5
//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetViewFilter([FromRoute] int id)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             var viewFilter = await _context.ViewFilters.SingleOrDefaultAsync(m => m.Id == id);

//             if (viewFilter == null)
//             {
//                 return NotFound();
//             }

//             return Ok(viewFilter);
//         }

//         // PUT: ViewFilters/5
//         [HttpPut("{id}")]
//         [RequirePermission("EditDropdownOptions")]
//         public async Task<IActionResult> PutViewFilter([FromRoute] int id, [FromBody] ViewFilter viewFilter)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             if (id != viewFilter.Id)
//             {
//                 return BadRequest();
//             }

//             _context.Entry(viewFilter).State = EntityState.Modified;

//             try
//             {
//                 await _context.SaveChangesAsync();
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!ViewFilterExists(id))
//                 {
//                     return NotFound();
//                 }
//                 else
//                 {
//                     throw;
//                 }
//             }

//             return NoContent();
//         }

//         // POST: ViewFilters
//         [HttpPost]
//         [RequirePermission("EditDropdownOptions")]
//         public async Task<IActionResult> PostViewFilter([FromBody] ViewFilter viewFilter)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             _context.ViewFilters.Add(viewFilter);
//             await _context.SaveChangesAsync();

//             return CreatedAtAction("GetViewFilter", new { id = viewFilter.Id }, viewFilter);
//         }

//         // DELETE: ViewFilters/5
//         [HttpDelete("{id}")]
//         [RequirePermission("EditDropdownOptions")]
//         public async Task<IActionResult> DeleteViewFilter([FromRoute] int id)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             var viewFilter = await _context.ViewFilters.SingleOrDefaultAsync(m => m.Id == id);
//             if (viewFilter == null)
//             {
//                 return NotFound();
//             }

//             _context.ViewFilters.Remove(viewFilter);
//             await _context.SaveChangesAsync();

//             return Ok(viewFilter);
//         }

//         private bool ViewFilterExists(int id)
//         {
//             return _context.ViewFilters.Any(e => e.Id == id);
//         }
//     }
// }