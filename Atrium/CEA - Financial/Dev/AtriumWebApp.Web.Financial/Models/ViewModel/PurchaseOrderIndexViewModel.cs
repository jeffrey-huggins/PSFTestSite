using System;
using System.Collections.Generic;
using System.ComponentModel;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class PurchaseOrderIndexViewModel
    {
        //public bool IsAdministrator { get; set; }
        public bool CanManageBudget { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public IList<PurchaseOrderViewModel> Items { get; set; }
        [DisplayName("Community")]
        public int CurrentCommunity { get; set; }
        public IList<Community> Communities { get; set; }
        [DisplayName("From")]
        public DateTime DateRangeFrom { get; set; }
        [DisplayName("To")]
        public DateTime DateRangeTo { get; set; }
        public PurchaseOrderViewModel PurchaseOrder { get; set; }
    }
}