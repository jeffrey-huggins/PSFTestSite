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
    public class ExpensesController : Controller
    {

        BudgetMasterIntactEntities BudgetContext { get; set; }
        public ExpensesController(BudgetMasterIntactEntities budgetContext)
        {
            BudgetContext = budgetContext;
        }

        public IActionResult Index(int facilityId, int budgetId)
        {
            Facility facility = BudgetContext.Facilities.Find(facilityId);
            MainFormViewModel viewModel = new MainFormViewModel()
            {
                OtherExpenses = BudgetContext.OtherExpensesGLAccounts.Where(a => a.FacilityID == facilityId).ToList(),
                FacilityBudget = facility.FacilityBudgets.FirstOrDefault(a => a.BudgetID == budgetId)
            };

            return PartialView(viewModel);
        }

        public IActionResult Display(int facilityId, int GLAccountIndex, int budgetId)
        {
            var expensesList = BudgetContext.OtherExpenses.Where(a => a.FacilityID == facilityId
                && a.GLAccountIndex == GLAccountIndex
                && a.BudgetID == budgetId);

            var account = BudgetContext.OtherExpensesGLAccounts.First(a => a.BudgetID == budgetId
                && a.FacilityID == facilityId
                && a.GLAccountIndex == GLAccountIndex);

            OtherExpensesViewModel viewModel = new OtherExpensesViewModel()
            {
                OtherExpense = expensesList.ToList(),
                GeneralLedgers = BudgetContext.GeneralLedgers.Where(a => a.GLAccountTypeID == 4).ToList(),
                Months = BudgetContext.MonthTables.ToList(),
                Account = account
            };

            return PartialView(viewModel);
        }

        public IActionResult CalcValues(int facilityId)
        {
            OtherExpensesCalcViewModel viewModel = new OtherExpensesCalcViewModel()
            {
                Calculations = BudgetContext.OtherExpensesFacilityGLCalcValues.Where(a => a.FacilityID == facilityId).ToList(),
                MasterCalculations = BudgetContext.OtherExpensesGLCalcValueMasters.ToList(),
                Months = BudgetContext.MonthTables.ToList()
            };
            return PartialView(viewModel);
        }

        public IActionResult CreateRow(OtherExpens expense)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Please double check all the fields and try again." });
            }
            if (BudgetContext.OtherExpenses.Count(a => a.FacilityID == expense.FacilityID
                && a.MonthID == expense.MonthID
                && a.GLAccountIndex == expense.GLAccountIndex) > 0)
            {
                return Json(new { Success = false, Message = "An expense of that type and month already exists for this facility." });
            }

            BudgetContext.OtherExpenses.Add(expense);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult Create(int facilityId, int GLAccountIndex, int budgetId, short recordTypeId)
        {
            if (BudgetContext.OtherExpenses.Count(a => a.FacilityID == facilityId &&
                a.GLAccountIndex == GLAccountIndex &&
                a.BudgetID == budgetId) > 0)
            {
                return Json(new { Success = false, Message = "Expense with that GLAccount already exists." });
            }

            var expenseList = new List<OtherExpens>();
            for (var i = 1; i <= 12; i++)
            {
                OtherExpens newExpense = new OtherExpens()
                {
                    FacilityID = facilityId,
                    BudgetID = budgetId,
                    GLAccountIndex = GLAccountIndex,
                    MonthID = i,
                };
                expenseList.Add(newExpense);
            }

            OtherExpensesGLAccount account = new OtherExpensesGLAccount()
            {
                BudgetID = budgetId,
                GLAccountIndex = GLAccountIndex,
                FacilityID = facilityId,
                OtherExpenses = expenseList,
                RecordTypeCd = recordTypeId
            };

            BudgetContext.OtherExpensesGLAccounts.Add(account);
            BudgetContext.SaveChanges();

            return Json(new { Success = true });
        }

        public IActionResult CreateCalcRow(OtherExpensesFacilityGLCalcValue value)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Please double check all the fields and try again." });
            }

            var current = BudgetContext.OtherExpensesFacilityGLCalcValues.FirstOrDefault(a => a.FacilityID == value.FacilityID
                && a.MonthID == value.MonthID && a.CalcValueID == value.CalcValueID);
            if (current != null)
            {
                return Json(new { Successs = false, Message = "A Calc value for that month already exists." });
            }
            BudgetContext.OtherExpensesFacilityGLCalcValues.Add(value);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult PropogateAmmount(int? facilityId, int? GLAccountIndex, int? budgetId, int? recordTypeId, decimal? ammount)
        {
            var results = BudgetContext.OtherExpenses_PropagateAmount(facilityId, budgetId, GLAccountIndex, recordTypeId, ammount);
            return Json(new { Success = true, data = results });
        }

        public IActionResult UpdateRecordType(int facilityId, int GLAccountIndex, int budgetId, short recordTypeId)
        {
            var account = BudgetContext.OtherExpensesGLAccounts.First(a => a.BudgetID == budgetId
                && a.FacilityID == facilityId
                && a.GLAccountIndex == GLAccountIndex);
            account.RecordTypeCd = recordTypeId;
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult UpdateRowMonth(OtherExpens expense, int oldMonth)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Please double check all the fields and try again." });
            }

            if(BudgetContext.OtherExpenses.Count(a => a.BudgetID == expense.BudgetID
                && a.FacilityID == expense.FacilityID
                && a.GLAccountIndex == expense.GLAccountIndex
                && a.MonthID == expense.MonthID) > 0)
            {
                return Json(new { Success = false, Message = "An Expense GL Account for that month already exists." });
            }

            var current = BudgetContext.OtherExpenses.FirstOrDefault(a => a.BudgetID == expense.BudgetID
                && a.FacilityID == expense.FacilityID
                && a.GLAccountIndex == expense.GLAccountIndex
                && a.MonthID == oldMonth);
            if (current == null)
            {
                return Json(new { Successs = false, Message = "Unable to find GL Expense Account to update." });
            }

            BudgetContext.OtherExpenses.Remove(current);
            BudgetContext.OtherExpenses.Add(expense);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult UpdateRow(OtherExpens expense)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Please double check all the fields and try again." });
            }
            var current = BudgetContext.OtherExpenses.FirstOrDefault(a => a.BudgetID == expense.BudgetID
                && a.FacilityID == expense.FacilityID 
                && a.GLAccountIndex == expense.GLAccountIndex 
                && a.MonthID == expense.MonthID);
            if (current == null)
            {
                return Json(new { Successs = false, Message = "Unable to find GL Expense Account to update." });
            }
            BudgetContext.Entry(current).CurrentValues.SetValues(expense);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });

        }

        public IActionResult ChangeGLAccount(int facilityId, int oldGLAccountIndex, int budgetId, int newGLAccountIndex)
        {
            var oldExpenseAccount = BudgetContext.OtherExpensesGLAccounts.FirstOrDefault(a => a.FacilityID == facilityId
                && a.GLAccountIndex == oldGLAccountIndex && a.BudgetID == budgetId);
            if (oldExpenseAccount == null)
            {
                return Json(new { Successs = false, Message = "Unable to find current GL Expense Account." });
            }
            var newExpenseAccount = BudgetContext.OtherExpensesGLAccounts.FirstOrDefault(a => a.FacilityID == facilityId
                && a.GLAccountIndex == newGLAccountIndex
                && a.BudgetID == budgetId);
            if (newExpenseAccount != null)
            {
                return Json(new { Success = false, Message = "A GL Expense Account already exists for that GL account." });
            }

            List<OtherExpens> otherExpenses = new List<OtherExpens>();

            foreach (var record in oldExpenseAccount.OtherExpenses)
            {
                var newRecord = new OtherExpens()
                {
                    BudgetID = record.BudgetID,
                    DollarsPPD = record.DollarsPPD,
                    FacilityID = record.FacilityID,
                    ExpenseAmount = record.ExpenseAmount,
                    GLAccountIndex = newGLAccountIndex,
                    MonthID = record.MonthID
                };
                otherExpenses.Add(newRecord);
            }

            newExpenseAccount = new OtherExpensesGLAccount()
            {
                BudgetID = budgetId,
                GLAccountIndex = newGLAccountIndex,
                FacilityID = facilityId,
                RecordTypeCd = oldExpenseAccount.RecordTypeCd,
                OtherExpenses = otherExpenses
            };

            BudgetContext.OtherExpensesGLAccounts.Remove(oldExpenseAccount);
            BudgetContext.OtherExpensesGLAccounts.Add(newExpenseAccount);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult UpdateCalcRow(OtherExpensesFacilityGLCalcValue calc)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Please double check all the fields and try again." });
            }
            var current = BudgetContext.OtherExpensesFacilityGLCalcValues.FirstOrDefault(a => a.FacilityID == calc.FacilityID
                && a.MonthID == calc.MonthID && a.CalcValueID == calc.CalcValueID);
            if(current == null)
            {
                return Json(new { Successs = false, Message = "Unable to find Calc Value to update." });
            }
            BudgetContext.Entry(current).CurrentValues.SetValues(calc);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult UpdateCalcRowKey(OtherExpensesFacilityGLCalcValue calc, int oldCalcValueID, int oldMonthId)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Please double check all the fields and try again." });
            }
            if (BudgetContext.OtherExpensesFacilityGLCalcValues.Count(a => a.FacilityID == calc.FacilityID
                && a.MonthID == oldMonthId
                && a.CalcValueID == oldCalcValueID) > 0)
            {
                return Json(new { Success = false, Message = "A Calc Value for that month and Calc ID already exists." });
            }
            var current = BudgetContext.OtherExpensesFacilityGLCalcValues.FirstOrDefault(a => a.FacilityID == calc.FacilityID
                && a.MonthID == calc.MonthID 
                && a.CalcValueID == calc.CalcValueID);
            if (current == null)
            {
                return Json(new { Successs = false, Message = "Unable to find Calc Value to update." });
            }
            BudgetContext.OtherExpensesFacilityGLCalcValues.Remove(current);
            BudgetContext.OtherExpensesFacilityGLCalcValues.Add(calc);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult RemoveGLAccount(int facilityId, int GLAccountIndex, int budgetId)
        {
            var oldExpenseAccount = BudgetContext.OtherExpensesGLAccounts.FirstOrDefault(a => a.FacilityID == facilityId
                && a.GLAccountIndex == GLAccountIndex && a.BudgetID == budgetId);
            if (oldExpenseAccount == null)
            {
                return Json(new { Successs = false, Message = "Unable to find current GL Expense Account." });
            }

            BudgetContext.OtherExpensesGLAccounts.Remove(oldExpenseAccount);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult RemoveRow(int facilityId, int GLAccountIndex, int budgetId, int monthId)
        {
            var oldExpenseAccount = BudgetContext.OtherExpenses.FirstOrDefault(a => a.FacilityID == facilityId
                && a.GLAccountIndex == GLAccountIndex && a.BudgetID == budgetId && a.MonthID == monthId);
            if (oldExpenseAccount == null)
            {
                return Json(new { Successs = false, Message = "Unable to find current GL Expense Account for that month." });
            }

            BudgetContext.OtherExpenses.Remove(oldExpenseAccount);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult RemoveCalcRow(int facilityId, int monthId, int calcValueId)
        {
            var oldCalcRow = BudgetContext.OtherExpensesFacilityGLCalcValues.FirstOrDefault(a => a.FacilityID == facilityId
                && a.MonthID == monthId && a.CalcValueID == calcValueId);

            if(oldCalcRow == null)
            {
                return Json(new { Success = false, Message = "Unable to find the Facility Calc" });
            }
            BudgetContext.OtherExpensesFacilityGLCalcValues.Remove(oldCalcRow);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }
    }
}