using System;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class HospitalDischarge
    {
        [Key]
        public int PatientId { get; set; }
        [Key]
        public DateTime CensusDate { get; set; }
        public DateTime AdmitDate { get; set; }
        public string DischargeType { get; set; }
        public string PatientStatus { get; set; }
        public bool DischargeReasonFlg { get; set; }
        public int? PayerId { get; set; }
        public int ERDischargeReasonId { get; set; }
        public int HospitalDischargeReasonId { get; set; }
        public bool PlannedFlg { get; set; }
        public int DidNotReturnReasonId { get; set; }
        public int HospitalId { get; set; }
    }
    public class HospitalDischargePTO
    {
        public int PatientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CensusDate { get; set; }
        public string DischargeType { get; set; }
        public string PatientStatus { get; set; }
        public bool DischargeReasonFlg { get; set; }
        public int ERDischargeReasonId { get; set; }
        public bool ERDischargeReasonIsIncluded { get; set; }
        public int HospitalDischargeReasonId { get; set; }
        public bool HospitalDischargeReasonIsIncluded { get; set; }
        public bool PlannedFlg { get; set; }
        public int DidNotReturnReasonId { get; set; }
        public int HospitalId { get; set; }
        public bool HospitalIsIncluded { get; set; }
        public string AdmitSrc { get; set; }
    }
}