using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Maintenance.Models
{
    public class PurchaseOrderVendor
    {
        [Key]
        public int POVendorId { get; set; }

        [Required]
        public int POVendorClassId { get; set; }

        [StringLength(20)]
        public string CorpVendorId { get; set; }

        [StringLength(64)]
        [Required]
        public string VendorName { get; set; }

        [StringLength(64)]
        [Required]
        public string Address1 { get; set; }

        [StringLength(64)]
        public string Address2 { get; set; }

        [StringLength(50)]
        [Required]
        public string City { get; set; }

        [StringLength(2)]
        [Required]
        public string StateCd { get; set; }

        [StringLength(20)]
        [Required]
        public string ZipCode { get; set; }

        [StringLength(16)]
        public string Phone { get; set; }

        [StringLength(16)]
        public string Fax { get; set; }

        [StringLength(128)]
        public string eMail { get; set; }

        public bool DataEntryFlg { get; set; }

        [Required]
        public int SortOrder { get; set; }

        public virtual ICollection<Equipment> Equipment { get; set; }
        public virtual ICollection<EquipmentRepair> Repairs { get; set; }
    }
}