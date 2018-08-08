using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIncidentEvent
    {
        [Key]
        public int PatientIncidentEventId { get; set; }
        public int PatientId { get; set; }
        [DisplayName("Type of Incident")]
        public int PatientIncidentTypeId { get; set; }
        [DisplayName("Location")]
        public int PatientIncidentLocationId { get; set; }
        [DisplayName("Date of Incident")]
        public DateTime IncidentDateTime { get; set; }
        public bool PhysicianNotifiedFlg { get; set; }
        public bool FamilyNotifiedFlg { get; set; }
        public bool ReportedToStateFlg { get; set; }
        public DateTime? ReportedToStateDateTime { get; set; }
        public bool StateInBuildingFlg { get; set; }
        public DateTime? StateInBuildingDate { get; set; }
        public bool RegionalNurseClosedFlg { get; set; }
        public int? RegionalNurseEmployeeId { get; set; }
        public string IncidentDetails { get; set; }
        public string OtherIncidentDetails { get; set; }
        public string Comments { get; set; }
        public int RoomId { get; set; }
        public bool DeletedFlg { get; set; }
        public DateTime? DeletedTS { get; set; }
        public string DeletedADDomainName { get; set; }
        public int? CurrentPayerId { get; set; }

		[ForeignKey("RegionalNurseEmployeeId")]
		public virtual Employee RegionalNurse { get; set; }
		[ForeignKey("PatientIncidentTypeId")]
		public virtual PatientIncidentType IncidentType { get; set; }
        public virtual ICollection<PatientIncidentEventIntervention> Interventions { get; set; }
        public virtual ICollection<PatientIncidentEventTreatment> Treatments { get; set; }
        public virtual ICollection<PatientIncidentEventDocument> Documents { get; set; }
    }
}