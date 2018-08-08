using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Models
{
    [Serializable]
    public class GeneralLedgerAccount
    {
        [Key]
        public int GeneralLedgerId { get; set; }
        public string AccountNbr { get; set; }
        public string AccountName { get; set; }
        public string AtriumPayerGroupCode { get; set; }
        public int GLAccountTypeID { get; set; }
    }
}