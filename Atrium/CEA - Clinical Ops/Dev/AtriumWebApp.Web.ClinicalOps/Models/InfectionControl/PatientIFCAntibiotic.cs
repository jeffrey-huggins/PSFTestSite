using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCAntibiotic
    {
        [Key]
        public int PatientIFCAntibioticId { get; set; }
        [MaxLength(60)]
        [Required]
        public string PatientIFCAntibioticName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}