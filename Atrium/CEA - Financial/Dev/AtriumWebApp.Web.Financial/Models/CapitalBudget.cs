using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Financial.Models
{
    public class CapitalBudget
    {
        public int Id { get; set; }
        public int CommunityId { get; set; }
        public string BudgetYear { get; set; }
        public int BedCount { get; set; }
        public decimal AmtPerBed { get; set; }
        //public decimal EmergencyBudgetAmt { get; set; }
    }
}