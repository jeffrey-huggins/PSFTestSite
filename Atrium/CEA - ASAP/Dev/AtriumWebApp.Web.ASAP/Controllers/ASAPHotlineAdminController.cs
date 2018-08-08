using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.ASAP.Models;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.ASAP.Controllers
{
    [RestrictAccessWithApp( Admin = true, AppCode = "ASAP")]
    public class ASAPHotlineAdminController : BaseAdminController
    {
        private const string AppCode = "ASAP";

        public ASAPHotlineAdminController(IOptions<AppSettingsConfig> config, ASAPHotlineContext context) : base(config, context)
        {
        }

        protected new ASAPHotlineContext Context
        {
            get { return (ASAPHotlineContext)base.Context; }
        }

        public ActionResult Index()
        {
            //If user doesn't have access, redirect to the home page
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            SetLookbackDaysAdmin(HttpContext, AppCode);
            SetEmployeeDropDown();
            var adminViewModel = CreateAdminViewModel(AppCode);
            var complaintTypeContext = new ASAPComplaintTypeContext();
            return View(new ASAPHotlineAdminViewModel
            {
                ComplaintTypes = complaintTypeContext.AsapComplaintTypes.ToList(),
                ContactList = Context.Contacts.ToList(),
                AdminViewModel = adminViewModel
            });
        }

        private void SetEmployeeDropDown()
        {
            var employeeList = (from emp in Context.Employees
                                where
                                    (emp.EmployeeStatus == "Active" || emp.EmployeeStatus == "Leave of Absence") &&
                                    (emp.JobClasses.Any(a => a.JobClass.JobDescription == "Administrative Executive" || a.JobClass.JobDescription == "Other Nursing Admin"))
                                orderby emp.LastName
                                select new SelectListItem
                                {
                                    Text = emp.LastName + ", " + emp.FirstName,
                                    Value = SqlFunctions.StringConvert((double)emp.EmployeeId)
                                }).ToList();
            ViewData["Contacts"] = new SelectList(employeeList, "Value", "Text");
        }

        
        public ActionResult NewComplaint(ASAPComplaintType complaintType)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            var complaintTypeContext = new ASAPComplaintTypeContext();
            complaintType.DataEntryFlg = true;
            complaintTypeContext.AsapComplaintTypes.Add(complaintType);
            complaintTypeContext.SaveChanges();
            return Json(new { Success = true, data = complaintType });
        }

        [HttpPost]
        public ActionResult SaveContact(ASAPContact contact)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("");
            }
            Context.Contacts.Add(contact);
            Context.SaveChanges();
            return Json(new { Success = true, data = contact });
        }

        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, AppCode);
            return RedirectToAction("");
        }

        public JsonResult ChangeDataFlg(string description, bool dFlag)
        {
            var complaintTypeContext = new ASAPComplaintTypeContext();
            var complaintType = complaintTypeContext.AsapComplaintTypes.Single(c => c.ASAPComplaintTypeDesc == description);
            complaintType.DataEntryFlg = dFlag;
            complaintTypeContext.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult DeleteRowAdmin(int rowId)
        {
            var complaintTypeContext = new ASAPComplaintTypeContext();
            try
            {
                complaintTypeContext.AsapComplaintTypes.Remove(complaintTypeContext.AsapComplaintTypes.Find(rowId));
                complaintTypeContext.SaveChanges();
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            return Json(new SaveResultViewModel { Success = true });
        }

        public JsonResult EditRowNameOrder(int rowId, string description, int order)
        {
            var complaintTypeContext = new ASAPComplaintTypeContext();
            var complaintType = complaintTypeContext.AsapComplaintTypes.Find(rowId);
            complaintType.ASAPComplaintTypeDesc = description;
            complaintType.SortOrder = order;
            complaintTypeContext.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        //public JsonResult EnableDisableContact(int empId, bool check)
        //{
        //    var contactContext = new ASAPContactContext();
        //    if (contactContext.Contacts.Any(e => e.EmployeeId == empId) && !check)
        //    {
        //        contactContext.Contacts.Remove(contactContext.Contacts.Find(empId));
        //    }
        //    else
        //    {
        //        contactContext.Contacts.Add(new ASAPContact {EmployeeId = empId});
        //    }
        //    return Json(new {Success = true});
        //}

        public JsonResult DeleteContact(int rowId)
        {
            try
            {
                Context.Contacts.Remove(Context.Contacts.Find(rowId));
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