using AtriumWebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class BudgetViewModel: PurchaseOrderIndexViewModel
    {
        //DDL Lists
        public IDictionary<string, string> BudgetQuarters { get; set; }
        public IDictionary<string, string> BudgetYears { get; set; }

        public int CommunityId { get; set; }
        public Community Community { get; set; }
        public string BudgetYear { get; set; }
        public decimal PreviousYearRolloverAmt { get; set; }
        public int BedCount { get; set; }
        public decimal AmtPerBed { get; set; }
        public decimal EmergencyBudgetAmt { get; set; }
        public IList<BudgetItem> BudgetItems { get; set; }
        public decimal BudgetedAmount { get { return BudgetItems != null ? BudgetItems.Sum(i => i.BudgetAmt) : 0; } }
    }
}