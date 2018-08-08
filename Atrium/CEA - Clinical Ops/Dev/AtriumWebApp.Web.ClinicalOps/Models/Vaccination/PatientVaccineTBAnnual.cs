using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientVaccineTBAnnual : PatientVaccine
    {
        public DateTime? PPDGivenDate { get; set; }
        public int? PPDSiteId { get; set; }
        public DateTime? PPDReadDate { get; set; }
        public bool PPDReactionFlg { get; set; }
        public int PPDReactionMeasurementId { get; set; }
        public string PPDLotNumber { get; set; }
        public bool QuestionnaireCompleteFlg { get; set; }
        public DateTime? QuestionnaireCompleteDate { get; set; }
        public bool BMATFlg { get; set; }
        public DateTime? BMATDate { get; set; }
    }
}