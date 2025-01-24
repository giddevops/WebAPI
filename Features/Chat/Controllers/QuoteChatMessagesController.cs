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
    [Route("QuoteChatMessages")]
    public class QuoteChatMessagesController : Controller {
        private readonly AppDBContext _context;

        public QuoteChatMessagesController(AppDBContext context) {
            _context = context;
        }

        // GET: QuoteChatMessages?quoteId=&chatMessageId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="quoteId"></param>
        /// <param name="chatMessageId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetQuoteChatMessageById([FromQuery] int? quoteId, [FromQuery] int? chatMessageId) {
            var quoteChatMessage = await _context.QuoteChatMessages
                .Include(item => item.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.QuoteId == quoteId);



            if (quoteChatMessage == null) {
                return NotFound();
            }

            return Ok(quoteChatMessage);
        }

        // GET: QuoteChatMessages
        [HttpGet]
        public IActionResult GetQuoteChatMessages(
            [FromQuery] int? quoteId, [FromQuery] int? chatMessageId
        // [FromQuery] int skip = 0,
        // [FromQuery] int perPage = 10
        ) {
            var query = from quoteChatMessage in _context.QuoteChatMessages select quoteChatMessage;

            if (quoteId != null) {
                query = query.Where(pois => pois.QuoteId == quoteId);
            } else {
                return BadRequest(new {
                    Error = "quoteId cannot be null"
                });
            }

            query = query
                .Include(q => q.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment);

            return Ok(query);
        }

        // PUT: QuoteChatMessages?chatMessageId=&quoteId=
        [HttpPut]
        public async Task<IActionResult> PutQuoteChatMessage([FromQuery] int? chatMessageId, [FromQuery] int? quoteId, [FromBody] QuoteChatMessage quoteChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(quoteChatMessage).State = EntityState.Modified;

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

        // POST: QuoteChatMessages
        [HttpPost]
        public async Task<IActionResult> PostQuoteChatMessage([FromBody] QuoteChatMessage quoteChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            quoteChatMessage.ChatMessage.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            quoteChatMessage.ChatMessage.CreatedAt = DateTime.UtcNow;
            if (String.IsNullOrWhiteSpace(quoteChatMessage.ChatMessage.Message)) {
                return BadRequest("Empty message");
            }

            //sanitize input so if somebody put in malicious code like a <script> tag or something it won't get displayed.
            var sanitizer = new HtmlSanitizer();
            quoteChatMessage.ChatMessage.Message = sanitizer.Sanitize(quoteChatMessage.ChatMessage.Message);
            _context.QuoteChatMessages.Add(quoteChatMessage);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (QuoteChatMessageExists(quoteChatMessage.QuoteId, quoteChatMessage.ChatMessageId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            //need to return all related properties
            var query = from item in _context.QuoteChatMessages select item;
            query = query
                .Where(item => item.QuoteId == quoteChatMessage.QuoteId && item.ChatMessageId == quoteChatMessage.ChatMessageId)
                .Include(q => q.ChatMessage)
                .Include(q => q.Quote);

            quoteChatMessage = await query.FirstOrDefaultAsync();


            //send emails to anybody mentioned
            var currentUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(User));
            var mentionedUsers = await quoteChatMessage.ChatMessage.GetMentionedUsers(_context);
            mentionedUsers.ForEach(user => user.SendMentionedInChatEmail(
                quoteChatMessage.ChatMessage,
                currentUser,
                _context,
                $"Quote #{quoteChatMessage.QuoteId}",
                $"https://gideon.gidindustrial.com/quotes/{quoteChatMessage.QuoteId}?QuotePageTab=Chat"
            ));
            mentionedUsers.ForEach(user => _context.ChatMessageUserMentions.Add(new ChatMessageUserMention{
                UserId=user.Id,
                ChatMessageId=quoteChatMessage.ChatMessageId
            }));
            await _context.SaveChangesAsync();

            if (quoteChatMessage.ChatMessage.InReplyToChatMessageId != null) {
                var originalChatMessage = await _context.ChatMessages.FirstOrDefaultAsync(item => item.Id == quoteChatMessage.ChatMessage.InReplyToChatMessageId);
                var originalUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == originalChatMessage.CreatedById);
                if (originalUser.Id != currentUser.Id) {
                    originalUser.SendChatMessageResponseEmail(
                        quoteChatMessage.ChatMessage,
                        currentUser,
                        _context,
                        $"Quote #{quoteChatMessage.QuoteId}",
                        $"https://gideon.gidindustrial.com/quotes/{quoteChatMessage.QuoteId}?QuotePageTab=Chat"
                    );
                }
            }

            return CreatedAtAction("GetQuoteChatMessage", new { id = quoteChatMessage.ChatMessageId }, quoteChatMessage);
        }

        // DELETE: QuoteChatMessages?quoteId=&chatMessageid=
        [HttpDelete]
        public async Task<IActionResult> DeleteQuoteChatMessage([FromQuery] int? quoteId, [FromQuery] int? chatMessageId) {
            if (quoteId == null) {
                return BadRequest(new {
                    Error = "quoteId querystring parameter is required"
                });
            }
            if (chatMessageId == null) {
                return BadRequest(new {
                    Error = "quoteId querystring parameter is required"
                });
            }

            var quoteChatMessage = await _context.QuoteChatMessages.FirstOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.QuoteId == quoteId);

            if (quoteChatMessage == null) {
                return NotFound();
            }

            _context.QuoteChatMessages.Remove(quoteChatMessage);
            await _context.SaveChangesAsync();

            return Ok(quoteChatMessage);
        }

        private bool QuoteChatMessageExists(int? quoteId, int? chatMessageId) {
            return _context.QuoteChatMessages.Any(e => e.ChatMessageId == chatMessageId && e.QuoteId == quoteId);
        }
    }
}