using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PatientPASRRLog
    {
        [Key]
        public int PASRRLogId { get; set; }
        public int PatientId { get; set; }
        public int PASRRTypeId { get; set; }
        public virtual PASRRType PASRRType { get; set; }
        public DateTime CompleteDate { get; set; }
        public int SigChangeTypeId { get; set; }
        public virtual PASRRSigChangeType SigChangeType { get; set; }
        public bool HospitalExemption { get; set; }
        public DateTime? HospitalExemptionExpirationDate { get; set; }
        public bool StayGreaterThan30Days { get; set; }
        public bool DementiaExemption { get; set; }
        public bool LevelIINeeded { get; set; }
        public DateTime? LevelIIRequestedDate { get; set; }
        public DateTime? LevelIICompletedDate { get; set; }
        public DateTime InsertedDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }
}