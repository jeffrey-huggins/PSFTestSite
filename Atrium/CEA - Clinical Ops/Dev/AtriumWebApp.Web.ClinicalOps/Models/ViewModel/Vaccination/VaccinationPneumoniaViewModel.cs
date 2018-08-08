using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class VaccinationPneumoniaViewModel : MasterVaccinationViewModel
    {
        [Required]
        public DateTime? NextDueDate { get; set; }
        [DisplayName("Vaccine Type")]
        public int VaccinePneumoniaTypeId { get; set; }
        public static implicit operator VaccinationPneumoniaViewModel(PatientVaccinePneumonia vaccine)
        {
            return new VaccinationPneumoniaViewModel()
            {
                VaccineId = vaccine.PatientVaccineId,
                ConsentSignedFlg = vaccine.ConsentSignedFlg,
                CurrentPayerId = vaccine.CurrentPayerId,
                DeletedADDomainName = vaccine.DeletedADDomainName,
                DeletedFlg = vaccine.DeletedFlg,
                DeletedTS = vaccine.DeletedTS,
                ImmunizationPriorToAdmissionFlg = vaccine.ImmunizationPriorToAdmissionFlg,
                LotNumber = vaccine.LotNumber,
                NextDueDate = vaccine.NextDueDate,
                OfferedRefusedDate = vaccine.OfferedRefusedDate,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                PersonId = vaccine.PatientId,
                RoomId = vaccine.RoomId,
                VaccineDate = vaccine.VaccineDate,
                VaccinePneumoniaTypeId = vaccine.VaccinePneumoniaTypeId,
                VaccineTypeId = vaccine.VaccineTypeId,
                VaccineType = vaccine.VaccineType
            };
        }
        public static implicit operator PatientVaccinePneumonia(VaccinationPneumoniaViewModel vm)
        {
            return new PatientVaccinePneumonia()
            {
                PatientVaccineId = vm.VaccineId,
                ConsentSignedFlg = vm.ConsentSignedFlg,
                CurrentPayerId = vm.CurrentPayerId.Value,
                DeletedADDomainName = vm.DeletedADDomainName,
                DeletedFlg = vm.DeletedFlg,
                DeletedTS = vm.DeletedTS,
                ImmunizationPriorToAdmissionFlg = vm.ImmunizationPriorToAdmissionFlg,
                LotNumber = vm.LotNumber,
                NextDueDate = vm.NextDueDate,
                OfferedRefusedDate = vm.OfferedRefusedDate,
                OfferedRefusedFlg = vm.OfferedRefusedFlg,
                PatientId = vm.PersonId,
                RoomId = vm.RoomId.Value,
                VaccineDate = vm.VaccineDate,
                VaccinePneumoniaTypeId = vm.VaccinePneumoniaTypeId,
                VaccineTypeId = vm.VaccineTypeId,
                VaccineType = vm.VaccineType
            };
        }
    }
}
