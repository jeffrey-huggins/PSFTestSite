using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AtriumWebApp.Models.Budget.ViewModels
{
    public class PayrollViewModel
    {
        public List<PayrollGLAccount> PayrollAccounts { get; set; }
        public List<GeneralLedger> GLAccounts { get; set; }
        public List<MonthTable> Months { get; set; }
        public List<PayGrp> PayerGroups { get; set; }
    }
}
