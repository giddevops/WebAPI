using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Ganss.XSS;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("InvoiceChatMessages")]
    public class InvoiceChatMessagesController : Controller {
        private readonly AppDBContext _context;

        public InvoiceChatMessagesController(AppDBContext context) {
            _context = context;
        }

        // GET: InvoiceChatMessages?invoiceId=&chatMessageId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="chatMessageId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetInvoiceChatMessageById([FromQuery] int? invoiceId, [FromQuery] int? chatMessageId) {
            var invoiceChatMessage = await _context.InvoiceChatMessages
                .Include(item => item.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.InvoiceId == invoiceId);



            if (invoiceChatMessage == null) {
                return NotFound();
            }

            return Ok(invoiceChatMessage);
        }

        // GET: InvoiceChatMessages
        [HttpGet]
        public IActionResult GetInvoiceChatMessages(
            [FromQuery] int? invoiceId, [FromQuery] int? chatMessageId
        // [FromQuery] int skip = 0,
        // [FromQuery] int perPage = 10
        ) {
            var query = from invoiceChatMessage in _context.InvoiceChatMessages select invoiceChatMessage;

            if (invoiceId != null) {
                query = query.Where(pois => pois.InvoiceId == invoiceId);
            } else {
                return BadRequest(new {
                    Error = "invoiceId cannot be null"
                });
            }

            query = query
                .Include(q => q.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment);

            return Ok(query);
        }

        // PUT: InvoiceChatMessages?chatMessageId=&invoiceId=
        [HttpPut]
        public async Task<IActionResult> PutInvoiceChatMessage([FromQuery] int? chatMessageId, [FromQuery] int? invoiceId, [FromBody] InvoiceChatMessage invoiceChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(invoiceChatMessage).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: InvoiceChatMessages
        [HttpPost]
        public async Task<IActionResult> PostInvoiceChatMessage([FromBody] InvoiceChatMessage invoiceChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            invoiceChatMessage.ChatMessage.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            invoiceChatMessage.ChatMessage.CreatedAt = DateTime.UtcNow;
            if (String.IsNullOrWhiteSpace(invoiceChatMessage.ChatMessage.Message)) {
                return BadRequest("Empty message");
            }

            //sanitize input so if somebody put in malicious code like a <script> tag or something it won't get displayed.
            var sanitizer = new HtmlSanitizer();
            invoiceChatMessage.ChatMessage.Message = sanitizer.Sanitize(invoiceChatMessage.ChatMessage.Message);
            _context.InvoiceChatMessages.Add(invoiceChatMessage);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (InvoiceChatMessageExists(invoiceChatMessage.InvoiceId, invoiceChatMessage.ChatMessageId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            //need to return all related properties
            var query = from item in _context.InvoiceChatMessages select item;
            query = query
                .Where(item => item.InvoiceId == invoiceChatMessage.InvoiceId && item.ChatMessageId == invoiceChatMessage.ChatMessageId)
                .Include(q => q.ChatMessage)
                .Include(q => q.Invoice);

            invoiceChatMessage = await query.FirstOrDefaultAsync();


            //send emails to anybody mentioned
            var currentUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(User));
            var mentionedUsers = await invoiceChatMessage.ChatMessage.GetMentionedUsers(_context);
            mentionedUsers.ForEach(user => user.SendMentionedInChatEmail(
                invoiceChatMessage.ChatMessage,
                currentUser,
                _context,
                $"Invoice #{invoiceChatMessage.InvoiceId}",
                $"https://gideon.gidindustrial.com/invoices/{invoiceChatMessage.InvoiceId}?InvoicePageTab=Chat"
            ));

            mentionedUsers.ForEach(user => _context.ChatMessageUserMentions.Add(new ChatMessageUserMention{
                UserId=user.Id,
                ChatMessageId=invoiceChatMessage.ChatMessageId
            }));
            await _context.SaveChangesAsync();

            if (invoiceChatMessage.ChatMessage.InReplyToChatMessageId != null) {
                var originalChatMessage = await _context.ChatMessages.FirstOrDefaultAsync(item => item.Id == invoiceChatMessage.ChatMessage.InReplyToChatMessageId);
                var originalUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == originalChatMessage.CreatedById);
                if (originalUser.Id != currentUser.Id) {
                    originalUser.SendChatMessageResponseEmail(
                        invoiceChatMessage.ChatMessage,
                        currentUser,
                        _context,
                        $"Invoice #{invoiceChatMessage.InvoiceId}",
                        $"https://gideon.gidindustrial.com/invoices/{invoiceChatMessage.InvoiceId}?InvoicePageTab=Chat"
                    );
                }
            }

            return CreatedAtAction("GetInvoiceChatMessage", new { id = invoiceChatMessage.ChatMessageId }, invoiceChatMessage);
        }

        // DELETE: InvoiceChatMessages?invoiceId=&chatMessageid=
        [HttpDelete]
        public async Task<IActionResult> DeleteInvoiceChatMessage([FromQuery] int? invoiceId, [FromQuery] int? chatMessageId) {
            if (invoiceId == null) {
                return BadRequest(new {
                    Error = "invoiceId querystring parameter is required"
                });
            }
            if (chatMessageId == null) {
                return BadRequest(new {
                    Error = "invoiceId querystring parameter is required"
                });
            }

            var invoiceChatMessage = await _context.InvoiceChatMessages.FirstOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.InvoiceId == invoiceId);

            if (invoiceChatMessage == null) {
                return NotFound();
            }

            _context.InvoiceChatMessages.Remove(invoiceChatMessage);
            await _context.SaveChangesAsync();

            return Ok(invoiceChatMessage);
        }

        private bool InvoiceChatMessageExists(int? invoiceId, int? chatMessageId) {
            return _context.InvoiceChatMessages.Any(e => e.ChatMessageId == chatMessageId && e.InvoiceId == invoiceId);
        }
    }
}