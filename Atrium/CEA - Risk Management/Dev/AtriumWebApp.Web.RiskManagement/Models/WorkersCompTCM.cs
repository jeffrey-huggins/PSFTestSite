using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompTCM
    {
        [Key]
        public int TCMId { get; set; }
        [Required]
        [StringLength(50)]
        public string TCMName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}