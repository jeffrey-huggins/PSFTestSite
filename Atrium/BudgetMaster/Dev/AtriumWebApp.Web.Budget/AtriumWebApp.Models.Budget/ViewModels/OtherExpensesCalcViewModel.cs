using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.Budget.ViewModels
{
    public class OtherExpensesCalcViewModel
    {
        public List<OtherExpensesFacilityGLCalcValue> Calculations { get; set; }
        public List<MonthTable> Months { get; set; }
        public List<OtherExpensesGLCalcValueMaster> MasterCalculations { get; set; }
    }
}
