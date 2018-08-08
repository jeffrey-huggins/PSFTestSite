using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Transactions;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using AtriumWebApp.Models;
using System.Data.Entity.Core.Objects;
using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "VAC,EVAC")]
    public class VaccinationController : BaseController
    {
        public VaccinationController(IOptions<AppSettingsConfig> config, VaccinationContext context) : base(config, context)
        {
        }

        private const string AppCode = "VAC";
        private const string EmpAppCode = "EVAC";

        protected new VaccinationContext Context
        {
            get { return (VaccinationContext)base.Context; }
        }

        [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "VAC")]
        public ActionResult Index()
        {
            //Record user access
            LogSession(AppCode);
            //Set Census Date Information and Manipulate when changed
            SetLookbackDays(HttpContext, AppCode);
            //Set initial date range values
            SetInitialTableRange(AppCode);

            ViewBag.VaccineTypes = Context.VaccineTypes.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.VaccineTypeName).ToList();

            if (!Session.TryGetObject("SideBar", out SideBarViewModel sideBar))
            {
                sideBar = SideBarService.InitSideBar(this, AppCode, Context);
            }

            VaccinationViewModel vm = new VaccinationViewModel()
            {
                SideBar = sideBar,
                RangeTo = OccurredRangeTo[AppCode],
                RangeFrom = OccurredRangeFrom[AppCode]
            };
            return View(vm);

        }

        public IActionResult GetVaccineList(int patientId, string fromString, string toString, bool isEmployee = false)
        {
            DateTime from = Convert.ToDateTime(fromString);
            DateTime to = Convert.ToDateTime(toString).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<VaccineHistoryViewModel> vm = new List<VaccineHistoryViewModel>();
            if (isEmployee)
            {
                vm = EmployeeVaccineHistory(patientId);
            }
            else
            {
                vm = PatientVaccineHistory(patientId);
            }
            vm = vm.Where(a => (!a.VaccineDate.HasValue) || (a.VaccineDate.Value <= to && a.VaccineDate.Value >= from)).ToList();
            return PartialView(vm);
        }

        public IActionResult EditVaccination(int? vaccineId, int patientId, int vaccineTypeId, bool isEmployee = false)
        {
            if (isEmployee)
            {
                var vm = GetEditOrNewEmployeeVaccine(vaccineId, patientId, vaccineTypeId);
                return PartialView("EditorTemplates/" + vm.GetType().Name, vm);
            }
            else
            {
                var vm = GetEditOrNewPatientVaccine(vaccineId, patientId, vaccineTypeId);
                return PartialView("EditorTemplates/" + vm.GetType().Name, vm);
            }
        }

        public IActionResult SaveInfluenzaVaccine(VaccinationInfluenzaViewModel vm)
        {
            if (vm.IsEmployee)
            {
                if(vm.VaccineId == 0)
                {
                    Context.EmployeeVaccineInfluenzaRecords.Add((EmployeeVaccineInfluenzaRecord)vm);
                }
                else
                {
                    var currentRecord = Context.EmployeeVaccineInfluenzaRecords.Find(vm.VaccineId);
                    if(currentRecord == null)
                    {
                        return Json(new { success = false, data = "Unable to locate the vaccine record" });
                    }
                    else
                    {
                        Context.Entry(currentRecord).CurrentValues.SetValues((EmployeeVaccineInfluenzaRecord)vm);
                    }
                }
            }
            else
            {
                if (vm.VaccineId == 0)
                {
                    Context.PatientVaccineInfluenzaRecords.Add((PatientVaccineInfluenza)vm);
                }
                else
                {
                    var currentRecord = Context.PatientVaccineInfluenzaRecords.Find(vm.VaccineId);
                    if (currentRecord == null)
                    {
                        return Json(new { success = false, data = "Unable to locate the vaccine record" });
                    }
                    else
                    {
                        Context.Entry(currentRecord).CurrentValues.SetValues((PatientVaccineInfluenza)vm);
                    }
                }
            }
            Context.SaveChanges();
            return Json(new { success = true });
        }

        public IActionResult SavePneumoniaVaccine(VaccinationPneumoniaViewModel vm)
        {
                if (vm.VaccineId == 0)
                {
                    Context.PatientVaccinePneumoniaRecords.Add((PatientVaccinePneumonia)vm);
                }
                else
                {
                    var currentRecord = Context.PatientVaccinePneumoniaRecords.Find(vm.VaccineId);
                    if (currentRecord == null)
                    {
                        return Json(new { success = false, data = "Unable to locate the vaccine record" });
                    }
                    else
                    {
                        Context.Entry(currentRecord).CurrentValues.SetValues((PatientVaccinePneumonia)vm);
                    }
                }
            
            Context.SaveChanges();
            return Json(new { success = true });
        }

        public IActionResult SaveTBAnnualVaccine(VaccinationTBAnnualViewModel vm)
        {
            
            if (vm.IsEmployee)
            {
                if (vm.VaccineId == 0)
                {
                    Context.EmployeeVaccineTBAnnualRecords.Add((EmployeeVaccineTBAnnualRecord)vm);
                }
                else
                {
                    var currentRecord = Context.EmployeeVaccineTBAnnualRecords.Find(vm.VaccineId);
                    if (currentRecord == null)
                    {
                        return Json(new { success = false, data = "Unable to locate the vaccine record" });
                    }
                    else
                    {
                        Context.Entry(currentRecord).CurrentValues.SetValues((EmployeeVaccineTBAnnualRecord)vm);
                    }
                }
            }
            else
            {
                if (vm.VaccineId == 0)
                {
                    Context.PatientVaccineTBAnnualRecords.Add((PatientVaccineTBAnnual)vm);
                }
                else
                {
                    var currentRecord = Context.PatientVaccineTBAnnualRecords.Find(vm.VaccineId);
                    if (currentRecord == null)
                    {
                        return Json(new { success = false, data = "Unable to locate the vaccine record" });
                    }
                    else
                    {
                        Context.Entry(currentRecord).CurrentValues.SetValues((PatientVaccineTBAnnual)vm);
                    }
                }
            }
            Context.SaveChanges();
            return Json(new { success = true });
        }

        public IActionResult SaveTBInitial2StepVaccine(VaccinationTBInitial2StepViewModel vm)
        {

            if (vm.IsEmployee)
            {
                if (vm.VaccineId == 0)
                {
                    Context.EmployeeVaccineTBInitial2StepRecords.Add((EmployeeVaccineTBInitial2StepRecord)vm);
                }
                else
                {
                    var currentRecord = Context.EmployeeVaccineTBInitial2StepRecords.Find(vm.VaccineId);
                    if (currentRecord == null)
                    {
                        return Json(new { success = false, data = "Unable to locate the vaccine record" });
                    }
                    else
                    {
                        Context.Entry(currentRecord).CurrentValues.SetValues((EmployeeVaccineTBInitial2StepRecord)vm);
                    }
                }
            }
            else
            {
                if (vm.VaccineId == 0)
                {
                    Context.PatientVaccineTBInitial2StepRecords.Add((PatientVaccineTBInitial2Step)vm);
                }
                else
                {
                    var currentRecord = Context.PatientVaccineTBInitial2StepRecords.Find(vm.VaccineId);
                    if (currentRecord == null)
                    {
                        return Json(new { success = false, data = "Unable to locate the vaccine record" });
                    }
                    else
                    {
                        Context.Entry(currentRecord).CurrentValues.SetValues((PatientVaccineTBInitial2Step)vm);
                    }
                }
            }
            Context.SaveChanges();
            return Json(new { success = true });
        }

        #region Employee Vaccination
        [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "EVAC")]
        public ActionResult Employee()
        {
            //Record user access
            LogSession(EmpAppCode);
            //Set Census Date Information and Manipulate when changed
            SetLookbackDays(HttpContext, EmpAppCode);
            //Set initial date range values
            SetInitialTableRange(EmpAppCode);

            ViewBag.VaccineTypes = Context.VaccineTypes.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.VaccineTypeName).ToList();

            if (!Session.TryGetObject("EmployeeSideBar", out EmployeeSidebarViewModel sideBar))
            {
                sideBar = SideBarService.InitEmployeeSideBar(this, EmpAppCode, Context);
            }

            EmployeeVaccinationViewModel vm = new EmployeeVaccinationViewModel()
            {
                SideBar = sideBar,
                RangeTo = OccurredRangeTo[EmpAppCode],
                RangeFrom = OccurredRangeFrom[EmpAppCode]
            };
            return View(vm);


        }

        #endregion


        #region Ajax Calls

        public IActionResult DeleteVaccine(int vaccineId, bool isEmployee = false)
        {
            if (isEmployee)
            {
                var record = Context.EmployeeVaccineRecords.Find(vaccineId);
                record.DeletedFlg = true;
                record.DeletedTS = DateTime.Now;
                record.DeletedADDomainName = User.Identity.Name;
                Context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                var record = Context.PatientVaccineRecords.Find(vaccineId);
                record.DeletedFlg = true;
                record.DeletedTS = DateTime.Now;
                record.DeletedADDomainName = User.Identity.Name;
                Context.SaveChanges();
                return Json(new { success = true });
            }
        }


        #endregion

        #region Private Helper Functions
        private List<VaccineHistoryViewModel> EmployeeVaccineHistory(int employeeId)
        {
            List<VaccineHistoryViewModel> vm = new List<VaccineHistoryViewModel>();
            var vaccineRecords = Context.EmployeeVaccineRecords.Where(a => a.PersonId == employeeId
                && !a.DeletedFlg).ToList();
            foreach (var vaccineRecord in vaccineRecords)
            {
                Type entityType = ObjectContext.GetObjectType(vaccineRecord.GetType());
                if (entityType == typeof(EmployeeVaccineInfluenzaRecord))
                {
                    vm.Add((EmployeeVaccineInfluenzaRecord)vaccineRecord);
                }
                else if (entityType == typeof(EmployeeVaccineTBAnnualRecord))
                {
                    vm.Add((EmployeeVaccineTBAnnualRecord)vaccineRecord);
                }
                else if (entityType == typeof(EmployeeVaccineTBInitial2StepRecord))
                {
                    vm.Add((EmployeeVaccineTBInitial2StepRecord)vaccineRecord);
                }
                else
                {
                    vm.Add(vaccineRecord);
                }
            }
            return vm;
        }
        private List<VaccineHistoryViewModel> PatientVaccineHistory(int patientId)
        {
            List<VaccineHistoryViewModel> vm = new List<VaccineHistoryViewModel>();
            var vaccineRecords = Context.PatientVaccineRecords.Where(a => a.PatientId == patientId
                && !a.DeletedFlg).ToList();
            foreach (var vaccineRecord in vaccineRecords)
            {
                Type entityType = ObjectContext.GetObjectType(vaccineRecord.GetType());
                if (entityType == typeof(PatientVaccineInfluenza))
                {
                    vm.Add((PatientVaccineInfluenza)vaccineRecord);
                }
                else if (entityType == typeof(PatientVaccinePneumonia))
                {
                    vm.Add((PatientVaccinePneumonia)vaccineRecord);
                }
                else if (entityType == typeof(PatientVaccineTBAnnual))
                {
                    vm.Add((PatientVaccineTBAnnual)vaccineRecord);
                }
                else if (entityType == typeof(PatientVaccineTBInitial2Step))
                {
                    vm.Add((PatientVaccineTBInitial2Step)vaccineRecord);
                }
                else
                {
                    vm.Add(vaccineRecord);
                }
            }
            return vm;
        }
        private object GetEditOrNewPatientVaccine(int? vaccineId, int patientId, int vaccineTypeId)
        {
            PatientVaccine vaccineRecord;
            if (vaccineId.HasValue)
            {
                vaccineRecord = Context.PatientVaccineRecords.FirstOrDefault(a => a.PatientVaccineId == vaccineId.Value);
                if (vaccineRecord == null)
                {
                    return null;
                }
            }
            else
            {
                var patient = Context.Residents.Find(patientId);
                switch (vaccineTypeId)
                {
                    case 1: // Influenza
                        vaccineRecord = new PatientVaccineInfluenza()
                        {
                            PatientId = patientId,
                            RoomId = patient.RoomId,
                            CurrentPayerId = patient.CurrentPayerId.Value,
                            VaccineTypeId = vaccineTypeId
                        };
                        break;
                    case 2: // Pneumonia
                        ViewBag.PneumoniaType = Context.VaccinePneumoniaTypes.Where(a => a.DataEntryFlg)
                            .OrderBy(a => a.SortOrder)
                            .ThenBy(a => a.VaccinePneumoniaTypeName).ToList();
                        vaccineRecord = new PatientVaccinePneumonia()
                        {
                            PatientId = patientId,
                            RoomId = patient.RoomId,
                            CurrentPayerId = patient.CurrentPayerId.Value,
                            VaccineTypeId = vaccineTypeId
                        };
                        break;
                    case 3: // TB Annual
                        vaccineRecord = new PatientVaccineTBAnnual()
                        {
                            PatientId = patientId,
                            RoomId = patient.RoomId,
                            CurrentPayerId = patient.CurrentPayerId.Value,
                            VaccineTypeId = vaccineTypeId
                        };
                        break;
                    case 4: // TB Inital 2-Step
                        vaccineRecord = new PatientVaccineTBInitial2Step()
                        {
                            PatientId = patientId,
                            RoomId = patient.RoomId,
                            CurrentPayerId = patient.CurrentPayerId.Value,
                            VaccineTypeId = vaccineTypeId
                        };
                        break;
                    default: // Invalid
                        throw new Exception(string.Format("Invalid VaccineTypeId: '{0}'", vaccineTypeId));
                }
            }
            Type entityType = ObjectContext.GetObjectType(vaccineRecord.GetType());
            if (entityType == typeof(PatientVaccineInfluenza))
            {
                VaccinationInfluenzaViewModel vm = (PatientVaccineInfluenza)vaccineRecord;
                vm.IsEmployee = false;
                return vm;
            }
            else if (entityType == typeof(PatientVaccinePneumonia))
            {
                VaccinationPneumoniaViewModel vm = (PatientVaccinePneumonia)vaccineRecord;
                vm.IsEmployee = false;
                ViewBag.PneumoniaType = Context.VaccinePneumoniaTypes.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.VaccinePneumoniaTypeName).ToList();
                return vm;
            }
            else if (entityType == typeof(PatientVaccineTBAnnual))
            {
                ViewBag.TBSite = Context.VaccineTBSites.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.SiteName).ToList();
                ViewBag.TBMeasurement = Context.VaccineTBReactionMeasurements.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.Description).ToList();
                VaccinationTBAnnualViewModel vm = (PatientVaccineTBAnnual)vaccineRecord;
                vm.IsEmployee = false;
                return vm;
            }
            else if (entityType == typeof(PatientVaccineTBInitial2Step))
            {
                ViewBag.TBSite = Context.VaccineTBSites.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.SiteName).ToList();
                ViewBag.TBMeasurement = Context.VaccineTBReactionMeasurements.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.Description).ToList();

                VaccinationTBInitial2StepViewModel vm = (PatientVaccineTBInitial2Step)vaccineRecord;
                vm.IsEmployee = false;
                return vm;
            }
            else
            {
                throw new Exception("Vaccine not valid.");
                //Throw exception?
            }
        }

        private object GetEditOrNewEmployeeVaccine(int? vaccineId, int employeeId, int vaccineTypeId)
        {
            EmployeeVaccineRecord vaccineRecord;
            if (vaccineId.HasValue)
            {
                vaccineRecord = Context.EmployeeVaccineRecords.FirstOrDefault(a => a.VaccineRecordId == vaccineId.Value);
                if (vaccineRecord == null)
                {
                    return null;
                }
            }
            else
            {
                var employee = Context.Employees.Find(employeeId);
                switch (vaccineTypeId)
                {
                    case 1: // Influenza
                        vaccineRecord = new EmployeeVaccineInfluenzaRecord()
                        {
                            PersonId = employeeId,
                            VaccineTypeId = vaccineTypeId
                        };
                        break;
                    case 2: // Pneumonia
                        throw new Exception("Employees unable to receive Pneumonia Vaccine");
                    case 3: // TB Annual
                        vaccineRecord = new EmployeeVaccineTBAnnualRecord()
                        {
                            PersonId = employeeId,
                            VaccineTypeId = vaccineTypeId
                        };
                        break;
                    case 4: // TB Inital 2-Step
                        vaccineRecord = new EmployeeVaccineTBInitial2StepRecord()
                        {
                            PersonId = employeeId,
                            VaccineTypeId = vaccineTypeId
                        };
                        break;
                    default: // Invalid
                        throw new Exception(string.Format("Invalid VaccineTypeId: '{0}'", vaccineTypeId));
                }
            }
            Type entityType = ObjectContext.GetObjectType(vaccineRecord.GetType());
            if (entityType == typeof(EmployeeVaccineInfluenzaRecord))
            {
                VaccinationInfluenzaViewModel vm = (EmployeeVaccineInfluenzaRecord)vaccineRecord;
                vm.IsEmployee = true;
                return vm;
            }
            else if (entityType == typeof(EmployeeVaccineTBAnnualRecord))
            {
                ViewBag.TBSite = Context.VaccineTBSites.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.SiteName).ToList();
                ViewBag.TBMeasurement = Context.VaccineTBReactionMeasurements.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.Description).ToList();

                VaccinationTBAnnualViewModel vm = (EmployeeVaccineTBAnnualRecord)vaccineRecord;
                vm.IsEmployee = true;
                return vm;
            }
            else if (entityType == typeof(EmployeeVaccineTBInitial2StepRecord))
            {
                ViewBag.TBSite = Context.VaccineTBSites.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.SiteName).ToList();
                ViewBag.TBMeasurement = Context.VaccineTBReactionMeasurements.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.Description).ToList();

                VaccinationTBInitial2StepViewModel vm = (EmployeeVaccineTBInitial2StepRecord)vaccineRecord;
                vm.IsEmployee = true;
                return vm;
            }
            else
            {
                throw new Exception("Vaccine not valid.");
                //Throw exception?
            }
        }

        #endregion
    }
}