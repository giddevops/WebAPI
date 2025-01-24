// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.EntityFrameworkCore;
// using GidIndustrial.Gideon.WebApi.Models;
// using DinkToPdf;

// namespace WebApi.Features.Controllers
// {
//     [Produces("application/json")]
//     [Route("QuoteAddressTypes")]
//     public class QuoteAddressTypesController : Controller
//     {
//         private readonly AppDBContext _context;

//         public QuoteAddressTypesController(AppDBContext context)
//         {
//             _context = context;
//         }

//         // GET: QuoteAddressTypes
//         [HttpGet]
//         public IEnumerable<QuoteAddressType> GetQuoteAddressTypes()
//         {
//             return _context.QuoteAddressTypes;
//         }

//         // GET: QuoteAddressTypes/5
//         [HttpGet("{id}")]
//         public async Task<IActionResult> GetQuoteAddressType([FromRoute] int id)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             var quoteAddressType = await _context.QuoteAddressTypes.SingleOrDefaultAsync(m => m.Id == id);

//             if (quoteAddressType == null)
//             {
//                 return NotFound();
//             }

//             return Ok(quoteAddressType);
//         }

//         // PUT: QuoteAddressTypes/5
//         [HttpPut("{id}")]
//         public async Task<IActionResult> PutQuoteAddressType([FromRoute] int id, [FromBody] QuoteAddressType quoteAddressType)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             if (id != quoteAddressType.Id)
//             {
//                 return BadRequest();
//             }

//             _context.Entry(quoteAddressType).State = EntityState.Modified;

//             try
//             {
//                 await _context.SaveChangesAsync();
//             }
//             catch (DbUpdateConcurrencyException)
//             {
//                 if (!QuoteAddressTypeExists(id))
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

//         // POST: QuoteAddressTypes
//         [HttpPost]
//         public async Task<IActionResult> PostQuoteAddressType([FromBody] QuoteAddressType quoteAddressType)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             _context.QuoteAddressTypes.Add(quoteAddressType);
//             await _context.SaveChangesAsync();

//             return CreatedAtAction("GetQuoteAddressType", new { id = quoteAddressType.Id }, quoteAddressType);
//         }

//         // DELETE: QuoteAddressTypes/5
//         [HttpDelete("{id}")]
//         public async Task<IActionResult> DeleteQuoteAddressType([FromRoute] int id)
//         {
//             if (!ModelState.IsValid)
//             {
//                 return BadRequest(ModelState);
//             }

//             var quoteAddressType = await _context.QuoteAddressTypes.SingleOrDefaultAsync(m => m.Id == id);
//             if (quoteAddressType == null)
//             {
//                 return NotFound();
//             }

//             _context.QuoteAddressTypes.Remove(quoteAddressType);
//             await _context.SaveChangesAsync();

//             return Ok(quoteAddressType);
//         }

//         private bool QuoteAddressTypeExists(int id)
//         {
//             return _context.QuoteAddressTypes.Any(e => e.Id == id);
//         }
//     }
// }