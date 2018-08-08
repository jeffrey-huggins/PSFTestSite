using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompDiagnosisType
    {
        [Key]
        public int DiagnosisId { get; set; }
        [Required]
        [StringLength(25)]
        public string DiagnosisCd { get; set; }
        [Required]
        [StringLength(128)]
        public string DiagnosisDesc { get; set; }
    }
}