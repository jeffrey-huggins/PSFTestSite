using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.Budget;
using AtriumWebApp.Models.Budget.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Budget.Controllers
{
    public class OtherRevenueController : Controller
    {
        BudgetMasterIntactEntities BudgetContext { get; set; }
        public OtherRevenueController( BudgetMasterIntactEntities budgetContext) 
        {
            BudgetContext = budgetContext;
        }

        public IActionResult Index(int facilityId, int budgetId)
        {
            Facility facility = BudgetContext.Facilities.Find(facilityId);

            MainFormViewModel viewModel = new MainFormViewModel()
            {
                OtherRevenue = BudgetContext.OtherRevenues.Where(a => a.FacilityID == facilityId).ToList(),
                FacilityBudget = facility.FacilityBudgets.FirstOrDefault(a => a.BudgetID == budgetId)
            };

            return PartialView(viewModel);
        }

        public IActionResult Display(int facilityId, int GLAccountIndex, int budgetId)
        {
            var otherRevenueList = BudgetContext.OtherRevenues.Where(a => a.FacilityID == facilityId
            && a.GLAccountIndex == GLAccountIndex
            && a.BudgetID == budgetId);

            OtherRevenueViewModel viewModel = new OtherRevenueViewModel()
            {
                MonthlyCensus = otherRevenueList.ToList(),
                GeneralLedgers = BudgetContext.GeneralLedgers.Where(a => a.GLAccountTypeID == 2).ToList(),
                Months = BudgetContext.MonthTables.ToList()

            };

            return PartialView(viewModel);
        }


        public IActionResult Delete(int facilityId, int budgetId, int GLAccountIndex)
        {
            var gl = BudgetContext.FacilityBudgetGLs.First(a => a.FacilityID == facilityId &&
                a.BudgetID == budgetId &&
                a.GLAccountIndex == GLAccountIndex);
            BudgetContext.FacilityBudgetGLs.Remove(gl);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult Create(int facilityId, int budgetId, int GLAccountIndex)
        {
            if (BudgetContext.OtherRevenues.Count(a => a.FacilityID == facilityId
                && a.GLAccountIndex == GLAccountIndex
                && a.BudgetID == budgetId) > 0)
            {
                return Json(new { Success = false, Message = "Census with that GLAccount already exists." });
            }

            var OtherRevenueList = new List<OtherRevenue>();
            for (var i = 1; i <= 12; i++)
            {
                OtherRevenue newCensus = new OtherRevenue()
                {
                    FacilityID = facilityId,
                    BudgetID = budgetId,
                    GLAccountIndex = GLAccountIndex,
                    MonthID = i,
                    RevenueAmt =0
                };
                OtherRevenueList.Add(newCensus);

            }
            FacilityBudgetGL gl = new FacilityBudgetGL()
            {
                BudgetID = budgetId,
                GLAccountIndex = GLAccountIndex,
                FacilityID = facilityId,
                OtherRevenues = OtherRevenueList
            };

            BudgetContext.FacilityBudgetGLs.Add(gl);
            BudgetContext.SaveChanges();

            return Json(new { Success = true });
        }

        public IActionResult Edit(int facilityId, int oldGLAccountIndex, int budgetId, int monthId, decimal? fixedAmount)
        {
            OtherRevenue currentRevenue = BudgetContext.OtherRevenues.FirstOrDefault(a => a.FacilityID == facilityId &&
            a.GLAccountIndex == oldGLAccountIndex &&
            a.BudgetID == budgetId &&
            a.MonthID == monthId);

            currentRevenue.RevenueAmt = fixedAmount;

            BudgetContext.SaveChanges();
            var monthlyADC = currentRevenue.MonthTable.Census.Where(a => a.FacilityID == currentRevenue.FacilityID 
                && a.MonthID == currentRevenue.MonthID).Sum(b => b.AvgDailyCensus);
            var PPD = currentRevenue.RevenueAmt / (monthlyADC * currentRevenue.MonthTable.NumDaysInMonth);
            string PPDValue = string.Empty;
            if (PPD.HasValue)
            {
                PPDValue = PPD.Value.ToString("C");
            }
            return Json(new { Success = true,PPD = PPDValue });
        }

        public IActionResult UpdateGLAccount(int facilityId, int oldGLAccountIndex, int budgetId, int newGLAccountIndex)
        {
            if (BudgetContext.OtherRevenues.Count(a => a.FacilityID == facilityId
                && a.GLAccountIndex == newGLAccountIndex
                && a.BudgetID == budgetId) > 0)
            {
                return Json(new { Success = false, Message = "Census with that GLAccount already exists." });
            }

            var oldOtherRevenueInformation = BudgetContext.OtherRevenues.Where(a => a.FacilityID == facilityId &&
            a.BudgetID == budgetId
            && a.GLAccountIndex == oldGLAccountIndex).ToList();

            List<OtherRevenue> OtherRevenueList = new List<OtherRevenue>();
            for (var i = 0; i < oldOtherRevenueInformation.Count(); i++)
            {
                var oldOtherRevenue = oldOtherRevenueInformation[i];
                OtherRevenue newOtherRevenue = new OtherRevenue()
                {
                    FacilityID = facilityId,
                    BudgetID = budgetId,
                    GLAccountIndex = newGLAccountIndex,
                    RevenueAmt = oldOtherRevenue.RevenueAmt,
                    MonthID = oldOtherRevenue.MonthID

                };
                OtherRevenueList.Add(newOtherRevenue);
            }

            FacilityBudgetGL newGl = new FacilityBudgetGL()
            {
                BudgetID = budgetId,
                GLAccountIndex = newGLAccountIndex,
                FacilityID = facilityId,
                OtherRevenues = OtherRevenueList
            };

            var gl = BudgetContext.FacilityBudgetGLs.First(a => a.FacilityID == facilityId &&
                a.BudgetID == budgetId &&
                a.GLAccountIndex == oldGLAccountIndex);
            BudgetContext.FacilityBudgetGLs.Add(newGl);
            BudgetContext.FacilityBudgetGLs.Remove(gl);
            BudgetContext.SaveChanges();

            return Json(new { Success = true });
        }

    }
}