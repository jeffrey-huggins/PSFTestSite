using AtriumWebApp.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class EmployeeVaccinationViewModel
    {
        public string RangeTo { get; set; }
        public string RangeFrom { get; set; }
        public EmployeeSidebarViewModel SideBar { get; set; }
    }
}
