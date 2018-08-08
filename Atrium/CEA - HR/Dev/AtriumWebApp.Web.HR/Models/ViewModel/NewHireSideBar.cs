using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.HR.Models.ViewModel
{
    public class NewHireSideBar
    {
		[DisplayName("Terminated:")]
		public bool IncludeTerminated { get; set; }
		[DisplayName("Hire Date:")]
		public DateTime LookBackDate { get; set; }		
		public SelectList FacilityList { get; set; }
		public SelectList EmployeeList { get; set; }
        public List<EmployeeDocumentStatus> EmployeeStatusList { get; set; }
		[DisplayName("Community:")]
		public int SelectedFacilityId { get; set; }
		[DisplayName("Employee")]
		public int SelectedEmployeeId { get; set; }
		
    }
}
