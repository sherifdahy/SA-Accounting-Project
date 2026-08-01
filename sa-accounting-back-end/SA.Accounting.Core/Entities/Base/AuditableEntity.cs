using SA.Accounting.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SA.Accounting.Core.Entities.Base
{
    public class AuditableEntity
    {
        public DateTime CreatedAt { get; set; }
        public int CreatedById { get; set; }
        public ApplicationUser CreatedBy { get; set; } = default!;


        // should be nullable 
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedById { get; set; }
        public ApplicationUser? UpdatedBy { get; set; }
    }
}

