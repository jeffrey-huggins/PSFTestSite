using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCOrganism
    {
        [Key]
        public int PatientIFCOrganismId { get; set; }
        [MaxLength(64)]
        [Required]
        public string PatientIFCOrganismName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
        
    }
}