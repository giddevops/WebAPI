using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Text.RegularExpressions;
using GidIndustrial.Gideon.WebApi.Libraries;
using GidIndustrial.Gideon.WebApi;
using QuickBooks.Models;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("ChatMessages")]
    public class ChatMessagesController : Controller {
        private readonly AppDBContext _context;
        private IConfiguration configuration;

        public ChatMessagesController(AppDBContext context, IConfiguration config) {
            _context = context;
            configuration = config;
        }

        // // GET: ChatMessage
        // [HttpGet]
        // public IEnumerable<ChatMessage> GetChatMessages()
        // {
        //     return _context.ChatMessages.OrderByDescending(item => item.CreatedAt);
        // }

        // GET: ChatMessage/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetChatMessage([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var chatMessage = await _context.ChatMessages.SingleOrDefaultAsync(m => m.Id == id);

            if (chatMessage == null) {
                return NotFound();
            }

            return Ok(chatMessage);
        }

        // PUT: ChatMessage/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutChatMessage([FromRoute] int id, [FromBody] ChatMessage chatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != chatMessage.Id) {
                return BadRequest();
            }

            var oldItem = await _context.ChatMessages.AsNoTracking()
                .Include(item => item.Leads)
                .Include(item => item.Quotes)
                .Include(item => item.PurchaseOrders)
                .Include(item => item.SalesOrders)
                .Include(item => item.Rmas)
                .Include(item => item.Invoices)
                .Include(item => item.Bills)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (oldItem == null) {
                return NotFound("That item was not found");
            }
            if (oldItem.CreatedById != GidIndustrial.Gideon.WebApi.Models.User.GetId(User)) {
                return BadRequest("You are not authorized to edit this because you didn't make it");
            }

            _context.Entry(chatMessage).State = EntityState.Modified;
            await chatMessage.UpdateMentionedUsers(_context);

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ChatMessageExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            var tasks = new[]
            {
                Task.Run(async () => {
                    //send emails to anybody mentioned
                    var currentUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(User));
                    var mentionedUsers = await chatMessage.GetMentionedUsers(_context);
                    var oldMentionedUsers = await oldItem.GetMentionedUsers(_context);
                    var newMentionedUsers = mentionedUsers.Where(item => !oldMentionedUsers.Any(oldUser => oldUser == item)).ToList();
                    var pageNameAndUrl = this.GetPageNameAndUrl(oldItem);
                    newMentionedUsers.ForEach(user => user.SendMentionedInChatEmail(
                        chatMessage,
                        currentUser,
                        _context,
                        pageNameAndUrl.PageName,
                        pageNameAndUrl.Url
                    ));
                })
            };

            return NoContent();
        }
        public class PageNameAndUrl {
            public string PageName;
            public string Url;
        }
        public PageNameAndUrl GetPageNameAndUrl(ChatMessage chatMessage) {

            string pageName = "";
            string url = "";
            if (chatMessage.Leads != null && chatMessage.Leads.Count > 0) {
                pageName = $"Lead #{chatMessage.Leads[0].LeadId}";
                url = $"https://gideon.gidindustrial.com/leads/{chatMessage.Leads[0].LeadId}?LeadPageTab=Chat";
            } else if (chatMessage.Quotes != null && chatMessage.Quotes.Count > 0) {
                pageName = $"Quote #{chatMessage.Quotes[0].QuoteId}";
                url = $"https://gideon.gidindustrial.com/quotes/{chatMessage.Quotes[0].QuoteId}?QuotePageTab=Chat";
            } else if (chatMessage.SalesOrders != null && chatMessage.SalesOrders.Count > 0) {
                pageName = $"Sales Order #{chatMessage.SalesOrders[0].SalesOrderId}";
                url = $"https://gideon.gidindustrial.com/sales-orders/{chatMessage.SalesOrders[0].SalesOrderId}?SalesOrderPageTab=Chat";
            } else if (chatMessage.PurchaseOrders != null && chatMessage.PurchaseOrders.Count > 0) {
                pageName = $"Purchase Order #{chatMessage.PurchaseOrders[0].PurchaseOrderId}";
                url = $"https://gideon.gidindustrial.com/purchase-orders/{chatMessage.PurchaseOrders[0].PurchaseOrderId}?PurchaseOrderPageTab=Chat";
            } else if (chatMessage.Rmas != null && chatMessage.Rmas.Count > 0) {
                pageName = $"Rma #{chatMessage.Rmas[0].RmaId}";
                url = $"https://gideon.gidindustrial.com/rmas/{chatMessage.Rmas[0].RmaId}?RmaPageTab=Chat";
            } else if (chatMessage.Invoices != null && chatMessage.Invoices.Count > 0) {
                pageName = $"Invoice #{chatMessage.Invoices[0].InvoiceId}";
                url = $"https://gideon.gidindustrial.com/rmas/{chatMessage.Invoices[0].InvoiceId}?InvoicePageTab=Chat";
            } else if (chatMessage.Bills != null && chatMessage.Bills.Count > 0) {
                pageName = $"Bill #{chatMessage.Bills[0].BillId}";
                url = $"https://gideon.gidindustrial.com/rmas/{chatMessage.Bills[0].BillId}?BillPageTab=Chat";
            }
            return new PageNameAndUrl {
                PageName = pageName,
                Url = url
            };
        }

        // POST: ChatMessage
        [HttpPost]
        public async Task<IActionResult> PostChatMessage([FromBody] ChatMessage chatMessage) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            ChatMessageControllerHelper helper = new ChatMessageControllerHelper();

            return await helper.PostChatMessageLocal(chatMessage, _context, this); 
        }


        [HttpGet("{id}/SendMentionedAlerts")]
        public async Task<IActionResult> SendMentionedAlerts([FromRoute] int id) {
            var chatMessage = await _context.ChatMessages.FirstOrDefaultAsync(item => item.Id == id);
            var mentionedUsers = await chatMessage.GetMentionedUsers(_context);
            var pageNameAndUrl = this.GetPageNameAndUrl(chatMessage);

            var currentUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(User));

            mentionedUsers.ForEach(user => user.SendMentionedInChatEmail(
                chatMessage,
                currentUser,
                _context,
                pageNameAndUrl.PageName,
                pageNameAndUrl.Url
            ));
            return Ok();
        }

        // DELETE: ChatMessage/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChatMessage([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var oldItem = await _context.ChatMessages.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if (oldItem == null) {
                return NotFound("That item was not found");
            }
            if (oldItem.CreatedById != GidIndustrial.Gideon.WebApi.Models.User.GetId(User)) {
                return BadRequest("You are not authorized to edit this because you didn't make it");
            }

            var chatMessage = await _context.ChatMessages
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (chatMessage == null) {
                return NotFound();
            }

            //remove attachments if needed
            if (chatMessage.Attachments.Count > 0) {
                chatMessage.Attachments.ForEach(async (chatMessageAttachment) => {
                    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configuration.GetConnectionString("AzureBlobStorage"));
                    //Create the blob client object.
                    CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                    //Get a reference to a container to use for the sample code, and create it if it does not exist.
                    CloudBlobContainer container = blobClient.GetContainerReference("sascontainer");
                    await container.CreateIfNotExistsAsync();

                    CloudBlockBlob blob = container.GetBlockBlobReference(chatMessageAttachment.Attachment.OfficialFilename);
                    await blob.DeleteAsync();
                });
            }


            _context.ChatMessages.Remove(chatMessage);
            await _context.SaveChangesAsync();

            return Ok(chatMessage);
        }

        private bool ChatMessageExists(int id) {
            return _context.ChatMessages.Any(e => e.Id == id);
        }
    }
}