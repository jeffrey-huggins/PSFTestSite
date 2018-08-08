using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
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

        public SOCEvent SOCEvent { get; set; }
    }
}
