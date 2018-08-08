using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtriumWebApp.Web.ClinicalOps.Models
{
    public class EmployeeVaccineRecord
    {
        [Key]
        public int VaccineRecordId { get; set; }
        public int PersonId { get; set; }
        public int VaccineTypeId { get; set; }
        //public int CurrentPayerId { get; set; }
        //public int RoomId { get; set; }

        //public bool ConsentSignedFlg { get; set; }
        //public bool ImmunizationPriorToAdmissionFlg { get; set; }
        public bool OfferedRefusedFlg { get; set; }
        public DateTime? OfferedRefusedDate { get; set; }
        public string LotNumber { get; set; }

        public bool DeletedFlg { get; set; }
        public DateTime? DeletedTS { get; set; }
        public string DeletedADDomainName { get; set; }
        public DateTime? LastModifiedDate { get; set; }

        [ForeignKey("VaccineTypeId")]
        public virtual VaccineType VaccineType { get; set; }
    }
}