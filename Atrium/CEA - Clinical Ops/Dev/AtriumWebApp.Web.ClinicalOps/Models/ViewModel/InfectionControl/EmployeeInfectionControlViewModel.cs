using System.Collections.Generic;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class EmployeeInfectionControlViewModel
    {
        public EmployeeSidebarViewModel SideBar { get; set; }
        public string RangeFrom { get; set; }
        public string RangeTo { get; set; }
    }
}