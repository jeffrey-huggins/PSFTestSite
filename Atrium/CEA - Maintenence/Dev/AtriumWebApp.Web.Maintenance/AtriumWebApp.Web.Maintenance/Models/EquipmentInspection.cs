using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Maintenance.Models
{
    public class EquipmentInspection
    {
        [Key]
        public int EquipmentInspectionId { get; set; }
        [Required]
        public int EquipmentMaintenancePlanId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Inspection Date")]
        public DateTime InspectionDate { get; set; }
        [MaxLength(128)]
        [DisplayName("Inspector")]
        public string InspectedByName { get; set; }
        [DisplayName("Inspection Vendor")]
        public int? InspectionVendorId { get; set; }
        [DisplayName("Passed?")]
        public bool PassFlg { get; set; }
        [MaxLength(1024)]
        public string Notes { get; set; }

        [ForeignKey("EquipmentMaintenancePlanId")]
        public virtual EquipmentMaintenancePlan MaintenancePlan { get; set; }
        public virtual ICollection<EquipmentInspectionDocument> Documents { get; set; }
        [ForeignKey("InspectionVendorId")]
        public virtual PurchaseOrderVendor InspectionVendor { get; set; }
    }
}
