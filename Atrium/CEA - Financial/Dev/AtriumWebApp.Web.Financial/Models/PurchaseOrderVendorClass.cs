using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace AtriumWebApp.Web.Financial.Models
{
    public class PurchaseOrderVendorClass
    {
        public int Id { get; set; }
        [StringLength(30)]
        [Required]
        public string Name { get; set; }
        public bool AllowDataEntry { get; set; }
        public int SortOrder { get; set; }
    }
}
