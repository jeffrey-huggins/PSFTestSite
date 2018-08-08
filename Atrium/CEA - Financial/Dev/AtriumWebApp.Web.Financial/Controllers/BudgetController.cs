using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Financial.Models;
using AtriumWebApp.Web.Financial.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "POR")]
    public class BudgetController : BaseController
    {
        private const string AppCode = "POR";
        private IDictionary<string, bool> _AdminAccess;

        public int? EditingId
        {
            get
            {
                int? id;
                Session.TryGetObject(AppCode + "EditingId", out id);
                return id;
            }
            private set { Session.SetItem(AppCode + "EditingId", value); }
        }

        private bool IsAdministrator
        {
            get
            {
                if (_AdminAccess == null)
                {
                    _AdminAccess = DetermineAdminAccess(PrincipalContext, UserPrincipal);
                }
                bool isAdministrator;
                if (_AdminAccess.TryGetValue(AppCode, out isAdministrator))
                {
                    return isAdministrator;
                }
                return false;
            }
        }
        public string CurrentYear()
        {
            return DateTime.Now.Year.ToString();
        }
        public BudgetController(IOptions<AppSettingsConfig> config, PurchaseOrderContext context) : base(config, context)
        {
        }
        protected new PurchaseOrderContext Context
        {
            get { return (PurchaseOrderContext)base.Context; }
        }

        public ActionResult Index()
        {
            LogSession(AppCode);

            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 

            SetDateRangeErrorValues();
            SetLookbackDays(HttpContext, AppCode);
            SetInitialTableRangeLookback(AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            var currentCommunity = CurrentFacility[AppCode];
            var fromDate = DateTime.Parse(OccurredRangeFrom[AppCode]);
            var toDate = DateTime.Parse(OccurredRangeTo[AppCode]);

            var viewModel = new BudgetViewModel
            {
                CurrentCommunity = currentCommunity,
                Communities = FacilityList[AppCode],
                DateRangeFrom = fromDate,
                DateRangeTo = toDate,
                CommunityId = currentCommunity,
                Community = FindCommunity(currentCommunity),
                BudgetYear = CurrentYear()
            };

            InitializeBudgetData(viewModel);
            viewModel.EmergencyBudgetAmt = GetOrInitializeEmergencyBudget(CurrentYear()).BudgetAmt;

            viewModel.BudgetQuarters = GetBudgetQuarters();
            viewModel.BudgetYears = GetBudgetYears();

            return View(viewModel);
        }

        private void InitializeBudgetData(BudgetViewModel viewModel)
        {
            var budget = GetOrInitializeBudget(viewModel.CommunityId, viewModel.BudgetYear);

            viewModel.BedCount = budget.BedCount;
            viewModel.AmtPerBed = budget.AmtPerBed;
        }

        private CapitalBudget GetOrInitializeBudget(int communityId, string budgetYear)
        {
            var budget = Context.CapitalBudgets.FirstOrDefault(c => c.BudgetYear == budgetYear && c.CommunityId == communityId);
            if (budget == null)
            {
                budget = new CapitalBudget
                {
                    CommunityId = communityId,
                    BudgetYear = budgetYear,
                    BedCount = 0,
                    AmtPerBed = 0,
                };
                Context.CapitalBudgets.Add(budget);
                Context.SaveChanges();
            }
            return budget;
        }

        private EmergencyBudget GetOrInitializeEmergencyBudget(string budgetYear)
        {
            var budget = Context.EmergencyBudgets.FirstOrDefault(c => c.BudgetYear == budgetYear);
            if (budget == null)
            {
                budget = new EmergencyBudget
                {
                    BudgetYear = budgetYear,
                    BudgetAmt = 0
                };
                Context.EmergencyBudgets.Add(budget);
                Context.SaveChanges();
            }
            return budget;
        }

        //
        // GET: /Budget/Details/5

        public ActionResult Details(int id = 0)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            BudgetItem budgetitem = Context.CapitalBudgetItems.Find(id);
            if (budgetitem == null)
            {
                return NotFound();
            }
            return View(budgetitem);
        }

        //
        // GET: /Budget/Create

        public ActionResult Create()
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            return View();
        }

        //
        // POST: /Budget/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BudgetItem budgetitem)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            if (ModelState.IsValid)
            {
                Context.CapitalBudgetItems.Add(budgetitem);
                Context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(budgetitem);
        }

        //
        // GET: /Budget/Edit/5

        public ActionResult Edit(int id = 0)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            BudgetItem budgetitem = Context.CapitalBudgetItems.Find(id);
            if (budgetitem == null)
            {
                return NotFound();
            }
            return View(budgetitem);
        }

        //
        // POST: /Budget/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BudgetItem budgetitem)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            if (ModelState.IsValid)
            {
                Context.Entry(budgetitem).State = EntityState.Modified;
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(budgetitem);
        }

        //
        // GET: /Budget/Delete/5

        public ActionResult Delete(int id = 0)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            BudgetItem budgetitem = Context.CapitalBudgetItems.Find(id);
            if (budgetitem == null)
            {
                return NotFound();
            }
            return View(budgetitem);
        }

        //
        // POST: /Budget/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; ion; 
            BudgetItem budgetitem = Context.CapitalBudgetItems.Find(id);
            Context.CapitalBudgetItems.Remove(budgetitem);
            Context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }



        #region Manage Budget

        public ActionResult GetBudgetData(BudgetFilterModel filterModel)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction;  
            var viewModel = new BudgetViewModel();
            var budgetModel = GetOrInitializeBudget(filterModel.CommunityId, filterModel.BudgetYear);
            viewModel.BedCount = budgetModel.BedCount;
            viewModel.AmtPerBed = budgetModel.AmtPerBed;
            viewModel.EmergencyBudgetAmt = GetOrInitializeEmergencyBudget(filterModel.BudgetYear).BudgetAmt;
            return Json((viewModel));
        }

        public ActionResult GetBudgetItemData(BudgetFilterModel filterModel)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            var viewModel = new BudgetViewModel();
            viewModel.BudgetItems = Context.CapitalBudgetItems.Where(i => i.CommunityId == filterModel.CommunityId && i.BudgetYear == filterModel.BudgetYear).ToList();
            return Json(viewModel);
        }

        public ActionResult SaveBudgetData(CapitalBudget budget)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            if (budget == null)
            {
                throw new ArgumentNullException("budget");
            }

            var original = Context.CapitalBudgets.FirstOrDefault(b => b.CommunityId == budget.CommunityId && b.BudgetYear == budget.BudgetYear);
            original.BedCount = budget.BedCount;
            original.AmtPerBed = budget.AmtPerBed;
            //original.EmergencyBudgetAmt = budget.EmergencyBudgetAmt;

            Context.SaveChanges();

            return Json(new { Success = true });
        }

        public ActionResult SaveEmergencyBudget(EmergencyBudget emergencyBudget)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            if (emergencyBudget == null)
            {
                throw new ArgumentNullException("emergencyBudget");
            }

            var original = Context.EmergencyBudgets.FirstOrDefault(b => b.BudgetYear == emergencyBudget.BudgetYear);
            original.BudgetAmt = emergencyBudget.BudgetAmt;

            Context.SaveChanges();

            return Json(new { Success = true });
        }

        public ActionResult GetBudgetItem(int id)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            var item = Context.CapitalBudgetItems.FirstOrDefault(i => i.Id == id);//viewmodel.BudgetItems.FirstOrDefault(i => i.Id == id);
            return Json((item));
        }

        public ActionResult AddBudgetItem(BudgetItem item)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            //Validate item
            if (item.Comments == null)
            {
                item.Comments = "";
            }
            //Add item and save
            Context.CapitalBudgetItems.Add(item);
            Context.SaveChanges();

            return Json(new { Success = true });
        }

        public ActionResult UpdateBudgetItem(BudgetItem updatedItem)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            var originalItem = Context.CapitalBudgetItems.FirstOrDefault(i => i.Id == updatedItem.Id);

            originalItem.BudgetYear = updatedItem.BudgetYear;
            originalItem.BudgetQtr = updatedItem.BudgetQtr;
            originalItem.Description = updatedItem.Description;
            originalItem.Comments = updatedItem.Comments ?? "";
            originalItem.BudgetAmt = updatedItem.BudgetAmt;
            originalItem.IsSpecialProject = updatedItem.IsSpecialProject;

            Context.SaveChanges();

            return Json(new { Success = true });
        }

        public ActionResult DeleteBudgetItem(int id)
        {
            if (!DetermineObjectAccess("0001", null, AppCode))
                return RedirectToAction("Index", "Home");
            //var redirectToAction = DetermineWebpageAccess(AppCode);
            //if (redirectToAction != null)
            //    return redirectToAction; 
            var item = Context.CapitalBudgetItems.FirstOrDefault(i => i.Id == id);
            Context.CapitalBudgetItems.Remove(item);

            Context.SaveChanges();

            return Json(new { Success = true });
        }

        private Dictionary<string, string> GetBudgetYears()
        {
            Dictionary<string, string> yearDictionary = new Dictionary<string, string>();
            var years = Context.GetBudgetYears(DateTime.Now.Year, DateTime.Now.AddYears(1).Year);

            foreach (var year in years)
            {
                yearDictionary.Add(year, year);
            }
            return yearDictionary;
        }

        private Dictionary<string, string> GetBudgetQuarters()
        {
            Dictionary<string, string> quarterDictionary = new Dictionary<string, string>();
            var quarters = Context.GetBudgetQuarters();
            foreach (var quarter in quarters)
            {
                quarterDictionary.Add("Q" + quarter, quarter);
            }
            return quarterDictionary;
        }

        //private BudgetViewModel LoadBudgetViewModel(Community community)
        //{
        //    var viewModel = new BudgetViewModel
        //    {
        //        CommunityId = community.CommunityId,
        //        Community = community,
        //        BudgetYear = CurrentYear()
        //    };

        //    viewModel.BudgetItems = Context.CapitalBudgetItems.Where(i => i.CommunityId == viewModel.CommunityId && i.BudgetYear == viewModel.BudgetYear).ToList();
        //    return viewModel;
        //}

        #endregion

    }
}