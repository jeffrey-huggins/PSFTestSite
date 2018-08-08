using System.Collections.Generic;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.Utilization.Models.ViewModel
{
    public class SkilledChartingAdminViewModel
    {
        public AdminViewModel AdminViewModel { get; set; }
        public List<SkilledChartingGuideline> SkilledChartingGuidlines { get; set; }
        public SkilledChartingGuideline NewGuideline { get { return new SkilledChartingGuideline(); } }
    }
}