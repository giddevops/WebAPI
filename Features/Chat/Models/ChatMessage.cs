using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ChatMessage {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? InReplyToChatMessageId { get; set; }
        // public ChatMessage InReplyToChatMessage { get; set; }

        public List<ChatMessage> Replies { get; set; }

        public List<LeadChatMessage> Leads { get; set; }
        public List<QuoteChatMessage> Quotes { get; set; }
        public List<SalesOrderChatMessage> SalesOrders { get; set; }
        public List<PurchaseOrderChatMessage> PurchaseOrders { get; set; }
        public List<RmaChatMessage> Rmas { get; set; }
        public List<InvoiceChatMessage> Invoices { get; set; }
        public List<BillChatMessage> Bills { get; set; }

        public string Message { get; set; }
        public List<ChatMessageAttachment> Attachments { get; set; }

        public async Task<List<User>> GetMentionedUsers(AppDBContext _context) {
            List<User> mentionedUsers = new List<User>();
            var matches = Regex.Matches(this.Message, @"@\[([-a-zA-Z ]+)\]").ToList();
            foreach (Match match in matches) {
                foreach (Match capture in match.Captures) {
                    var names = capture.Groups[1].Value.Split(" ");
                    if (names.Length == 2) {
                        var matchingUser = await _context.Users.FirstOrDefaultAsync(item => item.FirstName == names[0] && item.LastName == names[1]);
                        if (matchingUser != null) {
                            mentionedUsers.Add(matchingUser);
                        }
                    }
                }
            }
            return mentionedUsers;
        }
        

        public async Task UpdateMentionedUsers(AppDBContext _context) {
            var mentionedUsers = await this.GetMentionedUsers(_context);
            var existingMentions = await _context.ChatMessageUserMentions.AsNoTracking().Where(item => item.ChatMessageId == this.Id).ToListAsync();
            var toAdd = mentionedUsers.Where(user => !existingMentions.Any(mention => mention.UserId == user.Id)).ToList();
            var toDelete = existingMentions.Where(mention => !mentionedUsers.Any(user => user.Id == mention.UserId)).ToList();

            toAdd.ForEach(user => _context.ChatMessageUserMentions.Add(new ChatMessageUserMention {
                UserId = user.Id,
                ChatMessageId = this.Id
            }));

            toDelete.ForEach(mention => _context.ChatMessageUserMentions.Remove(mention));

        }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class ChatMessageDBConfiguration : IEntityTypeConfiguration<ChatMessage> {
        public void Configure(EntityTypeBuilder<ChatMessage> modelBuilder) {
            // modelBuilder.HasOne(item => item.InReplyToChatMessage)
            //     .WithMany(item => item.Replies)
            //     .HasForeignKey(item => item.InReplyToChatMessageId)
            //     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
