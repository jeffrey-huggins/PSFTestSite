using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.HR.Models.ViewModel
{
    public class EmployeeInfoViewModel
    {
		[DisplayName("Name/Community:")]
		public string EmployeeName { get; set; }
		public string CommunityName { get; set; }
		[DisplayName("Date of Hire:")]
		public DateTime HireDate { get; set; }
		[DisplayName("Date of Termination:")]
		public DateTime? TerminationDate { get; set; }
    }
}
