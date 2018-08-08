using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace AtriumWebApp.Models
{
    public class SOCRestraint
    {
        [Key]
        public int SOCRestraintId { get; set; }
        [Required]
        [MaxLength(30)]
        public string SOCRestraintName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}
