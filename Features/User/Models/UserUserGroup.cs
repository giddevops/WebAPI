using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class UserUserGroup {
        public User User { get; set; }
        public int? UserId { get; set; }

        public UserGroup UserGroup { get; set; }
        public int? UserGroupId { get; set; }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class UserUserGroupDBConfiguration : IEntityTypeConfiguration<UserUserGroup> {
        public void Configure(EntityTypeBuilder<UserUserGroup> modelBuilder) {
            modelBuilder.HasKey(item => new { item.UserId, item.UserGroupId });
            modelBuilder
                .HasOne(item => item.User)
                .WithMany(item => item.UserGroups)
                .HasForeignKey(item => item.UserId);

            modelBuilder
                .HasOne(item => item.UserGroup)
                .WithMany()
                .HasForeignKey(item => item.UserGroupId);
        }
    }
}