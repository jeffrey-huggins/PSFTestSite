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
    public class AdminCalcsController : Controller
    {
        BudgetMasterIntactEntities BudgetContext { get; set; }

        public AdminCalcsController(BudgetMasterIntactEntities budgetContext)
        {
            BudgetContext = budgetContext;
        }

        public IActionResult OtherCalcs()
        {
            return PartialView(BudgetContext.OtherExpensesGLCalcs.ToList());
        }

        public IActionResult OtherCalcValues()
        {
            return PartialView(BudgetContext.OtherExpensesGLCalcValueMasters.ToList());
        }

        public IActionResult OtherCalcValueSetup()
        {
            return PartialView(BudgetContext.OtherExpensesGLCalcs.ToList());
        }

        public IActionResult EditOtherCalcs(int id)
        {
            if (id == 0)
            {
                return PartialView(new OtherExpensesGLCalc());
            }


            var otherCalcs = BudgetContext.OtherExpensesGLCalcs.Find(id);
            if(otherCalcs == null)
            {
                return NotFound();
            }
            return PartialView(otherCalcs);
        }

        public IActionResult SaveOtherCalcs(OtherExpensesGLCalc calc)
        {

            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid." });
            }
            if (BudgetContext.OtherExpensesGLCalcs.Any(a => a.OtherExpensesCalcID != calc.OtherExpensesCalcID && a.CalcCd == calc.CalcCd)){
                return Json(new { Success = false, Message = "Calc Code needs to be unique." });
            }
            if (BudgetContext.OtherExpensesGLCalcs.Any(a => a.OtherExpensesCalcID != calc.OtherExpensesCalcID && a.CalcNm == calc.CalcNm))
            {
                return Json(new { Success = false, Message = "Calc Name needs to be unique." });
            }
            if (calc.OtherExpensesCalcID == 0)
            {
                BudgetContext.OtherExpensesGLCalcs.Add(calc);
            }
            else
            {
                var oldCalc = BudgetContext.OtherExpensesGLCalcs.Find(calc.OtherExpensesCalcID);
                if(oldCalc == null)
                {
                    return Json(new { Success = false, Message = "Unable to update calculation, invalid Index" });
                }
                BudgetContext.Entry(oldCalc).CurrentValues.SetValues(calc);
            }

            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult DeleteOtherCalcs(int id)
        {
            var oldcalc = BudgetContext.OtherExpensesGLCalcs.Find(id);
            if (oldcalc == null)
            {
                return Json(new { Success = false, Message = "Unable to delete calculation, invalid Index" });
            }
            BudgetContext.OtherExpensesGLCalcs.Remove(oldcalc);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult EditOtherCalcValues(int id)
        {
            if (id == 0)
            {
                return PartialView(new OtherExpensesGLCalcValueMaster());
            }


            var otherCalcs = BudgetContext.OtherExpensesGLCalcValueMasters.Find(id);
            if (otherCalcs == null)
            {
                return NotFound();
            }
            return PartialView(otherCalcs);
        }

        public IActionResult SaveOtherCalcValues(OtherExpensesGLCalcValueMaster calc)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid." });
            }
            if (calc.CalcValueID == 0)
            {
                BudgetContext.OtherExpensesGLCalcValueMasters.Add(calc);
            }

            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult DeleteOtherCalcValues(int id)
        {
            var oldcalc = BudgetContext.OtherExpensesGLCalcValueMasters.Find(id);
            if (oldcalc == null)
            {
                return Json(new { Success = false, Message = "Unable to delete calculation, invalid Index" });
            }
            BudgetContext.OtherExpensesGLCalcValueMasters.Remove(oldcalc);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult EditOtherCalcValueSetup(int id)
        {
            var otherCalcs = BudgetContext.OtherExpensesGLCalcs.Find(id);
            if (otherCalcs == null)
            {
                return NotFound();
            }
            CalcValueSetupViewModel viewModel = new CalcValueSetupViewModel()
            {
                Calc = otherCalcs,
                MasterCalcs = BudgetContext.OtherExpensesGLCalcValueMasters.ToList()
            };




            return PartialView(viewModel);
        }

        public IActionResult SaveOtherCalcValueSetup(OtherExpensesGLCalc calc)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid." });
            }
            if (calc.OtherExpensesCalcID == 0)
            {
                BudgetContext.OtherExpensesGLCalcs.Add(calc);
            }

            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }
    }
}