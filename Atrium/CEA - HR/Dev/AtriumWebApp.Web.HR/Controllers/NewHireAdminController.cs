using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using AtriumWebApp.Web.HR.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using AtriumWebApp.Web.HR.Models.ViewModel;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.HR.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "NEWHIRE", Admin=true)]
    public class NewHireAdminController : BaseAdminController
    {
        protected const string AppCode = "NEWHIRE";

        public NewHireAdminController(IOptions<AppSettingsConfig> config, EmployeeNewHireContext context) : base(config, context)
        {
        }

        //private STNATrainingContext Context = new STNATrainingContext();
        protected new EmployeeNewHireContext Context
        {
            get { return (EmployeeNewHireContext)base.Context; }
        }
        
        //
        // GET: /STNAAdmin/


        public ActionResult Index()
        {
            //If user doesn't have access, redirect to the home page
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            SelectList stateCodes = new SelectList(Context.States.OrderBy(a => a.StateCd).ToList(), "StateCd", "StateCd");
            
            ViewBag.stateCodes = stateCodes;
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            var adminViewModel = CreateAdminViewModel(AppCode);
			return View(new NewHireAdminViewModel
			{
				LookbackDays = Context.Applications.First(a => a.ApplicationCode == AppCode).LookbackDays,
                AdminViewModel = adminViewModel
            });
        }

		public IActionResult CheckListListing()
		{
			return PartialView(Context.MasterNewHireChecklist.OrderBy(a => a.SortOrder).ToList());
		}

		public IActionResult EditNewHireChecklist(int? id)
		{
			MasterNewHireChecklist checkList = new MasterNewHireChecklist();
			if (id.HasValue)
			{
				checkList = Context.MasterNewHireChecklist.Find(id.Value);
			}
			return PartialView("EditorTemplates/MasterNewHireChecklist",checkList);
		}

		public IActionResult SaveNewHireChecklist(MasterNewHireChecklist checklist)
		{
			if (ModelState.IsValid)
			{
				if(checklist.NewHireChecklistId == 0)
				{
					Context.MasterNewHireChecklist.Add(checklist);
				}
				else
				{
					var currentChecklist = Context.MasterNewHireChecklist.Find(checklist.NewHireChecklistId);
					Context.Entry(currentChecklist).CurrentValues.SetValues(checklist);
				}
				Context.SaveChanges();
				return Json(new { success = true });
			}
			else
			{
				return Json(new { success = false });
			}
		}

        #region Other Post Backs
        [HttpPost]
        public ActionResult SaveLookback(string lookbackDays)
        {
            BaseAdminController.SaveLookbackToApp(HttpContext, lookbackDays, AppCode);
            return RedirectToAction("");
        }
        #endregion


    }
}