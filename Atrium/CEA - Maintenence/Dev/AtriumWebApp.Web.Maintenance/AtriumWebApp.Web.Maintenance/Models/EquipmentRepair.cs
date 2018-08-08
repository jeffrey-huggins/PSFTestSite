using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Maintenance.Models
{
    public class EquipmentRepair
    {
        [Key]
        public int EquipmentRepairId { get; set; }
        [Required]
        public int EquipmentId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Repair Date")]
        public DateTime RepairDate { get; set; }
        [DisplayName("Repaired By")]
        [MaxLength(128)]
        public string RepairByName { get; set; }
        [DisplayName("Vendor")]
        public int? RepairVendorId { get; set; }
        [Required]
        [DataType(DataType.Currency)]
        [DisplayName("Cost")]
        public decimal RepairCostAmt { get; set; }
        [MaxLength(1024)]
        public string Notes { get; set; }

        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipment { get; set; }
        [ForeignKey("RepairVendorId")]
        public virtual PurchaseOrderVendor Vendor { get; set; }
        public virtual ICollection<EquipmentRepairDocument> Documents { get; set; }
    }
}
