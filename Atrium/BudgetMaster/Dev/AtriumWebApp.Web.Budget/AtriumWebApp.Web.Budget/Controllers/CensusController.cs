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
    public class CensusController : Controller
    {
        BudgetMasterIntactEntities BudgetContext { get; set; }
        public CensusController(BudgetMasterIntactEntities budgetContext)
            {
            BudgetContext = budgetContext;
        }

        public IActionResult Index(int facilityId, int budgetId)
        {
            Facility facility = BudgetContext.Facilities.Find(facilityId);

            MainFormViewModel viewModel = new MainFormViewModel()
            {
                Census = BudgetContext.Census1.Where(a => a.FacilityID == facilityId).ToList(),
                FacilityBudget = facility.FacilityBudgets.FirstOrDefault(a => a.BudgetID == budgetId)
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
            if (BudgetContext.Census1.Count(a => a.FacilityID == facilityId
                && a.GLAccountIndex == GLAccountIndex
                && a.BudgetID == budgetId) > 0)
            {
                return Json(new { Success = false, Message = "Census with that GLAccount already exists." });
            }

            var censusList = new List<Census>();
            for (var i = 1; i <= 12; i++)
            {
                Census newCensus = new Census()
                {
                    FacilityID = facilityId,
                    BudgetID = budgetId,
                    GLAccountIndex = GLAccountIndex,
                    MonthID = i,
                    AvgDailyCensus = 0,
                    AvgDailyRate = 0
                };
                censusList.Add(newCensus);

            }
            FacilityBudgetGL gl = new FacilityBudgetGL()
            {
                BudgetID = budgetId,
                GLAccountIndex = GLAccountIndex,
                FacilityID = facilityId,
                Census = censusList
            };

            BudgetContext.FacilityBudgetGLs.Add(gl);
            BudgetContext.SaveChanges();

            return Json(new { Success = true });
        }

        public IActionResult Display(int facilityId, int GLAccountIndex, int budgetId)
        {
            var censusList = BudgetContext.Census1.Where(a => a.FacilityID == facilityId
                && a.GLAccountIndex == GLAccountIndex
                && a.BudgetID == budgetId);
            var totalADC = BudgetContext.Database.SqlQuery<decimal>("SELECT dbo.fnGetTotalADC({0})", facilityId).FirstOrDefault();

            CensusViewModel viewModel = new CensusViewModel()
            {
                TotalADC = totalADC,
                MonthlyCensus = censusList.ToList(),
                GeneralLedgers = BudgetContext.GeneralLedgers.Where(a => a.GLAccountTypeID == 1).ToList(),
                Months = BudgetContext.MonthTables.ToList()

            };

            return PartialView(viewModel);
        }

        public IActionResult Edit(int facilityId, int oldGLAccountIndex, int budgetId, int monthId, decimal? dailyCensus, decimal? dailyRate)
        {
            Census currentCensus = BudgetContext.Census1.FirstOrDefault(a => a.FacilityID == facilityId &&
            a.GLAccountIndex == oldGLAccountIndex &&
            a.BudgetID == budgetId &&
            a.MonthID == monthId);

            currentCensus.AvgDailyCensus = dailyCensus;
            currentCensus.AvgDailyRate = dailyRate;

            BudgetContext.SaveChanges();
            string fixedAmmountValue = string.Empty;
            string monthlyADCValue = string.Empty;

            var fixedAmmount = dailyCensus * dailyRate * currentCensus.MonthTable.NumDaysInMonth;
            var monthlyADC = currentCensus.MonthTable.Census.Where(a => a.FacilityID == currentCensus.FacilityID 
                && a.MonthID == currentCensus.MonthID).Sum(b => b.AvgDailyCensus);
            if (fixedAmmount.HasValue)
            {
                fixedAmmountValue = fixedAmmount.Value.ToString("C");
            }
            if (monthlyADC.HasValue)
            {
                monthlyADCValue = monthlyADC.Value.ToString("N2");
            }
            return Json(new { Success = true, fixedAmmount = fixedAmmountValue, monthlyADC = monthlyADCValue });
        }

        public IActionResult UpdateGLAccount(int facilityId, int oldGLAccountIndex, int budgetId, int newGLAccountIndex)
        {
            if (BudgetContext.Census1.Count(a => a.FacilityID == facilityId
                && a.GLAccountIndex == newGLAccountIndex
                && a.BudgetID == budgetId) > 0)
            {
                return Json(new { Success = false, Message = "Census with that GLAccount already exists." });
            }

            var oldCensusInformation = BudgetContext.Census1.Where(a => a.FacilityID == facilityId &&
            a.BudgetID == budgetId
            && a.GLAccountIndex == oldGLAccountIndex).ToList();

            List<Census> censusList = new List<Census>();
            for (var i = 0; i < oldCensusInformation.Count(); i++)
            {
                var oldCensus = oldCensusInformation[i];
                Census newCensus = new Census()
                {
                    FacilityID = facilityId,
                    BudgetID = budgetId,
                    GLAccountIndex = newGLAccountIndex,
                    AvgDailyCensus = oldCensus.AvgDailyCensus,
                    AvgDailyRate = oldCensus.AvgDailyRate,
                    MonthID = oldCensus.MonthID
                    
                };
                censusList.Add(newCensus);
            }

            FacilityBudgetGL newGl = new FacilityBudgetGL()
            {
                BudgetID = budgetId,
                GLAccountIndex = newGLAccountIndex,
                FacilityID = facilityId,
                Census = censusList
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