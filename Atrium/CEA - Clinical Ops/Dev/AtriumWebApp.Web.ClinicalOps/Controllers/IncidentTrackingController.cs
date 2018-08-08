using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using System.Threading;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using Newtonsoft.Json;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "ITR")]
    public class IncidentTrackingController : BaseIncidentTrackingController
    {
        public IncidentTrackingController(IOptions<AppSettingsConfig> config, IncidentTrackingContext context) : base(config, context)
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

            if (!Session.TryGetObject("SideBar", out SideBarViewModel sideBar))
            {
                sideBar = SideBarService.InitSideBar(this, AppCode, Context);
            }

            IncidentTrackingViewModel vm = new IncidentTrackingViewModel()
            {
                SideBar = sideBar,
                RangeTo = OccurredRangeTo[AppCode],
                RangeFrom = OccurredRangeFrom[AppCode]
            };
            return View(vm);
        }

        public IActionResult GetIncidentList(int patientId, string fromString, string toString)
        {
            DateTime from = Convert.ToDateTime(fromString);
            DateTime to = Convert.ToDateTime(toString).AddHours(23).AddMinutes(59).AddSeconds(59);

            var incidentsForResident = Context.PatientIncidentEvents.Include("RegionalNurse")
                .Include("IncidentType")
                .Where(a => a.PatientId == patientId
                    && !a.DeletedFlg
                    && a.IncidentDateTime >= from
                    && a.IncidentDateTime <= to)
                .OrderByDescending(a => a.IncidentDateTime).ToList();
            return PartialView(incidentsForResident);
        }

        public IActionResult EditIncident(int? incidentEventId, int patientId)
        {
            var patient = Context.Residents.Find(patientId);
            var community = Context.Residents.Where(a => a.PatientId == patientId)
                .Select(a => a.CommunityId).FirstOrDefault();
            if (community == 0)
            {
                return Json(new { Success = false, data = "Unable to locate patient." });
            }
            ViewBag.IncidentTypes = Context.PatientIncidentTypes.Where(a => a.DataEntryFlg)
                .OrderBy(a => a.SortOrder)
                .ThenBy(a => a.PatientIncidentName).ToList();
            ViewBag.Locations = Context.IncidentLocations.Where(a => a.DataEntryFlg)
                .OrderBy(a => a.SortOrder)
                .ThenBy(a => a.PatientIncidentLocationName).ToList();
            List<int> nurseIds = Context.RegionalNurses.Where(a => a.CommunityId == patient.CommunityId)
                .Select(a => a.RegionalNurseEmployeeId).ToList();
            nurseIds.AddRange(Context.CloseAllCommunitiesEmployees.Select(a => a.EmployeeId));
            ViewBag.Nurses = Context.Employees.Where(a => nurseIds.Any(id => id == a.EmployeeId)).OrderBy(a => a.LastName).ToList();
            PatientIncidentEventViewModel vm = new PatientIncidentEventViewModel()
            {
                InterventionList = new List<IncidentInterventionSelection>(),
                TreatmentList = new List<IncidentTreatmentSelection>(),
            };




            if (incidentEventId.HasValue)
            {
                vm.Event = Context.PatientIncidentEvents.Include("Treatments").Include("Interventions").FirstOrDefault(a => a.PatientIncidentEventId == incidentEventId.Value);
                if (vm.Event == null)
                {
                    return Json(new { success = false, data = "Incident not found." });
                }

                foreach (var treatment in Context.IncidentTreatments.Where(a => a.DataEntryFlg))
                {
                    vm.TreatmentList.Add(new IncidentTreatmentSelection()
                    {
                        Selected = vm.Event.Treatments.Any(a => a.PatientIncidentTreatmentId == treatment.PatientIncidentTreatmentId),
                        Type = treatment
                    });
                }
                foreach (var intervention in Context.IncidentInterventions.Where(a => a.DataEntryFlg))
                {
                    vm.InterventionList.Add(new IncidentInterventionSelection()
                    {
                        Selected = vm.Event.Interventions.Any(a => a.PatientIncidentInterventionId == intervention.PatientIncidentInterventionId),
                        Type = intervention
                    });
                }
            }
            else
            {
                foreach (var treatment in Context.IncidentTreatments.Where(a => a.DataEntryFlg))
                {
                    vm.TreatmentList.Add(new IncidentTreatmentSelection()
                    {
                        Selected = false,
                        Type = treatment
                    });
                }
                foreach (var intervention in Context.IncidentInterventions.Where(a => a.DataEntryFlg))
                {
                    vm.InterventionList.Add(new IncidentInterventionSelection()
                    {
                        Selected = false,
                        Type = intervention
                    });
                }
                vm.Event = new PatientIncidentEvent()
                {
                    CurrentPayerId = patient.CurrentPayerId,
                    PatientId = patientId,
                    RoomId = patient.RoomId
                };
            }
            vm.TreatmentList = vm.TreatmentList.OrderBy(a => a.Type.SortOrder)
                .ThenBy(a => a.Type.PatientIncidentTreatmentName).ToList();
            vm.InterventionList = vm.InterventionList.OrderBy(a => a.Type.SortOrder)
                .ThenBy(a => a.Type.PatientIncidentInterventionName).ToList();

            return EditorFor(vm);
        }



        public IActionResult SaveIncident([FromForm]PatientIncidentEventViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Event.PatientIncidentEventId == 0)
                {
                    Context.PatientIncidentEvents.Add(vm.Event);
                }
                else
                {
                    var currentIncident = Context.PatientIncidentEvents.Find(vm.Event.PatientIncidentEventId);
                    Context.Entry(currentIncident).CurrentValues.SetValues(vm.Event);
                    foreach (var treatment in Context.EventTreatments.Where(e => e.PatientIncidentEventId == vm.Event.PatientIncidentEventId).ToList())
                    {
                        Context.EventTreatments.Remove(treatment);
                    }
                    foreach (var intervention in Context.EventInterventions.Where(e => e.PatientIncidentEventId == vm.Event.PatientIncidentEventId).ToList())
                    {
                        Context.EventInterventions.Remove(intervention);
                    }
                }
                Context.SaveChanges();
                foreach (var treatment in vm.TreatmentList.Where(a => a.Selected))
                {
                    Context.EventTreatments.Add(new PatientIncidentEventTreatment()
                    {
                        PatientIncidentTreatmentId = treatment.Type.PatientIncidentTreatmentId,
                        PatientIncidentEventId = vm.Event.PatientIncidentEventId
                    });
                }
                foreach (var intervention in vm.InterventionList.Where(a => a.Selected))
                {
                    Context.EventInterventions.Add(new PatientIncidentEventIntervention()
                    {
                        PatientIncidentInterventionId = intervention.Type.PatientIncidentInterventionId,
                        PatientIncidentEventId = vm.Event.PatientIncidentEventId
                    });
                }
                Context.SaveChanges();
                return Content(JsonConvert.SerializeObject(new { success = true, id = vm.Event.PatientIncidentEventId }));
            }
            return Content(JsonConvert.SerializeObject(new { success = false, data = "Please check all fields and try again." }));
        }

        public IActionResult CreateNewDocument(int parentId)
        {
            return EditorFor(new DocumentViewModel());
        }

        public IActionResult StreamIncidentEventDocument(int id)
        {
            var document = Context.PatientIncidentEventDocuments.SingleOrDefault(d => d.PatientIncidentEventDocumentId == id);

            if (document == null)
            {
                ModelState.AddModelError("DocumentMissing", "Document could not be found");
            }

            document.Document = SevenZipHelper.DecompressStreamLZMA(document.Document).ToArray();

            return File(document.Document, document.ContentType, document.DocumentFileName);
        }

        public IActionResult DeleteIncidentEventDocument(int id)
        {
            try
            {
                var currentDoc = Context.PatientIncidentEventDocuments.Find(id);
                Context.PatientIncidentEventDocuments.Remove(currentDoc);
                Context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }

        public IActionResult SaveDocuments([FromForm]ICollection<DocumentViewModel> Documents)
        {
            try
            {
                foreach (var vm in Documents)
                {
                    if (vm.Document != null)
                    {
                        PatientIncidentEventDocument document = vm;
                        if(Context.PatientIncidentEventDocuments.Any(a => a.DocumentFileName == document.DocumentFileName && a.PatientIncidentEventId == document.PatientIncidentEventId))
                        {
                            continue;
                        }
                        document.Document = SevenZipHelper.CompressStreamLZMA(document.Document).ToArray();
                        Context.PatientIncidentEventDocuments.Add(document);
                    }
                }
                Context.SaveChanges();
                return Content("{\"Success\":true }");
            }
            catch(Exception ex)
            {
                return Content("{\"Success\":false }");
            }
        }

        #region Ajax Calls

        public JsonResult DeleteRow(int rowId)
        {
            var pie = Context.PatientIncidentEvents.Find(rowId);
            if (pie == null)
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            //pieContext.PatientIncidentEvents.Remove(pie);
            pie.DeletedFlg = true;
            pie.DeletedTS = DateTime.Now;
            pie.DeletedADDomainName = User.Identity.Name;
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

        #endregion
    }
}