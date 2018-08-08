using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCEventDiagnosis
    {
        [Key]
        public int PatientIFCEventId { get; set; }
        public PatientIFCEvent PatientIFCEvent { get; set; }

        [Key]
        public int PatientIFCDiagnosisId { get; set; }
        [ForeignKey("PatientIFCDiagnosisId")]
        public virtual PatientIFCDiagnosis Diagnosis { get; set; }
    }
}