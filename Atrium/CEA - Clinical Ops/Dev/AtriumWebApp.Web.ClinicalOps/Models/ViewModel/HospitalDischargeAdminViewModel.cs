using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class HospitalDischargeAdminViewModel
    {
        public AdminViewModel AdminViewModel { get; set; }
        public IList<DischargeReason> DischargeReasons { get; set; }
        public IList<DidNotReturnReason> DidNotReturnReasons { get; set; }
        public IList<Hospital> Hospitals { get; set; }
        public IList<AtriumPayerGroup> PayerGroups { get; set; }
        public IList<ApplicationCommunityAtriumPayerGroupInfo> CommunityPayerGroupInfo { get; set; }
        public DischargeReason NewDischarge
        {
            get { return new DischargeReason(); }
            set { NewDischarge = value; }
        }
        public DidNotReturnReason NewDNRR
        {
            get { return new DidNotReturnReason(); }
            set { NewDNRR = value; }
        }
        public Hospital NewHospital
        {
            get { return new Hospital(); }
            set { NewHospital = value; }
        }
    }
}
