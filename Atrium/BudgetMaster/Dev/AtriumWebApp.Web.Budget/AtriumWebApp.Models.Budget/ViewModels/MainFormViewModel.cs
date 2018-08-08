using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.Budget.ViewModels
{
    public class MainFormViewModel
    {
        public List<Census> Census { get; set; }
        public List<OtherRevenue> OtherRevenue { get; set; }
        public Facility Facility { get; set; }
        public FacilityBudget FacilityBudget { get; set; }
        public List<OtherExpensesGLAccount> OtherExpenses { get; set; }
    }
}
