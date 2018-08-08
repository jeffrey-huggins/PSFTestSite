using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.ClinicalOps.Enumerations;
using AtriumWebApp.Web.ClinicalOps.Exceptions;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System.Data.Entity.Core.Objects;
using Newtonsoft.Json;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "IFC")]
    public class InfectionControlController : BaseController
    {
        private const string AppCode = "IFC";
        private VaccinationContext VaccinationContext { get; set; }
        public InfectionControlController(IOptions<AppSettingsConfig> config, InfectionControlContext context, VaccinationContext vaccineContext) : base(config, context)
        {
            VaccinationContext = vaccineContext;
        }

        protected new InfectionControlContext Context
        {
            get { return (InfectionControlContext)base.Context; }
        }

        public IActionResult Index()
        {
            //Record user access
            LogSession(AppCode);
            //Set Census Date Information and Manipulate when changed
            InitializeCensusDateChangedSessionVariable();
            SetLookbackDays(HttpContext, AppCode);
            ManipulateCensusDate(AppCode);
            //Set initial date range values
            SetDateRangeErrorValues();
            SetInitialTableRange(AppCode);

            if (!Session.TryGetObject("SideBar", out SideBarViewModel sideBar))
            {
                sideBar = SideBarService.InitSideBar(this, AppCode, Context);
            }

            InfectionControlViewModel vm = new InfectionControlViewModel()
            {
                SideBar = sideBar,
                RangeTo = OccurredRangeTo[AppCode],
                RangeFrom = OccurredRangeFrom[AppCode]
            };
            return View(vm);
        }

        public IActionResult GetVaccineList(int patientId)
        {
            List<VaccineHistoryViewModel> vm = new List<VaccineHistoryViewModel>();
            var vaccineRecords = VaccinationContext.PatientVaccineRecords.Where(a => a.PatientId == patientId
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

            return PartialView(vm);
        }

        public IActionResult GetInfectionList(int patientId, string fromString, string toString)
        {
            DateTime from = Convert.ToDateTime(fromString);
            DateTime to = Convert.ToDateTime(toString).AddHours(23).AddMinutes(59).AddSeconds(59);
            List<PatientIFCEvent> events = Context.PatientIFCEvents
                .Include("Organisms")
                .Include("Symptoms.Symptom")
                .Include("Antibiotics.Antibiotic")
                .Include("Site")
                .Where(a => a.PatientId == patientId && a.OnsetDate >= from && a.OnsetDate <= to && !a.DeletedFlg).ToList();
            return PartialView(events);
        }

        public IActionResult EditOrCreateInfectionVM(int? infectionId, int patientId)
        {
            ViewBag.InfectionSites = Context.Sites.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.PatientIFCSiteName).ToList();
            PatientIFCEventViewModel vm = new PatientIFCEventViewModel()
            {
                Antibiotics = new List<AntibioticSelection>(),
                RecultureDates = new List<RecultureDatesViewModel>(),
                Diagnosis = new List<DiagnosisSelection>(),
                Symptoms = new List<SymptomSelection>(),
                Organisms = new List<OrganismSelection>(),
                Precautions = new List<PrecautionSelection>(),
                Limits = Context.PatientIFCLimits.First()
            };

            foreach (var antibiotic in Context.Antibiotics.Where(a => a.DataEntryFlg))
            {
                vm.Antibiotics.Add(new AntibioticSelection()
                {
                    Selected = false,
                    Type = antibiotic
                });
            }
            foreach (var diagnosis in Context.Diagnoses.Where(a => a.DataEntryFlg))
            {
                vm.Diagnosis.Add(new DiagnosisSelection()
                {
                    Selected = false,
                    Type = diagnosis
                });
            }
            foreach (var symptom in Context.Symptoms.Where(a => a.DataEntryFlg))
            {
                vm.Symptoms.Add(new SymptomSelection()
                {
                    Selected = false,
                    Type = symptom
                });
            }
            foreach (var organism in Context.Organisms.Where(a => a.DataEntryFlg))
            {
                vm.Organisms.Add(new OrganismSelection()
                {
                    Selected = false,
                    Type = organism
                });
            }
            foreach (var precaution in Context.Precautions.Where(a => a.DataEntryFlg))
            {
                vm.Precautions.Add(new PrecautionSelection()
                {
                    Selected = false,
                    Type = precaution
                });
            }

            if (infectionId.HasValue)
            {
                foreach(var culture in Context.ReCultureDates.Where(a => a.PatientIFCEventId == infectionId.Value).ToList())
                {
                    vm.RecultureDates.Add(new RecultureDatesViewModel()
                    {
                        DeleteDate = false,
                        Reculture = culture
                    });
                }
                vm.Event = Context.PatientIFCEvents.Find(infectionId.Value);
                if (vm.Event == null)
                {
                    return Json(new { success = false, data = "Unable to locate infection control." });
                }
                foreach (var eventType in vm.Event.Antibiotics)
                {
                    vm.Antibiotics.Find(a => a.Type.PatientIFCAntibioticId == eventType.PatientIFCAntibioticId).Selected = true;
                }
                foreach (var eventType in vm.Event.Diagnosis)
                {
                    vm.Diagnosis.Find(a => a.Type.PatientIFCDiagnosisId == eventType.PatientIFCDiagnosisId).Selected = true;
                }
                foreach (var eventType in vm.Event.Organisms)
                {
                    vm.Organisms.Find(a => a.Type.PatientIFCOrganismId == eventType.PatientIFCOrganismId).Selected = true;
                }
                foreach (var eventType in vm.Event.Precautions)
                {
                    vm.Precautions.Find(a => a.Type.PatientIFCTypeOfPrecautionId == eventType.PatientIFCTypeOfPrecautionId).Selected = true;
                }
                foreach (var eventType in vm.Event.Symptoms)
                {
                    vm.Symptoms.Find(a => a.Type.PatientIFCSymptomId == eventType.PatientIFCSymptomId).Selected = true;
                }
            }
            else
            {
                var patient = Context.Residents.Find(patientId);
                if (patient == null)
                {
                    return Json(new { success = false, data = "Unable to locate patient." });
                }
                vm.Event = new PatientIFCEvent()
                {
                    PatientId = patientId,
                    RoomId = patient.RoomId,
                    CurrentPayerId = patient.CurrentPayerId
                };
                vm.RecultureDates = new List<RecultureDatesViewModel>();
            }
            vm.Organisms = vm.Organisms.OrderBy(a => a.Type.SortOrder).ThenBy(a => a.Type.PatientIFCOrganismName).ToList();
            vm.Precautions = vm.Precautions.OrderBy(a => a.Type.SortOrder).ThenBy(a => a.Type.PatientIFCTypeOfPrecautionName).ToList();
            vm.Symptoms = vm.Symptoms.OrderBy(a => a.Type.SortOrder).ThenBy(a => a.Type.PatientIFCSymptomName).ToList();
            vm.Antibiotics = vm.Antibiotics.OrderBy(a => a.Type.SortOrder).ThenBy(a => a.Type.PatientIFCAntibioticName).ToList();
            vm.Diagnosis = vm.Diagnosis.OrderBy(a => a.Type.SortOrder).ThenBy(a => a.Type.PatientIFCDiagnosisName).ToList();
            return EditorFor(vm);
        }

        public IActionResult SaveInfection([FromForm]PatientIFCEventViewModel vm)
        {
            PatientIFCEvent ifcEvent;
            int id = 0;
            if(vm.Event.PatientIFCEventId != 0)
            {
                ifcEvent = Context.PatientIFCEvents.Find(vm.Event.PatientIFCEventId);
                id = ifcEvent.PatientIFCEventId;
                Context.Entry(ifcEvent).CurrentValues.SetValues(vm.Event);
                Context.EventOrganisms.RemoveRange(Context.EventOrganisms.Where(a => a.PatientIFCEventId == id));
                Context.EventAntibiotics.RemoveRange(Context.EventAntibiotics.Where(a => a.PatientIFCEventId == id));
                Context.EventDiagnoses.RemoveRange(Context.EventDiagnoses.Where(a => a.PatientIFCEventId == id));
                Context.EventSymptoms.RemoveRange(Context.EventSymptoms.Where(a => a.PatientIFCEventId == id));
                Context.TypeOfPrecautions.RemoveRange(Context.TypeOfPrecautions.Where(a => a.PatientIFCEventId == id));
                Context.ReCultureDates.RemoveRange(Context.ReCultureDates.Where(a => a.PatientIFCEventId == id));
            }
            else {
                ifcEvent = Context.PatientIFCEvents.Add(vm.Event);
            }
            Context.SaveChanges();
            id = ifcEvent.PatientIFCEventId;
            foreach (var organism in vm.Organisms.Where(a => a.Selected).Select(a => a.Type))
            {
                Context.EventOrganisms.Add(new PatientIFCEventOrganism()
                {
                    PatientIFCEventId = id,
                    PatientIFCOrganismId = organism.PatientIFCOrganismId,
                });
            }
            foreach (var symptom in vm.Symptoms.Where(a => a.Selected).Select(a => a.Type))
            {
                Context.EventSymptoms.Add(new PatientIFCEventSymptom()
                {
                    PatientIFCEventId = id,
                    PatientIFCSymptomId = symptom.PatientIFCSymptomId
                });
            }
            foreach (var diagnosis in vm.Diagnosis.Where(a => a.Selected).Select(a => a.Type))
            {
                Context.EventDiagnoses.Add(new PatientIFCEventDiagnosis()
                {
                    PatientIFCEventId = id,
                    PatientIFCDiagnosisId = diagnosis.PatientIFCDiagnosisId
                });
            }
            foreach (var precaution in vm.Precautions.Where(a => a.Selected).Select(a => a.Type))
            {
                Context.TypeOfPrecautions.Add(new PatientIFCEventTypeOfPrecaution()
                {
                    PatientIFCEventId = id,
                    PatientIFCTypeOfPrecautionId = precaution.PatientIFCTypeOfPrecautionId
                });
            }
            foreach (var antibiotic in vm.Antibiotics.Where(a => a.Selected).Select(a => a.Type))
            {
                Context.EventAntibiotics.Add(new PatientIFCEventAntibiotic()
                {
                    PatientIFCEventId = id,
                    PatientIFCAntibioticId = antibiotic.PatientIFCAntibioticId
                });
            }
            int recultureLoop = 0;
            if (vm.RecultureDates != null && vm.RecultureDates.Count > 0)
            {
                foreach (var recultureDate in vm.RecultureDates.Where(a => !a.DeleteDate).Select(a => a.Reculture))
                {
                    recultureLoop++;
                    Context.ReCultureDates.Add(new PatientIFCEventReCulture()
                    {
                        PatientIFCEventId = id,
                        RecultureDate = recultureDate.RecultureDate,
                        ReCultureId = recultureLoop
                    });
                }
            }
            Context.SaveChanges();
            return Content(JsonConvert.SerializeObject(new { success = true }));
        }

        public JsonResult DeleteRow(int rowId)
        {
            var ifc = Context.PatientIFCEvents.Find(rowId);
            var symptoms = (from symp in Context.EventSymptoms
                            where symp.PatientIFCEventId == rowId
                            select symp).ToList();
            if (ifc == null)
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            foreach (var symp in symptoms)
            {
                Context.EventSymptoms.Remove(symp);
            }
            //ifcContext.IFCEvents.Remove(ifc);
            ifc.DeletedFlg = true;
            ifc.DeletedTS = DateTime.Now;
            ifc.DeletedADDomainName = User.Identity.Name;
            try
            {
                Context.SaveChanges();
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

    }
}