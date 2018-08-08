using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class EmployeeVaccineTBInitial2StepRecord : EmployeeVaccineRecord
    {
        public DateTime? PPDStep1GivenDate { get; set; }
        public int PPDStep1SiteId { get; set; }
        public DateTime? PPDStep1ReadDate { get; set; }
        public bool PPDStep1ReactionFlg { get; set; }
        public int PPDStep1ReactionMeasurementId { get; set; }
        //public string PPDStep1LotNumber { get; set; }
        public DateTime? PPDStep2GivenDate { get; set; }
        public int PPDStep2SiteId { get; set; }
        public DateTime? PPDStep2ReadDate { get; set; }
        public bool PPDStep2ReactionFlg { get; set; }
        public int PPDStep2ReactionMeasurementId { get; set; }
        //public string PPDStep2LotNumber { get; set; }
        public bool? ChestXRayFlg { get; set; }
        public DateTime? ChestXRayDate { get; set; }
        public bool QuestionnaireCompleteFlg { get; set; }
        public DateTime? QuestionnaireCompleteDate { get; set; }
        public bool BMATFlg { get; set; }
        public DateTime? BMATDate { get; set; }
    }
}