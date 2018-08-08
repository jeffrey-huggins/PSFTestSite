using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompClaimType
    {
        [Key]
        public int ClaimTypeId { get; set; }
        [Required]
        [StringLength(32)]
        public string ClaimTypeDesc { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}