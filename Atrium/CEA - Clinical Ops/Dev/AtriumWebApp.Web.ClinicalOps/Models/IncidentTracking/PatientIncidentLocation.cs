using System;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    [Serializable]
    public class PatientIncidentLocation
    {
        [Key]
        public int PatientIncidentLocationId { get; set; }
        [Required]
        [MaxLength(60)]
        public string PatientIncidentLocationName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}