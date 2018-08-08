using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIncidentEventTreatment
    {
        [Key]
        public int PatientIncidentEventId { get; set; }
        [ForeignKey("PatientIncidentEventId")]
        public virtual PatientIncidentEvent PatientIncidentEvent { get; set; }

        [Key]
        public int PatientIncidentTreatmentId { get; set; }
    }
}