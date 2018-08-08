using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCDiagnosis
    {
        [Key]
        public int PatientIFCDiagnosisId { get; set; }
        [MaxLength(60)]
        [Required]
        public string PatientIFCDiagnosisName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}