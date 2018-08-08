using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class SOCEvent
    {
        [Key]
        public int SOCEventId { get; set; }
        public int PatientId { get; set; }
        public int SOCMeasureId { get; set; }
        public DateTime OccurredDate { get; set; }
        public bool OccurredFlg { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public bool ResolvedFlg { get; set; }
        public int RoomId { get; set; }
        public bool DeletedFlg { get; set; }
        public DateTime? DeletedTS { get; set; }
        public string DeletedADDomainName { get; set; }
        public int? CurrentPayerId { get; set; }
        public bool ShortStayFlg { get; set; }

		[ForeignKey("SOCMeasureId")]
		public virtual Measure SOCMeasure { get; set; }

		public virtual List<SOCEventRestraintNoted> Restraints { get; set; }

    }
}