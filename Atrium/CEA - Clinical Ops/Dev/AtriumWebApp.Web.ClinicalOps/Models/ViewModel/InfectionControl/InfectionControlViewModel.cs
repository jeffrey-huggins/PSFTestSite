using System.Collections.Generic;
using AtriumWebApp.Web.ClinicalOps.Enumerations;
using Microsoft.AspNetCore.Mvc.Rendering;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class InfectionControlViewModel
    {
        public SideBarViewModel SideBar { get; set; }
        public string RangeFrom { get; set; }
        public string RangeTo { get; set; }
    }
}