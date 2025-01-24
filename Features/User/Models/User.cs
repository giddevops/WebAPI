using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Libraries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class User {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // public string NameIdentifier { get; set; }
        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }

        public string WorkPhone { get; set; }
        public string MobilePhone { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }

        public Guid? AzureObjectId { get; set; }

        public List<Lead> Leads { get; set; }
        public List<UserUserGroup> UserGroups { get; set; }

        [NotMapped]
        public List<string> Permissions { get; set; }

        public List<LeadRoutingRule> LeadRoutingRules { get; set; }

        public GidLocationOption DefaultGidLocationOption { get; set; }
        public int? DefaultGidLocationOptionId { get; set; }

        // LeadFilter
        public int? MostRecentLeadFilterId { get; set; }
        public int? MostRecentQuoteFilterId { get; set; }
        public int? MostRecentSalesOrderFilterId { get; set; }
        public int? MostRecentInvoiceFilterId { get; set; }
        public int? MostRecentPurchaseOrderFilterId { get; set; }
        public int? MostRecentRmaFilterId { get; set; }
        public int? MostRecentInventoryItemFilterId { get; set; }
        public int? MostRecentProductFilterId { get; set; }
        public int? MostRecentSourceFilterId { get; set; }
        public int? MostRecentCompanyFilterId { get; set; }
        public int? MostRecentContactFilterId { get; set; }
        public int? MostRecentBillFilterId { get; set; }

        public bool? AutoCCSelf { get; set; }

        public static bool HasPermission(AppDBContext dbContext, ClaimsPrincipal user, string permissionName) {
            var permission = dbContext.Permissions.AsNoTracking().FirstOrDefault(p => p.Name == permissionName);
            List<string> allowedGroups = JsonConvert.DeserializeObject<List<string>>(permission.AllowedGroups);
            List<string> userGroups = user.Claims.Where(c => c.Type == "groups").Select(c => c.Value).ToList<string>();

            var matchingGroups = allowedGroups.Intersect(userGroups);

            if (matchingGroups.Count() == 0) {
                return false;
            }
            return true;
        }
        public static int? GetId(System.Security.Claims.ClaimsPrincipal user) {
            if (user == null)
                return null;
            var userIdClaim = user.Claims.FirstOrDefault(t => t.Type == CustomClaimTypes.UserId);
            if (userIdClaim != null)
                return Convert.ToInt32(userIdClaim.Value);
            return null;
        }
        public static void GetAllUsers(AppDBContext _context) {
            var usersJSON = MicrosoftGraphApiConnector.DoGraphRequest("https://graph.microsoft.com/v1.0/users?$filter=accountEnabled eq true&$select=id,displayName,givenName,surname,memberOf,mail,businessPhones,mobilePhone,jobTitle,department").GetAwaiter().GetResult();
            var usersResponse = JsonConvert.DeserializeObject<UsersResponse>(usersJSON);
            usersResponse.Value.ForEach(user => {
                var existingUser = _context.Users.FirstOrDefault(item => item.AzureObjectId == user.Id);
                if (existingUser == null && !String.IsNullOrWhiteSpace(user.Surname)) {
                    var newUser = new User {
                        AzureObjectId = user.Id,
                        CreatedAt = DateTime.UtcNow,
                        FirstName = user.GivenName,
                        LastName = user.Surname,
                        DisplayName = user.DisplayName,
                        Email = user.Mail,
                        Department = user.Department,
                        AutoCCSelf = true
                    };

                    if (user.BusinessPhones.Count > 0) {
                        newUser.WorkPhone = user.BusinessPhones[0];
                        newUser.JobTitle = user.JobTitle;
                    } else {
                        newUser.WorkPhone = null;
                    }

                    newUser.MobilePhone = user.MobilePhone;
                    _context.Users.Add(newUser);
                } else if (existingUser != null) {
                    existingUser.AzureObjectId = user.Id;
                    existingUser.CreatedAt = DateTime.UtcNow;
                    existingUser.FirstName = user.GivenName;
                    existingUser.LastName = user.Surname;
                    existingUser.DisplayName = user.DisplayName;
                    if(existingUser.DisplayName != "Steve Lowe")
                        existingUser.Email = user.Mail;
                    existingUser.Department = user.Department;

                    if (user.BusinessPhones.Count > 0) {
                        existingUser.WorkPhone = user.BusinessPhones[0];
                        existingUser.JobTitle = user.JobTitle;
                    } else {
                        existingUser.WorkPhone = null;
                    }
                    existingUser.UpdatedAt = DateTime.UtcNow;

                    existingUser.MobilePhone = user.MobilePhone;
                    _context.Entry(existingUser).State = EntityState.Modified;
                }
            });
            _context.SaveChanges();
        }

        /// <summary>
        /// This method sends an email to users who are mentioned in a chat message
        /// </summary>
        /// <param name="chatMessage">The chat message the user was mentioned in </param>
        /// <param name="mentioningUser">The user object of the person who mentioned the user</param>
        /// <param name="_context">The appdbcontext so it can access the database</param>
        /// <param name="pageName">The name of the page on which the user mentioned the other</param>
        /// <param name="pageUrl">The complete url (include https://) of the page where the user was mentioned</param>
        /// <returns></returns>
        public async void SendMentionedInChatEmail(ChatMessage chatMessage, User mentioningUser, AppDBContext _context, string pageName, string pageUrl) {
            var subject = "You were mentioned in a chat message";
            if (mentioningUser != null)
                subject = $"{mentioningUser.FirstName} {mentioningUser.LastName} mentioned you in a chat message in Gideon";
            else
                mentioningUser = new User
                {
                    FirstName = "Default",
                    LastName = "User",
                    DisplayName = "Default User"
                };
            var email = await EmailGenerator.GenerateEmail(_context, new EmailGeneratorParameters {
                To = this.Email,
                From = "gideon@gidindustrial.com",
                Subject = subject,
                HtmlContent = $@"
<p>Hi {this.FirstName},</p>
<p>{mentioningUser.FirstName} {mentioningUser.LastName} mentioned you in a chat message in Gideon in regards to {pageName}</p>
<p><a href='{pageUrl}'>Click Here to View The Chat Message</a><p>
<p>{mentioningUser.FirstName} {mentioningUser.LastName} wrote:</p>
<p>{chatMessage.Message}</p>
"
            });

            var client = EmailGenerator.GetNewSendGridClient();
            var response = await client.SendEmailAsync(email);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                return;
            } else {
                throw new Exception(await response.Body.ReadAsStringAsync());
                // return; //dont' bother throwing an exception for now. don't want to mess it up so it can't save just because the email didn't send
            }
        }

        public async void SendChatMessageResponseEmail(ChatMessage chatMessage, User replyingUser, AppDBContext _context, string pageName, string pageUrl) {
            var email = await EmailGenerator.GenerateEmail(_context, new EmailGeneratorParameters {
                To = this.Email,
                From = "gideon@gidindustrial.com",
                Subject = $"{replyingUser.FirstName} {replyingUser.LastName} replied to a chat message in Gideon",
                HtmlContent = $@"
<p>Hi {this.FirstName},</p>
<p>{replyingUser.FirstName} {replyingUser.LastName} replied to you in a chat message in Gideon in regards to {pageName}</p>
<p><a href='{pageUrl}'>Click Here to View The Chat Message</a><p>
<p>{replyingUser.FirstName} {replyingUser.LastName} wrote:</p>
<p>{chatMessage.Message}</p>
"
            });

            var client = EmailGenerator.GetNewSendGridClient();
            var response = await client.SendEmailAsync(email);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                return;
            } else {
                throw new Exception(await response.Body.ReadAsStringAsync());
                // return; //dont' bother throwing an exception for now. don't want to mess it up so it can't save just because the email didn't send
            }
        }
    }

    public class UsersResponse {
        public List<AzureUser> Value;
    }


    public class AzureUser {
        public Guid? Id;
        public List<string> BusinessPhones;
        public string DisplayName;
        public string Mail;
        public string MobilePhone;
        public string JobTitle;
        public string Department;

        public List<AzureUserGroup> Groups;

        public string Surname { get; set; }
        public string GivenName { get; set; }

        public dynamic MemberOf { get; set; }

        // {"@odata.context":"https://graph.microsoft.com/v1.0/$metadata#users/$entity","id":"32f4ebf0-bcda-4884-95a3-9c60cccb64d8","businessPhones":["+1 (214) 842-3889"],"displayName":"Dane Truelson","givenName":"Dane","jobTitle":"Software Development Consultant","mail":"dane@gidindustrial.com","mobilePhone":"+1 (214) 842-3889","officeLocation":null,"preferredLanguage":null,"surname":"Truelson","userPrincipalName":"dtruelson@gidindustrial.com"}
    }
    public class UserSelectStub {
        public int Id;
        public string Value;
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class UserDBConfiguration : IEntityTypeConfiguration<User> {
        public void Configure(EntityTypeBuilder<User> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Property(item => item.AzureObjectId)
                .IsRequired(false);

            modelBuilder.HasOne(item => item.DefaultGidLocationOption)
                .WithMany()
                .HasForeignKey(item => item.DefaultGidLocationOptionId);

            modelBuilder.HasAlternateKey(item => item.AzureObjectId);

        }
    }
}
