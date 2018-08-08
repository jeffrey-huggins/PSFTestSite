using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCSymptom
    {
        [Key]
        public int PatientIFCSymptomId { get; set; }
        [MaxLength(60)]
        [Required]
        public string PatientIFCSymptomName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}