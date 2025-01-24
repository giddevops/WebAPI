using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebApi.Features.Controllers;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class QuickBooksLogItem {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string ResponseMessage { get; set; }
        public string RequestMessage { get; set; }
        public int? StatusCode { get; set; }
        public bool Successful { get; set; }
        public string ErrorMessage { get; set; }
        public string InfoMessage { get; set; }
    }
    class QuickBooksLogItemDBConfiguration : IEntityTypeConfiguration<QuickBooksLogItem> {
        public void Configure(EntityTypeBuilder<QuickBooksLogItem> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}