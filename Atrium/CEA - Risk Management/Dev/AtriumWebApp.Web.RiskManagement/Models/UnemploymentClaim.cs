using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class UnemploymentClaim
    {
        [Key]
        [StringLength(256)]
        public string ClaimId { get; set; }
        public int EmployeeId { get; set; }
        [DisplayName("Application")]
        public DateTime ApplicationDate { get; set; }
        [DisplayName("Claim Received")]
        public DateTime ClaimReceivedDate { get; set; }
        public DateTime? ClaimDueDate { get; set; }
        public DateTime? DocsRequestedDate { get; set; }
        public DateTime? DocsDueDate { get; set; }
        public DateTime? DocsReceivedDate { get; set; }
        public DateTime? AppealSentDate { get; set; }
        public DateTime? HearingSetDate { get; set; }
        [Required]
        [StringLength(10)]
        [DisplayName("Reason")]
        public string ReasonCd { get; set; }
        public bool AppealedFlg { get; set; }
        public bool ReceivingBenefitsFlg { get; set; }
        public bool IneligibleFlg { get; set; }
        [DisplayName("Max Claim Amount")]
        public decimal MaxClaimAmt { get; set; }
        public bool DeniedFlg { get; set; }
        public bool ChargeToStateFlg { get; set; }
        public decimal? ChargeToStateAmt { get; set; }
        public bool PreventableFlg { get; set; }
        [StringLength(1024)]
        public string PreventableComments { get; set; }
        [StringLength(256)]
        public string LastUser { get; set; }
        
        [ForeignKey("ReasonCd")]
        public virtual UnemploymentClaimReason Reason { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; }

        public virtual List<UnemploymentBenefit> Payments { get; set; }
    }
}