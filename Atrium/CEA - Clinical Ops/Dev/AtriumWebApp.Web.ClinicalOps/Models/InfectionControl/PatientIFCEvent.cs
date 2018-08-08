using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCEvent
    {
        [Key]
        public int PatientIFCEventId { get; set; }
        public int PatientId { get; set; }
        [DisplayName("Onset Date")]
        public DateTime OnsetDate { get; set; }
        public bool AcquiredPriorToAdmissionFlg { get; set; }
        public bool HospitalizationRequiredFlg { get; set; }
        public bool NosocomialFlg { get; set; }
        public DateTime? CultureDate { get; set; }
        public DateTime? XRayDate { get; set; }
        public string PreventativeMeasuresDetail { get; set; }
        [DisplayName("Site")]
        public int PatientIFCSiteId { get; set; }
        public string OtherDiagnosisDetails { get; set; }
        //public int PatientIFCOrganismId { get; set; }
        public int RoomId { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public bool DeletedFlg { get; set; }
        public DateTime? DeletedTS { get; set; }
        public string DeletedADDomainName { get; set; }
        public int? CurrentPayerId { get; set; }
        public bool ShortStayFlg { get; set; }

        [ForeignKey("PatientIFCSiteId")]
        public virtual PatientIFCSite Site { get; set; }

        public virtual List<PatientIFCEventAntibiotic> Antibiotics { get; set; }
        public virtual List<PatientIFCEventDiagnosis> Diagnosis { get; set; }
        public virtual List<PatientIFCEventOrganism> Organisms { get; set; }
        public virtual List<PatientIFCEventSymptom> Symptoms { get; set; }
        public virtual List<PatientIFCEventTypeOfPrecaution> Precautions { get; set; }
    }
}