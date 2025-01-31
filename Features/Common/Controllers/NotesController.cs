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
    [Route("Notes")]
    public class NotesController : Controller
    {
        private readonly AppDBContext _context;

        public NotesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: Notes
        [HttpGet]
        public async Task<IActionResult> GetNote([FromQuery] int? LeadId, [FromQuery] int? QuoteId, [FromQuery] int? ContactId, [FromQuery] int? CompanyId, [FromQuery] int? ProductId, [FromQuery] int? InventoryItemId, [FromQuery] int? SourceId, [FromQuery] int? SalesOrderId, [FromQuery] int? PurchaseOrderId, [FromQuery] int? RmaId)
        {
            IEnumerable<Note> Notes = null;
            if (LeadId != null)
            {
                Notes = await _context.LeadNotes
                    .Where(leadNote => leadNote.LeadId == LeadId)
                    .Include(l => l.Note)
                    .Select(l => l.Note).ToListAsync();
            }
            else if (QuoteId != null)
            {
                Notes = await _context.QuoteNotes
                    .Where(quoteNote => quoteNote.QuoteId == QuoteId)
                    .Include(l => l.Note)
                    .Select(l => l.Note).ToListAsync();
            }
            else if (ContactId != null)
            {
                Notes = await _context.ContactNotes
                    .Where(contactNotes => contactNotes.ContactId == ContactId)
                    .Include(l => l.Note)
                    .Select(l => l.Note).ToListAsync();
            }
            else if (CompanyId != null)
            {
                Notes = await _context.CompanyNotes
                    .Where(companyNote => companyNote.CompanyId == CompanyId)
                    .Include(l => l.Note)
                    .Select(l => l.Note).ToListAsync();
            }
            else if (ProductId != null)
            {
                Notes = await _context.ProductNotes
                    .Where(productNote => productNote.ProductId == ProductId)
                    .Include(l => l.Note)
                    .Select(l => l.Note).ToListAsync();
            }
            else if (InventoryItemId != null)
            {
                Notes = await _context.InventoryItemNotes
                    .Where(inventoryItemNote => inventoryItemNote.InventoryItemId == InventoryItemId)
                    .Include(l => l.Note)
                    .Select(l => l.Note).ToListAsync();
            }
            else if (SalesOrderId != null)
            {
                Notes = await _context.SalesOrderNotes
                    .Where(salesOrderNote => salesOrderNote.SalesOrderId == SalesOrderId)
                    .Include(l => l.Note)
                    .Select(l => l.Note).ToListAsync();
            }
            else if (PurchaseOrderId != null)
            {
                Notes = await _context.PurchaseOrderNotes
                    .Where(salesOrderNote => salesOrderNote.PurchaseOrderId == PurchaseOrderId)
                    .Include(l => l.Note)
                    .Select(l => l.Note).ToListAsync();
            }
            else if (RmaId != null)
            {
                Notes = await _context.RmaNotes
                    .Where(note => note.RmaId == RmaId)
                    .Include(l => l.Note)
                    .Select(l => l.Note).ToListAsync();
            }
            else if (SourceId != null)
            {
                Notes = await _context.SourceNotes
                    .Where(salesOrderNote => salesOrderNote.SourceId == SourceId)
                    .Include(l => l.Note)
                    .Select(l => l.Note).ToListAsync();
            }
            else
            {
                return BadRequest(new {
                    Error = "No item specified in the querystring"
                });
            }
            return Ok(Notes);
        }

        // GET: Notes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNote([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var note = await _context.Notes.SingleOrDefaultAsync(m => m.Id == id);

            if (note == null)
            {
                return NotFound();
            }

            return Ok(note);
        }

        // PUT: Notes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNote([FromRoute] int id, [FromBody] Note note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != note.Id)
            {
                return BadRequest();
            }

            note.UpdatedAt = DateTime.UtcNow;

            _context.Entry(note).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NoteExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(note);
        }

        // POST: Notes
        [HttpPost]
        public async Task<IActionResult> PostNote([FromBody] Note note)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            note.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            _context.Notes.Add(note);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNote", new { id = note.Id }, note);
        }

        // DELETE: Notes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var note = await _context.Notes.SingleOrDefaultAsync(m => m.Id == id);
            if (note == null)
            {
                return NotFound();
            }

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return Ok(note);
        }

        private bool NoteExists(int id)
        {
            return _context.Notes.Any(e => e.Id == id);
        }
    }
}