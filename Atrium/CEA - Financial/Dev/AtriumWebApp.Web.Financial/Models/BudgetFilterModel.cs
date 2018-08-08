using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AtriumWebApp.Web.Financial.Models
{
    public class BudgetFilterModel
    {
        public string BudgetYear { get; set; }
        public int CommunityId { get; set; }
    }
}