using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class CompositeWoundDescribe
    {
        [Key]
        public int CompositeWoundDescribeId { get; set; }
        [Required]
        [MaxLength(20)]
        public string CompositeWoundDescribeName { get; set; }
        public bool DataEntryFlg { get; set; }
        public bool ReportFlg { get; set; }
        public int SortOrder { get; set; }
    }
}
