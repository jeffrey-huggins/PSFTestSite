using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompVOCRehab
    {
        [Key]
        public int VOCRehabID { get; set; }
        [Required]
        [StringLength(50)]
        public string VOCRehabName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}