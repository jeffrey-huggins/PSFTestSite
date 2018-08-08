using System;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompClaim
    {
        [Key]
        [Required]
        [StringLength(256)]
        public string ClaimId { get; set; }
        public DateTime? ClosedDate { get; set; }
        public DateTime? SettlementDate { get; set; }
        public DateTime? InjuryDate { get; set; }
        public DateTime? FROIDate { get; set; }
        public DateTime ReportedtoCarrierDate { get; set; }
        public bool ReportedToCarrierDateOverrideFlag { get; set; }
        public DateTime? LawsuitOpenedDate { get; set; }
        public DateTime? LawsuitClosedDate { get; set; }
        public DateTime? LastWorkedDate { get; set; }
        public bool PreventableFlg { get; set; }
        [StringLength(1024)]
        public string PreventableComments { get; set; }
        public bool HighExposureFlg { get; set; }
        [StringLength(1024)]
        public string HighExposureComments { get; set; }
        [StringLength(20)]
        public string ClaimNum { get; set; }
        [StringLength(256)]
        public string ClaimPhysician { get; set; }
        [Required]
        [StringLength(256)]
        public string ClaimDescription { get; set; }
        public int InsuranceCarrierId { get; set; }
        public bool LegalFirmFlg { get; set; }
        public int? LegalFirmID { get; set; }
        public bool VOCRehabFlg { get; set; }
        public int? VOCRehabID { get; set; }
        public bool TCMFlg { get; set; }
        public int? TCMId { get; set; }
        public DateTime? LightDutyBeginDate { get; set; }
        public DateTime? LightDutyEndDate { get; set; }
        public DateTime? FullDutyBeginDate { get; set; }
        public DateTime? FullDutyEndDate { get; set; }
        [StringLength(25)]
        public string ClaimStatus { get; set; }
        [StringLength(256)]
        public string LastUser { get; set; }
        public int? ClaimTypeId { get; set; }

        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}