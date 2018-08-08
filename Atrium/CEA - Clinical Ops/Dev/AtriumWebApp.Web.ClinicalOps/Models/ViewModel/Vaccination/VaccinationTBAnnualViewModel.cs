using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class VaccinationTBAnnualViewModel : MasterVaccinationViewModel
    {
        public DateTime? BMATDate { get; set; }
        public bool BMATFlg { get; set; }
        public bool PPDReactionFlg { get; set; }
        public int? PPDReactionMeasurementId { get; set; }
        public DateTime? PPDReadDate { get; set; }
        public int? PPDSiteId { get; set; }
        public DateTime? QuestionnaireCompleteDate { get; set; }
        public bool QuestionnaireCompleteFlg { get; set; }

        public static implicit operator VaccinationTBAnnualViewModel(PatientVaccineTBAnnual vaccine)
        {
            return new VaccinationTBAnnualViewModel()
            {
                BMATDate = vaccine.BMATDate,
                BMATFlg = vaccine.BMATFlg,
                VaccineId = vaccine.PatientVaccineId,
                ConsentSignedFlg = vaccine.ConsentSignedFlg,
                CurrentPayerId = vaccine.CurrentPayerId,
                DeletedADDomainName = vaccine.DeletedADDomainName,
                DeletedFlg = vaccine.DeletedFlg,
                DeletedTS = vaccine.DeletedTS,
                ImmunizationPriorToAdmissionFlg = vaccine.ImmunizationPriorToAdmissionFlg,
                LotNumber = vaccine.PPDLotNumber,
                OfferedRefusedDate = vaccine.OfferedRefusedDate,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                PersonId = vaccine.PatientId,
                PPDReactionFlg = vaccine.PPDReactionFlg,
                PPDReactionMeasurementId = vaccine.PPDReactionMeasurementId,
                PPDReadDate = vaccine.PPDReadDate,
                PPDSiteId = vaccine.PPDSiteId,
                QuestionnaireCompleteDate = vaccine.QuestionnaireCompleteDate,
                QuestionnaireCompleteFlg = vaccine.QuestionnaireCompleteFlg,
                RoomId = vaccine.RoomId,
                VaccineDate = vaccine.PPDGivenDate,
                VaccineTypeId = vaccine.VaccineTypeId,
                VaccineType = vaccine.VaccineType
            };
        }

        public static implicit operator PatientVaccineTBAnnual(VaccinationTBAnnualViewModel vm)
        {
            return new PatientVaccineTBAnnual()
            {
                BMATDate = vm.BMATDate,
                BMATFlg = vm.BMATFlg,
                PatientVaccineId = vm.VaccineId,
                ConsentSignedFlg = vm.ConsentSignedFlg,
                CurrentPayerId = vm.CurrentPayerId.Value,
                DeletedADDomainName = vm.DeletedADDomainName,
                DeletedFlg = vm.DeletedFlg,
                DeletedTS = vm.DeletedTS,
                ImmunizationPriorToAdmissionFlg = vm.ImmunizationPriorToAdmissionFlg,
                PPDLotNumber = vm.LotNumber,
                OfferedRefusedDate = vm.OfferedRefusedDate,
                OfferedRefusedFlg = vm.OfferedRefusedFlg,
                PatientId = vm.PersonId,
                PPDReactionFlg = vm.PPDReactionFlg,
                PPDReactionMeasurementId = vm.PPDReactionMeasurementId ?? -1,
                PPDReadDate = vm.PPDReadDate,
                PPDSiteId = vm.PPDSiteId,
                QuestionnaireCompleteDate = vm.QuestionnaireCompleteDate,
                QuestionnaireCompleteFlg = vm.QuestionnaireCompleteFlg,
                RoomId = vm.RoomId.Value,
                PPDGivenDate = vm.VaccineDate,
                VaccineTypeId = vm.VaccineTypeId,
                VaccineType = vm.VaccineType
            };
        }

        public static implicit operator VaccinationTBAnnualViewModel(EmployeeVaccineTBAnnualRecord vaccine)
        {
            return new VaccinationTBAnnualViewModel()
            {
                BMATDate = vaccine.BMATDate,
                BMATFlg = vaccine.BMATFlg,
                VaccineId = vaccine.VaccineRecordId,
                DeletedADDomainName = vaccine.DeletedADDomainName,
                DeletedFlg = vaccine.DeletedFlg,
                DeletedTS = vaccine.DeletedTS,
                LotNumber = vaccine.LotNumber,
                OfferedRefusedDate = vaccine.OfferedRefusedDate,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                PersonId = vaccine.PersonId,
                PPDReactionFlg = vaccine.PPDReactionFlg,
                PPDReactionMeasurementId = vaccine.PPDReactionMeasurementId,
                PPDReadDate = vaccine.PPDReadDate,
                PPDSiteId = vaccine.PPDSiteId,
                QuestionnaireCompleteDate = vaccine.QuestionnaireCompleteDate,
                QuestionnaireCompleteFlg = vaccine.QuestionnaireCompleteFlg,
                VaccineDate = vaccine.PPDGivenDate,
                VaccineTypeId = vaccine.VaccineTypeId,
                VaccineType = vaccine.VaccineType
            };
        }

        public static implicit operator EmployeeVaccineTBAnnualRecord(VaccinationTBAnnualViewModel vm)
        {
            return new EmployeeVaccineTBAnnualRecord()
            {
                BMATDate = vm.BMATDate,
                BMATFlg = vm.BMATFlg,
                VaccineRecordId = vm.VaccineId,
                DeletedADDomainName = vm.DeletedADDomainName,
                DeletedFlg = vm.DeletedFlg,
                DeletedTS = vm.DeletedTS,
                LotNumber = vm.LotNumber,
                OfferedRefusedDate = vm.OfferedRefusedDate,
                OfferedRefusedFlg = vm.OfferedRefusedFlg,
                PersonId = vm.PersonId,
                PPDReactionFlg = vm.PPDReactionFlg,
                PPDReactionMeasurementId = vm.PPDReactionMeasurementId ?? -1,
                PPDReadDate = vm.PPDReadDate,
                PPDSiteId = vm.PPDSiteId,
                QuestionnaireCompleteDate = vm.QuestionnaireCompleteDate,
                QuestionnaireCompleteFlg = vm.QuestionnaireCompleteFlg,
                PPDGivenDate = vm.VaccineDate,
                VaccineTypeId = vm.VaccineTypeId,
                VaccineType = vm.VaccineType
            };
        }
    }
}
