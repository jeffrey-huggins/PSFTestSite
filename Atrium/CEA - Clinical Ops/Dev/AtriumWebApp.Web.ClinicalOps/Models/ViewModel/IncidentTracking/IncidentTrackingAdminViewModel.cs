using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class IncidentTrackingAdminViewModel
    {
        public List<PatientIncidentType> IncidentTypes { get; set; }
        public List<IncidentTreatment> Treatments { get; set; }
        public List<PatientIncidentLocation> Locations { get; set; }
        public List<IncidentIntervention> Interventions { get; set; }
        public List<Employee> RegionalNurses { get; set; }
        public List<RegionalNurseCommunityInfo> RegionalNurseForCommunity { get; set; }
        public List<Employee> CloseAllCommmunityEmployees { get; set; }
        public List<CloseIncidentAllCommunity> CurrentCloseAllCommunityEmployees { get; set; }
        public PatientIncidentType NewIncidentType
        {
            get { return new PatientIncidentType(); }
        }
        public PatientIncidentLocation NewLocation
        {
            get { return new PatientIncidentLocation(); }
        }
        public IncidentIntervention NewIntervention
        {
            get { return new IncidentIntervention(); }
        }
        public IncidentTreatment NewTreatment
        {
            get { return new IncidentTreatment(); }
        }
        public AdminViewModel AdminViewModel { get; set; }

        public List<ApplicationCommunityAtriumPayerGroupInfo> CommunityPayerGroupInfo { get; set; }
        public List<AtriumPayerGroup> PayerGroups { get; set; }
    }
}
