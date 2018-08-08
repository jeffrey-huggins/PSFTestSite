using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class IncidentTreatmentSelection
    {
		public bool Selected { set; get; }
		public IncidentTreatment Type { get; set; }
	}
}
