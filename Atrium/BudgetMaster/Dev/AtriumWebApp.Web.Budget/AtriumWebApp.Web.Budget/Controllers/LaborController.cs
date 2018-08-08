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
    public class LaborController : Controller
    {
        BudgetMasterIntactEntities BudgetContext { get; set; }

        public LaborController( BudgetMasterIntactEntities budgetContext)
        {
            BudgetContext = budgetContext;
        }

        public IActionResult Index(int facilityId, int budgetId)
        {
            var laborBudget = BudgetContext.PayrollGLAccounts.Where(a => a.FacilityID == facilityId && a.BudgetID == budgetId)
                .OrderBy(a => a.GeneralLedger.GLAccountNb).ToList();
            PayrollViewModel viewModel = new PayrollViewModel()
            {
                PayrollAccounts = laborBudget,
                GLAccounts = BudgetContext.GeneralLedgers.Where(a => a.GLAccountTypeID == 3).OrderBy(a => a.GLAccountNb).ToList(),
                Months = BudgetContext.MonthTables.ToList(),
                PayerGroups = BudgetContext.PayGrps.ToList()
            };
            return PartialView(viewModel);
        }

        public IActionResult Create(PayrollGLAccount account)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Please double check all the fields and try again." });
            }
            BudgetContext.PayrollGLAccounts.Add(account);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult Edit(PayrollGLAccount account)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Please double check all the fields and try again." });
            }
            PayrollGLAccount current = BudgetContext.PayrollGLAccounts.First(a => a.FacilityID == account.FacilityID &&
                a.BudgetID == account.BudgetID &&
                a.GLAccountIndex == account.GLAccountIndex);
            BudgetContext.Entry(current).CurrentValues.SetValues(account);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult UpdateGLAccount(PayrollGLAccount account, int oldGlID)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Please double check all the fields and try again." });
            }
            int payrollCount = BudgetContext.PayrollGLAccounts.Count(a => a.FacilityID == account.FacilityID &&
                a.BudgetID == account.BudgetID &&
                a.GLAccountIndex == account.GLAccountIndex);
            if(payrollCount > 0)
            {
                return Json(new { Success = false, Message = "Payroll with that GLAccount already exists." });
            }
            PayrollGLAccount current = BudgetContext.PayrollGLAccounts.First(a => a.FacilityID == account.FacilityID &&
                a.BudgetID == account.BudgetID &&
                a.GLAccountIndex == oldGlID);
            BudgetContext.PayrollGLAccounts.Remove(current);
            BudgetContext.PayrollGLAccounts.Add(account);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult ChangeGLAccountDesc(int glAccount,string description)
        {
            GeneralLedger ledger = BudgetContext.GeneralLedgers.Find(glAccount);
            ledger.GLAccountNm = description;
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult Delete(int facilityID, int budgetID, int glAccount)
        {
            var account = BudgetContext.PayrollGLAccounts.First(a => a.FacilityID == facilityID &&
                a.BudgetID == budgetID &&
                a.GLAccountIndex == glAccount);
            BudgetContext.PayrollGLAccounts.Remove(account);
            BudgetContext.SaveChanges();
            

            return Json(new { Success = true });
            
            
        }
    }
}