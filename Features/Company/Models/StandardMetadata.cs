//this file is unused

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class StandardMetadata
    {
        public DateTime CreatedAt;
    }

    /// <summary>
    /// This sets up foreign keys in the database
    /// </summary>
    class StandardMetadataDBConfiguration : IEntityTypeConfiguration<StandardMetadata>
    {
        public void Configure(EntityTypeBuilder<StandardMetadata> modelBuilder)
        {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("CONVERT(date, GETUTCDATE())");
        }
    }
}
