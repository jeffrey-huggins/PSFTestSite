using System.Collections.Generic;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.Utilization.Models.ViewModel
{
    public class PELIAdminViewModel
    {
        public AdminHomeViewModel AdminHomeViewModel { get; set; }
        public AdminViewModel AdminViewModel { get; set; }
        public IEnumerable<PELIType> PELITypes { get; set; }
        public PELIType NewPELIType { get { return new PELIType(); } }
    }
}