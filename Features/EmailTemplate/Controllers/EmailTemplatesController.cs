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
    [Route("EmailTemplates")]
    public class EmailTemplatesController : Controller
    {
        private readonly AppDBContext _context;

        public EmailTemplatesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: EmailTemplates
        [HttpGet]
        public ListResult GetEmailTemplates(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] DateTime? createdAtStartDate = null,
            [FromQuery] DateTime? createdAtEndDate = null,
            [FromQuery] Boolean fetchRelated = false
        )
        {
            IQueryable<EmailTemplate> query = from emailTemplate in _context.EmailTemplates select emailTemplate;

            if (createdAtStartDate != null)
                query = query.Where(l => l.CreatedAt >= createdAtStartDate);
            if (createdAtEndDate != null)
                query = query.Where(l => l.CreatedAt <= createdAtEndDate);


            query = query.OrderByDescending(l => l.CreatedAt);

            return new ListResult
            {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: EmailTemplates/5
        [HttpGet("DefaultTemplate")]
        public async Task<IActionResult> GetDefaultEmailTemplate([FromQuery] string type)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            EmailTemplate emailTemplate;

            dynamic mainItem;

            if (type == "quote")
            {
                var quoteTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "quote");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == quoteTemplateType.Id);
            }
            else if (type == "purchase-order")
            {
                var purchaseOrderTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "purchase order");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == purchaseOrderTemplateType.Id);
            }
            else if (type == "sales-order")
            {
                var salesOrderTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "sales order");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == salesOrderTemplateType.Id);
            }
            else if (type == "cancel-sales-order")
            {
                var cancelSalesOrderTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "cancel sales order");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == cancelSalesOrderTemplateType.Id);
            }
            else if (type == "cancel-purchase-order")
            {
                var purchaseOrderTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "cancel purchase order");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == purchaseOrderTemplateType.Id);
            }
            else if (type == "rma")
            {
                var purchaseOrderTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "rma");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == purchaseOrderTemplateType.Id);
            }
            else if (type == "pro-forma-invoice")
            {
                var purchaseOrderTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "pro forma invoice");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == purchaseOrderTemplateType.Id);
            }
            else if (type == "lead-no-bid")
            {
                var leadNoBidTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "lead no bid");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == leadNoBidTemplateType.Id);
            }
            else if (type == "lead-repair")
            {
                var leadRepairTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "lead repair");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == leadRepairTemplateType.Id);
            }
            else if (type == "lead-product-unavailable")
            {
                var leadProductUnavailableTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "lead product unavailable");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == leadProductUnavailableTemplateType.Id);
            }
            else if (type == "signature")
            {
                var leadNoBidTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "signature");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == leadNoBidTemplateType.Id);
            }
            else if (type == "invoice")
            {
                var leadNoBidTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "invoice");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == leadNoBidTemplateType.Id);
            }
            else if (type == "cancel-invoice")
            {
                var leadNoBidTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "cancel invoice");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == leadNoBidTemplateType.Id);
            }
            else if (type == "repair-authorization")
            {
                var repairAuthorizationTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "repair authorization");
                emailTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == repairAuthorizationTemplateType.Id);
            }
            else
            {
                return BadRequest(new {
                    Error = "the get request contained an invalid type"
                });
            }

            if (emailTemplate == null)
            {
                return NotFound();
            }

            if(type != "signature"){
                var signatureTemplateType = await _context.EmailTemplateTypes.FirstOrDefaultAsync(t => t.Value.ToLower() == "signature");
                var signatureTemplate = await _context.EmailTemplates.FirstOrDefaultAsync(et => et.EmailTemplateTypeId == signatureTemplateType.Id);
                
                emailTemplate.HtmlContent += signatureTemplate.HtmlContent;
            }

            emailTemplate.ReplaceVariables(_context, GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));

            return Ok(emailTemplate);
        }

        // GET: EmailTemplates/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmailTemplate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailTemplate = await _context.EmailTemplates.SingleOrDefaultAsync(m => m.Id == id);

            if (emailTemplate == null)
            {
                return NotFound();
            }

            return Ok(emailTemplate);
        }

        // PUT: EmailTemplates/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmailTemplate([FromRoute] int id, [FromBody] EmailTemplate emailTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != emailTemplate.Id)
            {
                return BadRequest();
            }

            _context.Entry(emailTemplate).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmailTemplateExists(id))
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

        // POST: EmailTemplates
        [HttpPost]
        public async Task<IActionResult> PostEmailTemplate([FromBody] EmailTemplate emailTemplate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.EmailTemplates.Add(emailTemplate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmailTemplate", new { id = emailTemplate.Id }, emailTemplate);
        }

        // DELETE: EmailTemplates/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmailTemplate([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var emailTemplate = await _context.EmailTemplates.SingleOrDefaultAsync(m => m.Id == id);
            if (emailTemplate == null)
            {
                return NotFound();
            }

            _context.EmailTemplates.Remove(emailTemplate);
            await _context.SaveChangesAsync();

            return Ok(emailTemplate);
        }

        private bool EmailTemplateExists(int id)
        {
            return _context.EmailTemplates.Any(e => e.Id == id);
        }
    }
}