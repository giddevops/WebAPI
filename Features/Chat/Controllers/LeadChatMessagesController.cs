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
    [Route("LeadChatMessages")]
    public class LeadChatMessagesController : Controller {
        private readonly AppDBContext _context;

        public LeadChatMessagesController(AppDBContext context) {
            _context = context;
        }

        // GET: LeadChatMessages?leadId=&chatMessageId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="leadId"></param>
        /// <param name="chatMessageId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetLeadChatMessageById([FromQuery] int? leadId, [FromQuery] int? chatMessageId) {
            var leadChatMessage = await _context.LeadChatMessages
                .Include(item => item.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.LeadId == leadId);

            if (leadChatMessage == null) {
                return NotFound();
            }

            return Ok(leadChatMessage);
        }

        // GET: LeadChatMessages
        [HttpGet]
        public IActionResult GetLeadChatMessages(
            [FromQuery] int? leadId, [FromQuery] int? chatMessageId
        // [FromQuery] int skip = 0,
        // [FromQuery] int perPage = 10
        ) {
            var query = from leadChatMessage in _context.LeadChatMessages select leadChatMessage;

            if (leadId != null) {
                query = query.Where(pois => pois.LeadId == leadId);
            } else {
                return BadRequest(new {
                    Error = "leadId cannot be null"
                });
            }

            query = query
                .Include(q => q.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment);

            return Ok(query);
        }

        // PUT: LeadChatMessages?chatMessageId=&leadId=
        [HttpPut]
        public async Task<IActionResult> PutLeadChatMessage([FromQuery] int? chatMessageId, [FromQuery] int? leadId, [FromBody] LeadChatMessage leadChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(leadChatMessage).State = EntityState.Modified;

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

        // POST: LeadChatMessages
        [HttpPost]
        public async Task<IActionResult> PostLeadChatMessage([FromBody] LeadChatMessage leadChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            int? userId = null;
            User tempUser = null;

            if (leadChatMessage.ChatMessage.CreatedById != null)
            {
                userId = leadChatMessage.ChatMessage.CreatedById;
                tempUser = _context.Users.Where(i => i.Id == userId).FirstOrDefault<User>();  
            }
            
                
            if(userId == null)
                leadChatMessage.ChatMessage.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            leadChatMessage.ChatMessage.CreatedAt = DateTime.UtcNow;
            if (String.IsNullOrWhiteSpace(leadChatMessage.ChatMessage.Message)) {
                return BadRequest("Empty message");
            }

            //sanitize input so if somebody put in malicious code like a <script> tag or something it won't get displayed.
            var sanitizer = new HtmlSanitizer();
            leadChatMessage.ChatMessage.Message = sanitizer.Sanitize(leadChatMessage.ChatMessage.Message);
            _context.LeadChatMessages.Add(leadChatMessage);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (LeadChatMessageExists(leadChatMessage.LeadId, leadChatMessage.ChatMessageId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            //need to return all related properties
            var query = from item in _context.LeadChatMessages select item;
            query = query
                .Where(item => item.LeadId == leadChatMessage.LeadId && item.ChatMessageId == leadChatMessage.ChatMessageId)
                .Include(q => q.ChatMessage)
                .Include(q => q.Lead);

            leadChatMessage = await query.FirstOrDefaultAsync();


            //send emails to anybody mentioned
            var currentUser = userId == null ? 
                await _context.Users.FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(User)) : tempUser;
            var mentionedUsers = await leadChatMessage.ChatMessage.GetMentionedUsers(_context);
            mentionedUsers.ForEach(user => user.SendMentionedInChatEmail(
                leadChatMessage.ChatMessage,
                currentUser,
                _context,
                $"Lead #{leadChatMessage.LeadId}",
                $"https://gideon.gidindustrial.com/leads/{leadChatMessage.LeadId}?LeadPageTab=Chat"
            ));
            
            mentionedUsers.ForEach(user => _context.ChatMessageUserMentions.Add(new ChatMessageUserMention{
                UserId=user.Id,
                ChatMessageId=leadChatMessage.ChatMessageId
            }));
            await _context.SaveChangesAsync();

            if (leadChatMessage.ChatMessage.InReplyToChatMessageId != null) {
                var originalChatMessage = await _context.ChatMessages.FirstOrDefaultAsync(item => item.Id == leadChatMessage.ChatMessage.InReplyToChatMessageId);
                var originalUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == originalChatMessage.CreatedById);
                if (originalUser.Id != currentUser.Id) {
                    originalUser.SendChatMessageResponseEmail(
                        leadChatMessage.ChatMessage,
                        currentUser,
                        _context,
                        $"Lead #{leadChatMessage.LeadId}",
                        $"https://gideon.gidindustrial.com/leads/{leadChatMessage.LeadId}?LeadPageTab=Chat"
                    );
                }
            }

            return CreatedAtAction("GetLeadChatMessage", new { id = leadChatMessage.ChatMessageId }, leadChatMessage);
        }

        // DELETE: LeadChatMessages?leadId=&chatMessageid=
        [HttpDelete]
        public async Task<IActionResult> DeleteLeadChatMessage([FromQuery] int? leadId, [FromQuery] int? chatMessageId) {
            if (leadId == null) {
                return BadRequest(new {
                    Error = "leadId querystring parameter is required"
                });
            }
            if (chatMessageId == null) {
                return BadRequest(new {
                    Error = "leadId querystring parameter is required"
                });
            }

            var leadChatMessage = await _context.LeadChatMessages.FirstOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.LeadId == leadId);

            if (leadChatMessage == null) {
                return NotFound();
            }

            _context.LeadChatMessages.Remove(leadChatMessage);
            await _context.SaveChangesAsync();

            return Ok(leadChatMessage);
        }

        private bool LeadChatMessageExists(int? leadId, int? chatMessageId) {
            return _context.LeadChatMessages.Any(e => e.ChatMessageId == chatMessageId && e.LeadId == leadId);
        }
    }
}