using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.Budget.ViewModels
{
    public class OtherRevenueViewModel
    {
        public List<OtherRevenue> MonthlyCensus { get; set; }
        public List<GeneralLedger> GeneralLedgers { get; set; }
        public List<MonthTable> Months { get; set; }
    }
}
