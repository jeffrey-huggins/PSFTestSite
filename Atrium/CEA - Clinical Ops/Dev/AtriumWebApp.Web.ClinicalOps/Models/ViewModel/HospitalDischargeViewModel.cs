using System;
using System.Collections.Generic;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class HospitalDischargeViewModel
    {
        public IList<HospitalDischargePTO> DischargesForFacility { get; set; }
        public IList<DischargeReason> DischargeReasons { get; set; }
        public IList<DischargeReason> ERDischargeReasons { get; set; }
        public IList<DischargeReason> HospitalDischargeReasons { get; set; }
        public IList<DidNotReturnReason> DidNotReturnReasons { get; set; }
        public IList<Hospital> Hospitals { get; set; }
        public IList<Hospital> AllHospitals { get; set; }
    }
}