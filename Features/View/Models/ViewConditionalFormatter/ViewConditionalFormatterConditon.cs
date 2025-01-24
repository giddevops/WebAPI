using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ViewConditionalFormatterCondition {
        public int? Id { get; set; }

        [Required]
        public int ViewConditionalFormatterId { get; set; }
        public ViewConditionalFormatter ViewConditionalFormatter { get; set; }
        
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public string ViewFilterCondition { get; set; }
        public string Value { get; set; }
    }
}