using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Web.ClinicalOps.Models;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class SOCInterventionSelection
    {
		public bool Selected { set; get; }
		public SOCFallIntervention Type { get; set; }
    }
}
