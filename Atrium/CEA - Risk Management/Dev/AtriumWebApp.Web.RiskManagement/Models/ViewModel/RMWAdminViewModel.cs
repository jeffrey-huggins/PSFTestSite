using System.Collections.Generic;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.RiskManagement.Models.ViewModel
{
    public class RMWAdminViewModel
    {
        public List<WorkersCompInsurance> Insurances { get; set; }
        public List<WorkersCompLegalFirm> LegalFirms { get; set; }
        public List<WorkersCompVOCRehab> VOCRehabs { get; set; }
        public List<WorkersCompTCM> TCMs { get; set; }
        public List<WorkersCompClaimType> ClaimTypes { get; set; }
        public AdminViewModel AdminViewModel { get; set; }
        public WorkersCompInsurance NewInsurance { get { return new WorkersCompInsurance(); } }
        public WorkersCompLegalFirm NewLegal { get { return new WorkersCompLegalFirm(); } }
        public WorkersCompVOCRehab NewVOC { get { return new WorkersCompVOCRehab(); } }
        public WorkersCompTCM NewTCM { get { return new WorkersCompTCM(); } }
        public WorkersCompClaimType NewClaimType { get { return new WorkersCompClaimType(); } }
    }
}