using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.Budget;
using AtriumWebApp.Models.Budget.ViewModels;
using AtriumWebApp.Web.Budget.Library;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using OfficeOpenXml;

namespace AtriumWebApp.Web.Budget.Controllers
{
    public class AdminBudgetController : Controller
    {
        BudgetMasterIntactEntities BudgetContext { get; set; }

        public AdminBudgetController(BudgetMasterIntactEntities budgetContext)
        {
            BudgetContext = budgetContext;
        }

        public IActionResult BudgetTypes()
        {
            return PartialView(BudgetContext.GLAccountTypes.ToList());
        }

        public IActionResult EditBT(int BTId)
        {

            var budgetTypes = new SelectList(BudgetContext.GLAccountTypes, "GLAccountTypeID", "GLAccountTypeNm");
            ViewBag.BudgetTypes = budgetTypes;

            if (BTId == 0)
            {
                return PartialView(new GLAccountType());
            }
            var bt = BudgetContext.GLAccountTypes.Find(BTId);
            if (bt == null)
            {
                return NotFound();
            }
            return PartialView(bt);
        }

        public IActionResult SaveBT(GLAccountType budgetType)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid for this general ledger." });
            }
            string type = "existing";
            var oldBT = BudgetContext.GLAccountTypes.Find(budgetType.GLAccountTypeID);
            if (oldBT == null)
            {
                oldBT = budgetType;
                BudgetContext.GLAccountTypes.Add(oldBT);
                type = "new";
            }
            else
            {
                BudgetContext.Entry(oldBT).CurrentValues.SetValues(budgetType);
            }

            BudgetContext.SaveChanges();
            return Json(new { Success = true, Type = type });
        }

        public IActionResult DeleteBT(int BTId)
        {
            var oldBT = BudgetContext.GLAccountTypes.Find(BTId);
            if (oldBT == null)
                return Json(new { Success = false, Message = "Unable to locate a Budget Type with id " + BTId });
            BudgetContext.GLAccountTypes.Remove(oldBT);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });

        }

        public IActionResult EditBY()
        {
            return PartialView(BudgetContext.Budgets.FirstOrDefault());
        }

        public IActionResult SaveBY(AtriumWebApp.Models.Budget.Budget budget)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid." });
            }
            var oldBY = BudgetContext.Budgets.Find(budget.BudgetID);
            if (oldBY == null)
            {
                return Json(new { Success = false, Message = "Unable to locate the budget.  Only one budget year is supported" });
            }
            BudgetContext.Entry(oldBY).CurrentValues.SetValues(budget);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

    }
}