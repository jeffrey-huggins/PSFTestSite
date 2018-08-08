using AtriumWebApp.Web.ClinicalOps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class SOCFallTypeSelection
    {
		public bool Selected { set; get; }
		public SOCFallType Type { get; set; }
	}
}
