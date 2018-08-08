using System;
using System.Collections.Generic;
using System.Linq;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using Newtonsoft.Json;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "SOC")]
    public class StandardsOfCareController : BaseStandardsOfCareController
    {
        public StandardsOfCareController(IOptions<AppSettingsConfig> config, SOCContext context) : base(config, context)
        {
        }

        public ActionResult Index()
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

            var measuresList = new SelectList(Context.MasterSOCMeasure.Where(a => a.DataEntryFlg && !a.CalcMeasureFlg).OrderBy(a => a.SortOrder), "SOCMeasureId", "SOCMeasureName");

            ViewBag.Measures = measuresList;
            if (!Session.TryGetObject("SideBar", out SideBarViewModel sideBar))
            {
                sideBar = SideBarService.InitSideBar(this, AppCode, Context);
            }

            StandardsOfCareViewModel vm = new StandardsOfCareViewModel()
            {
                SideBar = sideBar,
                OccuredTo = OccurredRangeTo[AppCode],
                OccuredFrom = OccurredRangeFrom[AppCode]
            };
            return View(vm);
        }

        #region Private Helper Functions

        private void MarkForDelete(SOCEvent soc, string deletedByName)
        {
            Session.TryGetObject(AppCode + "CurrentSOCEventId", out int currentSOCEventId);
            if (Session.Contains(AppCode + "CurrentSOCEventId") && soc.SOCEventId == currentSOCEventId)
            {
                Session.Remove(AppCode + "CurrentSOCEventId");
            }
            soc.DeletedFlg = true;
            soc.DeletedTS = DateTime.Now;
            soc.DeletedADDomainName = deletedByName;
        }

        #endregion

        #region Save New/Edit

        public IActionResult EditEvent(int? socEventId, int measureId, int residentId)
        {
            var measure = Context.MasterSOCMeasure.Find(measureId);
            switch (measure.SubAppCode)
            {
                case ("ANTIPSYCH"):
                    return EditAnti(socEventId, residentId, measureId);
                case ("FALL"):
                case ("FALLWI"):
                    return EditFall(socEventId, residentId, measureId);
                case "PRWNDPRIOR":
                case "PRWNDAFTER":
                    return EditWound(socEventId, residentId, measureId, "pressure");
                case "SCWND":
                    return EditWound(socEventId, residentId, measureId, "composite");
                case ("RESTRAINT"):
                    return EditRestraint(socEventId, residentId, measureId);
                case ("CATHETER"):
                    return EditCatheter(socEventId, residentId, measureId);
                default:
                    return EditGeneric(socEventId, residentId, measureId);
            }
        }

        #region Catheters
        public IActionResult EditCatheter(int? eventId, int residentId, int measureId)
        {
            ViewBag.CatheterTypes = Context.CatheterTypes.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.SOCCatheterTypeName).ToList();
            SOCEventCatheter socEvent;
            if (eventId.HasValue)
            {
                socEvent = (SOCEventCatheter)Context.SOCEvents.FirstOrDefault(a => a.SOCEventId == eventId.Value);
            }
            else
            {
                var resident = Context.Residents.Find(residentId);
                socEvent = new SOCEventCatheter()
                {
                    PatientId = resident.PatientId,
                    RoomId = resident.RoomId,
                    CurrentPayerId = resident.CurrentPayerId,
                    SOCMeasureId = measureId
                };
            }
            return EditorFor(socEvent);
        }

        public IActionResult SaveCatheterEvent(SOCEventCatheter vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.ResolvedDate.HasValue)
                {
                    vm.ResolvedFlg = true;
                }
                if(vm.OccurredDate != null)
                {
                    vm.OccurredFlg = true;
                }
                if (vm.SOCEventId == 0)
                {
                    Context.SOCEvents.Add(vm);
                }
                else
                {
                    var currentEvent = Context.SOCEvents.Find(vm.SOCEventId);
                    Context.Entry(currentEvent).CurrentValues.SetValues(vm);
                }
                Context.SaveChanges();
                return Content(JsonConvert.SerializeObject(new { Success = true }));
            }

            return Content(JsonConvert.SerializeObject(new { Success = false }));
        }

        #endregion

        #region Restraints

        public IActionResult EditRestraint(int? eventId, int residentId, int measureId)
        {
            SOCEvent socEvent;
            if (eventId.HasValue)
            {
                socEvent = Context.SOCEvents.FirstOrDefault(a => a.SOCEventId == eventId.Value);
            }
            else
            {
                var resident = Context.Residents.Find(residentId);
                socEvent = new SOCEvent()
                {
                    PatientId = resident.PatientId,
                    RoomId = resident.RoomId,
                    CurrentPayerId = resident.CurrentPayerId,
                    SOCMeasureId = measureId
                };
            }
            return PartialView("EditorTemplates/SOCEventrestraint", socEvent);
        }

        public IActionResult EditRestraintNote(int eventId, int? noteId)
        {
            ViewBag.RestraintTypes = Context.Restraints.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.SOCRestraintName).ToList();

            SOCEventRestraintNoted note = new SOCEventRestraintNoted();

            if (noteId.HasValue)
            {
                note = Context.EventRestraintNotes.Find(noteId.Value);
            }
            else
            {
                note.SOCEventId = eventId;
            }
            return EditorFor(note);

        }

        public IActionResult SaveRestraintNote(SOCEventRestraintNoted note)
        {
            if (ModelState.IsValid)
            {
                if (note.SOCEventRestraintNotedId == 0)
                {
                    Context.EventRestraintNotes.Add(note);
                }
                else
                {
                    var currentNote = Context.EventRestraintNotes.Find(note.SOCEventRestraintNotedId);
                    Context.Entry(currentNote).CurrentValues.SetValues(note);
                }
                Context.SaveChanges();
                return Content(JsonConvert.SerializeObject(new { Success = true }));
            }
            return Content(JsonConvert.SerializeObject(new { Success = false }));
        }

        public IActionResult ListRestraintNotes(int eventId)
        {
            List<SOCEventRestraintNoted> notes = new List<SOCEventRestraintNoted>();
            if (eventId > 0)
            {
                var notesList = Context.EventRestraintNotes.Include("Restraint").Where(a => a.SOCEventId == eventId);
                if (notesList.Any())
                {
                    notes = notesList.ToList();
                }
            }
            return PartialView("DisplayTemplates/RestraintNotes", notes);
        }

        #endregion

        #region Wounds
        public IActionResult EditWound(int? eventId, int residentId, int measureId, string type)
        {
            Measure SOCMeasure = Context.MasterSOCMeasure.Find(measureId);
            ViewBag.CompositeWoundDescribe = Context.CompositeWoundDescribes.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.CompositeWoundDescribeName).ToList();
            DocumentViewModel document = new DocumentViewModel();
            SOCEventWound socEvent;
            if (eventId.HasValue)
            {
                socEvent = (SOCEventWound)Context.SOCEvents.Include("SOCMeasure").FirstOrDefault(a => a.SOCEventId == eventId.Value);
                var woundDocument = Context.PressureWoundDocuments.FirstOrDefault(a => a.SOCEventId == eventId.Value);
                if (woundDocument != null)
                {
                    document = woundDocument;
                }
            }
            else
            {
                var resident = Context.Residents.Find(residentId);
                socEvent = new SOCEventWound()
                {
                    PatientId = resident.PatientId,
                    RoomId = resident.RoomId,
                    CurrentPayerId = resident.CurrentPayerId,
                    SOCMeasureId = measureId,
                    SOCMeasure = SOCMeasure
                };
            }
            SOCWoundViewModel vm = new SOCWoundViewModel()
            {
                Wound = socEvent,
                File = document,
                WoundType = type
            };
            return EditorFor(vm);
        }
        public IActionResult SaveWound([FromForm]SOCWoundViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Wound.ResolvedDate.HasValue)
                {
                    vm.Wound.ResolvedFlg = true;
                }
                if (vm.Wound.OccurredDate != null)
                {
                    vm.Wound.OccurredFlg = true;
                }
                if (vm.Wound.SOCEventId == 0)
                {
                    Context.SOCEvents.Add(vm.Wound);
                }
                else
                {

                    var currentWound = (SOCEventWound)Context.SOCEvents.FirstOrDefault(a => a.SOCEventId == vm.Wound.SOCEventId);
                    if (currentWound == null)
                    {
                        return Content("{\"Success\":false, \"data\":\"Unable to find the wound in the database\"}");
                    }
                    Context.Entry(currentWound).CurrentValues.SetValues(vm.Wound);
                    if (!vm.Wound.UnavoidableFlg)
                    {
                        var document = Context.PressureWoundDocuments.FirstOrDefault(a => a.SOCEventId == vm.Wound.SOCEventId);
                        if (document != null)
                        {
                            Context.PressureWoundDocuments.Remove(document);
                        }
                    }
                }
                Context.SaveChanges();
                if (vm.File != null && vm.File.Document != null)
                {
                    SOCPressureWoundDocument document = vm.File;
                    document.Document = SevenZipHelper.CompressStreamLZMA(document.Document).ToArray();
                    document.SOCEventId = vm.Wound.SOCEventId;
                    Context.PressureWoundDocuments.Add(document);
                }
                Context.SaveChanges();
                return Content("{\"Success\":true, \"ID\":" + vm.Wound.SOCEventId + "}");
            }

            return Content("{\"Success\":false}");
        }
        public IActionResult EditWoundNote(int eventId, int? noteId)
        {
            var SOCEvent = (SOCEventWound)Context.SOCEvents.Find(eventId);

            ViewBag.PressureWounds = Context.PressureWoundStages.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder).ThenBy(a => a.PressureWoundStageName).ToList();

            SOCEventWoundNoted note = new SOCEventWoundNoted();
            if (SOCEvent == null)
            {
                note.AdmittedWithFlg = false;
            }
            else
            {
                note.AdmittedWithFlg = SOCEvent.AdmittedWithFlg;
            }

            if (noteId.HasValue)
            {
                note = Context.SOCEventWoundNotes.Find(noteId.Value);
            }
            else
            {
                note.SOCEventId = eventId;
                note.Signature = ViewBag.CurrentUserDisplayName;
            }
            return EditorFor(note);
        }

        public IActionResult SaveWoundNote(SOCEventWoundNoted note)
        {
            if (String.IsNullOrEmpty(note.Measurement))
            {
                note.Measurement = "";
            }
            if (ModelState.IsValid)
            {
                if (note.SOCEventWoundNotedId == 0)
                {
                    Context.SOCEventWoundNotes.Add(note);
                }
                else
                {
                    var currentNote = Context.SOCEventWoundNotes.Find(note.SOCEventWoundNotedId);
                    Context.Entry(currentNote).CurrentValues.SetValues(note);
                }
                Context.SaveChanges();
                return Content(JsonConvert.SerializeObject(new { Success = true }));
            }

            return Content(JsonConvert.SerializeObject(new { Success = false }));
        }

        public IActionResult WoundNoteListing(int eventId)
        {

            List<SOCEventWoundNoted> notes = new List<SOCEventWoundNoted>();
            if (eventId > 0)
            {
                var notesList = Context.SOCEventWoundNotes.Include("SOCEvent.CompositeWoundDescription").Include("WoundStage").Where(a => a.SOCEventId == eventId);
                if (notesList.Any())
                {
                    notes = notesList.ToList();
                }
            }
            return PartialView("DisplayTemplates/WoundNotes", notes);
        }
        #endregion

        #region Generic

        public IActionResult EditGeneric(int? eventId, int residentId, int measureId)
        {
            SOCEvent socEvent;
            if (eventId.HasValue)
            {
                socEvent = Context.SOCEvents.FirstOrDefault(a => a.SOCEventId == eventId.Value);
            }
            else
            {
                var resident = Context.Residents.Find(residentId);
                socEvent = new SOCEvent()
                {
                    PatientId = resident.PatientId,
                    RoomId = resident.RoomId,
                    CurrentPayerId = resident.CurrentPayerId,
                    SOCMeasureId = measureId
                };
            }
            return EditorFor(socEvent);
        }

        public IActionResult SaveGeneric(SOCEvent socEvent)
        {
            if (ModelState.IsValid)
            {
                if (socEvent.ResolvedDate.HasValue)
                {
                    socEvent.ResolvedFlg = true;
                }
                if (socEvent.OccurredDate != null)
                {
                    socEvent.OccurredFlg = true;
                }
                if (socEvent.SOCEventId == 0)
                {
                    Context.SOCEvents.Add(socEvent);
                }
                else
                {
                    var currentEvent = Context.SOCEvents.Find(socEvent.SOCEventId);
                    Context.Entry(currentEvent).CurrentValues.SetValues(socEvent);
                }
                Context.SaveChanges();
                return Content(JsonConvert.SerializeObject(new { Success = true, id = socEvent.SOCEventId }));

            }

            return Content(JsonConvert.SerializeObject(new { Success = false }));
        }

        #endregion

        #region AntiPsychotic

        public IActionResult EditAnti(int? eventId, int residentId, int measureId)
        {
            ViewBag.Medications = Context.AntiPsychoticMedications.Where(a => a.DataEntryFlg).ToList();
            ViewBag.Diagnosis = Context.AntiPsychoticDiagnoses.Where(a => a.DataEntryFlg).ToList();

            SOCEventAntiPsychotic antiPsychoticEvent;
            if (eventId.HasValue)
            {
                antiPsychoticEvent = (SOCEventAntiPsychotic)Context.SOCEvents.FirstOrDefault(a => a.SOCEventId == eventId.Value);
            }
            else
            {
                var resident = Context.Residents.Find(residentId);
                antiPsychoticEvent = new SOCEventAntiPsychotic()
                {
                    CurrentPayerId = resident.CurrentPayerId,
                    PatientId = residentId,
                    RoomId = resident.RoomId,
                    SOCMeasureId = measureId
                };
            }
            return EditorFor(antiPsychoticEvent);
        }

        public IActionResult SaveAnti(SOCEventAntiPsychotic antiPsychotic)
        {
            if (ModelState.IsValid)
            {
                if (antiPsychotic.ResolvedDate.HasValue)
                {
                    antiPsychotic.ResolvedFlg = true;
                }
                if (antiPsychotic.OccurredDate != null)
                {
                    antiPsychotic.OccurredFlg = true;
                }
                if (antiPsychotic.SOCEventId == 0)
                {
                    Context.SOCEvents.Add(antiPsychotic);
                }
                else
                {
                    var currentAnti = (SOCEventAntiPsychotic)Context.SOCEvents.Find(antiPsychotic.SOCEventId);
                    Context.Entry(currentAnti).CurrentValues.SetValues(antiPsychotic);
                }
                Context.SaveChanges();
                return Content(JsonConvert.SerializeObject(new { Success = true, id = antiPsychotic.SOCEventId }));
            }

            return Content(JsonConvert.SerializeObject(new { Success = false }));
        }

        public IActionResult AntiPsychoticNoteListing(int eventId)
        {
            List<SOCEventAntiPsychoticNoted> notes = new List<SOCEventAntiPsychoticNoted>();
            if (eventId > 0)
            {
                var notesList = Context.AntiPsychoticNoted.Where(a => a.SOCEventId == eventId);
                if (notesList.Any())
                {
                    notes = notesList.ToList();
                }
            }
            return PartialView("DisplayTemplates/AntiPsychoticNotes", notes);
        }

        public IActionResult EditAntiPsychoticNote(int eventId, int? noteId)
        {
            SOCEventAntiPsychoticNoted note = new SOCEventAntiPsychoticNoted();
            if (noteId.HasValue)
            {
                note = Context.AntiPsychoticNoted.Find(noteId.Value);
            }
            else
            {
                note.SOCEventId = eventId;
            }
            return EditorFor(note);
        }

        public IActionResult AntiPsychoticNote(SOCEventAntiPsychoticNoted note)
        {
            if (ModelState.IsValid)
            {
                if (note.SOCEventAntiPsychoticNotedId == 0)
                {
                    Context.AntiPsychoticNoted.Add(note);
                }
                else
                {
                    var currentAnti = Context.AntiPsychoticNoted.Find(note.SOCEventAntiPsychoticNotedId);
                    Context.Entry(currentAnti).CurrentValues.SetValues(note);
                }
                Context.SaveChanges();
                return Content(JsonConvert.SerializeObject(new { Success = true }));
            }

            return Content(JsonConvert.SerializeObject(new { Success = false }));
        }


        #endregion

        #region Falls

        public IActionResult EditFall(int? eventId, int residentId, int measureId)
        {
            ViewBag.FallLocations = Context.FallLocations.Where(a => a.DataEntryFlg).ToList();
            List<SOCFallTreatmentSelection> treatments = new List<SOCFallTreatmentSelection>();
            var treatmentList = Context.FallTreatmentTypes.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder);
            foreach (var treatment in treatmentList)
            {
                treatments.Add(new SOCFallTreatmentSelection()
                {
                    Selected = false,
                    Type = treatment
                });
            }

            List<SOCFallInjurySelection> injuries = new List<SOCFallInjurySelection>();
            var injuryList = Context.FallInjuryTypes.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder);
            foreach (var injury in injuryList)
            {
                injuries.Add(new SOCFallInjurySelection()
                {
                    Selected = false,
                    Type = injury
                });
            }

            List<SOCFallTypeSelection> types = new List<SOCFallTypeSelection>();
            var typeList = Context.FallTypes.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder);
            foreach (var type in typeList)
            {
                types.Add(new SOCFallTypeSelection()
                {
                    Selected = false,
                    Type = type
                });
            }

            List<SOCInterventionSelection> interventions = new List<SOCInterventionSelection>();
            var interventionList = Context.FallInterventionTypes.Where(a => a.DataEntryFlg).OrderBy(a => a.SortOrder);
            foreach (var intervention in interventionList)
            {
                interventions.Add(new SOCInterventionSelection()
                {
                    Selected = false,
                    Type = intervention
                });
            }

            SOCEventFall fallEvent;
            if (eventId.HasValue)
            {
                fallEvent = (SOCEventFall)Context.SOCEvents.FirstOrDefault(a => a.SOCEventId == eventId.Value);
                var fallTreatments = Context.FallTreatments.Where(a => a.SOCEventId == fallEvent.SOCEventId);
                foreach (var treatment in fallTreatments)
                {
                    treatments.Find(a => a.Type.SOCFallTreatmentId == treatment.SOCFallTreatmentId).Selected = true;
                }

                var injuryTypes = Context.EventFallInjuryTypes.Where(a => a.SOCEventId == fallEvent.SOCEventId);
                foreach (var injury in injuryTypes)
                {
                    injuries.Find(a => a.Type.SOCFallInjuryTypeId == injury.SOCFallInjuryTypeId).Selected = true;
                }

                var fallTypes = Context.EventFallTypes.Where(a => a.SOCEventId == fallEvent.SOCEventId);
                foreach (var type in fallTypes)
                {
                    types.Find(a => a.Type.SOCFallTypeId == type.SOCFallTypeId).Selected = true;
                }

                var interventionTypes = Context.FallInterventions.Where(a => a.SOCEventId == fallEvent.SOCEventId);
                foreach (var intervention in interventionTypes)
                {
                    interventions.Find(a => a.Type.SOCFallInterventionId == intervention.SOCFallInterventionId).Selected = true;
                }
            }
            else
            {
                var resident = Context.Residents.Find(residentId);
                fallEvent = new SOCEventFall()
                {
                    CurrentPayerId = resident.CurrentPayerId,
                    PatientId = residentId,
                    RoomId = resident.RoomId,
                    SOCMeasureId = measureId
                };
            }


            SOCFallRecordViewModel vm = new SOCFallRecordViewModel()
            {
                FallEvent = fallEvent,
                InjuryList = injuries.OrderBy(a => a.Type.SortOrder).ThenBy(a => a.Type.SOCFallInjuryTypeName).ToList(),
                TreatmentList = treatments.OrderBy(a => a.Type.SortOrder).ThenBy(a => a.Type.SOCFallTreatmentName).ToList(),
                FallTypeList = types.OrderBy(a => a.Type.SortOrder).ThenBy(a => a.Type.SOCFallTypeName).ToList(),
                InterventionList = interventions.OrderBy(a => a.Type.SortOrder).ThenBy(a => a.Type.SOCFallInterventionName).ToList()
            };

            return EditorFor(vm);
        }

        public IActionResult SaveFall([FromForm]SOCFallRecordViewModel fall)
        {
            if (ModelState.IsValid)
            {

                if (fall.FallEvent.SOCEventId == 0)
                {
                    fall.FallEvent.OccurredDate = fall.FallEvent.FallTime;
                    fall.FallEvent.ResolvedDate = fall.FallEvent.OccurredDate;
                    if (fall.FallEvent.ResolvedDate.HasValue)
                    {
                        fall.FallEvent.ResolvedFlg = true;
                    }
                    if (fall.FallEvent.OccurredDate != null)
                    {
                        fall.FallEvent.OccurredFlg = true;
                    }
                    Context.SOCEvents.Add(fall.FallEvent);
                }
                else
                {
                    fall.FallEvent.OccurredDate = fall.FallEvent.FallTime;
                    fall.FallEvent.ResolvedDate = fall.FallEvent.OccurredDate;
                    if (fall.FallEvent.ResolvedDate.HasValue)
                    {
                        fall.FallEvent.ResolvedFlg = true;
                    }
                    if (fall.FallEvent.OccurredDate != null)
                    {
                        fall.FallEvent.OccurredFlg = true;
                    }
                    var currentFall = (SOCEventFall)Context.SOCEvents.Find(fall.FallEvent.SOCEventId);
                    Context.Entry(currentFall).CurrentValues.SetValues(fall.FallEvent);
                    //Remove all old fall event records
                    foreach (var injury in Context.EventFallInjuryTypes.Where(e => e.SOCEventId == fall.FallEvent.SOCEventId).ToList())
                    {
                        Context.EventFallInjuryTypes.Remove(injury);
                    }
                    foreach (var treatment in Context.FallTreatments.Where(e => e.SOCEventId == fall.FallEvent.SOCEventId).ToList())
                    {
                        Context.FallTreatments.Remove(treatment);
                    }
                    foreach (var intervention in Context.FallInterventions.Where(e => e.SOCEventId == fall.FallEvent.SOCEventId).ToList())
                    {
                        Context.FallInterventions.Remove(intervention);
                    }
                    foreach (var type in Context.EventFallTypes.Where(e => e.SOCEventId == fall.FallEvent.SOCEventId).ToList())
                    {
                        Context.EventFallTypes.Remove(type);
                    }
                }
                Context.SaveChanges();
                //Add new fall event records
                foreach (var intervention in fall.InterventionList.Where(a => a.Selected))
                {
                    var eventIntervention = new SOCEventFallIntervention
                    {
                        SOCEventId = fall.FallEvent.SOCEventId,
                        SOCFallInterventionId = intervention.Type.SOCFallInterventionId
                    };
                    Context.FallInterventions.Add(eventIntervention);
                }
                foreach (var fallType in fall.FallTypeList.Where(a => a.Selected))
                {
                    var eventIntervention = new SOCEventFallType
                    {
                        SOCEventId = fall.FallEvent.SOCEventId,
                        SOCFallTypeId = fallType.Type.SOCFallTypeId
                    };
                    Context.EventFallTypes.Add(eventIntervention);
                }
                foreach (var injury in fall.InjuryList.Where(a => a.Selected))
                {
                    var eventIntervention = new SOCEventFallInjuryType
                    {
                        SOCEventId = fall.FallEvent.SOCEventId,
                        SOCFallInjuryTypeId = injury.Type.SOCFallInjuryTypeId
                    };
                    Context.EventFallInjuryTypes.Add(eventIntervention);
                }
                foreach (var treatment in fall.TreatmentList.Where(a => a.Selected))
                {
                    var eventIntervention = new SOCEventFallTreatment
                    {
                        SOCEventId = fall.FallEvent.SOCEventId,
                        SOCFallTreatmentId = treatment.Type.SOCFallTreatmentId
                    };
                    Context.FallTreatments.Add(eventIntervention);
                }
                Context.SaveChanges();

                return Content(JsonConvert.SerializeObject(new { Success = true }));
            }

            return Content(JsonConvert.SerializeObject(new { Success = false }));
        }

        #endregion

        #endregion

        #region Update Post Backs

        public IActionResult GetSOCEventList(int patientId, string fromString, string toString)
        {
            DateTime from = Convert.ToDateTime(fromString);
            DateTime to = Convert.ToDateTime(toString).AddHours(23).AddMinutes(59).AddSeconds(59);
            var socs = Context.SOCEvents.Include("SOCMeasure").Where(a => a.PatientId == patientId &&
                a.OccurredDate >= from && a.OccurredDate <= to && !a.DeletedFlg).ToList();

            return PartialView(socs);
        }

        #endregion
        #region Ajax Calls

        [HttpPost]
        public JsonResult DeleteRow(int rowId)
        {
            var soc = Context.SOCEvents.Find(rowId);
            if (soc == null)
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            MarkForDelete(soc, User.Identity.Name);
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

        [HttpPost]
        public JsonResult DeleteRowWound(int rowId)
        {
            var eventWoundNoted = Context.SOCEventWoundNotes.Find(rowId);
            if (eventWoundNoted == null)
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            Context.SOCEventWoundNotes.Remove(eventWoundNoted);
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult DeleteRowAnti(int rowId)
        {
            var eventAntiPsychoticNoted = Context.AntiPsychoticNoted.Find(rowId);
            if (eventAntiPsychoticNoted == null)
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            Context.AntiPsychoticNoted.Remove(eventAntiPsychoticNoted);
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult DeleteRowRestraint(int rowId)
        {

            var eventRestraintNoted = Context.EventRestraintNotes.Find(rowId);
            if (eventRestraintNoted == null)
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            Context.EventRestraintNotes.Remove(eventRestraintNoted);
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        public ActionResult StreamWoundDocument(int id)
        {
            var document = Context.PressureWoundDocuments.SingleOrDefault(d => d.SOCPressureWoundDocumentId == id);

            if (document == null)
            {
                ModelState.AddModelError("DocumentMissing", "Document could not be found");
            }

            document.Document = SevenZipHelper.DecompressStreamLZMA(document.Document).ToArray();

            return File(document.Document, document.ContentType, document.DocumentFileName);
        }

        public ActionResult DeleteWoundDocument(int documentId)
        {

            Context.PressureWoundDocuments.Remove(Context.PressureWoundDocuments.Single(d => d.SOCPressureWoundDocumentId == documentId));
            Context.SaveChanges();

            return Json(new SaveResultViewModel { Success = true });
        }
        #endregion
    }
}