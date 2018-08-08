using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class UnemploymentClaimReason
    {
        [Key]
        [StringLength(10)]
        public string ReasonCd { get; set; }
        [StringLength(32)]
        public string ReasonDesc { get; set; }
    }
}