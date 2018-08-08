using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Maintenance.Models;
using AtriumWebApp.Web.Maintenance.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AtriumWebApp.Web.Maintenance.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "EQMAN")]
    public class EquipmentManagementController : BaseController
    {
        protected const string AppCode = "EQMAN";
        protected new EquipmentManagementContext Context
        {
            get { return (EquipmentManagementContext)base.Context; }
        }

        public EquipmentManagementController(IOptions<AppSettingsConfig> config, EquipmentManagementContext context) : base(config, context)
        {
        }

        public IActionResult Index(int? communityId, int? equipmentId)
        {
            //Record user access
            LogSession(AppCode);
            SetLookbackDays(HttpContext, AppCode);
            GetCommunitiesDropDownWithFilter(AppCode);
            var communities = FacilityList[AppCode];
            int selectedCommunity = communityId ?? communities.First().CommunityId;
            var equipment = Context.Equipment.Include("MaintenancePlans.Inspections").Where(a => a.CommunityId == selectedCommunity).ToList();

            EquipmentManagementViewModel vm = new EquipmentManagementViewModel()
            {
                Communities = new SelectList(communities, "CommunityId", "CommunityShortName"),
                Equipment = equipment
            };
            if (equipmentId.HasValue)
            {
                vm.SelectedEquipment = equipment.FirstOrDefault(a => a.EquipmentId == equipmentId.Value);
            }
            //vm.Equipment.First().MaintenancePlans.First().Inspections
            vm.Communities.First(a => a.Value == selectedCommunity.ToString()).Selected = true;

            return View(vm);
        }

        #region Equipment
        public IActionResult EquipmentDetails(int equipmentId)
        {
            Equipment equipment = Context.Equipment.Include("MaintenancePlans")
                .Include("Community")
                .Include("Vendor")
                .FirstOrDefault(a => a.EquipmentId == equipmentId);
            var lookbackDays = Context.Applications.First(a => a.ApplicationCode == AppCode).LookbackDays;
            DateTime lookback = DateTime.Today.AddDays(lookbackDays * -1);
            EquipmentDetailsViewModel vm = new EquipmentDetailsViewModel
            {
                Equipment = equipment,
                Plans = equipment.MaintenancePlans.ToList(),
                Inspections = Context.Inspections.Where(a => a.MaintenancePlan.EquipmentId == equipmentId
                    && a.InspectionDate >= lookback).ToList(),
                Repairs = Context.Repairs.Where(a => a.EquipmentId == equipmentId
                    && a.RepairDate >= lookback).ToList(),
                LookbackDays = lookbackDays
            };

            return PartialView(vm);
        }

        public IActionResult EquipmentList(int communityId)
        {
            var equipment = Context.Equipment.Where(a => a.CommunityId == communityId).ToList();
            return PartialView(equipment);
        }

        public IActionResult EditEquipment(int? equipmentId, int communityId)
        {
            Equipment eq = new Equipment() {
                CommunityId = communityId,
                PurchaseDate = DateTime.Now,
                InstalledDate = DateTime.Now
            };
            if (equipmentId.HasValue)
            {
                eq = Context.Equipment.Find(equipmentId.Value);
            }
            ViewBag.Vendors = new SelectList(Context.Vendors.Where(a => a.DataEntryFlg).ToList(), "POVendorId", "VendorName");
            return EditorFor(eq);
        }

        public IActionResult SaveEquipment(Equipment equipment)
        {
            if (ModelState.IsValid)
            {
                if (equipment.EquipmentId == 0)
                {
                    Context.Equipment.Add(equipment);
                }
                else
                {
                    Equipment current = Context.Equipment.Find(equipment.EquipmentId);
                    Context.Entry(current).CurrentValues.SetValues(equipment);
                }
                Context.SaveChanges();
            }
            return Json(new { success = true, data = equipment.EquipmentId });//RedirectToAction("Index",new { communityId = equipment.CommunityId, equipmentId = equipment.EquipmentId });
        }

        #endregion

        #region Plans
        public IActionResult PlanList(bool upcoming, int communityId)
        {
            if (upcoming)
            {
                var upcomingPlans = Context.MaintenancePlans.Where(a => a.Equipment.CommunityId == communityId && a.NextInspectionDate >= DateTime.Today).ToList();
                return PartialView(upcomingPlans);
            }
            else
            {
                var dueList = Context.MaintenancePlans.Where(a => a.Equipment.CommunityId == communityId && a.NextInspectionDate < DateTime.Today).ToList();
                return PartialView(dueList);
            }
        }

        public IActionResult EditMaintenancePlan(int? planId, int equipmentId)
        {
            EquipmentMaintenancePlan plan = new EquipmentMaintenancePlan() { EquipmentId = equipmentId };
            if (planId.HasValue)
            {
                plan = Context.MaintenancePlans.Find(planId.Value);
            }
            return EditorFor(plan);
        }

        public IActionResult SaveMaintenancePlan(EquipmentMaintenancePlan vm)
        {

            if (ModelState.IsValid)
            {
                bool isNew = vm.EquipmentMaintenancePlanId == 0;
                if (isNew)
                {
                    vm.NextInspectionDate = GetNextInspectionDate(vm.Interval, vm.StartDate);
                    Context.MaintenancePlans.Add(vm);
                }
                else
                {
                    EquipmentMaintenancePlan current = Context.MaintenancePlans.Find(vm.EquipmentMaintenancePlanId);
                    Context.Entry(current).CurrentValues.SetValues(vm);
                }
                Context.SaveChanges();
            }
            SetNextInspectionDate(vm);
            return Json(new { success = true, data = vm.EquipmentMaintenancePlanId }) ;
        }
        #endregion

        #region Inspections
        public IActionResult EditInspection(int? inspectionId, int planId)
        {
            EditEquipmentInspectionViewModel vm = new EditEquipmentInspectionViewModel()
            {
                Documents = new List<DocumentViewModel>()
            };
            if (inspectionId.HasValue)
            {
                vm.Inspection = Context.Inspections.Find(inspectionId.Value);
                foreach (var document in vm.Inspection.Documents)
                {
                    vm.Documents.Add(document);
                }
            }
            else
            {
                vm.Inspection = new EquipmentInspection()
                {
                    EquipmentMaintenancePlanId = planId,
                    InspectionDate = DateTime.Now
                };
            }
            ViewBag.Vendors = new SelectList(Context.Vendors.Where(a => a.DataEntryFlg).ToList(), "POVendorId", "VendorName");
            return EditorFor(vm);
        }
        public IActionResult SaveInspection([FromForm]EditEquipmentInspectionViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Inspection.EquipmentInspectionId == 0)
                {
                    vm.Inspection.Documents = new List<EquipmentInspectionDocument>();
                    if (vm.Documents != null)
                    {
                        foreach (var document in vm.Documents.Where(a => !a.Delete))
                        {
                            vm.Inspection.Documents.Add(document);
                        }
                    }
                    Context.Inspections.Add(vm.Inspection);
                }
                else
                {
                    var currentInspection = Context.Inspections.Find(vm.Inspection.EquipmentInspectionId);
                    if (vm.Documents != null)
                    {
                        //Remove the documents that have been deleted
                        foreach (var deletedDocument in vm.Documents.Where(a => a.Delete && a.Id > 0))
                        {
                            var document = currentInspection.Documents.First(a => a.EquipmentInspectionDocumentId == deletedDocument.Id);
                            Context.InspectionDocuments.Remove(document);
                        }
                        //Add documents that have been added
                        foreach (var document in vm.Documents.Where(a => a.Id == 0 && !a.Delete))
                        {
                            currentInspection.Documents.Add(document);
                        }
                    }

                    Context.Entry(currentInspection).CurrentValues.SetValues(vm.Inspection);
                }
                Context.SaveChanges();
                //update maintenance plan date
                SetNextInspectionDate(Context.MaintenancePlans.Find(vm.Inspection.EquipmentMaintenancePlanId));
            }
            return Content( JsonConvert.SerializeObject(new  { success = true, data = vm.Inspection.EquipmentInspectionId }));
        }
        #endregion

        #region Repairs

        public IActionResult EditRepair(int? repairId, int equipmentId)
        {
            EditEquipmentRepairViewModel vm = new EditEquipmentRepairViewModel()
            {
                Documents = new List<DocumentViewModel>()
            };
            if (repairId.HasValue)
            {
                vm.Repair = Context.Repairs.Find(repairId.Value);
                foreach (var document in vm.Repair.Documents)
                {
                    vm.Documents.Add(document);
                }
            }
            else
            {
                vm.Repair = new EquipmentRepair()
                {
                    EquipmentId = equipmentId,
                    RepairDate = DateTime.Now
                };
            }
            ViewBag.Vendors = new SelectList(Context.Vendors.Where(a => a.DataEntryFlg).ToList(), "POVendorId", "VendorName");
            return EditorFor(vm);
        }
        public IActionResult SaveRepair([FromForm]EditEquipmentRepairViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.Repair.EquipmentRepairId == 0)
                {
                    vm.Repair.Documents = new List<EquipmentRepairDocument>();
                    if (vm.Documents != null)
                    {
                        foreach (var document in vm.Documents.Where(a => !a.Delete))
                        {
                            vm.Repair.Documents.Add(document);
                        }
                    }
                    Context.Repairs.Add(vm.Repair);
                }
                else
                {
                    var currentRepair = Context.Repairs.Find(vm.Repair.EquipmentRepairId);
                    if (vm.Documents != null)
                    {
                        //Remove the documents that have been deleted
                        foreach (var deletedDocument in vm.Documents.Where(a => a.Delete && a.Id > 0))
                        {
                            var document = currentRepair.Documents.First(a => a.EquipmentRepairDocumentId == deletedDocument.Id);
                            Context.RepairDocuments.Remove(document);
                        }
                        //Add documents that have been added
                        foreach (var document in vm.Documents.Where(a => a.Id == 0 && !a.Delete))
                        {
                            currentRepair.Documents.Add(document);
                        }
                    }
                    Context.Entry(currentRepair).CurrentValues.SetValues(vm.Repair);
                }
                Context.SaveChanges();
            }
            return Content(JsonConvert.SerializeObject(new { success=true,id = vm.Repair.EquipmentRepairId }));
        }
        #endregion

        #region Vendors
        public IActionResult ViewVendor(int id)
        {
            var vendor = Context.Vendors.Find(id);
            return DisplayFor(vendor);
        }
        #endregion

        #region Documents
        public IActionResult NewDocument(int index)
        {
            DocumentViewModel viewModel = new DocumentViewModel()
            {
                Id = 0
            };
            ViewData["index"] = index;
            return EditorFor(viewModel);
        }
        public ActionResult StreamDocument(int id, string docSource)
        {
            string fileName = string.Empty;
            string contentType = string.Empty;
            byte[] file = new byte[0];
            if (docSource == "repair")
            {
                var document = Context.RepairDocuments.SingleOrDefault(d => d.EquipmentRepairDocumentId == id);
                if (document == null)
                {
                    ModelState.AddModelError("DocumentMissing", "Document could not be found");
                }
                fileName = document.DocumentFileName;
                contentType = document.ContentType;
                file = document.Document;
            }
            else if (docSource == "inspection")
            {
                var document = Context.InspectionDocuments.SingleOrDefault(d => d.EquipmentInspectionDocumentId == id);
                if (document == null)
                {
                    ModelState.AddModelError("DocumentMissing", "Document could not be found");
                }
                fileName = document.DocumentFileName;
                contentType = document.ContentType;
                file = document.Document;
            }
            else
            {
                ModelState.AddModelError("DocumentMissing", "Document could not be found");
            }


            return File(file, contentType, fileName);
        }
        #endregion

        private void SetNextInspectionDate(EquipmentMaintenancePlan plan)
        {
            DateTime nextInspectionDate = plan.StartDate;
            //update next inspection date, it may have changed
            var lastInspection = Context.Inspections.Where(a => a.EquipmentMaintenancePlanId == plan.EquipmentMaintenancePlanId)
                .OrderByDescending(a => a.InspectionDate).FirstOrDefault();
            if (lastInspection != null && plan.StartDate < lastInspection.InspectionDate)
            {
                nextInspectionDate = GetNextInspectionDate(plan.Interval, lastInspection.InspectionDate);
            }           
            Context.MaintenancePlans.Find(plan.EquipmentMaintenancePlanId).NextInspectionDate = nextInspectionDate;
            Context.SaveChanges();
        }

        private DateTime GetNextInspectionDate(string interval, DateTime date)
        {
            var dueDate = date;
            switch (interval)
            {
                case "Daily":
                    dueDate = dueDate.AddDays(1);
                    break;
                case "Weekly":
                    dueDate = dueDate.AddDays(7);
                    break;
                case "Monthly":
                    dueDate = dueDate.AddMonths(1);
                    break;
                case "Quarterly":
                    dueDate = dueDate.AddMonths(3);
                    break;
                case "Yearly":
                    dueDate = dueDate.AddYears(1);
                    break;
            }
            return dueDate;
        }

    }
}