using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Financial.Models
{
    public class EmergencyBudget
    {
        public int Id { get; set; }
        public string BudgetYear { get; set; }
        public decimal BudgetAmt { get; set; }
    }
}