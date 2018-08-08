using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Maintenance.Models
{
    public class EquipmentInspectionDocument
    {
        [Key]
        public int EquipmentInspectionDocumentId { get; set; }
        [Required]
        public int EquipmentInspectionId { get; set; }
        [Required]
        [MaxLength(256)]
        public string DocumentFileName { get; set; }
        [Required]
        [MaxLength(256)]
        public string ContentType { get; set; }
        [Required]
        [DataType(DataType.Upload)]
        public byte [] Document { get; set; }

        [ForeignKey("EquipmentInspectionId")]
        public virtual EquipmentInspection Inspection { get; set; }
    }
}
