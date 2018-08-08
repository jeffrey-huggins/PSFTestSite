using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class PatientIFCTypeOfPrecaution
    {
        [Key]
        public int PatientIFCTypeOfPrecautionId { get; set; }
        [MaxLength(60)]
        [Required]
        public string PatientIFCTypeOfPrecautionName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}