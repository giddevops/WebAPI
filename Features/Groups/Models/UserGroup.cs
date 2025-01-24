using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class UserGroup {
        public int? Id { get; set; }

        public string Name { get; set; }
        public string AzureId { get; set; }
        public bool? Deactivated { get; set; }

        public static void SetupUserGroups(AppDBContext _context) {
            var groupsJSON = MicrosoftGraphApiConnector.DoGraphRequest("https://graph.microsoft.com/v1.0/groups?$filter=securityEnabled eq true&$expand=members").GetAwaiter().GetResult();
            var groupsResponse = JsonConvert.DeserializeObject<UserGroupsResponse>(groupsJSON);
            // _context.UserUserGroups.RemoveRange(_context.UserUserGroups);
            _context.SaveChanges();
            groupsResponse.Value.ForEach(azureGroup => {
                var userGroup = _context.UserGroups.FirstOrDefault(item => item.AzureId == azureGroup.Id);
                if (userGroup == null) {
                    userGroup = new UserGroup {
                        AzureId = azureGroup.Id,
                        Deactivated = false,
                        Name = azureGroup.DisplayName
                    };
                    _context.UserGroups.Add(userGroup);
                }
                _context.SaveChanges();
                var userUserGroups = _context.UserUserGroups.Where(item => item.UserGroupId == userGroup.Id).ToList();
                var dict = new Dictionary<int?, bool> { };
                foreach(var userUserGroup in userUserGroups)
                {
                    dict[userUserGroup.UserId] = true;
                }
                azureGroup.Members.ForEach(azureUser => {
                    var user = _context.Users
                        .FirstOrDefault(item => item.AzureObjectId == azureUser.Id);
                    if (user != null && !dict.ContainsKey(user.Id)){
                        _context.UserUserGroups.Add(new UserUserGroup
                        {
                            UserId = user.Id,
                            UserGroupId = userGroup.Id,
                        });
                    }
                });
            });
             _context.SaveChanges();
        }
    }    
    class UserGroupDBConfiguration : IEntityTypeConfiguration<UserGroup> {
        public void Configure(EntityTypeBuilder<UserGroup> modelBuilder) {
            modelBuilder.HasIndex(item => item.AzureId);
        }
    }
    public class UserGroupsResponse {
        public List<AzureUserGroup> Value;
    }
    public class AzureUserGroup {
        public string Id;
        public string DisplayName;
        public List<AzureUser> Members;
    }
}