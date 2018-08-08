using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCEventSymptom
    {
        [Key]
        public int PatientIFCEventId { get; set; }
        public PatientIFCEvent PatientIFCEvent { get; set; }

        [Key]
        public int PatientIFCSymptomId { get; set; }
        [ForeignKey("PatientIFCSymptomId")]
        public virtual PatientIFCSymptom Symptom { get; set; }
    }
}