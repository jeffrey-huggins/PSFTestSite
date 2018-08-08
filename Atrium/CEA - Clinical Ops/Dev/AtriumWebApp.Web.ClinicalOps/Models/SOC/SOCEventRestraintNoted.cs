using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class SOCEventRestraintNoted
    {
        [Key]
        public int SOCEventRestraintNotedId { get; set; }
        public int SOCEventId { get; set; }
        public int SOCRestraintId { get; set; }
        public string Comments { get; set; }
        public bool DiagnosisSupportsRestraintFlg { get; set; }
        public string DiagnosisSupportsRestraintDesc { get; set; }
        public DateTime? AttemptedReducedDate { get; set; }

		[ForeignKey("SOCRestraintId")]
		public virtual SOCRestraint Restraint { get; set; }
		[ForeignKey("SOCEventId")]
		public SOCEvent SOCEvent { get; set; }
    }
}
