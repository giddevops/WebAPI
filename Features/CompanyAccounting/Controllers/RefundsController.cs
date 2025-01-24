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
//     [Route("Refunds")]
//     public class RefundsController : Controller
//     {
//         private readonly AppDBContext _context;

//         public RefundsController(AppDBContext context)
//         {
//             _context = context;
//         }

//         // GET: Refunds
//         [HttpGet]
//         public ListResult GetRefunds(
//             [FromQuery] int skip = 0,
//             [FromQuery] int perPage = 10
//         ){
//             var query = from refund in _context.Refunds select refund;

//             query = query
//                 .OrderByDescending(item => item.CreatedAt);

//             return new ListResult {
//                 Items = query.Skip(skip).Take(perPage),
//                 Count = query.Count()
//             };
//         }

//         // GET: Refunds/5
//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetRefund([FromRoute] int id)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             var refund = await _context.Refunds.SingleOrDefaultAsync(m => m.Id == id);

//             if (refund == null)
//             {
//                 return NotFound();
//             }

//             return Ok(refund);
//         }

//         // PUT: Refunds/5
//         [HttpPut("{id}")]
//         public async Task<IActionResult> PutRefund([FromRoute] int id, [FromBody] Refund refund)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             if (id != refund.Id)
//             {
//                 return BadRequest();
//             }

//             _context.Entry(refund).State = EntityState.Modified;

//             try
//             {
//                 await _context.SaveChangesAsync();
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!RefundExists(id))
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

//         // POST: Refunds
//         [HttpPost]
//         public async Task<IActionResult> PostRefund([FromBody] Refund refund)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }
//             refund.CreatedAt = DateTime.UtcNow;
//             _context.Refunds.Add(refund);
//             await _context.SaveChangesAsync();

//             return CreatedAtAction("GetRefund", new { id = refund.Id }, refund);
//         }

//         // DELETE: Refunds/5
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteRefund([FromRoute] int id)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             var refund = await _context.Refunds.SingleOrDefaultAsync(m => m.Id == id);
//             if (refund == null)
//             {
//                 return NotFound();
//             }

//             _context.Refunds.Remove(refund);
//             await _context.SaveChangesAsync();

//             return Ok(refund);
//         }

//         private bool RefundExists(int id)
//         {
//             return _context.Refunds.Any(e => e.Id == id);
//         }
//     }
// }