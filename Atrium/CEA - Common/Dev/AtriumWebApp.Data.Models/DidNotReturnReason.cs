using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class DidNotReturnReason
    {
        [Key]
        public int DidNotReturnReasonId { get; set; }
        [Required]
        [MaxLength(64)]
        public string DidNotReturnReasonDesc { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
    }
}
