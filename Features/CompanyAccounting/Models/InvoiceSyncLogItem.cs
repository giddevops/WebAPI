using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class InvoiceSyncLogItem {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string QuickBooksErrorMessage { get; set; }
        public int? InvoiceId { get; set; }
    }
}
