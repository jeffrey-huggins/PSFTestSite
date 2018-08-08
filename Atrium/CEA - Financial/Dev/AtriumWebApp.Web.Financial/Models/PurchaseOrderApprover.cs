using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Web.Financial.Models
{
    public class PurchaseOrderApprover
    {
        public int Id { get; set; }
        [StringLength(256)]
        [Required]
        public string Name { get; set; }
        [Range(2, 4)]
        public int ApprovalLevel { get; set; }

        public PurchaseOrderApprover()
        {
            ApprovalLevel = 2;
        }
    }
}
