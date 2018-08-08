using System.Collections.Generic;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class VaccinationAdminViewModel
    {
        public AdminViewModel AdminViewModel { get; set; }
        public List<ApplicationCommunityAtriumPayerGroupInfo> CommunityPayerGroupInfo { get; set; }
        public List<AtriumPayerGroup> PayerGroups { get; set; }
    }
}