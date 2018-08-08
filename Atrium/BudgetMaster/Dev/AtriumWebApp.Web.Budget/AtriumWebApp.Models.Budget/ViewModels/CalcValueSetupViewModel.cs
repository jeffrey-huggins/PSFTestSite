using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.Budget.ViewModels
{
    public class CalcValueSetupViewModel
    {
        public OtherExpensesGLCalc Calc { get; set; }
        public List<OtherExpensesGLCalcValueMaster> MasterCalcs { get; set; }
    }
}
