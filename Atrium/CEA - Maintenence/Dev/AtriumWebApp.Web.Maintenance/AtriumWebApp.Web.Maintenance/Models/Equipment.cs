using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Maintenance.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }
        [Required]
        public int CommunityId { get; set; }
        [Required]
        [MaxLength(64)]
        [DisplayName("Serial Number")]
        public string SerialNbr { get; set; }
        [Required]
        [MaxLength(64)]
        [DisplayName("Model Number")]
        public string ModelNbr { get; set; }
        [Required]
        [MaxLength(128)]
        public string Description { get; set; }
        [Required]
        [DisplayName("Vendor")]
        public int PurchaseVendorId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Purchase Date")]
        [DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime PurchaseDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Install Date")]
        [DisplayFormat(DataFormatString = "MM/dd/yyyy")]
        public DateTime InstalledDate { get; set; }
        [Required]
        [MaxLength(32)]
        public string Condition { get; set; }
        [DisplayName("Decomissioned?")]
        public bool DecommissionedFlg { get; set; }
        [DisplayName("Life Expectancy")]
        [Required]
        public int LifeExpectancyCnt { get; set; }

        [ForeignKey("CommunityId")]
        public virtual Community Community { get; set; }
        [ForeignKey("PurchaseVendorId")]
        public virtual PurchaseOrderVendor Vendor { get; set; }

        
        public virtual ICollection<EquipmentMaintenancePlan> MaintenancePlans { get; set; }
        public virtual ICollection<EquipmentRepair> Repairs { get; set; }
    }
}
