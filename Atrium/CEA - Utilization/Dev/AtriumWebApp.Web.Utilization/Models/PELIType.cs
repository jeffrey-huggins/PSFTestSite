using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PELIType
    {
        [Key]
        public int Id { get; set; } // [MasterPELIIncompleteReasonId]
        [Required]
        [MaxLength(64)]
        public string Description { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}