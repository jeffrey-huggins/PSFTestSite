using System.Collections.Generic;

namespace AtriumWebApp.Web.Utilization.Models.ViewModel
{
    public class SkilledChartingViewModel
    {
        public List<SkilledChartingGuideline> SkilledChartingGuidlines { get; set; }
        public List<PatientSkilledCharting> PatientSkilledChartingRecords { get; set; }
        public List<PatientSkilledChartingCustom> PatientSkilledChartingCustomRecords { get; set; }
    }
}