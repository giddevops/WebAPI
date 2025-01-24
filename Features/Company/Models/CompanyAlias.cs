using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class CompanyAlias
    {
        public int? Id { get; set; }
        public int? CompanyId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Alias { get; set; }
    }

    /// <summary>
    /// set up createdAt, fluent api config
    /// </summary>
    class CompanyAliasDBConfiguration : IEntityTypeConfiguration<CompanyAlias>
    {
        public void Configure(EntityTypeBuilder<CompanyAlias> modelBuilder)
        {
            modelBuilder.HasIndex(item => item.Alias)
                .IsUnique();
        }
    }
}
