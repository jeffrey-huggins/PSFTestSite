using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.Budget.ViewModels
{
    public class CensusViewModel
    {
        public decimal TotalADC { get; set; }
        public List<Census> MonthlyCensus { get; set; }
        public List<GeneralLedger> GeneralLedgers { get; set; }
        public List<MonthTable> Months { get; set; }
    }
}
