using AtriumWebApp.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class VaccinationViewModel
    {
        public string RangeTo { get; set; }
        public string RangeFrom { get; set; }
        public SideBarViewModel SideBar { get; set; }

        public int PatientId { get; set; }
        public int RoomId { get; set; }
        public int? PatientGroupPayerId { get; set; }
    }
}
