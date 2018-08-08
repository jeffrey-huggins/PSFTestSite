using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using AtriumWebApp.Models;

namespace AtriumWebApp.Models.ViewModel
{
    public class SideBarViewModel
    {
        public List<Community> FacilityList { get; set; }
        public List<Patient> ResidentList { get; set; }
        public Community SelectedCommunity { get; set; }
        public Patient SelectedResident { get; set; }
        public DateTime LookbackDate { get; set; }
        public List<string> ResidentDiagnosis { get; set; }
        public string AdmissionDiagnosis { get; set; }
        public bool CensusDateInvalid { get; set; }
        public bool CensusDateInFuture { get; set; }
		public string RedirectUrl { get; set; }
    }
}
