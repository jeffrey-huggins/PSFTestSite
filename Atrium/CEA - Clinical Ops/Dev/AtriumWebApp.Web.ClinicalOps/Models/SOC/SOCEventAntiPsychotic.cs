using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class SOCEventAntiPsychotic : SOCEvent
    {
        public int AntiPsychoticMedicationId { get; set; }
        //public string MedicationName { get; set; }
        public int AntiPsychoticDiagnosisId { get; set; }
		[MaxLength(256)]
        public string OtherDiagnosisDetail { get; set; }
        public DateTime? ConsentDate { get; set; }

		[ForeignKey("AntiPsychoticMedicationId")]
		public virtual SOCAntiPsychoticMedication Medication { get; set; }
		[ForeignKey("AntiPsychoticDiagnosisId")]
		public virtual SOCAntiPsychoticDiagnosis Diagnosis { get; set; }
    }
}
