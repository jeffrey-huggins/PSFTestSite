using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ContractRenewal
    {
        public int Id { get; set; }
        public string Name { get; set; }
        
        [DisplayName("IsDataEntry")]
        public bool IsDataEntry { get; set; }
        public bool IsReportable { get; set; }
        public int SortOrder { get; set; }
    }
}