using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompInsurance
    {
        [Key]
        public int InsuranceId { get; set; }
        [Required]
        [StringLength(50)]
        public string InsuranceNm { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}