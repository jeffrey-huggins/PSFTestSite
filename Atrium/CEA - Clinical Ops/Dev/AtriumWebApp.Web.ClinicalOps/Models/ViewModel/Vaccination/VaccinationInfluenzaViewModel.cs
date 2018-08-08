using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class VaccinationInfluenzaViewModel : MasterVaccinationViewModel
    {

        public static implicit operator VaccinationInfluenzaViewModel(PatientVaccineInfluenza vaccine)
        {
            return new VaccinationInfluenzaViewModel()
            {
                VaccineId = vaccine.PatientVaccineId,
                ConsentSignedFlg = vaccine.ConsentSignedFlg,
                CurrentPayerId = vaccine.CurrentPayerId,
                DeletedADDomainName = vaccine.DeletedADDomainName,
                DeletedFlg = vaccine.DeletedFlg,
                DeletedTS = vaccine.DeletedTS,
                ImmunizationPriorToAdmissionFlg = vaccine.ImmunizationPriorToAdmissionFlg,
                LotNumber = vaccine.LotNumber,
                OfferedRefusedDate = vaccine.OfferedRefusedDate,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                PersonId = vaccine.PatientId,
                RoomId = vaccine.RoomId,
                VaccineDate = vaccine.VaccineDate,
                VaccineTypeId = vaccine.VaccineTypeId,
                VaccineType = vaccine.VaccineType
            };
        }

        public static implicit operator PatientVaccineInfluenza(VaccinationInfluenzaViewModel vm)
        {
            return new PatientVaccineInfluenza()
            {
                ConsentSignedFlg = vm.ConsentSignedFlg,
                CurrentPayerId = vm.CurrentPayerId.Value,
                DeletedADDomainName = vm.DeletedADDomainName,
                DeletedFlg = vm.DeletedFlg,
                DeletedTS = vm.DeletedTS,
                ImmunizationPriorToAdmissionFlg = vm.ImmunizationPriorToAdmissionFlg,
                LotNumber = vm.LotNumber,
                OfferedRefusedDate = vm.OfferedRefusedDate,
                OfferedRefusedFlg = vm.OfferedRefusedFlg,
                PatientId = vm.PersonId,
                PatientVaccineId = vm.VaccineId,
                RoomId = vm.RoomId.Value,
                VaccineDate = vm.VaccineDate,
                VaccineTypeId = vm.VaccineTypeId
            };
        }

        public static implicit operator VaccinationInfluenzaViewModel(EmployeeVaccineInfluenzaRecord vaccine)
        {
            return new VaccinationInfluenzaViewModel()
            {
                DeletedADDomainName = vaccine.DeletedADDomainName,
                DeletedFlg = vaccine.DeletedFlg,
                DeletedTS = vaccine.DeletedTS,
                LotNumber = vaccine.LotNumber,
                OfferedRefusedDate = vaccine.OfferedRefusedDate,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                VaccineTypeId = vaccine.VaccineTypeId,
                VaccineId = vaccine.VaccineRecordId,
                VaccineDate = vaccine.VaccineDate,
                PersonId = vaccine.PersonId,
                VaccineType = vaccine.VaccineType
            };
        }

        public static implicit operator EmployeeVaccineInfluenzaRecord(VaccinationInfluenzaViewModel vm)
        {
            return new EmployeeVaccineInfluenzaRecord()
            {
                DeletedADDomainName = vm.DeletedADDomainName,
                DeletedFlg = vm.DeletedFlg,
                DeletedTS = vm.DeletedTS,
                LotNumber = vm.LotNumber,
                OfferedRefusedDate = vm.OfferedRefusedDate,
                OfferedRefusedFlg = vm.OfferedRefusedFlg,
                VaccineTypeId = vm.VaccineTypeId,
                VaccineRecordId = vm.VaccineId,               
                VaccineDate = vm.VaccineDate,
                PersonId = vm.PersonId
            };
        }
    }
}
