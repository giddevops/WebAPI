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
    [Route("EmailTemplateTypes")]
    public class EmailTemplateTypesController : Controller
    {
        private readonly AppDBContext _context;

        public EmailTemplateTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: EmailTemplateTypes
        [HttpGet]
        public IEnumerable<EmailTemplateType> GetEmailTemplateTypes()
        {
            return _context.EmailTemplateTypes.OrderBy(item => item.Value);
        }

        // GET: EmailTemplateTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmailTemplateType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailTemplateType = await _context.EmailTemplateTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (emailTemplateType == null)
            {
                return NotFound();
            }

            return Ok(emailTemplateType);
        }

        // PUT: EmailTemplateTypes/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmailTemplateType([FromRoute] int id, [FromBody] EmailTemplateType emailTemplateType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != emailTemplateType.Id)
            {
                return BadRequest();
            }

            _context.Entry(emailTemplateType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailTemplateTypeExists(id))
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

        // POST: EmailTemplateTypes
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostEmailTemplateType([FromBody] EmailTemplateType emailTemplateType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.EmailTemplateTypes.Add(emailTemplateType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmailTemplateType", new { id = emailTemplateType.Id }, emailTemplateType);
        }

        // DELETE: EmailTemplateTypes/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmailTemplateType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailTemplateType = await _context.EmailTemplateTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (emailTemplateType == null)
            {
                return NotFound();
            }

            _context.EmailTemplateTypes.Remove(emailTemplateType);
            await _context.SaveChangesAsync();

            return Ok(emailTemplateType);
        }

        private bool EmailTemplateTypeExists(int id)
        {
            return _context.EmailTemplateTypes.Any(e => e.Id == id);
        }
    }
}