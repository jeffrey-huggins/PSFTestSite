using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class SOCEventAntiPsychotic : SOCEvent
    {
        public int AntiPsychoticMedicationId { get; set; }
        //public string MedicationName { get; set; }
        public int AntiPsychoticDiagnosisId { get; set; }
        public string OtherDiagnosisDetail { get; set; }
        public DateTime? ConsentDate { get; set; }
    }
}
