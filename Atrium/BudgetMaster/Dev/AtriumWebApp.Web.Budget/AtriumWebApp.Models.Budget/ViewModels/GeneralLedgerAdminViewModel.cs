using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.Budget.ViewModels
{
    public class GeneralLedgerAdminViewModel
    {
        public List<GeneralLedger> GeneralLedgers { get; set; }
        public List<LaborExpenseGLGrp> LaborGLGroups { get; set; }
        public List<GLAccountType> AccountTypes { get; set; }
        public List<PayGrp> PayGrpCds { get; set; }
    }
}
