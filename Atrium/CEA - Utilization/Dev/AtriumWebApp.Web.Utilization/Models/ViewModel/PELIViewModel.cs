using System.Collections.Generic;

namespace AtriumWebApp.Web.Utilization.Models.ViewModel
{
    public class PELIViewModel
    {
        public List<PELIType> PELITypes { get; set; }
        public List<PELIType> PELITypesALL { get; set; }
        public List<PatientPELILog> PatientPELILogs { get; set; }
    }
}