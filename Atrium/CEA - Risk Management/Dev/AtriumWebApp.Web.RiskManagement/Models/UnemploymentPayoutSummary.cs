using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class UnemploymentPayoutSummary
    {
        [Key]
        public int PayoutSummaryId { get; set; }
        public int UnemploymentClaimEventId { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime PayoutDate { get; set; }
        public decimal ClaimPayout { get; set; }
        public decimal? OtherExpenses { get; set; }
    }
}