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
//     [Route("ViewDisplayFields")]
//     public class ViewDisplayFieldsController : Controller
//     {
//         private readonly AppDBContext _context;

//         public ViewDisplayFieldsController(AppDBContext context)
//         {
//             _context = context;
//         }

//         // GET: ViewDisplayFields
//         [HttpGet]
//         public IEnumerable<ViewDisplayField> GetViewDisplayFields()
//         {
//             return _context.ViewDisplayFields;
//         }

//         // GET: ViewDisplayFields/5
//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetViewDisplayField([FromRoute] int id)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             var viewDisplayField = await _context.ViewDisplayFields.SingleOrDefaultAsync(m => m.Id == id);

//             if (viewDisplayField == null)
//             {
//                 return NotFound();
//             }

//             return Ok(viewDisplayField);
//         }

//         // PUT: ViewDisplayFields/5
//         [HttpPut("{id}")]
//         [RequirePermission("EditDropdownOptions")]
//         public async Task<IActionResult> PutViewDisplayField([FromRoute] int id, [FromBody] ViewDisplayField viewDisplayField)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             if (id != viewDisplayField.Id)
//             {
//                 return BadRequest();
//             }

//             _context.Entry(viewDisplayField).State = EntityState.Modified;

//             try
//             {
//                 await _context.SaveChangesAsync();
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!ViewDisplayFieldExists(id))
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

//         // POST: ViewDisplayFields
//         [HttpPost]
//         [RequirePermission("EditDropdownOptions")]
//         public async Task<IActionResult> PostViewDisplayField([FromBody] ViewDisplayField viewDisplayField)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             _context.ViewDisplayFields.Add(viewDisplayField);
//             await _context.SaveChangesAsync();

//             return CreatedAtAction("GetViewDisplayField", new { id = viewDisplayField.Id }, viewDisplayField);
//         }

//         // DELETE: ViewDisplayFields/5
//         [HttpDelete("{id}")]
//         [RequirePermission("EditDropdownOptions")]
//         public async Task<IActionResult> DeleteViewDisplayField([FromRoute] int id)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             var viewDisplayField = await _context.ViewDisplayFields.SingleOrDefaultAsync(m => m.Id == id);
//             if (viewDisplayField == null)
//             {
//                 return NotFound();
//             }

//             _context.ViewDisplayFields.Remove(viewDisplayField);
//             await _context.SaveChangesAsync();

//             return Ok(viewDisplayField);
//         }

//         private bool ViewDisplayFieldExists(int id)
//         {
//             return _context.ViewDisplayFields.Any(e => e.Id == id);
//         }
//     }
// }