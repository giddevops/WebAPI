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
    [Route("AttachmentTypes")]
    public class AttachmentTypesController : Controller
    {
        private readonly AppDBContext _context;

        public AttachmentTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: AttachmentTypes
        [HttpGet]
        public IEnumerable<AttachmentType> GetAttachmentTypes()
        {
            return _context.AttachmentTypes;
        }

        // GET: AttachmentTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAttachmentType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var attachmentType = await _context.AttachmentTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (attachmentType == null)
            {
                return NotFound();
            }

            return Ok(attachmentType);
        }

        // PUT: AttachmentTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutAttachmentType([FromRoute] int id, [FromBody] AttachmentType attachmentType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != attachmentType.Id)
            {
                return BadRequest();
            }

            _context.Entry(attachmentType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AttachmentTypeExists(id))
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

        // POST: AttachmentTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostAttachmentType([FromBody] AttachmentType attachmentType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.AttachmentTypes.Add(attachmentType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAttachmentType", new { id = attachmentType.Id }, attachmentType);
        }

        // DELETE: AttachmentTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteAttachmentType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var attachmentType = await _context.AttachmentTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (attachmentType == null)
            {
                return NotFound();
            }

            _context.AttachmentTypes.Remove(attachmentType);
            await _context.SaveChangesAsync();

            return Ok(attachmentType);
        }

        private bool AttachmentTypeExists(int id)
        {
            return _context.AttachmentTypes.Any(e => e.Id == id);
        }
    }
}