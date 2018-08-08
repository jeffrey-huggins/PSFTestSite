using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AtriumWebApp.Web.Utilization.Models
{
    public class PASRRType
    {
        [Key]
        public int PASRRTypeId { get; set; }
        public string PASRRTypeName { get; set; }
        public bool DataEntryFlg { get; set; }
        public int SortOrder { get; set; }
        public virtual List<PASRRTypeStateCode> StateCodes { get; set; }
    }
}