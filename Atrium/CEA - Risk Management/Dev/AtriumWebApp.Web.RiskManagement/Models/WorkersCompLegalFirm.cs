using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.RiskManagement.Models
{
    public class WorkersCompLegalFirm
    {
        [Key]
        public int LegalFirmID { get; set; }
        [Required]
        [StringLength(50)]
        public string LegalFirmNm { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}