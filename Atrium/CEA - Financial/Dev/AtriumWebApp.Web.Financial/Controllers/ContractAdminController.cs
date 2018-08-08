using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO.Compression;
using System.IO;
using System.CodeDom.Compiler;
using AtriumWebApp.Web.Financial.Library;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "CON", Admin = true)]
    public class ContractAdminController : BaseController
    {
        delegate string ProcessTask(string id, List<int> facilityIds);
        ExportContractDocuments Export = new ExportContractDocuments();

        private const string AppCode = "CON";
        public ContractAdminController(IOptions<AppSettingsConfig> config, ContractManagementContext context) : base(config, context)
        {
        }

        protected new ContractManagementContext Context
        {
            get { return (ContractManagementContext)base.Context; }
        }

        public ActionResult Index()
        {
            var addressTypes = Context.AddressTypes.ToList();
            var contactTypes = Context.ContactTypes.ToList();
            var categories = Context.ContractCategories.Include(a => a.Children).ToList();
            var selectedCategoryId = categories.First().Id;
            var subcategories = Context.ContractSubCategories.Where(x => x.ContractCategoryId == selectedCategoryId).ToList();
            var paymentTerms = Context.ContractPaymentTerms.ToList();
            var renewalTypes = Context.ContractRenewals.ToList();
            var terminationNotices = Context.ContractTerminationNotices.ToList();

            var viewModel = new ContractAdminViewModel()
            {
                AddressTypes = addressTypes,
                ContactTypes = contactTypes,
                Categories = categories,
                SubCategories = subcategories,
                SelectedCategoryId = categories.First().Id,
                PaymentTerms = paymentTerms,
                RenewalTypes = renewalTypes,
                TerminationNotices = terminationNotices,
                CurrentRenewalType = new ContractRenewal(),
                CurrentTerminationNotice = new ContractTerminationNotice(),
                CurrentPaymentTerm = new ContractPaymentTerm(),
                Facilities = FilterCommunitiesByADGroups(AppCode)

            };



            return View(viewModel);
        }

        [HttpPost]
        public ActionResult CreateAddressType(ProviderAddressType newAddressType)
        {
            Context.AddressTypes.Add(newAddressType);
            Context.SaveChanges();
            return Json(new { Success = true, data = newAddressType });
        }

        [HttpPost]
        public ActionResult CreateContactType(ProviderContactType newContactType)
        {
            Context.ContactTypes.Add(newContactType);
            Context.SaveChanges();
            return Json(new { Success = true, data = newContactType });
        }

        [HttpPost]
        public ActionResult CreateCategory(ContractCategory newContractCategory)
        {
            Context.ContractCategories.Add(newContractCategory);
            Context.SaveChanges();
            return Json(new { Success = true, data = newContractCategory });
        }

        [HttpPost]
        public ActionResult CreateSubCategory(ContractSubCategory newContractSubcategory)
        {

            var newSubCategory = Context.ContractSubCategories.Add(newContractSubcategory);
            Context.SaveChanges();

            return Json(new { Success = true, data = newSubCategory });
        }

        [HttpPost]
        public ActionResult CreatePaymentTerm(ContractPaymentTerm newPaymentTerm)
        {
            var highestSortOrder = Context.ContractPaymentTerms.Max(x => x.SortOrder);

            //new ContractPaymentTerm { Name = newType,
            //    IsDataEntry = true,
            //    IsReportable = true,
            //    SortOrder = highestSortOrder + 1 }

            Context.ContractPaymentTerms.Add(newPaymentTerm);
            Context.SaveChanges();
            return Json(new { Success = true, data = newPaymentTerm });
        }

        [HttpPost]
        public ActionResult CreateRenewalType(ContractRenewal newContractRenewal)
        {
            var highestSortOrder = Context.ContractRenewals.Max(x => x.SortOrder) + 1;
            newContractRenewal.SortOrder = highestSortOrder;
            Context.ContractRenewals.Add(newContractRenewal);
            Context.SaveChanges();
            return Json(new { Success = true, data = newContractRenewal });
        }

        [HttpPost]
        public ActionResult CreateTerminationNotice(ContractTerminationNotice newTermNotice)
        {
            var highestSortOrder = Context.ContractTerminationNotices.Max(x => x.SortOrder);

            Context.ContractTerminationNotices.Add(newTermNotice);
            Context.SaveChanges();
            return Json(new { Success = true, data = newTermNotice });
        }

        [HttpPost]
        public JsonResult EditAddressType(int id, string name)
        {
            var record = Context.AddressTypes.Find(id);
            record.Name = name;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult EditContactType(int id, string name)
        {
            var record = Context.ContactTypes.Find(id);
            record.Name = name;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult EditCategory(int id, string name)
        {
            var record = Context.ContractCategories.Find(id);
            record.Name = name;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult EditSubCategory(int id, string name)
        {
            var record = Context.ContractSubCategories.Find(id);
            record.Name = name;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult EditPaymentTerm(int id, string name, bool isDataEntry, bool isReportable, int? sortOrder)
        {
            var record = Context.ContractPaymentTerms.Find(id);
            if (sortOrder.HasValue)
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = string.Empty;
                }
                record.SortOrder = sortOrder.Value;
                record.Name = name;
            }

            record.IsDataEntry = isDataEntry;
            record.IsReportable = isReportable;


            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult EditRenewalType(int id, string name, bool isDataEntry, bool isReportable, int? sortOrder)
        {
            var record = Context.ContractRenewals.Find(id);
            if (sortOrder.HasValue)
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = string.Empty;
                }
                record.SortOrder = sortOrder.Value;
                record.Name = name;
            }

            record.IsDataEntry = isDataEntry;
            record.IsReportable = isReportable;


            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult EditTerminationNotice(int id, string name, bool isDataEntry, bool isReportable, int? sortOrder)
        {
            var record = Context.ContractTerminationNotices.Find(id);
            if (sortOrder.HasValue)
            {
                if (string.IsNullOrEmpty(name))
                {
                    name = string.Empty;
                }
                record.SortOrder = sortOrder.Value;
                record.Name = name;
            }

            record.IsDataEntry = isDataEntry;
            record.IsReportable = isReportable;


            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult DeleteAddressType(int id)
        {
            Context.AddressTypes.Remove(Context.AddressTypes.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult DeleteContactType(int id)
        {
            Context.ContactTypes.Remove(Context.ContactTypes.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult DeleteCategory(int id)
        {
            Context.ContractCategories.Remove(Context.ContractCategories.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult DeleteSubCategory(int id)
        {
            Context.ContractSubCategories.Remove(Context.ContractSubCategories.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult DeletePaymentTerm(int id)
        {
            Context.ContractPaymentTerms.Remove(Context.ContractPaymentTerms.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult DeleteRenewalType(int id)
        {
            Context.ContractRenewals.Remove(Context.ContractRenewals.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult DeleteTerminationNotice(int id)
        {
            Context.ContractTerminationNotices.Remove(Context.ContractTerminationNotices.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public ActionResult GetSubCategories(int selectedCategoryId)
        {
            var subcategories = (from sub in Context.ContractSubCategories
                                 where sub.ContractCategoryId == selectedCategoryId
                                 select new
                                 {
                                     Id = sub.Id,
                                     Name = sub.Name
                                 }).ToArray();

            return Json(subcategories);
        }

        [HttpGet]
        public IActionResult GetProcessingFiles()
        {
            var statuses = ExportContractDocuments.ProcessStatus.ToList();
            return PartialView(statuses);
        }

        [HttpGet]
        public IActionResult GetContracts(int[] facilityIds)
        {
            List<int> selectedFacilities = facilityIds.ToList();
            if (facilityIds.Length == 0)
            {
                return Json(new { success = false, id = "" });
            }
            string id = Guid.NewGuid().ToString();
            Export.Add(id);
            ProcessTask processTask = new ProcessTask(Export.CreateFiles);
            processTask.BeginInvoke(id, selectedFacilities, EndExport, processTask);
            return Json(new { success = true, id });

        }

        public void EndExport(IAsyncResult result)
        {
            ProcessTask processTask = (ProcessTask)result.AsyncState;
            processTask.EndInvoke(result);
        }

        public IActionResult GetExportFileStatus(string id)
        {
            ExportContractDocumentsStatus status = Export.GetStatus(id);
            return Json(new { status = status.ProgressStatus, status.Progress });
        }

        public IActionResult GetExportFile(string id)
        {
            var result = Export.GetResult(id);
            if (result != null)
            {
                Export.Remove(id);
                if (!System.IO.File.Exists(result))
                {
                    return View("There was an issue downloading the file.  It may have been downloaded by another person.  Please try again.");
                }
                FileStream file = new FileStream(result, FileMode.Open);

                return File(file, "application/octet-stream", "Contracts.zip");

            }
            return View("There was an issue downloading the file.  It may have been downloaded by another person.  Please try again.");
        }

    }
}