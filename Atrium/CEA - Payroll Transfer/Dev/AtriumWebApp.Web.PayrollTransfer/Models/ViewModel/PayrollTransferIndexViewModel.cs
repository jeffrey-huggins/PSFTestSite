using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.PayrollTransfer.Models.ViewModel
{
    public class PayrollTransferIndexViewModel
    {
        public bool IsAdministrator { get; set; }
        public IList<PayrollTransferViewModel> Items { get; set; }
        [DisplayName("Community")]
        public int CurrentCommunity { get; set; }
        public IList<Community> Communities { get; set; }
        [DisplayName("From")]
        public DateTime DateRangeFrom { get; set; }
        [DisplayName("To")]
        public DateTime DateRangeTo { get; set; }
        public PayrollTransferViewModel PayrollTransfer { get; set; }
        public string AppCode { get; set; }
        public bool UseEditTab { get; set; }
        public bool UseContractorEditTab { get; set; }
        public bool CanValidatePBJ { get; set; }
    }
}