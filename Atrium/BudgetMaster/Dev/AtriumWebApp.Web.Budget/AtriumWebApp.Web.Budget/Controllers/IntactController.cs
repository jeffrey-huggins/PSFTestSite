using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models.Budget;
using AtriumWebApp.Models.Budget.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace AtriumWebApp.Web.Budget.Controllers
{
    public class IntactController : Controller
    {
        BudgetMasterIntactEntities BudgetContext { get; set; }
        public IntactController(BudgetMasterIntactEntities budgetContext)
        {
            BudgetContext = budgetContext;
        }

        public IActionResult Index()
        {
            BudgetMasterViewModel viewModel = new BudgetMasterViewModel()
            {
                Facilities = BudgetContext.Facilities.ToList()
            };

            return View(viewModel);
        }

        public IActionResult MainForm(int id)
        {
            Facility facility = BudgetContext.Facilities.Find(id);
            MainFormViewModel viewModel = new MainFormViewModel()
            {
                Facility = facility,
                Census = BudgetContext.Census1.Where(a => a.FacilityID == id).ToList(),
                OtherRevenue = BudgetContext.OtherRevenues.Where(a => a.FacilityID == id).ToList()
            };
            return PartialView(viewModel);
        }
    }
}