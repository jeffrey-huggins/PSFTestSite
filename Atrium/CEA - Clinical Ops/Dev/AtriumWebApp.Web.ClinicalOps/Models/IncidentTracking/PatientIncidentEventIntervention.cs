using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIncidentEventIntervention
    {
        [Key]
        public int PatientIncidentEventId { get; set; }
        [ForeignKey("PatientIncidentEventId")]
        public virtual PatientIncidentEvent PatientIncidentEvent { get; set; }

        [Key]
        public int PatientIncidentInterventionId { get; set; }
    }
}