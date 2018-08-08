using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using AtriumWebApp.Web.PayrollTransfer.Models;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.PayrollTransfer.Models.ViewModel;
using AtriumWebApp.Models;
using System.Globalization;
using AtriumWebApp.Web.Base.Library;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.IO;
using OfficeOpenXml;
using AtriumWebApp.Web.PayrollTransfer.Manager;
using Newtonsoft.Json;

namespace AtriumWebApp.Web.PayrollTransfer.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "ADR,POR,CON,PRTR")]
    public class PayrollTransferController : BaseController
    {
        private const string AppCode = "PRTR";
        private IDictionary<string, bool> _AdminAccess;

        public PayrollTransferController(IOptions<AppSettingsConfig> config, PayrollTransferContext context) : base(config, context)
        {
        }

        protected new PayrollTransferContext Context
        {
            get { return (PayrollTransferContext)base.Context; }
        }

        public int? EditingId
        {
            get
            {
                Session.TryGetObject(AppCode + "EditingId", out int? id);
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

        //
        // GET: /PayrollTransfer/

        public ActionResult Index()
        {
            LogSession(AppCode);
            SetDateRangeErrorValues();
            SetLookbackDays(HttpContext, AppCode);
            SetInitialTableRangeLookback(AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            var currentCommunity = CurrentFacility[AppCode];
            var fromDate = DateTime.Parse(OccurredRangeFrom[AppCode]);
            var toDate = DateTime.Parse(OccurredRangeTo[AppCode]);
            var items = Context.EmployeePayrollTransfers.Where(po => po.TransferDate >= fromDate
                                                        && po.TransferDate <= toDate
                                                        && po.DeletedFlg != true).ToList();
            if (!Session.Contains("UseEditTab"))
            {
                Session.SetItem("UseEditTab", false);
            }
            if (!Session.Contains("UseEditContractorTab"))
            {
                Session.SetItem("UseEditContractorTab", false);
            }
            Session.TryGetObject("UseEditTab", out bool useEditTab);
            Session.TryGetObject("UseEditContractorTab", out bool useEditContractorTab);

            

            var viewModel = new PayrollTransferIndexViewModel
            {
                IsAdministrator = this.IsAdministrator,
                //Items = items.Select(pt =>
                //{
                //    var vm = new PayrollTransferViewModel();
                //    CopyModelToViewModel(pt, vm);
                //    return vm;
                //}).ToList(),
                CurrentCommunity = currentCommunity,
                Communities = FacilityList[AppCode],
                AppCode = AppCode,
                UseEditTab = useEditTab,
                UseContractorEditTab = useEditContractorTab,
                DateRangeFrom = fromDate,
                DateRangeTo = toDate,
                CanValidatePBJ = DetermineObjectAccess("0001", null, AppCode)
        };
            if (EditingId.HasValue)
            {
                viewModel.PayrollTransfer = ((PartialViewResult)Edit(EditingId.Value)).Model as PayrollTransferViewModel;
            }

            Session.TryGetObject(AppCode + "LookbackDate", out string lookbackDateString);
            // Load employee lists
            var lookbackDate = DateTime.Parse(lookbackDateString);
            LoadFullEmployeeList(viewModel.CurrentCommunity, lookbackDate);
            LoadActiveEmployeeList(viewModel.CurrentCommunity);

            return View(viewModel);
        }

        //
        // GET: /PayrollTransfer/Details/5

        public ActionResult Details(int id = 0)
        {
            EmployeePayrollTransfer employeepayrolltransfer = Context.EmployeePayrollTransfers.Find(id);
            if (employeepayrolltransfer == null)
            {
                return NotFound();
            }
            return View(employeepayrolltransfer);
        }

        public IActionResult CreateOrEditPayrollTransfer(int employeeId, int? payrollTransferId)
        {
            Employee employee;
            var vm = new EmployeePayrollTransfer();
            if (payrollTransferId.HasValue)
            {
                vm = Context.EmployeePayrollTransfers.Include("Employee.JobClasses.GLAccount").First(a => a.Id == payrollTransferId.Value);
                if(vm.EmployeeId != employeeId)
                {
                    employee = LoadEmployee(employeeId);
                    vm.Employee = employee;
                    vm.EmployeeId = employeeId;
                    vm.SourceLedger = employee.JobClasses.First(a => a.PrimaryFlg).GLAccount;
                    vm.SourceGeneralLedgerId = employee.JobClasses.First(a => a.PrimaryFlg).GLAccount.GeneralLedgerId;
                    vm.SourceCommunityId = employee.CommunityId;
                }
                else
                {
                    employee = vm.Employee;
                }
                
            }
            else
            {
                employee = LoadEmployee(employeeId);
                vm = new EmployeePayrollTransfer()
                {
                    Employee = employee,
                    EmployeeId = employeeId,
                    SourceLedger = employee.JobClasses.First(a => a.PrimaryFlg).GLAccount,
                    SourceGeneralLedgerId = employee.JobClasses.First(a => a.PrimaryFlg).GLAccount.GeneralLedgerId,
                    SourceCommunityId = employee.CommunityId,
                    DestinationGeneralLedgerId = employee.JobClasses.First(a => a.PrimaryFlg).GLAccount.GeneralLedgerId,
                    DestinationCommunityId = employee.CommunityId,
                    TransferDate = DateTime.Now
                };
            }
            
            ViewBag.SourceLedgers = employee.JobClasses.Where(a => a.StartDate <= vm.TransferDate && a.StopDate >= vm.TransferDate).Select(a => a.GLAccount).ToList();
            ViewBag.DestinationLedgers = Context.GLAccounts.ToList();
            ViewBag.Communities = FacilityList[AppCode];
            ViewBag.Employees = GetActiveEmployees(employee.CommunityId).ToList();
            return EditorFor(vm);
        }

        public IActionResult SavePayrollTransfer(EmployeePayrollTransfer vm)
        {
            if(vm.Id == 0)
            {
                Context.EmployeePayrollTransfers.Add(vm);
            }
            else
            {
                var current = Context.EmployeePayrollTransfers.Find(vm.Id);
                Context.Entry(current).CurrentValues.SetValues(vm);
            }
            Context.SaveChanges();
            return Json(new { success = true, data = vm.Id });
        }

        public IActionResult GetEmployeeLedgersForDate(int employeeId, DateTime transferDate)
        {
            
            var employee = Context.Employees.Include("JobClasses.GLAccount").First(a => a.EmployeeId == employeeId);
            var test = employee.JobClasses.Where(a => a.StartDate <= transferDate && a.StopDate >= transferDate).Select(a => a.GLAccount).Distinct().ToList();
            return Json(test);
            //ViewBag.SourceLedgers = employee.JobClasses.Where(a => a.StartDate <= vm.TransferDate && a.StopDate >= vm.TransferDate).Select(a => a.GLAccount).ToList();
            //ViewBag.DestinationLedgers = Context.GLAccounts.ToList();
            //ViewBag.Communities = FacilityList[AppCode];
            //ViewBag.Employees = GetActiveEmployees(employee.CommunityId).ToList();
            //return EditorFor(vm);
        }
        //
        // GET: /PayrollTransfer/Create

        public ActionResult Create()
        {
            //Set Census Date Information and Manipulate when changed
            InitializeCensusDateChangedSessionVariable();
            SetLookbackDays(HttpContext, AppCode);
            ManipulateCensusDate(AppCode);

            //Set initial date range values
            SetDateRangeErrorValues();
            SetInitialTableRange(AppCode);

            //Set Community Drop Down based on user access privileges
            GetCommunitiesDropDownWithFilter(AppCode);
            ViewData["Communities"] = new SelectList(FacilityList[AppCode], "CommunityId", "CommunityShortName");

            Community com;
            if (!Session.Contains(AppCode + "CurrentFacility"))
            {
                com = FacilityList[AppCode].First();
                CurrentFacility[AppCode] = com.CommunityId;
            }
            else
            {
                com = FacilityList[AppCode].Single(c => c.CommunityId == CurrentFacility[AppCode]);
            }


            Session.SetItem("UseEditTab", false);
            Session.TryGetObject(AppCode + "Employees", out IList<Employee> employeeList);
            PayrollTransferViewModel viewModel = new PayrollTransferViewModel
            {
                Communities = FacilityList[AppCode],
                SourceCommunityId = CurrentFacility[AppCode],
                Employees = employeeList,
                GLAccounts = GetGLAccounts(),
                TransferDate = DateTime.Today,
                IsAdministrator = IsAdministrator,
                AppCode = AppCode
            };

            Session.TryGetObject(AppCode + "LookbackDate", out string lookbackDateString);
            // Load employee lists
            var lookbackDate = DateTime.Parse(lookbackDateString);
            LoadFullEmployeeList(viewModel.SourceCommunityId, lookbackDate);
            LoadActiveEmployeeList(viewModel.SourceCommunityId);

            return View(viewModel);
        }

        public ActionResult CreateContractorEntry()
        {
            Session.SetItem("UseEditTab", false);
            Session.SetItem("UseEditContractorTab", false);
            Session.TryGetObject(AppCode + "Contractors", out IList<PTContractor> contractorList);
            ContractorPayrollEntryViewModel viewModel = new ContractorPayrollEntryViewModel
            {
                Communities = FacilityList[AppCode],
                SourceCommunityId = CurrentFacility[AppCode],
                Contractors = contractorList,
                //Employees = (IList<Employee>)Session[AppCode + "Employees"],
                GLAccounts = GetGLAccounts(),
                TransferDate = DateTime.Today,
                IsAdministrator = IsAdministrator,
                AppCode = AppCode
            };

            Session.TryGetObject(AppCode + "LookbackDate", out string lookbackDateString);
            // Load employee lists
            var lookbackDate = DateTime.Parse(lookbackDateString);
            LoadContractorList(viewModel.SourceCommunityId);

            return View(viewModel);
        }


        //
        // POST: /PayrollTransfer/Create

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Create(EmployeePayrollTransfer employeepayrolltransfer)
        {

            //var employeepayrolltransfer = (EmployeePayrollTransfer)data;
            if (ModelState.IsValid)
            {
                Context.EmployeePayrollTransfers.Add(employeepayrolltransfer);
                Context.SaveChanges();
                return Json(employeepayrolltransfer); //RedirectToAction("Index");
            }

            return View(employeepayrolltransfer);
        }
        public class PayrollPost
        {
            public int Count { get; set; }
            public string PayrollTransferItems { get; set; }//IList<EmployeePayrollTransfer> PayrollTransferItems { get; set; }
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult CreateItems([FromBody] IList<EmployeePayrollTransfer> payrollTransfer)
        {

            foreach (EmployeePayrollTransfer item in payrollTransfer)
            {
                //if new, add
                if (item.Id == 0)
                {
                    if (IsValidPayrollItem(item))
                    {
                        Context.EmployeePayrollTransfers.Add(item);
                        Context.SaveChanges();
                    }
                }
                else //else, modify
                {
                    item.LastModifiedDate = DateTime.Now;
                    Context.Entry(item).State = EntityState.Modified;
                    Context.SaveChanges();
                }
            }


            Session.SetItem("UseEditTab", true);
            Session.SetItem("UseEditContractorTab", false);

            return Json(payrollTransfer); 

        }

        private bool IsValidPayrollItem(EmployeePayrollTransfer item)
        {
            if (item.EmployeeId == 0 || item.SourceCommunityId == 0 || item.SourceGeneralLedgerId == 0
                || item.DestinationCommunityId == 0 || item.DestinationGeneralLedgerId == 0 || item.TransferDate == DateTime.MinValue)
            {
                return false;
            }
            return true;
        }


        private bool IsValidContractorPayrollItem(ContractorPayrollTransfer item)
        {
            if (item.PTContractorId == 0 || item.CommunityId == 0 || item.GeneralLedgerId == 0 || item.TransferDate == DateTime.MinValue)
            {
                return false;
            }
            return true;
        }

        public ActionResult CreateContractorItems(ContractorPayrollTransfer[] payrollTransferItems)//(string transferItems)
        {
            foreach (ContractorPayrollTransfer item in payrollTransferItems)
            {
                //if new, add
                if (item.ContractorPayrollTransferId == 0)
                {
                    if (IsValidContractorPayrollItem(item))
                    {
                        Context.ContractorPayrollTransfers.Add(item);
                        Context.SaveChanges();
                    }
                }
                else //else, modify
                {
                    item.LastModifiedDate = DateTime.Now;
                    Context.Entry(item).State = EntityState.Modified;
                    Context.SaveChanges();
                }
            }

            Context.SaveChanges();

            Session.SetItem("UseEditTab", false);
            Session.SetItem("UseEditContractorTab", true);

            return Json(payrollTransferItems); //RedirectToAction("Index");

            //return View(payrollTransferItems);
        }

        //
        // GET: /PayrollTransfer/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Session.SetItem("UseEditTab", true);
            Session.SetItem("UseEditContractorTab", false);

            EmployeePayrollTransfer employeepayrolltransfer = Context.EmployeePayrollTransfers.Find(id);
            if (employeepayrolltransfer == null)
            {
                return NotFound();
            }
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            LoadActiveEmployeeList(employeepayrolltransfer.SourceCommunityId);

            PayrollTransferViewModel viewModel = new PayrollTransferViewModel();
            CopyModelToViewModel(employeepayrolltransfer, viewModel);
            return PartialView(viewModel); //return View(employeepayrolltransfer); //return View(viewModel);
        }

        public ActionResult EditContractorEntry(int id = 0)
        {
            Session.SetItem("UseEditTab", false);
            Session.SetItem("UseEditContractorTab", true);

            ContractorPayrollTransfer contractorPayrollTransfer = Context.ContractorPayrollTransfers.Find(id);
            if (contractorPayrollTransfer == null)
            {
                return NotFound();
            }
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            LoadContractorList(contractorPayrollTransfer.CommunityId);

            ContractorPayrollEntryViewModel viewModel = new ContractorPayrollEntryViewModel();
            CopyModelToViewModel(contractorPayrollTransfer, viewModel);
            return View(viewModel); //return View(contractorPayrollTransfer); //return View(viewModel);
        }

        //
        // POST: /PayrollTransfer/Edit/5

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeePayrollTransfer employeepayrolltransfer)
        {
            if (ModelState.IsValid)
            {
                employeepayrolltransfer.LastModifiedDate = DateTime.Now;
                Context.Entry(employeepayrolltransfer).State = EntityState.Modified;
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employeepayrolltransfer);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult EditContractorEntry(ContractorPayrollTransfer contractorPayrollTransfer)
        {
            if (ModelState.IsValid)
            {
                contractorPayrollTransfer.LastModifiedDate = DateTime.Now;
                Context.Entry(contractorPayrollTransfer).State = EntityState.Modified;
                Context.SaveChanges();
                return Json(contractorPayrollTransfer);//RedirectToAction("Index");
            }
            return View(contractorPayrollTransfer);
        }

        //
        // GET: /PayrollTransfer/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Session.SetItem("UseEditTab", true);
            EmployeePayrollTransfer employeepayrolltransfer = Context.EmployeePayrollTransfers.Find(id);
            if (employeepayrolltransfer == null)
            {
                return NotFound();
            }
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            LoadActiveEmployeeList(employeepayrolltransfer.SourceCommunityId);

            PayrollTransferViewModel viewModel = new PayrollTransferViewModel();
            CopyModelToViewModel(employeepayrolltransfer, viewModel);
            return PartialView(viewModel); //View(employeepayrolltransfer);
        }

        public ActionResult DeleteContractorEntry(int id = 0)
        {
            Session.SetItem("UseEditTab", false);
            Session.SetItem("UseEditContractorTab", true);
            ContractorPayrollTransfer contractorPayrollTransfer = Context.ContractorPayrollTransfers.Find(id);
            if (contractorPayrollTransfer == null)
            {
                return NotFound();
            }
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            LoadContractorList(contractorPayrollTransfer.CommunityId);

            ContractorPayrollEntryViewModel viewModel = new ContractorPayrollEntryViewModel();
            CopyModelToViewModel(contractorPayrollTransfer, viewModel);
            return View(viewModel); //View(contractorPayrollTransfer);
        }

        //
        // POST: /PayrollTransfer/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            EmployeePayrollTransfer employeepayrolltransfer = Context.EmployeePayrollTransfers.Find(id);
            employeepayrolltransfer.DeletedFlg = true;
            employeepayrolltransfer.DeletedTS = DateTime.Now;
            employeepayrolltransfer.LastModifiedDate = DateTime.Now;
            Context.Entry(employeepayrolltransfer).State = EntityState.Modified;
            //Context.EmployeePayrollTransfers.Remove(employeepayrolltransfer);
            Context.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("DeleteContractorEntry")]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteContractorEntryConfirmed(int id)
        {
            ContractorPayrollTransfer contractorPayrollTransfer = Context.ContractorPayrollTransfers.Find(id);
            contractorPayrollTransfer.DeletedFlg = true;
            contractorPayrollTransfer.DeletedTS = DateTime.Now;
            contractorPayrollTransfer.LastModifiedDate = DateTime.Now;
            Context.Entry(contractorPayrollTransfer).State = EntityState.Modified;
            //Context.EmployeePayrollTransfers.Remove(contractorPayrollTransfer);
            Context.SaveChanges();
            return Json("Success");//RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

        #region Employee SideDDL

        [HttpPost] //Employee Side Drop Down List update 
        public ActionResult EmpSideDDL(string ActiveEmployees, string FullEmployees, int CurrentCommunity, string returnUrl, string TerminatedShown)
        {
            int employeeId;
            if (bool.Parse(TerminatedShown))
            {
                if (!Int32.TryParse(FullEmployees, out employeeId))
                {
                    return Redirect(returnUrl);
                }
            }
            else
            {
                if (!Int32.TryParse(ActiveEmployees, out employeeId))
                {
                    return Redirect(returnUrl);
                }
            }

            CurrentFacility[AppCode] = CurrentCommunity;
            Session.SetItem(AppCode + "TerminatedShown", TerminatedShown);
            Session.Remove(AppCode + "CurrentInfectionId");

            LoadEmployeeInformation(employeeId);

            return Redirect(returnUrl);
        }

        private void LoadEmployeeInformation(int employeeId)
        {
            //Get employee based on submit
            var employee = Context.Employees.Single(e => e.EmployeeId == employeeId);
            Session.SetItem(AppCode + "TerminatedShown", employeeId);
            Session.SetItem(AppCode + "CurrentEmployeeName", employee.LastName + ", " + employee.FirstName);
            Session.SetItem(AppCode + "CurrentEmployeeDateHire", employee.HireDate.ToString("d"));
            Session.SetItem(AppCode + "CurrentEmployeeDateTerm", string.Format("{0:d}", employee.TerminationDate));
        }


        #endregion

        #region Ajax Calls

        [HttpPost]
        public ActionResult Lookback(string lookbackDate, string returnUrl, string CurrentCommunity)
        {
            return SaveLookbackDate(lookbackDate, returnUrl, CurrentCommunity, AppCode);
        }

        public JsonResult GetGLAccountId(int employeeId)
        {
            var employee = Context.Employees.Where(e => e.EmployeeId == employeeId).FirstOrDefault();
            return Json(employee.JobClasses.First(a => a.PrimaryFlg).JobClassGeneralLedgerId);
        }



        public void ChangeSourceCommunity(int communityId)
        {
            Session.TryGetObject(AppCode + "LookbackDate", out string lookbackDateString);
            var lookbackDate = DateTime.Parse(lookbackDateString);
            LoadFullEmployeeList(communityId, lookbackDate);
            LoadActiveEmployeeList(communityId);

            CurrentFacility[AppCode] = communityId;
            Session.Remove(AppCode + "CurrentEmployeeId");
            Session.Remove(AppCode + "CurrentEmployeeName");
            Session.Remove(AppCode + "CurrentEmployeeDateHire");
            Session.Remove(AppCode + "CurrentEmployeeDateTerm");
            Session.Remove(AppCode + "CurrentInfectionId");
        }

        public ActionResult GetEmployeeList(int communityId, string terminationDate)
        {

            IList<Employee> employees;
            if (!string.IsNullOrEmpty(terminationDate))
            {
               CultureInfo enUS = new CultureInfo("en-US");
                DateTime lookbackDate;
                if (!DateTime.TryParseExact(terminationDate, "d", enUS, DateTimeStyles.None, out lookbackDate))
                {
                    return Json(new
                    {
                        success = false,
                        error = "Termination date is invalid."
                    });

                }
                if (lookbackDate > DateTime.Today)
                {
                    return Json(new
                    {
                        success = false,
                        error = "Termination date cannot be in the future."
                    });
                }
                employees = Context.Employees.Where(e => e.CommunityId == communityId &&
                        (e.TerminationDate == null || e.TerminationDate >= lookbackDate) &&
                        (e.JobClasses.Any(a => a.JobClassGeneralLedgerId.HasValue)))
            .OrderBy(e => e.LastName).ToList();
            }
            else
            {
                employees = Context.Employees.Where(e => e.CommunityId == communityId &&
                        e.TerminationDate == null &&
                        (e.JobClasses.Any(a => a.JobClassGeneralLedgerId.HasValue)) &&
                       (e.EmployeeStatus == "Active" || e.EmployeeStatus == "Leave of Absence"))
            .OrderBy(e => e.LastName).ToList();
            }

            return Json(employees.Select(e => new
            {
                Text = e.LastName + ", " + e.FirstName,
                Value = e.EmployeeId.ToString(),
                JobClassID = e.JobClasses.First(a => (a.PrimaryFlg && a.JobClassGeneralLedgerId.HasValue) 
                    || a.JobClassGeneralLedgerId.HasValue).JobClassGeneralLedgerId,
                HireDate = e.HireDate,
                TermDate = e.TerminationDate
            }).ToList());
        }

        public ActionResult GetActiveEmployeeList(int communityId)
        {
            //LoadActiveEmployeeList(communityId);
            var employees = GetActiveEmployees(communityId);
            return Json(employees.Select(e => new
            {
                Text = e.LastName + ", " + e.FirstName,
                Value = e.EmployeeId.ToString(),
                JobClassID = e.JobClasses.First(a => a.PrimaryFlg).JobClassGeneralLedgerId,
            }).ToList()); //Json(ViewData[AppCode + "ActiveEmployeeList"]);
        }

        public ActionResult GetContractorList(int communityId)
        {
            //LoadActiveEmployeeList(communityId);
            var contractors = GetContractors(communityId);
            return Json(GetContractorSelectList(contractors)); //Json(ViewData[AppCode + "ActiveEmployeeList"]);
        }

        public ActionResult GetCommunityPayrollTransferItems(int communityId, DateTime fromDate, DateTime toDate, int? employeeId)
        {

            CurrentFacility[AppCode] = communityId;
            if (employeeId.HasValue)
            {
                var empPayrollItems = Context.EmployeePayrollTransfers
                    .Where(p => p.EmployeeId == employeeId.Value && p.DeletedFlg != true &&
                                p.TransferDate >= fromDate.Date && p.TransferDate <= toDate.Date
                    ).ToList()
                    .Select(pt =>
                    {
                        var vm = new PayrollTransferViewModel();
                        CopyModelToViewModel(pt, vm);
                        return vm;
                    }).ToList();
                return Json(empPayrollItems.ToList());
            }

            var payrollItems = Context.EmployeePayrollTransfers
                .Where(p => p.SourceCommunityId == communityId && p.DeletedFlg != true &&
                            p.TransferDate >= fromDate.Date && p.TransferDate <= toDate.Date
                ).ToList()
                .Select(pt =>
                {
                    var vm = new PayrollTransferViewModel();
                    CopyModelToViewModel(pt, vm);
                    return vm;
                }).ToList();

            return Json(payrollItems);

        }

        public ActionResult GetCommunityContractorPayrollItems(int communityId, DateTime fromDate, DateTime toDate)
        {
            CurrentFacility[AppCode] = communityId;
            var payrollItems = Context.ContractorPayrollTransfers
                .Where(p => p.CommunityId == communityId && p.DeletedFlg != true
                            && p.TransferDate >= fromDate.Date && p.TransferDate <= toDate.Date
                ).ToList()
                .Select(pt =>
                {
                    var vm = new ContractorPayrollEntryViewModel();
                    CopyModelToViewModel(pt, vm);
                    return vm;
                }).ToList();

            return Json(payrollItems);

        }


        public void ClearTabVariables()
        {
            Session.SetItem("UseEditTab", false);
            Session.SetItem("UseEditContractorTab", false);
        }


        #endregion

        #region Private Helper Methods

        private void LoadFullEmployeeList(int communityId, DateTime lookbackDate)
        {
            var employees = Context.Employees
                .Where(e => e.CommunityId == communityId &&
                          ((e.TerminationDate != null && e.TerminationDate.Value >= lookbackDate) ||
                           (e.TerminationDate == null && (e.EmployeeStatus == "Active" || e.EmployeeStatus == "Leave of Absence"))))
                .OrderBy(e => e.LastName)
                .ThenByDescending(e => e.TerminationDate);

            ViewData[AppCode + "FullEmployeeList"] = GetEmployeeSelectList(employees);
        }

        private void LoadActiveEmployeeList(int communityId)
        {
            var employees = GetActiveEmployees(communityId);

            ViewData[AppCode + "ActiveEmployeeList"] = GetEmployeeSelectList(employees);
        }

        private void LoadContractorList(int communityId)
        {
            var contractors = GetContractors(communityId);

            ViewData[AppCode + "ContractorList"] = GetContractorSelectList(contractors);
        }

        private IEnumerable<Employee> GetActiveEmployees(int communityId)
        {
            var employees = Context.Employees
                .Where(e => e.CommunityId == communityId &&
                            e.TerminationDate == null &&
                            e.JobClasses.Count > 0 &&
                           (e.EmployeeStatus == "Active" || e.EmployeeStatus == "Leave of Absence"))
                .OrderBy(e => e.LastName);
            return employees;
        }

        private IEnumerable<PTContractor> GetContractors(int communityId)
        {
            var contractors = Context.PTContractors
                .Where(e => e.Communities.FirstOrDefault(c => c.CommunityId == communityId) != null)
                .OrderBy(e => e.LastName);
            //e.CommunityId == communityId 

            return contractors;
        }

        private SelectList GetEmployeeSelectList(IEnumerable<Employee> employees)
        {
            return new SelectList(employees.Select(e => new SelectListItem()
            {
                Text = e.LastName + ", " + e.FirstName,
                Value = e.EmployeeId.ToString()
            }), "Value", "Text");
        }

        private SelectList GetContractorSelectList(IEnumerable<PTContractor> contractors)
        {
            return new SelectList(contractors.Select(e => new SelectListItem()
            {
                Text = e.LastName + ", " + e.FirstName,
                Value = e.PTContractorId.ToString()
            }), "Value", "Text");
        }

        private void CopyModelToViewModel(EmployeePayrollTransfer model, PayrollTransferViewModel viewModel)
        {
            var sourceCommunity = FindCommunity(model.SourceCommunityId);
            var destinationCommunity = FindCommunity(model.DestinationCommunityId);
            var employee = LoadEmployee(model.EmployeeId); //FindVendor(model.VendorId);
            //var glAccounts = GetGLAccounts();
            var sourceGLAccount = GetGLAccount(model.SourceGeneralLedgerId);
            var destinationGLAccount = GetGLAccount(model.DestinationGeneralLedgerId);

            viewModel.Id = model.Id;
            viewModel.EmployeeId = model.EmployeeId;
            viewModel.Employee = employee;
            viewModel.SourceCommunityId = model.SourceCommunityId;
            viewModel.SourceCommunity = sourceCommunity;
            viewModel.SourceGeneralLedgerId = model.SourceGeneralLedgerId;
            viewModel.TransferDate = model.TransferDate;
            viewModel.DestinationGeneralLedgerId = model.DestinationGeneralLedgerId;
            viewModel.DestinationCommunityId = model.DestinationCommunityId;
            viewModel.DestinationCommunity = destinationCommunity;
            viewModel.HourCnt = model.HourCnt;
            viewModel.PayAmt = model.PayAmt;
            viewModel.PayType = model.PayType;
            viewModel.PBJOnlyFlg = model.PBJOnlyFlg;
            viewModel.SourceGLAccount = sourceGLAccount;
            viewModel.DestinationGLAccount = destinationGLAccount;


            viewModel.IsAdministrator = IsAdministrator;
            viewModel.AppCode = AppCode;
            viewModel.Communities = FacilityList[AppCode];
            viewModel.GLAccounts = GetGLAccounts();
            Session.TryGetObject(AppCode + "Employees", out IList<Employee> employeeList);
            viewModel.Employees = employeeList;

        }

        private void CopyModelToViewModel(ContractorPayrollTransfer model, ContractorPayrollEntryViewModel viewModel)
        {
            var sourceCommunity = FindCommunity(model.CommunityId);
            //var destinationCommunity = FindCommunity(model.DestinationCommunityId);
            var contractor = LoadContractor(model.PTContractorId); //FindVendor(model.VendorId);
            //var glAccounts = GetGLAccounts();
            var sourceGLAccount = GetGLAccount(model.GeneralLedgerId);
            //var destinationGLAccount = GetGLAccount(model.DestinationGeneralLedgerId);

            viewModel.Id = model.ContractorPayrollTransferId;
            viewModel.ContractorId = model.PTContractorId;
            viewModel.Contractor = contractor;
            viewModel.SourceCommunityId = model.CommunityId;
            viewModel.SourceCommunity = sourceCommunity;
            viewModel.SourceGeneralLedgerId = model.GeneralLedgerId;
            viewModel.TransferDate = model.TransferDate;
            //viewModel.DestinationGeneralLedgerId = model.DestinationGeneralLedgerId;
            //viewModel.DestinationCommunityId = model.DestinationCommunityId;
            //viewModel.DestinationCommunity = destinationCommunity;
            viewModel.HourCnt = model.HourCnt;
            viewModel.PayAmt = model.PayAmt;
            //viewModel.PayType = model.PayType;
            viewModel.PBJOnlyFlg = model.PBJOnlyFlg;
            viewModel.SourceGLAccount = sourceGLAccount;
            //viewModel.DestinationGLAccount = destinationGLAccount;


            viewModel.IsAdministrator = IsAdministrator;
            viewModel.AppCode = AppCode;
            viewModel.Communities = FacilityList[AppCode];
            viewModel.GLAccounts = GetGLAccounts();
            //viewModel.Contractors = (IList<PTContractor>)Session[AppCode + "Employees"];
        }

        private Employee LoadEmployee(int employeeId)
        {
            return Context.Employees.Include("JobClasses.GLAccount").First(a => a.EmployeeId == employeeId);
        }

        private PTContractor LoadContractor(int contractorId)
        {
            return Context.PTContractors.Where(e => e.PTContractorId == contractorId).FirstOrDefault();
        }

        private IList<GeneralLedgerAccount> GetGLAccounts()
        {
            return Context.GLAccounts.ToList().OrderBy(a => a.AccountName).ThenBy(a => a.AccountNbr).ToList();
            //return items.Select(gl =>
            //{
            //    var vm = new GLAccountVewModel
            //    {
            //        GLAccountId = gl.GeneralLedgerId,
            //        AccountNameNbr = gl.AccountNbr + " - " + gl.AccountName,
            //        AtriumPayerGroupCode = gl.AtriumPayerGroupCode,
            //        GLAccountTypeID = gl.GLAccountTypeID
            //    };
            //    return vm;
            //}).OrderBy(i => i.AccountNameNbr).ToList();
        }

        private GeneralLedgerAccount GetGLAccount(int generalLedgerId)
        {
            //var items = Context.GLAccounts.Where(a => a.GeneralLedgerId == generalLedgerId).ToList().Select(gl =>
            //{
            //    var vm = new GLAccountVewModel
            //    {
            //        GLAccountId = gl.GeneralLedgerId,
            //        AccountNameNbr = gl.AccountNbr + " - " + gl.AccountName,
            //        AtriumPayerGroupCode = gl.AtriumPayerGroupCode,
            //        GLAccountTypeID = gl.GLAccountTypeID
            //    };
            //    return vm;
            //}).ToList();


            return Context.GLAccounts.Find(generalLedgerId);
        }

        #endregion
        [HttpGet]
        public IActionResult ValidateDocuments()
        {
            return View(new PBJValidationViewModel());
        }

        [HttpPost]
        public IActionResult ValidateDocuments([FromForm] PBJValidationViewModel vm)
        {
            var manager = new PBJValidationManager(vm);
            return PartialView("ValidateResults", manager.errorList);
        }

    }
}