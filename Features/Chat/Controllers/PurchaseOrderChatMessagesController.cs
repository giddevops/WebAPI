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
    [Route("PurchaseOrderChatMessages")]
    public class PurchaseOrderChatMessagesController : Controller {
        private readonly AppDBContext _context;

        public PurchaseOrderChatMessagesController(AppDBContext context) {
            _context = context;
        }

        // GET: PurchaseOrderChatMessages?purchaseOrderId=&chatMessageId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="purchaseOrderId"></param>
        /// <param name="chatMessageId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetPurchaseOrderChatMessageById([FromQuery] int? purchaseOrderId, [FromQuery] int? chatMessageId) {
            var purchaseOrderChatMessage = await _context.PurchaseOrderChatMessages
                .Include(item => item.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.PurchaseOrderId == purchaseOrderId);



            if (purchaseOrderChatMessage == null) {
                return NotFound();
            }

            return Ok(purchaseOrderChatMessage);
        }

        // GET: PurchaseOrderChatMessages
        [HttpGet]
        public IActionResult GetPurchaseOrderChatMessages(
            [FromQuery] int? purchaseOrderId, [FromQuery] int? chatMessageId
        // [FromQuery] int skip = 0,
        // [FromQuery] int perPage = 10
        ) {
            var query = from purchaseOrderChatMessage in _context.PurchaseOrderChatMessages select purchaseOrderChatMessage;

            if (purchaseOrderId != null) {
                query = query.Where(pois => pois.PurchaseOrderId == purchaseOrderId);
            } else {
                return BadRequest(new {
                    Error = "purchaseOrderId cannot be null"
                });
            }

            query = query
                .Include(q => q.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment);

            return Ok(query);
        }

        // PUT: PurchaseOrderChatMessages?chatMessageId=&purchaseOrderId=
        [HttpPut]
        public async Task<IActionResult> PutPurchaseOrderChatMessage([FromQuery] int? chatMessageId, [FromQuery] int? purchaseOrderId, [FromBody] PurchaseOrderChatMessage purchaseOrderChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(purchaseOrderChatMessage).State = EntityState.Modified;

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

        // POST: PurchaseOrderChatMessages
        [HttpPost]
        public async Task<IActionResult> PostPurchaseOrderChatMessage([FromBody] PurchaseOrderChatMessage purchaseOrderChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            purchaseOrderChatMessage.ChatMessage.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            purchaseOrderChatMessage.ChatMessage.CreatedAt = DateTime.UtcNow;
            if (String.IsNullOrWhiteSpace(purchaseOrderChatMessage.ChatMessage.Message)) {
                return BadRequest("Empty message");
            }

            //sanitize input so if somebody put in malicious code like a <script> tag or something it won't get displayed.
            var sanitizer = new HtmlSanitizer();
            purchaseOrderChatMessage.ChatMessage.Message = sanitizer.Sanitize(purchaseOrderChatMessage.ChatMessage.Message);
            _context.PurchaseOrderChatMessages.Add(purchaseOrderChatMessage);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (PurchaseOrderChatMessageExists(purchaseOrderChatMessage.PurchaseOrderId, purchaseOrderChatMessage.ChatMessageId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            //need to return all related properties
            var query = from item in _context.PurchaseOrderChatMessages select item;
            query = query
                .Where(item => item.PurchaseOrderId == purchaseOrderChatMessage.PurchaseOrderId && item.ChatMessageId == purchaseOrderChatMessage.ChatMessageId)
                .Include(q => q.ChatMessage)
                .Include(q => q.PurchaseOrder);

            purchaseOrderChatMessage = await query.FirstOrDefaultAsync();

            //send emails to anybody mentioned
            var currentUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(User));
            var mentionedUsers = await purchaseOrderChatMessage.ChatMessage.GetMentionedUsers(_context);
            mentionedUsers.ForEach(user => user.SendMentionedInChatEmail(
                purchaseOrderChatMessage.ChatMessage,
                currentUser,
                _context,
                $"Purchase Order #{purchaseOrderChatMessage.PurchaseOrderId}",
                $"https://gideon.gidindustrial.com/purchase-orders/{purchaseOrderChatMessage.PurchaseOrderId}?PurchaseOrderPageTab=Chat"
            ));
            
            mentionedUsers.ForEach(user => _context.ChatMessageUserMentions.Add(new ChatMessageUserMention{
                UserId=user.Id,
                ChatMessageId=purchaseOrderChatMessage.ChatMessageId
            }));
            await _context.SaveChangesAsync();
            
            if (purchaseOrderChatMessage.ChatMessage.InReplyToChatMessageId != null) {
                var originalChatMessage = await _context.ChatMessages.FirstOrDefaultAsync(item => item.Id == purchaseOrderChatMessage.ChatMessage.InReplyToChatMessageId);
                var originalUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == originalChatMessage.CreatedById);
                if (originalUser.Id != currentUser.Id) {
                    originalUser.SendChatMessageResponseEmail(
                        purchaseOrderChatMessage.ChatMessage,
                        currentUser,
                        _context,
                        $"Purchase Order #{purchaseOrderChatMessage.PurchaseOrderId}",
                        $"https://gideon.gidindustrial.com/purchase-orders/{purchaseOrderChatMessage.PurchaseOrderId}?PurchaseOrderPageTab=Chat"
                    );
                }
            }

            return CreatedAtAction("GetPurchaseOrderChatMessage", new { id = purchaseOrderChatMessage.ChatMessageId }, purchaseOrderChatMessage);
        }

        // DELETE: PurchaseOrderChatMessages?purchaseOrderId=&chatMessageid=
        [HttpDelete]
        public async Task<IActionResult> DeletePurchaseOrderChatMessage([FromQuery] int? purchaseOrderId, [FromQuery] int? chatMessageId) {
            if (purchaseOrderId == null) {
                return BadRequest(new {
                    Error = "purchaseOrderId querystring parameter is required"
                });
            }
            if (chatMessageId == null) {
                return BadRequest(new {
                    Error = "purchaseOrderId querystring parameter is required"
                });
            }

            var purchaseOrderChatMessage = await _context.PurchaseOrderChatMessages.FirstOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.PurchaseOrderId == purchaseOrderId);

            if (purchaseOrderChatMessage == null) {
                return NotFound();
            }

            _context.PurchaseOrderChatMessages.Remove(purchaseOrderChatMessage);
            await _context.SaveChangesAsync();

            return Ok(purchaseOrderChatMessage);
        }

        private bool PurchaseOrderChatMessageExists(int? purchaseOrderId, int? chatMessageId) {
            return _context.PurchaseOrderChatMessages.Any(e => e.ChatMessageId == chatMessageId && e.PurchaseOrderId == purchaseOrderId);
        }
    }
}