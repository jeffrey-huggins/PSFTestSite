using System;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientVaccineHistory
    {
        public int PatientVaccineId { get; set; }
        public VaccineType VaccineType { get; set; }
        public DateTime? VaccineDate { get; set; }
        public bool ConsentSignedFlg { get; set; }
        public bool OfferedRefusedFlg { get; set; }
    }
}