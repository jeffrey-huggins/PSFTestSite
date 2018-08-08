using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class SOCEventWoundNoted
    {
        [Key]
        public int SOCEventWoundNotedId { get; set; }
        public int SOCEventId { get; set; }
        public DateTime NotedDate { get; set; }
        public int? PressureWoundStageId { get; set; }
        [StringLength(256)]
        public string Status { get; set; }
        public string LabDiagnostic { get; set; }
        [Required]
        [StringLength(256)]
        public string Treatment { get; set; }
        
        [StringLength(256)]
        public string Measurement { get; set; }
        [StringLength(128)]
        public string ReliefDevice { get; set; }
        public bool Drainage { get; set; }
        [StringLength(128)]
        public string DrainageDesc { get; set; }
        [Required]
        [StringLength(1024)]
        public string Intervention { get; set; }
        [StringLength(16)]
        public string IdealBodyWeight { get; set; }
        [StringLength(16)]
        public string ActualWeight { get; set; }
        [StringLength(16)]
        public string FoodIntake { get; set; }
        [StringLength(16)]
        public string SkinTurgor { get; set; }
        [StringLength(16)]
        public string Urine { get; set; }
        [StringLength(16)]
        public string PainLevel { get; set; }
        public DateTime? DietaryDate { get; set; }
        public DateTime? PhysicianDate { get; set; }
        public DateTime? FamilyDate { get; set; }
        [StringLength(16)]
        public string Progress { get; set; }
        [StringLength(32)]
        public string Signature { get; set; }
        public bool AdmittedWithFlg { get; set; }
        public decimal? LengthNbr {get;set;}
        public decimal? WidthNbr {get;set;}
        public decimal? DepthNbr { get; set; }

        public SOCEventWound SOCEvent { get; set; }

		[ForeignKey("PressureWoundStageId")]
		public virtual PressureWoundStage WoundStage { get; set; }
    }
}
