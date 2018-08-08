using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    [Serializable]
    public class PatientIncidentType
    {
        [Key]
        public int PatientIncidentTypeId { get; set; }
        [Required]
        [MaxLength(60)]
        public string PatientIncidentName { get; set; }
        public bool DataEntryFlg { get; set; }
        public bool ReportFlg { get; set; }
        public int SortOrder { get; set; }
    }
}