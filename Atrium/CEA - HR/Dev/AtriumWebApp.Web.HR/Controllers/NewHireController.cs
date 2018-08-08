using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Base.Library;
using Microsoft.Extensions.Options;
using AtriumWebApp.Web.HR.Models;
using AtriumWebApp.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using AtriumWebApp.Web.HR.Models.ViewModel;

namespace AtriumWebApp.Web.HR.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "NEWHIRE")]
    public class NewHireController : BaseController
    {
        protected const string AppCode = "NEWHIRE";

        protected new EmployeeNewHireContext Context
        {
            get { return (EmployeeNewHireContext)base.Context; }
        }

        public NewHireController(IOptions<AppSettingsConfig> config, EmployeeNewHireContext context) : base(config, context)
        {

        }

        public IActionResult Index()
        {
            // Record user access
            LogSession(AppCode);
            NewHireSideBar vm = new NewHireSideBar();
            vm.LookBackDate = DateTime.Now.AddDays(-Context.Applications.First(a => a.ApplicationCode == AppCode).LookbackDays);
            var communities = FilterCommunitiesByADGroups(AppCode);
            var selectedCommunity = communities[0];
            SelectList communitySelector = new SelectList(communities, "CommunityId", "CommunityShortName", selectedCommunity);
            List<Employee> employees = Context.Employees.Where(a => a.CommunityId == selectedCommunity.CommunityId
                    && a.HireDate >= vm.LookBackDate
                    && a.TerminationType != "Terminated").ToList();
            vm.EmployeeStatusList = new List<EmployeeDocumentStatus>();
            foreach (var employee in employees)
            {
                vm.EmployeeStatusList.Add(GetEmployeeStatus(employee));
            }
            vm.FacilityList = communitySelector;
            vm.SelectedFacilityId = selectedCommunity.CommunityId;
            return View(vm);
        }

        public IActionResult GetEmployeeInfo(int id)
        {
            Employee employee = Context.Employees.FirstOrDefault(a => a.EmployeeId == id);

            if (employee == null)
            {
                return Json(new { success = false, data = "Employee not found." });
            }
            EmployeeInfoViewModel vm = new EmployeeInfoViewModel()
            {
                EmployeeName = employee.LastName + ", " + employee.FirstName,
                CommunityName = Context.Facilities.Find(employee.CommunityId).CommunityShortName,
                TerminationDate = employee.TerminationDate,
                HireDate = employee.HireDate
            };
            return PartialView("DisplayTemplates/EmployeeInfoViewModel", vm);
        }

        public IActionResult SideBarUpdate(NewHireSideBar vm)
        {
            var communities = FilterCommunitiesByADGroups(AppCode);
            var selectedCommunity = communities.First(a => a.CommunityId == vm.SelectedFacilityId);
            SelectList communitySelector = new SelectList(communities, "CommunityId", "CommunityShortName", selectedCommunity);
            vm.FacilityList = communitySelector;

            List<Employee> employees;
            if (vm.IncludeTerminated)
            {
                employees = Context.Employees.Where(a => a.CommunityId == vm.SelectedFacilityId
                    && a.HireDate >= vm.LookBackDate).ToList();
            }
            else
            {
                employees = Context.Employees.Where(a => a.CommunityId == vm.SelectedFacilityId
                    && a.HireDate >= vm.LookBackDate
                    && a.TerminationType != "Terminated").ToList();
            }
            vm.EmployeeStatusList = new List<EmployeeDocumentStatus>();
            foreach (var employee in employees)
            {

                vm.EmployeeStatusList.Add(GetEmployeeStatus(employee));
            }

            vm.EmployeeList = new SelectList(employees.OrderBy(a => a.LastName).Select(a => new SelectListItem()
            {
                Text = a.LastName + ", " + a.FirstName,
                Value = a.EmployeeId.ToString()
            }), "Value", "Text");
            return PartialView("EditorTemplates/NewHireSideBar", vm);

            //return EditorFor(vm);
        }

        private EmployeeDocumentStatus GetEmployeeStatus(Employee employee)
        {
            var hireDocument = Context.EmployeeNewHire.Include("CheckList").FirstOrDefault(a => a.EmployeeId == employee.EmployeeId && a.CurrentHireDate == employee.HireDate);

            string status = string.Empty;
            if (hireDocument == null)
            {
                status = "notStarted";
            }
            else if (hireDocument.CompletedFlg)
            {
                status = "complete";
            }
            else
            {
                status = "inProgress";
                //Use this odd method to prevent retrieving files from the database
                //!hireDocument.CheckList.Any(a => a.Documents.Count == 0)
                foreach (var checklist in hireDocument.CheckList)
                {
                    if (!Context.EmployeeNewHireChecklistDocument.Any(a => a.EmployeeNewHireChecklistId == checklist.EmployeeNewHireChecklistId))
                    {
                        status = "notStarted";
                        break;
                    }
                }
            }
            return new EmployeeDocumentStatus()
            {
                EmployeeDisplayName = employee.LastName + ", " + employee.FirstName,
                EmployeeDocStatus = status,
                EmployeeId = employee.EmployeeId
            };
        }

        public IActionResult NewHireListing(int employeeId)
        {
            var employee = Context.Employees.FirstOrDefault(a => a.EmployeeId == employeeId);
            if (employee == null)
            {
                return NotFound();
            }
            var employeeNewHire = Context.EmployeeNewHire.Where(a => a.EmployeeId == employeeId);
            List<EmployeeNewHire> checkListSelection = new List<EmployeeNewHire>();

            if (!employeeNewHire.Any())
            {
                EmployeeNewHire newHire = new EmployeeNewHire()
                {
                    CompletedFlg = false,
                    CurrentHireDate = employee.HireDate,
                    EmployeeId = employeeId,
                    CheckList = new List<EmployeeNewHireChecklist>()
                };
                var currentChecklists = Context.MasterNewHireChecklist.Where(a => a.EffectiveBeginDate <= employee.HireDate
                    && a.EffectiveEndDate >= employee.HireDate).ToList();
                foreach (var checklist in currentChecklists)
                {
                    newHire.CheckList.Add(new EmployeeNewHireChecklist()
                    {
                        NewHireChecklistId = checklist.NewHireChecklistId
                    });
                }
                Context.EmployeeNewHire.Add(newHire);
                Context.SaveChanges();
                checkListSelection.Add(newHire);
            }
            else
            {
                foreach (var checklist in employeeNewHire)
                {
                    checkListSelection.Add(checklist);
                }
            }
            return PartialView(checkListSelection.OrderByDescending(a => a.CurrentHireDate).ToList());
        }

        public IActionResult EmployeeChecklist(int employeeNewHireId)
        {
            var employeeNewHire = Context.EmployeeNewHire.Include("CheckList.CheckListInfo").Include("CheckList.Documents")
                .FirstOrDefault(a => a.EmployeeNewHireId == employeeNewHireId);
            if (employeeNewHire == null)
            {
                return NotFound();
            }
            return PartialView(employeeNewHire);
        }

        public IActionResult UploadFile([FromForm]DocumentViewModel vm)
        {
            try
            {
                EmployeeNewHireChecklistDocument document = vm;
                document.Document = SevenZipHelper.CompressStreamLZMA(document.Document).ToArray();
                if (Context.EmployeeNewHireChecklistDocument.Any(a => a.DocumentFileName == document.DocumentFileName
                     && document.EmployeeNewHireChecklistId == a.EmployeeNewHireChecklistId))
                {
                    return Content("{\"success\":false, \"data\": \"A file with that name has already been uploaded for this checklist.\"}");
                }
                Context.EmployeeNewHireChecklistDocument.Add(document);
                Context.SaveChanges();
                return Content("{\"success\":true}");
            }
            catch (Exception ex)
            {
                return Content("{\"success\":false, \"data\":" + ex.Message + "}");
            }
        }

        public IActionResult GetFile(int docId)
        {
            string fileName = string.Empty;
            string contentType = string.Empty;
            byte[] file = new byte[0];
            var document = Context.EmployeeNewHireChecklistDocument.FirstOrDefault(a => a.EmployeeNewHireChecklistDocumentId == docId);
            if (document == null)
            {
                return NotFound();
            }
            fileName = document.DocumentFileName;
            contentType = document.ContentType;
            file = SevenZipHelper.DecompressStreamLZMA(document.Document).ToArray();
            return File(file, contentType, fileName);
        }

        public IActionResult DeleteFile(int docId)
        {
            var document = Context.EmployeeNewHireChecklistDocument.FirstOrDefault(a => a.EmployeeNewHireChecklistDocumentId == docId);
            if (document == null)
            {
                return NotFound();
            }
            Context.EmployeeNewHireChecklistDocument.Remove(document);
            Context.SaveChanges();
            return Json(new { success = true });
        }

        public IActionResult SetCompletedFlg(int newHireId, bool status)
        {
            var newHire = Context.EmployeeNewHire.FirstOrDefault(a => a.EmployeeNewHireId == newHireId);
            if (newHire == null)
            {
                return Json(new { success = false, data = "Unable to locate the checklist." });
            }

            newHire.CompletedFlg = status;
            Context.SaveChanges();
            return Json(new { success = true });
        }
    }
}