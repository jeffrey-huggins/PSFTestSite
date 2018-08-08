using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class IncidentTreatment
    {
        [Key]
        public int PatientIncidentTreatmentId { get; set; }
        [Required]
        [MaxLength(60)]
        public string PatientIncidentTreatmentName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}