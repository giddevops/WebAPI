using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class Permission
    {
        public int Id { get; set; }
        public string Name { get; set; }
        // public string PrettyName { get; set; }
        public string AllowedGroups { get; set; }
    }
}
