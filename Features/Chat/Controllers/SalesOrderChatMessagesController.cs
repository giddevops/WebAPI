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
    [Route("SalesOrderChatMessages")]
    public class SalesOrderChatMessagesController : Controller {
        private readonly AppDBContext _context;

        public SalesOrderChatMessagesController(AppDBContext context) {
            _context = context;
        }

        // GET: SalesOrderChatMessages?salesOrderId=&chatMessageId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="salesOrderId"></param>
        /// <param name="chatMessageId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetSalesOrderChatMessageById([FromQuery] int? salesOrderId, [FromQuery] int? chatMessageId) {
            var salesOrderChatMessage = await _context.SalesOrderChatMessages
                .Include(item => item.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.SalesOrderId == salesOrderId);



            if (salesOrderChatMessage == null) {
                return NotFound();
            }

            return Ok(salesOrderChatMessage);
        }

        // GET: SalesOrderChatMessages
        [HttpGet]
        public IActionResult GetSalesOrderChatMessages(
            [FromQuery] int? salesOrderId, [FromQuery] int? chatMessageId
        // [FromQuery] int skip = 0,
        // [FromQuery] int perPage = 10
        ) {
            var query = from salesOrderChatMessage in _context.SalesOrderChatMessages select salesOrderChatMessage;

            if (salesOrderId != null) {
                query = query.Where(pois => pois.SalesOrderId == salesOrderId);
            } else {
                return BadRequest(new {
                    Error = "salesOrderId cannot be null"
                });
            }

            query = query
                .Include(q => q.ChatMessage)
                    .ThenInclude(item => item.Attachments)
                        .ThenInclude(item => item.Attachment);

            return Ok(query);
        }

        // PUT: SalesOrderChatMessages?chatMessageId=&salesOrderId=
        [HttpPut]
        public async Task<IActionResult> PutSalesOrderChatMessage([FromQuery] int? chatMessageId, [FromQuery] int? salesOrderId, [FromBody] SalesOrderChatMessage salesOrderChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(salesOrderChatMessage).State = EntityState.Modified;

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

        // POST: SalesOrderChatMessages
        [HttpPost]
        public async Task<IActionResult> PostSalesOrderChatMessage([FromBody] SalesOrderChatMessage salesOrderChatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ChatMessageControllerHelper helper = new ChatMessageControllerHelper();

            return await helper.PostSalesOrderChatMessageLocal(salesOrderChatMessage, _context, this);
        }


        // DELETE: SalesOrderChatMessages?salesOrderId=&chatMessageid=
        [HttpDelete]
        public async Task<IActionResult> DeleteSalesOrderChatMessage([FromQuery] int? salesOrderId, [FromQuery] int? chatMessageId) {
            if (salesOrderId == null) {
                return BadRequest(new {
                    Error = "salesOrderId querystring parameter is required"
                });
            }
            if (chatMessageId == null) {
                return BadRequest(new {
                    Error = "salesOrderId querystring parameter is required"
                });
            }

            var salesOrderChatMessage = await _context.SalesOrderChatMessages.FirstOrDefaultAsync(m => m.ChatMessageId == chatMessageId && m.SalesOrderId == salesOrderId);

            if (salesOrderChatMessage == null) {
                return NotFound();
            }

            _context.SalesOrderChatMessages.Remove(salesOrderChatMessage);
            await _context.SaveChangesAsync();

            return Ok(salesOrderChatMessage);
        }

        public bool SalesOrderChatMessageExists(int? salesOrderId, int? chatMessageId) {
            return _context.SalesOrderChatMessages.Any(e => e.ChatMessageId == chatMessageId && e.SalesOrderId == salesOrderId);
        }
    }
}