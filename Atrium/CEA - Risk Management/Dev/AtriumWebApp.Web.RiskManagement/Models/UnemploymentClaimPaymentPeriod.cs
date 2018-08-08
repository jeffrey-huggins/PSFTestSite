using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class UnemploymentClaimPaymentPeriod
    {
        [Key]
        [StringLength(3)]
        public string PaymentPeriodCd { get; set; }
        [StringLength(10)]
        public string PaymentPeriodDesc { get; set; }
    }
}