using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Maintenance.Models
{
    public class EquipmentMaintenancePlan
    {
        [Key]
        public int EquipmentMaintenancePlanId { get; set; }
        [Required]
        public int EquipmentId { get; set; }
        [Required]
        [MaxLength(64)]
        public string Description { get; set; }
        [Required]
        [MaxLength(64)]
        public string Interval { get; set; }
        [Required]
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        [Required]
        [DisplayName("Next Inspection Date")]
        [DataType(DataType.Date)]
        public DateTime NextInspectionDate { get; set; }

        [ForeignKey("EquipmentId")]
        public virtual Equipment Equipment { get; set; }

        public ICollection<EquipmentInspection> Inspections { get; set; }
    }
}
