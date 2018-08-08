using AtriumWebApp.Web.ClinicalOps.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class SOCFallTreatmentSelection
    {
		public bool Selected { set; get; }
		public SOCFallTreatment Type { get; set; }
	}
}
