using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    public class ContractPaymentTerm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDataEntry { get; set; }
        public bool IsReportable { get; set; }
        public int SortOrder { get; set; }
    }
}