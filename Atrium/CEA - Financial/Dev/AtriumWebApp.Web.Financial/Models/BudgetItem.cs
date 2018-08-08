using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Financial.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        public int CommunityId { get; set; }
        public string BudgetYear { get; set; }
        public string BudgetQtr { get; set; }
        public string Description { get; set; }
        public string Comments { get; set; }
        public decimal BudgetAmt { get; set; }
        [DisplayName("Special Project")]
        public bool IsSpecialProject { get; set; }
    }
}