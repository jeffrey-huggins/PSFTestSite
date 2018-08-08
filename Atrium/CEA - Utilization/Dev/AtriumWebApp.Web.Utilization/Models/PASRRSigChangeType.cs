using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PASRRSigChangeType
    {
        [Key]
        public int SigChangeTypeId { get; set; }
        public string SigChangeTypeName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
        public virtual List<PASRRSigChangeTypeStateCode> StateCodes { get; set; }
    }
}