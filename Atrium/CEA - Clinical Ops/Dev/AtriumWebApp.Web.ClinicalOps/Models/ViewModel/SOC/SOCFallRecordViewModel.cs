using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Web.ClinicalOps.Models;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class SOCFallRecordViewModel
    {
		public SOCEventFall FallEvent { get; set; }
		public List<SOCInterventionSelection> InterventionList { get; set; }
		public List<SOCFallTypeSelection> FallTypeList { get; set; }
		public List<SOCFallInjurySelection> InjuryList { get; set; }
		public List<SOCFallTreatmentSelection> TreatmentList { get; set; }
    }
}
