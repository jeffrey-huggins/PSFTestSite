using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class PatientIncidentEventViewModel
    {
		public PatientIncidentEvent Event { get; set; }
		public List<IncidentInterventionSelection> InterventionList { get; set; }
		public List<IncidentTreatmentSelection> TreatmentList { get; set; }
    }
}
