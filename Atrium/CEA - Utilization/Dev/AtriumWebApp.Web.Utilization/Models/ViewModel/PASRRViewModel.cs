using System.Collections.Generic;

namespace AtriumWebApp.Web.Utilization.Models.ViewModel
{
    public class PASRRViewModel
    {
        public List<PASRRType> PASRRTypes { get; set; }
        public List<PASRRSigChangeType> SigChangeTypes { get; set; }
        public List<PatientPASRRLog> PatientPASRRLogs { get; set; }
    }
}