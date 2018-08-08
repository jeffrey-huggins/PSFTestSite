using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class UnemploymentBenefit
    {
        [Key]
        [StringLength(256)]
        public string ClaimId { get; set; }
        [Key]
        public int BenefitKey { get; set; }
        [DisplayName("Week Claimed")]
        public DateTime BenefitDate { get; set; }
        [DisplayName("Claim Payout")]
        public decimal BenefitAmt { get; set; }
        [ForeignKey("ClaimId")]
        public virtual UnemploymentClaim Claim { get; set; }
    }
}