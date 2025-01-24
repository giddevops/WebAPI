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
    [Route("RmaChatMessages")]
    public class RmaChatMessagesController : Controller {
        private readonly AppDBContext _context;

        public RmaChatMessagesController(AppDBContext context) {
            _context = context;
        }

        // GET: RmaChatMessages?rmaId=&chatMessageId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="rmaId"></param>
        /// <param name="chatMessageId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetRmaChatMessageById([FromQuery] int? rmaId, [FromQuery] int? chatMessageId) {
            var rmaChatMessage = await _context.RmaChatMessages
                .Include(item => item.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.RmaId == rmaId);



            if (rmaChatMessage == null) {
                return NotFound();
            }

            return Ok(rmaChatMessage);
        }

        // GET: RmaChatMessages
        [HttpGet]
        public IActionResult GetRmaChatMessages(
            [FromQuery] int? rmaId, [FromQuery] int? chatMessageId
        // [FromQuery] int skip = 0,
        // [FromQuery] int perPage = 10
        ) {
            var query = from rmaChatMessage in _context.RmaChatMessages select rmaChatMessage;

            if (rmaId != null) {
                query = query.Where(pois => pois.RmaId == rmaId);
            } else {
                return BadRequest(new {
                    Error = "rmaId cannot be null"
                });
            }

            query = query
                .Include(q => q.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment);

            return Ok(query);
        }

        // PUT: RmaChatMessages?chatMessageId=&rmaId=
        [HttpPut]
        public async Task<IActionResult> PutRmaChatMessage([FromQuery] int? chatMessageId, [FromQuery] int? rmaId, [FromBody] RmaChatMessage rmaChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(rmaChatMessage).State = EntityState.Modified;

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

        // POST: RmaChatMessages
        [HttpPost]
        public async Task<IActionResult> PostRmaChatMessage([FromBody] RmaChatMessage rmaChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            rmaChatMessage.ChatMessage.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            rmaChatMessage.ChatMessage.CreatedAt = DateTime.UtcNow;
            if (String.IsNullOrWhiteSpace(rmaChatMessage.ChatMessage.Message)) {
                return BadRequest("Empty message");
            }

            //sanitize input so if somebody put in malicious code like a <script> tag or something it won't get displayed.
            var sanitizer = new HtmlSanitizer();
            rmaChatMessage.ChatMessage.Message = sanitizer.Sanitize(rmaChatMessage.ChatMessage.Message);
            _context.RmaChatMessages.Add(rmaChatMessage);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (RmaChatMessageExists(rmaChatMessage.RmaId, rmaChatMessage.ChatMessageId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            //need to return all related properties
            var query = from item in _context.RmaChatMessages select item;
            query = query
                .Where(item => item.RmaId == rmaChatMessage.RmaId && item.ChatMessageId == rmaChatMessage.ChatMessageId)
                .Include(q => q.ChatMessage)
                .Include(q => q.Rma);

            rmaChatMessage = await query.FirstOrDefaultAsync();

            //send emails to anybody mentioned
            var currentUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(User));
            var mentionedUsers = await rmaChatMessage.ChatMessage.GetMentionedUsers(_context);
            mentionedUsers.ForEach(user => user.SendMentionedInChatEmail(
                rmaChatMessage.ChatMessage,
                currentUser,
                _context,
                $"Rma #{rmaChatMessage.RmaId}",
                $"https://gideon.gidindustrial.com/rmas/{rmaChatMessage.RmaId}?RmaPageTab=Chat"
            ));

            mentionedUsers.ForEach(user => _context.ChatMessageUserMentions.Add(new ChatMessageUserMention{
                UserId=user.Id,
                ChatMessageId=rmaChatMessage.ChatMessageId
            }));
            await _context.SaveChangesAsync();

            if (rmaChatMessage.ChatMessage.InReplyToChatMessageId != null) {
                var originalChatMessage = await _context.ChatMessages.FirstOrDefaultAsync(item => item.Id == rmaChatMessage.ChatMessage.InReplyToChatMessageId);
                var originalUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == originalChatMessage.CreatedById);
                if (originalUser.Id != currentUser.Id) {
                    originalUser.SendChatMessageResponseEmail(
                        rmaChatMessage.ChatMessage,
                        currentUser,
                        _context,
                        $"RMA #{rmaChatMessage.RmaId}",
                        $"https://gideon.gidindustrial.com/rmas/{rmaChatMessage.RmaId}?RmaPageTab=Chat"
                    );
                }
            }

            return CreatedAtAction("GetRmaChatMessage", new { id = rmaChatMessage.ChatMessageId }, rmaChatMessage);
        }

        // DELETE: RmaChatMessages?rmaId=&chatMessageid=
        [HttpDelete]
        public async Task<IActionResult> DeleteRmaChatMessage([FromQuery] int? rmaId, [FromQuery] int? chatMessageId) {
            if (rmaId == null) {
                return BadRequest(new {
                    Error = "rmaId querystring parameter is required"
                });
            }
            if (chatMessageId == null) {
                return BadRequest(new {
                    Error = "rmaId querystring parameter is required"
                });
            }

            var rmaChatMessage = await _context.RmaChatMessages.FirstOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.RmaId == rmaId);

            if (rmaChatMessage == null) {
                return NotFound();
            }

            _context.RmaChatMessages.Remove(rmaChatMessage);
            await _context.SaveChangesAsync();

            return Ok(rmaChatMessage);
        }

        private bool RmaChatMessageExists(int? rmaId, int? chatMessageId) {
            return _context.RmaChatMessages.Any(e => e.ChatMessageId == chatMessageId && e.RmaId == rmaId);
        }
    }
}