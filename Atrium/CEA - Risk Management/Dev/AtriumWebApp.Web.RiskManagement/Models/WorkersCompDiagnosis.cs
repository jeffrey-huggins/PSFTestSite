using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompDiagnosis
    {
        [Key]
        [Required]
        [StringLength(256)]
        public string ClaimId { get; set; }
        [Key]
        public int DiagnosisId { get; set; }
    }
}