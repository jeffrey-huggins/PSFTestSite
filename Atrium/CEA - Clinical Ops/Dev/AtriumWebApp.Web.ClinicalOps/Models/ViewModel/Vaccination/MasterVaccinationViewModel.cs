using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class MasterVaccinationViewModel
    {
        public int VaccineId { get; set; }
        public int? CurrentPayerId { get; set; }
        public int PersonId { get; set; }
        public int? RoomId { get; set; }
        public int VaccineTypeId { get; set; }
        public bool ConsentSignedFlg { get; set; }
        public string DeletedADDomainName { get; set; }
        public bool DeletedFlg { get; set; }
        public DateTime? DeletedTS { get; set; }
        public bool ImmunizationPriorToAdmissionFlg { get; set; }
        public string LotNumber { get; set; }

        public DateTime? OfferedRefusedDate { get; set; }
        public bool OfferedRefusedFlg { get; set; }
        public DateTime? VaccineDate { get; set; }
        public bool IsEmployee { get; set; }
        public VaccineType VaccineType { get; set; }
    }
}
