using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCSite
    {
        [Key]
        public int PatientIFCSiteId { get; set; }
        [MaxLength(60)]
        [Required]
        public string PatientIFCSiteName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}