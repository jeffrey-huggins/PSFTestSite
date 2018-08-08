using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class IncidentTrackingViewModel
    {
		public SideBarViewModel SideBar { get; set; }
		public string RangeFrom { get; set; }
		public string RangeTo { get; set; }

    }
}