using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class IncidentIntervention
    {
        [Key]
        public int PatientIncidentInterventionId { get; set; }
        [Required]
        [MaxLength(60)]
        public string PatientIncidentInterventionName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}