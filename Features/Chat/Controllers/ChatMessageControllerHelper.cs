using Ganss.XSS;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Features.Controllers
{
    public class ChatMessageControllerHelper
    {
        public class PageNameAndUrl
        {
            public string PageName;
            public string Url;
        }

        public PageNameAndUrl GetPageNameAndUrl(ChatMessage chatMessage)
        {

            string pageName = "";
            string url = "";
            if (chatMessage.Leads != null && chatMessage.Leads.Count > 0)
            {
                pageName = $"Lead #{chatMessage.Leads[0].LeadId}";
                url = $"https://gideon.gidindustrial.com/leads/{chatMessage.Leads[0].LeadId}?LeadPageTab=Chat";
            }
            else if (chatMessage.Quotes != null && chatMessage.Quotes.Count > 0)
            {
                pageName = $"Quote #{chatMessage.Quotes[0].QuoteId}";
                url = $"https://gideon.gidindustrial.com/quotes/{chatMessage.Quotes[0].QuoteId}?QuotePageTab=Chat";
            }
            else if (chatMessage.SalesOrders != null && chatMessage.SalesOrders.Count > 0)
            {
                pageName = $"Sales Order #{chatMessage.SalesOrders[0].SalesOrderId}";
                url = $"https://gideon.gidindustrial.com/sales-orders/{chatMessage.SalesOrders[0].SalesOrderId}?SalesOrderPageTab=Chat";
            }
            else if (chatMessage.PurchaseOrders != null && chatMessage.PurchaseOrders.Count > 0)
            {
                pageName = $"Purchase Order #{chatMessage.PurchaseOrders[0].PurchaseOrderId}";
                url = $"https://gideon.gidindustrial.com/purchase-orders/{chatMessage.PurchaseOrders[0].PurchaseOrderId}?PurchaseOrderPageTab=Chat";
            }
            else if (chatMessage.Rmas != null && chatMessage.Rmas.Count > 0)
            {
                pageName = $"Rma #{chatMessage.Rmas[0].RmaId}";
                url = $"https://gideon.gidindustrial.com/rmas/{chatMessage.Rmas[0].RmaId}?RmaPageTab=Chat";
            }
            else if (chatMessage.Invoices != null && chatMessage.Invoices.Count > 0)
            {
                pageName = $"Invoice #{chatMessage.Invoices[0].InvoiceId}";
                url = $"https://gideon.gidindustrial.com/rmas/{chatMessage.Invoices[0].InvoiceId}?InvoicePageTab=Chat";
            }
            else if (chatMessage.Bills != null && chatMessage.Bills.Count > 0)
            {
                pageName = $"Bill #{chatMessage.Bills[0].BillId}";
                url = $"https://gideon.gidindustrial.com/rmas/{chatMessage.Bills[0].BillId}?BillPageTab=Chat";
            }
            return new PageNameAndUrl
            {
                PageName = pageName,
                Url = url
            };
        }

        public async Task<IActionResult> PostChatMessageLocal(ChatMessage chatMessage, AppDBContext _context, Controller controller)
        {
            var mentionedUsers = await chatMessage.GetMentionedUsers(_context);
            var pageNameAndUrl = GetPageNameAndUrl(chatMessage);
            int? userId = GidIndustrial.Gideon.WebApi.Models.User.GetId(controller.User);
            if (userId == null)
                userId = chatMessage.CreatedById;

            var currentUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(controller.User));
            var mentioningUser = currentUser != null ? currentUser : (await _context.Users.FirstOrDefaultAsync(item => item.Id == userId));

            mentionedUsers.ForEach(user => user.SendMentionedInChatEmail(
                chatMessage,
                mentioningUser,
                _context,
                pageNameAndUrl.PageName,
                pageNameAndUrl.Url
            ));
            _context.ChatMessages.Add(chatMessage);
            await _context.SaveChangesAsync();

            if (chatMessage.InReplyToChatMessageId != null)
            {
                var originalChatMessage = await _context.ChatMessages.FirstOrDefaultAsync(item => item.Id == chatMessage.InReplyToChatMessageId);
                var originalUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == originalChatMessage.CreatedById);
                originalUser.SendChatMessageResponseEmail(
                    chatMessage,
                    currentUser,
                    _context,
                    pageNameAndUrl.PageName,
                    pageNameAndUrl.Url
                );
            }

            return controller.CreatedAtAction("GetChatMessage", new { id = chatMessage.Id }, chatMessage);
        }

        public bool SalesOrderChatMessageExists(int? salesOrderId, int? chatMessageId, AppDBContext _context)
        {
            return _context.SalesOrderChatMessages.Any(e => e.ChatMessageId == chatMessageId && e.SalesOrderId == salesOrderId);
        }

        public async Task<IActionResult> PostSalesOrderChatMessageLocal(SalesOrderChatMessage salesOrderChatMessage, AppDBContext _context, Controller controller)
        {
            salesOrderChatMessage.ChatMessage.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(controller.User);
            salesOrderChatMessage.ChatMessage.CreatedAt = DateTime.UtcNow;
            if (String.IsNullOrWhiteSpace(salesOrderChatMessage.ChatMessage.Message))
            {
                return controller.BadRequest("Empty message");
            }

            //sanitize input so if somebody put in malicious code like a <script> tag or something it won't get displayed.
            var sanitizer = new HtmlSanitizer();
            salesOrderChatMessage.ChatMessage.Message = sanitizer.Sanitize(salesOrderChatMessage.ChatMessage.Message);
            _context.SalesOrderChatMessages.Add(salesOrderChatMessage);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (SalesOrderChatMessageExists(salesOrderChatMessage.SalesOrderId, salesOrderChatMessage.ChatMessageId, _context))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            //need to return all related properties
            var query = from item in _context.SalesOrderChatMessages select item;
            query = query
                .Where(item => item.SalesOrderId == salesOrderChatMessage.SalesOrderId && item.ChatMessageId == salesOrderChatMessage.ChatMessageId)
                .Include(q => q.ChatMessage)
                .Include(q => q.SalesOrder);

            salesOrderChatMessage = await query.FirstOrDefaultAsync();

            //send emails to anybody mentioned
            var currentUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(controller.User));
            var mentionedUsers = await salesOrderChatMessage.ChatMessage.GetMentionedUsers(_context);
            mentionedUsers.ForEach(user => user.SendMentionedInChatEmail(
                salesOrderChatMessage.ChatMessage,
                currentUser,
                _context,
                $"Sales Order #{salesOrderChatMessage.SalesOrderId}",
                $"https://gideon.gidindustrial.com/sales-orders/{salesOrderChatMessage.SalesOrderId}?SalesOrderPageTab=Chat"
            ));

            mentionedUsers.ForEach(user => _context.ChatMessageUserMentions.Add(new ChatMessageUserMention
            {
                UserId = user.Id,
                ChatMessageId = salesOrderChatMessage.ChatMessageId
            }));
            await _context.SaveChangesAsync();

            if (salesOrderChatMessage.ChatMessage.InReplyToChatMessageId != null)
            {
                var originalChatMessage = await _context.ChatMessages.FirstOrDefaultAsync(item => item.Id == salesOrderChatMessage.ChatMessage.InReplyToChatMessageId);
                var originalUser = await _context.Users.FirstOrDefaultAsync(item => item.Id == originalChatMessage.CreatedById);
                if (originalUser.Id != currentUser.Id)
                {
                    originalUser.SendChatMessageResponseEmail(
                        salesOrderChatMessage.ChatMessage,
                        currentUser,
                        _context,
                        $"Sales Order #{salesOrderChatMessage.SalesOrderId}",
                        $"https://gideon.gidindustrial.com/sales-orders/{salesOrderChatMessage.SalesOrderId}?SalesOrderPageTab=Chat"
                    );
                }
            }

            return controller.CreatedAtAction("GetSalesOrderChatMessage", new { id = salesOrderChatMessage.ChatMessageId }, salesOrderChatMessage);
        }
    }
}
