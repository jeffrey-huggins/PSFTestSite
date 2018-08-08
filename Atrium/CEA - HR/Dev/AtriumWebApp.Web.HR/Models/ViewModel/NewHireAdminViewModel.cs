using AtriumWebApp.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.HR.Models.ViewModel
{
    public class NewHireAdminViewModel
    {
		public int LookbackDays { get; set; }
		public AdminViewModel AdminViewModel { get; set; }
	}
}
