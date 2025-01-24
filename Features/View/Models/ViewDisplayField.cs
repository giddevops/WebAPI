using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ViewDisplayField {
        public int? Id { get; set; }

        [Required]
        public int ViewId { get; set; }
        public View View { get; set; }

        public string Header { get; set; }
        public string FieldName { get; set; }

        [Column(TypeName = "int")]
        public SortOrder? SortOrder { get; set; }
        public bool Sortable { get; set; }

        public string FieldType { get; set; }

    }
}