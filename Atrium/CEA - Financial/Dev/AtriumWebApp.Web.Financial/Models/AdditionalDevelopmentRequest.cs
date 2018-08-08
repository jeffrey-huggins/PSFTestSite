using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Financial.Models
{
    public class AdditionalDevelopmentRequest
    {
        [Key]
        public string RequestId { get; set; }
        public int CommunityId { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }
        [StringLength(24)]
        public string MedicareNumber { get; set; }
        public int ADRPayerId { get; set; }
        public decimal ARAmount { get; set; }
        public DateTime ADRCMSDate { get; set; }
        public DateTime? ADRReceivedDate { get; set; }
        public DateTime? ADRReturnMailDate { get; set; }
        public DateTime? ADRDenialDate { get; set; }
        public DateTime? ServiceBeginDate { get; set; }
        public DateTime? ServiceEndDate { get; set; }
        public DateTime? RedeterminationMailDate { get; set; }
        public DateTime? RedeterminationDenialDate { get; set; }
        public DateTime? ReconsiderationMailDate { get; set; }
        public DateTime? ReconsiderationDenialDate { get; set; }
        public DateTime? ALJMailDate { get; set; }
        public DateTime? ALJHearingDate { get; set; }
        public DateTime? ALJDenialDate { get; set; }
        [StringLength(7168)]
        public string RequestNotes { get; set; }
        [StringLength(17, MinimumLength = 17)]
        [Required]
        public string DCN { get; set; }
        public bool ClosedFlg { get; set; }
        public string RevisedDCN { get; set; }
        public string RACLetterID { get; set; }
        public string DemandLetter { get; set; }
        public string C2CMedicareAppeal { get; set; }
        public DateTime? RemitApproveDate { get; set; }
    }
}
