
using System;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi {
    public class AuthenticationToken {
        public int? Id { get; set; }
        public string Token { get; set; }
        public int? UserId { get; set; }
        
        [Required]
        public DateTime? CreatedAt { get; set; }
    }
    class AuthenticationTokenDBConfiguration : IEntityTypeConfiguration<AuthenticationToken> {
        public void Configure(EntityTypeBuilder<AuthenticationToken> modelBuilder) {
            modelBuilder.HasIndex(item => item.Token); 
            modelBuilder.HasIndex(item => item.CreatedAt); 
        }
    }
}