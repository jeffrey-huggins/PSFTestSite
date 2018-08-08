using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class VaccinationTBInitial2StepViewModel : MasterVaccinationViewModel
    {
        public DateTime? QuestionnaireCompleteDate { get; set; }
        public bool QuestionnaireCompleteFlg { get; set; }
        public DateTime? BMATDate { get; set; }
        public bool BMATFlg { get; set; }
        public DateTime? ChestXRayDate { get; set; }
        public bool? ChestXRayFlg { get; set; }
        public bool PPDStep1ReactionFlg { get; set; }
        public int? PPDStep1ReactionMeasurementId { get; set; }
        public DateTime? PPDStep1ReadDate { get; set; }
        public int? PPDStep1SiteId { get; set; }

        public DateTime? PPDStep2GivenDate { get; set; }
        public string PPDStep2LotNumber { get; set; }
        public bool PPDStep2ReactionFlg { get; set; }
        public int? PPDStep2ReactionMeasurementId { get; set; }
        public DateTime? PPDStep2ReadDate { get; set; }
        public int? PPDStep2SiteId { get; set; }

        public static implicit operator VaccinationTBInitial2StepViewModel(PatientVaccineTBInitial2Step vaccine)
        {

            return new VaccinationTBInitial2StepViewModel()
            {
                BMATDate = vaccine.BMATDate,
                BMATFlg = vaccine.BMATFlg,
                ChestXRayDate = vaccine.ChestXRayDate,
                ChestXRayFlg = vaccine.ChestXRayFlg,
                PPDStep1ReactionFlg = vaccine.PPDStep1ReactionFlg,
                PPDStep1ReactionMeasurementId = vaccine.PPDStep1ReactionMeasurementId,
                PPDStep1ReadDate = vaccine.PPDStep1ReadDate,
                PPDStep1SiteId =vaccine.PPDStep1SiteId,
                PPDStep2GivenDate = vaccine.PPDStep2GivenDate,
                PPDStep2LotNumber = vaccine.PPDStep2LotNumber,
                PPDStep2ReactionFlg = vaccine.PPDStep2ReactionFlg,
                PPDStep2ReactionMeasurementId = vaccine.PPDStep2ReactionMeasurementId,
                PPDStep2ReadDate = vaccine.PPDStep2ReadDate,
                PPDStep2SiteId = vaccine.PPDStep2SiteId,
                VaccineId = vaccine.PatientVaccineId,
                ConsentSignedFlg = vaccine.ConsentSignedFlg,
                CurrentPayerId = vaccine.CurrentPayerId,
                DeletedADDomainName = vaccine.DeletedADDomainName,
                DeletedFlg = vaccine.DeletedFlg,
                DeletedTS = vaccine.DeletedTS,
                ImmunizationPriorToAdmissionFlg = vaccine.ImmunizationPriorToAdmissionFlg,
                LotNumber = vaccine.PPDStep1LotNumber,
                OfferedRefusedDate = vaccine.OfferedRefusedDate,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                PersonId = vaccine.PatientId,
                QuestionnaireCompleteDate = vaccine.QuestionnaireCompleteDate,
                QuestionnaireCompleteFlg = vaccine.QuestionnaireCompleteFlg,
                RoomId = vaccine.RoomId,
                VaccineDate = vaccine.PPDStep1GivenDate,
                VaccineTypeId = vaccine.VaccineTypeId,
                VaccineType = vaccine.VaccineType
            };
        }
        public static implicit operator PatientVaccineTBInitial2Step(VaccinationTBInitial2StepViewModel vm)
        {

            return new PatientVaccineTBInitial2Step()
            {
                BMATDate = vm.BMATDate,
                BMATFlg = vm.BMATFlg,
                ChestXRayDate = vm.ChestXRayDate,
                ChestXRayFlg = vm.ChestXRayFlg,
                PPDStep1ReactionFlg = vm.PPDStep1ReactionFlg,
                PPDStep1ReactionMeasurementId = vm.PPDStep1ReactionMeasurementId ?? -1,
                PPDStep1ReadDate = vm.PPDStep1ReadDate,
                PPDStep1SiteId = vm.PPDStep1SiteId ?? -1,
                PPDStep2GivenDate = vm.PPDStep2GivenDate,
                PPDStep2LotNumber = vm.PPDStep2LotNumber,
                PPDStep2ReactionFlg = vm.PPDStep2ReactionFlg,
                PPDStep2ReactionMeasurementId = vm.PPDStep2ReactionMeasurementId ?? -1,
                PPDStep2ReadDate = vm.PPDStep2ReadDate,
                PPDStep2SiteId = vm.PPDStep2SiteId ?? -1,
                PatientVaccineId = vm.VaccineId,
                ConsentSignedFlg = vm.ConsentSignedFlg,
                CurrentPayerId = vm.CurrentPayerId.Value,
                DeletedADDomainName = vm.DeletedADDomainName,
                DeletedFlg = vm.DeletedFlg,
                DeletedTS = vm.DeletedTS,
                ImmunizationPriorToAdmissionFlg = vm.ImmunizationPriorToAdmissionFlg,
                PPDStep1LotNumber = vm.LotNumber,
                OfferedRefusedDate = vm.OfferedRefusedDate,
                OfferedRefusedFlg = vm.OfferedRefusedFlg,
                PatientId = vm.PersonId,
                QuestionnaireCompleteDate = vm.QuestionnaireCompleteDate,
                QuestionnaireCompleteFlg = vm.QuestionnaireCompleteFlg,
                RoomId = vm.RoomId.Value,
                PPDStep1GivenDate = vm.VaccineDate,
                VaccineTypeId = vm.VaccineTypeId,
                VaccineType = vm.VaccineType
                
            };
        }
        public static implicit operator VaccinationTBInitial2StepViewModel(EmployeeVaccineTBInitial2StepRecord vaccine)
        {

            return new VaccinationTBInitial2StepViewModel()
            {
                BMATDate = vaccine.BMATDate,
                BMATFlg = vaccine.BMATFlg,
                ChestXRayDate = vaccine.ChestXRayDate,
                ChestXRayFlg = vaccine.ChestXRayFlg,
                PPDStep1ReactionFlg = vaccine.PPDStep1ReactionFlg,
                PPDStep1ReactionMeasurementId = vaccine.PPDStep1ReactionMeasurementId,
                PPDStep1ReadDate = vaccine.PPDStep1ReadDate,
                PPDStep1SiteId = vaccine.PPDStep1SiteId,
                PPDStep2GivenDate = vaccine.PPDStep2GivenDate,
                PPDStep2ReactionFlg = vaccine.PPDStep2ReactionFlg,
                PPDStep2ReactionMeasurementId = vaccine.PPDStep2ReactionMeasurementId,
                PPDStep2ReadDate = vaccine.PPDStep2ReadDate,
                PPDStep2SiteId = vaccine.PPDStep2SiteId,
                VaccineId = vaccine.VaccineRecordId,
                DeletedADDomainName = vaccine.DeletedADDomainName,
                DeletedFlg = vaccine.DeletedFlg,
                DeletedTS = vaccine.DeletedTS,
                LotNumber = vaccine.LotNumber,
                OfferedRefusedDate = vaccine.OfferedRefusedDate,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                PersonId = vaccine.PersonId,
                QuestionnaireCompleteDate = vaccine.QuestionnaireCompleteDate,
                QuestionnaireCompleteFlg = vaccine.QuestionnaireCompleteFlg,
                VaccineDate = vaccine.PPDStep1GivenDate,
                VaccineTypeId = vaccine.VaccineTypeId,
                VaccineType = vaccine.VaccineType
            };
        }
        public static implicit operator EmployeeVaccineTBInitial2StepRecord(VaccinationTBInitial2StepViewModel vm)
        {

            return new EmployeeVaccineTBInitial2StepRecord()
            {
                BMATDate = vm.BMATDate,
                BMATFlg = vm.BMATFlg,
                ChestXRayDate = vm.ChestXRayDate,
                ChestXRayFlg = vm.ChestXRayFlg,
                PPDStep1ReactionFlg = vm.PPDStep1ReactionFlg,
                PPDStep1ReactionMeasurementId = vm.PPDStep1ReactionMeasurementId ?? -1,
                PPDStep1ReadDate = vm.PPDStep1ReadDate,
                PPDStep1SiteId = vm.PPDStep1SiteId ?? -1,
                PPDStep2GivenDate = vm.PPDStep2GivenDate,
                PPDStep2ReactionFlg = vm.PPDStep2ReactionFlg,
                PPDStep2ReactionMeasurementId = vm.PPDStep2ReactionMeasurementId ?? -1,
                PPDStep2ReadDate = vm.PPDStep2ReadDate,
                PPDStep2SiteId = vm.PPDStep2SiteId ?? -1,
                VaccineRecordId = vm.VaccineId,
                DeletedADDomainName = vm.DeletedADDomainName,
                DeletedFlg = vm.DeletedFlg,
                DeletedTS = vm.DeletedTS,
                LotNumber = vm.LotNumber,
                OfferedRefusedDate = vm.OfferedRefusedDate,
                OfferedRefusedFlg = vm.OfferedRefusedFlg,
                PersonId = vm.PersonId,
                QuestionnaireCompleteDate = vm.QuestionnaireCompleteDate,
                QuestionnaireCompleteFlg = vm.QuestionnaireCompleteFlg,
                PPDStep1GivenDate = vm.VaccineDate,
                VaccineTypeId = vm.VaccineTypeId,
                VaccineType = vm.VaccineType
            };
        }
    }
}
