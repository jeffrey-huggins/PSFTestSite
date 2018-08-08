using System;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
    public class VaccineHistoryViewModel
    {
        public int PatientVaccineId { get; set; }
        public VaccineType VaccineType { get; set; }
        public DateTime? VaccineDate { get; set; }
        public bool ConsentSignedFlg { get; set; }
        public bool OfferedRefusedFlg { get; set; }
        public bool IsEmployee { get; set; }
        public bool PriorToAdmission { get; set; }

        public static implicit operator VaccineHistoryViewModel(EmployeeVaccineRecord vaccine)
        {
            DateTime? date = null;
            if (vaccine.OfferedRefusedFlg)
            {
                date = vaccine.OfferedRefusedDate.Value;
            }
            var vm = new VaccineHistoryViewModel
            {
                PatientVaccineId = vaccine.VaccineRecordId,
                VaccineType = vaccine.VaccineType,
                VaccineDate = date,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                IsEmployee = true
            };
            return vm;
        }

        public static implicit operator VaccineHistoryViewModel(EmployeeVaccineInfluenzaRecord vaccine)
        {
            DateTime? date = null;
            if (vaccine.VaccineDate.HasValue)
            {
                date = vaccine.VaccineDate.Value;
            }
            else if (vaccine.OfferedRefusedFlg)
            {
                date = vaccine.OfferedRefusedDate.Value;
            }
            var vm = new VaccineHistoryViewModel
            {
                PatientVaccineId = vaccine.VaccineRecordId,
                VaccineType = vaccine.VaccineType,
                VaccineDate = date,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                IsEmployee = true
            };
            return vm;
        }

        public static implicit operator VaccineHistoryViewModel(EmployeeVaccineTBAnnualRecord vaccine)
        {
            DateTime? date = null;
            if (vaccine.PPDGivenDate.HasValue)
            {
                date = vaccine.PPDGivenDate.Value;
            }
            else if (vaccine.QuestionnaireCompleteFlg)
            {
                date = vaccine.QuestionnaireCompleteDate.Value;
            }
            else if (vaccine.BMATFlg)
            {
                date = vaccine.BMATDate.Value;
            }

            var vm = new VaccineHistoryViewModel
            {
                PatientVaccineId = vaccine.VaccineRecordId,
                VaccineType = vaccine.VaccineType,
                VaccineDate = date,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                IsEmployee = true
            };
            return vm;
        }

        public static implicit operator VaccineHistoryViewModel(EmployeeVaccineTBInitial2StepRecord vaccine)
        {
            DateTime? date = null;
            if (vaccine.PPDStep1GivenDate.HasValue)
            {
                date = vaccine.PPDStep1GivenDate.Value;
            }
            else if (vaccine.PPDStep2GivenDate.HasValue)
            {
                date = vaccine.PPDStep2GivenDate.Value;
            }
            else if (vaccine.ChestXRayFlg.HasValue)
            {
                if (vaccine.ChestXRayFlg.Value == true)
                {
                    date = vaccine.ChestXRayDate.Value;
                }
                else if (vaccine.QuestionnaireCompleteFlg)
                {
                    date = vaccine.QuestionnaireCompleteDate.Value;
                }
            }
            else if (vaccine.BMATFlg)
            {
                date = vaccine.BMATDate.Value;
            }
            var vm = new VaccineHistoryViewModel
            {
                PatientVaccineId = vaccine.VaccineRecordId,
                VaccineType = vaccine.VaccineType,
                VaccineDate = date,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                IsEmployee = true
            };
            return vm;
        }

        public static implicit operator VaccineHistoryViewModel(PatientVaccine vaccine)
        {

            DateTime? date = null;
            if (vaccine.OfferedRefusedFlg)
            {
                date = vaccine.OfferedRefusedDate.Value;
            }
            var vm = new VaccineHistoryViewModel
            {
                PatientVaccineId = vaccine.PatientVaccineId,
                VaccineType = vaccine.VaccineType,
                VaccineDate = date,
                ConsentSignedFlg = vaccine.ConsentSignedFlg,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                IsEmployee = false,
                PriorToAdmission = vaccine.ImmunizationPriorToAdmissionFlg
            };
            return vm;
        }

        public static implicit operator VaccineHistoryViewModel(PatientVaccineInfluenza vaccine)
        {
            DateTime? date = null;
            if (vaccine.VaccineDate.HasValue)
            {
                date = vaccine.VaccineDate.Value;
            }
            else if (vaccine.OfferedRefusedFlg)
            {
                date = vaccine.OfferedRefusedDate.Value;
            }
            var vm = new VaccineHistoryViewModel
            {
                PatientVaccineId = vaccine.PatientVaccineId,
                VaccineType = vaccine.VaccineType,
                VaccineDate = date,
                ConsentSignedFlg = vaccine.ConsentSignedFlg,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                IsEmployee = false,
                PriorToAdmission = vaccine.ImmunizationPriorToAdmissionFlg
            };
            return vm;
        }

        public static implicit operator VaccineHistoryViewModel(PatientVaccinePneumonia vaccine)
        {
            DateTime? date = null;
            if (vaccine.VaccineDate.HasValue)
            {
                date = vaccine.VaccineDate.Value;
            }
            else if (vaccine.OfferedRefusedFlg)
            {
                date = vaccine.OfferedRefusedDate.Value;
            }
            var vm = new VaccineHistoryViewModel
            {
                PatientVaccineId = vaccine.PatientVaccineId,
                VaccineType = vaccine.VaccineType,
                VaccineDate = date,
                ConsentSignedFlg = vaccine.ConsentSignedFlg,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                IsEmployee = false,
                PriorToAdmission = vaccine.ImmunizationPriorToAdmissionFlg
            };
            return vm;
        }

        public static implicit operator VaccineHistoryViewModel(PatientVaccineTBAnnual vaccine)
        {
            DateTime? date = null;
            if (vaccine.PPDGivenDate.HasValue)
            {
                date = vaccine.PPDGivenDate.Value;
            }
            else if (vaccine.QuestionnaireCompleteFlg)
            {
                date = vaccine.QuestionnaireCompleteDate.Value;
            }
            else if (vaccine.BMATFlg)
            {
                date = vaccine.BMATDate.Value;
            }

            var vm = new VaccineHistoryViewModel
            {
                PatientVaccineId = vaccine.PatientVaccineId,
                VaccineType = vaccine.VaccineType,
                VaccineDate = date,
                ConsentSignedFlg = vaccine.ConsentSignedFlg,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                IsEmployee = false,
                PriorToAdmission = vaccine.ImmunizationPriorToAdmissionFlg
            };
            return vm;
        }

        public static implicit operator VaccineHistoryViewModel(PatientVaccineTBInitial2Step vaccine)
        {
            DateTime? date = null;
            if (vaccine.PPDStep1GivenDate.HasValue)
            {
                date = vaccine.PPDStep1GivenDate.Value;
            }
            else if (vaccine.PPDStep2GivenDate.HasValue)
            {
                date = vaccine.PPDStep2GivenDate.Value;
            }
            else if (vaccine.ChestXRayFlg.HasValue)
            {
                if (vaccine.ChestXRayFlg.Value == true)
                {
                    date = vaccine.ChestXRayDate.Value;
                }
                else if (vaccine.QuestionnaireCompleteFlg)
                {
                    date = vaccine.QuestionnaireCompleteDate.Value;
                }
            }
            else if (vaccine.BMATFlg)
            {
                date = vaccine.BMATDate.Value;
            }
            var vm = new VaccineHistoryViewModel
            {
                PatientVaccineId = vaccine.PatientVaccineId,
                VaccineType = vaccine.VaccineType,
                VaccineDate = date,
                ConsentSignedFlg = vaccine.ConsentSignedFlg,
                OfferedRefusedFlg = vaccine.OfferedRefusedFlg,
                IsEmployee = false,
                PriorToAdmission = vaccine.ImmunizationPriorToAdmissionFlg
            };
            return vm;
        }
    }
}