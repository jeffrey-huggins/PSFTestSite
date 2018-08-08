using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Financial.Models;
using AtriumWebApp.Web.Financial.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "POR")]
    public class PurchaseOrderController : BaseController
    {

        public PurchaseOrderController(IOptions<AppSettingsConfig> config, PurchaseOrderContext context) : base(config, context)
        {
        }


        private const string AppCode = "POR";
        private IDictionary<string, bool> _AdminAccess;

        protected new PurchaseOrderContext Context
        {
            get { return (PurchaseOrderContext)base.Context; }
        }

        public int? EditingId
        {
            get
            {
                int? id;
                Session.TryGetObject(AppCode + "EditingId", out id);
                return id;
            }
            private set
            {
                if (value == null)
                {
                    Session.Remove(AppCode + "EditingId");
                }
                else
                {
                    Session.SetItem(AppCode + "EditingId", value);
                }
            }
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

        public ViewResult Index()
        {
            LogSession(AppCode);
            SetDateRangeErrorValues();
            SetLookbackDays(HttpContext, AppCode);
            SetInitialTableRangeLookback(AppCode);
            GetCommunitiesForEmployeeDropDownWithFilter(AppCode);
            var currentCommunity = CurrentFacility[AppCode];
            var fromDate = DateTime.Parse(OccurredRangeFrom[AppCode]);
            var toDate = DateTime.Parse(OccurredRangeTo[AppCode]);
            var items = Context.PurchaseOrders.Where(po => po.CommunityId == currentCommunity
                                                        && po.EnterDate >= fromDate
                                                        && po.EnterDate <= toDate).ToList();
            var viewModel = new PurchaseOrderIndexViewModel
            {
                CanDelete = DetermineObjectAccess("0003", currentCommunity, AppCode),
                CanEdit = DetermineObjectAccess("0002", currentCommunity, AppCode),
                CanManageBudget = DetermineObjectAccess("0001", currentCommunity, AppCode),
                Items = items.Select(po =>
                {
                    var vm = new PurchaseOrderViewModel();
                    CopyModelToViewModel(po, vm);
                    return vm;
                }).ToList(),
                CurrentCommunity = currentCommunity,
                Communities = FacilityList[AppCode],
                DateRangeFrom = fromDate,
                DateRangeTo = toDate
            };
            if (EditingId.HasValue)
            {
                viewModel.PurchaseOrder = ((PartialViewResult)Edit(EditingId.Value)).Model as PurchaseOrderViewModel;
            }
            return View(viewModel);
        }



        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 600)]
        public ActionResult Create(int id)
        {
            var community = FindCommunity(id);
            if (community == null)
            {
                return NotFound("Community could not be found.");
            }
            var viewModel = new PurchaseOrderViewModel
            {
                CurrentYear = CurrentYear(),
                CommunityId = id,
                Community = community,
                EnterDate = DateTime.Now,
                Level1Approver = UserPrincipal.Name,
                Level1ApprovalDate = DateTime.Now,
                AssetClasses = Context.PurchaseOrderAssetClasses.ToList(),
                PurchaseOrderTypes = Context.PurchaseOrderTypes.ToList(),
                Vendors = Context.PurchaseOrderVendors.Where(v => v.AllowDataEntry).OrderBy(v => v.VendorName).ToList()
            };
            SetApprovalPermission(viewModel);
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Create( PurchaseOrderViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var model = new PurchaseOrder
                    {
                        EnterDate = DateTime.Now,
                        Level1ApproverName = UserPrincipal.Name,
                        Level1ApproverADName = CurrentDirectoryUserName,
                        Level1ApprovalDate = DateTime.Now
                    };

                    CopyViewModelToModel(viewModel, model);
                    ApplyApprovals(viewModel, model);
                    Context.PurchaseOrders.Add(model);
                    Context.SaveChanges();
                    EditingId = model.HasFinalApproval || model.IsDenied ? (int?)null : model.Id;
                    CopyModelToViewModel(model, viewModel);

                    return Content("{ \"Success\" : true }");
                }

                viewModel.CurrentYear = CurrentYear();
                viewModel.Community = FindCommunity(viewModel.CommunityId);
                viewModel.Vendor = FindVendor(viewModel.VendorId);
                viewModel.EnterDate = DateTime.Now;
                viewModel.Level1Approver = UserPrincipal.Name;
                viewModel.Level1ApprovalDate = DateTime.Now;
                viewModel.AssetClasses = Context.PurchaseOrderAssetClasses.ToList();
                viewModel.PurchaseOrderTypes = Context.PurchaseOrderTypes.ToList();
                viewModel.Vendors = Context.PurchaseOrderVendors.Where(v => v.AllowDataEntry).OrderBy(v => v.VendorName).ToList();

                SetApprovalPermission(viewModel);
                return PartialView(viewModel);
                //return Json(new { Success = true });
            }
            catch (Exception ex)
            {
                return Content("{ \"Success\" : false }");
            }
        }

        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 600)]
        public ActionResult View(int id)
        {
            var model = FindPurchaseOrder(id);
            if (model == null)
            {
                return NotFound("Could not find purchase order with specified Id.");
            }
            var viewModel = new PurchaseOrderViewModel
            {
                AssetClasses = Context.PurchaseOrderAssetClasses.ToList(),
                PurchaseOrderTypes = Context.PurchaseOrderTypes.ToList(),
                Vendors = Context.PurchaseOrderVendors.Where(v => v.AllowDataEntry || v.Id == model.VendorId).OrderBy(v => v.VendorName).ToList()
            };
            CopyModelToViewModel(model, viewModel);
            return PartialView(viewModel);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, Duration = 600)]
        public ActionResult Edit(int id)
        {
            var model = FindPurchaseOrder(id);
            if (model == null)
            {
                return NotFound("Could not find purchase order with specified Id.");
            }
            if (!DetermineObjectAccess("0002", model.CommunityId, AppCode))
            {
                if (model.HasFinalApproval)
                {
                    return new UnauthorizedResult();//HttpUnauthorizedResult("Cannot edit purchase order that already has final approval.");
                }
                if (model.IsDenied)
                {
                    return new UnauthorizedResult();//HttpUnauthorizedResult("Cannot edit purchase order that already is denied.");
                }
            }
            var viewModel = new PurchaseOrderViewModel
            {
                CurrentYear = CurrentYear(),
                AssetClasses = Context.PurchaseOrderAssetClasses.ToList(),
                PurchaseOrderTypes = Context.PurchaseOrderTypes.ToList(),
                Vendors = Context.PurchaseOrderVendors.Where(v => v.AllowDataEntry || v.Id == model.VendorId).OrderBy(v => v.VendorName).ToList()
            };
            CopyModelToViewModel(model, viewModel);
            SetApprovalPermission(viewModel);
            EditingId = id;
            return PartialView(viewModel);
        }

        [HttpPost]
        public ActionResult Edit(PurchaseOrderViewModel viewModel)
        {
            if(viewModel.PurchaseOrderId == 0)
            {
                return Create(viewModel);
            }
            try
            {
                var model = FindPurchaseOrder(viewModel.PurchaseOrderId);
                if (model == null)
                {
                    return NotFound("Could not find purchase order with specified Id.");
                }
                if (ModelState.IsValid)
                {
                    if (!DetermineObjectAccess("0002", model.CommunityId, AppCode))
                    {
                        if (model.HasFinalApproval)
                        {
                            return new UnauthorizedResult();//HttpUnauthorizedResult("Cannot edit purchase order that already has final approval.");
                        }
                        if (model.IsDenied)
                        {
                            return new UnauthorizedResult();//HttpUnauthorizedResult("Cannot edit purchase order that already is denied.");
                        }
                    }

                    foreach (var existingItem in model.Items.ToList())
                    {
                        Context.Entry(existingItem).State = EntityState.Deleted;
                    }

                    model.Items.Clear();
                    CopyViewModelToModel(viewModel, model);
                    ApplyApprovals(viewModel, model);
                    Context.SaveChanges();
                    EditingId = null;
                    CopyModelToViewModel(model, viewModel);

                    return Content("{ \"Success\" : true }");
                }

                viewModel.CurrentYear = CurrentYear();
                viewModel.Community = FindCommunity(viewModel.CommunityId);
                viewModel.Vendor = FindVendor(viewModel.VendorId);
                viewModel.AssetClasses = Context.PurchaseOrderAssetClasses.ToList();
                viewModel.PurchaseOrderTypes = Context.PurchaseOrderTypes.ToList();
                viewModel.Vendors = Context.PurchaseOrderVendors.Where(v => v.AllowDataEntry || v.Id == model.VendorId).OrderBy(v => v.VendorName).ToList();
                viewModel.EnterDate = model.EnterDate;
                viewModel.Level1ApprovalDate = model.Level1ApprovalDate;
                viewModel.Level1Approver = model.Level1ApproverName;
                viewModel.Level2ApprovalDate = model.Level2ApprovalDate;
                viewModel.Level2Approver = model.Level2ApproverName;
                viewModel.Level3ApprovalDate = model.Level3ApprovalDate;
                viewModel.Level3Approver = model.Level3ApproverName;
                viewModel.Level4ApprovalDate = model.Level4ApprovalDate;
                viewModel.Level4Approver = model.Level4ApproverName;
                viewModel.FinalApprovalDate = model.FinalApprovalDate;
                viewModel.FinalApprover = model.FinalApproverName;
                viewModel.DenialDate = model.DenialDate;
                viewModel.Denier = model.DenierName;
                viewModel.ActualInvoiceCost = model.ActualInvoiceCost;
                SetApprovalPermission(viewModel);

                return PartialView(viewModel);
            }
            catch (Exception ex)
            {
                return Content("{ \"Success\" : false }");
            }
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var model = FindPurchaseOrder(id);
            if (model == null)
            {
                return NotFound("Could not find purchase order with specified Id.");
            }
            if (!DetermineObjectAccess("0003", model.CommunityId, AppCode) && (model.IsLevel2Approved || model.IsLevel3Approved || model.IsLevel4Approved || model.HasFinalApproval))
            {
                return new UnauthorizedResult();//HttpUnauthorizedResult("Cannot delete a purchase order that already has approval.");
            }
            if (model.IsDenied)
            {
                return new UnauthorizedResult(); //HttpUnauthorizedResult("Cannot delete a purchase order that was denied.");
            }
            Context.PurchaseOrders.Remove(model);
            Context.SaveChanges();
            if (EditingId == id)
            {
                EditingId = null;
            }
            var viewModel = new PurchaseOrderViewModel();
            CopyModelToViewModel(model, viewModel);
            return Json(new PurchaseOrderSaveResultViewModel
            {
                Success = true,
                Result = viewModel
            });
        }

        [ResponseCache(Duration = 600)]
        public PartialViewResult CreatePurchaseOrderItem()
        {
            return PartialView(new PurchaseOrderItemViewModel()
            {
                CanDelete = DetermineObjectAccess("0003", null, AppCode),
                CanEdit = DetermineObjectAccess("0002", null, AppCode),
                CanManageBudget = DetermineObjectAccess("0001", null, AppCode)
            });
        }

        [ResponseCache(Duration = 600)]
        public PartialViewResult CreatePurchaseOrderDocument()
        {
            return PartialView(new PurchaseOrderDocumentViewModel());
        }

        public ActionResult StreamPurchaseOrderDocument(int id)
        {
            var document = Context.PurchaseOrderDocuments.SingleOrDefault(d => d.Id == id);

            if (document == null)
                ModelState.AddModelError("DocumentMissing", "Document could not be found");

            return File(document.Document, document.ContentType, document.FileName);
        }

        [HttpPost]
        public ActionResult UpdateCurrentCommunity(string currentCommunity, string returnUrl)
        {
            var facilityId = 0;
            if (!Int32.TryParse(currentCommunity, out facilityId))
            {
                return Redirect(returnUrl);
            }
            CurrentFacility[AppCode] = facilityId;
            var facility = Context.Facilities.Single(fac => fac.CommunityId == facilityId);
            CurrentFacilityName[AppCode] = facility.CommunityShortName;
            EditingId = null;
            return Redirect(returnUrl);
        }

        [HttpPost]
        public ActionResult UpdateRange(string dateRangeFrom, string dateRangeTo, string returnUrl)
        {
            EditingId = null;
            return UpdateTableRange(dateRangeFrom, dateRangeTo, returnUrl, AppCode);
        }

        private void CopyViewModelToModel(PurchaseOrderViewModel viewModel, PurchaseOrder model)
        {
            model.Id = viewModel.PurchaseOrderId;
            model.CustomPurchaseOrderNumber = viewModel.CustomPurchaseOrderNumber;
            model.Terms = viewModel.Terms;
            model.Description = viewModel.Description;
            model.Comments = viewModel.Comments;
            model.CommunityId = viewModel.CommunityId;
            model.VendorId = viewModel.VendorId;
            model.CompleteShipping = viewModel.CompleteShipping;
            model.EstimatedFreightCost = viewModel.EstimatedFreightCost;
            model.IsFreightIncluded = viewModel.IsFreightIncluded;
            model.EstimatedTaxCost = viewModel.EstimatedTaxCost;
            model.IsTaxIncluded = viewModel.IsTaxIncluded;
            model.FreightOnBoard = viewModel.FreightOnBoard;
            model.IsCapitalExpense = viewModel.IsCapitalExpense;
            model.IsEmergency = viewModel.IsEmergency;
            model.IsSpecialProject = viewModel.IsSpecialProject;
            model.IsPurchaseOrder = viewModel.IsPurchaseOrder;
            model.PurchaseDate = viewModel.PurchaseDate;
            model.PurchaseOrderAssetClassId = viewModel.PurchaseOrderAssetClassId;
            model.PurchaseOrderTypeId = viewModel.PurchaseOrderTypeId;
            model.ShipVia = viewModel.ShipVia;
            model.StartShipping = viewModel.StartShipping;
            model.ActualInvoiceCost = viewModel.ActualInvoiceCost;

            foreach (var item in viewModel.Items)
            {
                model.Items.Add(new PurchaseOrderItem
                {
                    AccountNumber = item.AccountNumber,
                    EstimatedItemCost = item.EstimatedCost,
                    ItemDescription = item.ItemDescription,
                    OrderQuantity = item.OrderQuantity,
                    StockNumber = item.StockNumber,
                    Unit = item.Unit
                });
            }

            foreach (var existingDoc in model.Documents.ToList())
            {
                if (!viewModel.Documents.Any(d => d.Id == existingDoc.Id))
                {
                    Context.Entry(existingDoc).State = EntityState.Deleted;
                    model.Documents.Remove(existingDoc);
                }
            }

            foreach (var doc in viewModel.Documents)
            {
                if (!model.Documents.Any(d => d.Id == doc.Id && d.Id != 0) && doc.Document != null)
                {
                    foreach (var file in doc.Document)
                    {
                        byte[] docData;
                        using (var reader = new BinaryReader(file.OpenReadStream()))
                        {

                            docData = reader.ReadBytes((int)file.Length);
                        }

                        model.Documents.Add(new PurchaseOrderDocument
                        {
                            FileName = Path.GetFileName(file.FileName),
                            ContentType = file.ContentType,
                            Document = docData
                        });
                    }

                }
            }
        }

        private void CopyModelToViewModel(PurchaseOrder model, PurchaseOrderViewModel viewModel)
        {
            var community = FindCommunity(model.CommunityId);
            var vendor = FindVendor(model.VendorId);
            viewModel.CanDelete = DetermineObjectAccess("0003", model.CommunityId, AppCode);
            viewModel.CanEdit = DetermineObjectAccess("0002", model.CommunityId, AppCode);
            viewModel.CanManageBudget = DetermineObjectAccess("0001", model.CommunityId, AppCode);
            viewModel.PurchaseOrderId = model.Id;
            viewModel.CustomPurchaseOrderNumber = model.CustomPurchaseOrderNumber;
            viewModel.Terms = model.Terms;
            viewModel.Description = model.Description;
            viewModel.Comments = model.Comments;
            viewModel.CommunityId = model.CommunityId;
            viewModel.Community = FindCommunity(model.CommunityId);
            viewModel.VendorId = model.VendorId;
            viewModel.Vendor = vendor;
            viewModel.CompleteShipping = model.CompleteShipping;
            viewModel.EstimatedFreightCost = model.EstimatedFreightCost;
            viewModel.IsFreightIncluded = model.IsFreightIncluded;
            viewModel.EstimatedTaxCost = model.EstimatedTaxCost;
            viewModel.IsTaxIncluded = model.IsTaxIncluded;
            viewModel.FreightOnBoard = model.FreightOnBoard;
            viewModel.IsCapitalExpense = model.IsCapitalExpense;
            viewModel.IsEmergency = model.IsEmergency;
            viewModel.IsSpecialProject = model.IsSpecialProject;
            viewModel.IsPurchaseOrder = model.IsPurchaseOrder;
            viewModel.PurchaseDate = model.PurchaseDate;
            viewModel.PurchaseOrderAssetClassId = model.PurchaseOrderAssetClassId;
            viewModel.PurchaseOrderTypeId = model.PurchaseOrderTypeId;
            viewModel.ShipVia = model.ShipVia;
            viewModel.StartShipping = model.StartShipping;
            viewModel.EnterDate = model.EnterDate;
            viewModel.Level1ApprovalDate = model.Level1ApprovalDate;
            viewModel.Level1Approver = model.Level1ApproverName;
            viewModel.Level2ApprovalDate = model.Level2ApprovalDate;
            viewModel.Level2Approver = model.Level2ApproverName;
            viewModel.Level3ApprovalDate = model.Level3ApprovalDate;
            viewModel.Level3Approver = model.Level3ApproverName;
            viewModel.Level4ApprovalDate = model.Level4ApprovalDate;
            viewModel.Level4Approver = model.Level4ApproverName;
            viewModel.FinalApprover = model.FinalApproverName;
            viewModel.FinalApprovalDate = model.FinalApprovalDate;
            viewModel.Denier = model.DenierName;
            viewModel.DenialDate = model.DenialDate;
            viewModel.DenialComments = model.DenialComments;
            viewModel.ActualInvoiceCost = model.ActualInvoiceCost;

            viewModel.Items.Clear();
            viewModel.Documents.Clear();

            foreach (var item in model.Items)
            {
                viewModel.Items.Add(new PurchaseOrderItemViewModel
                {
                    CanDelete = DetermineObjectAccess("0003", null, AppCode),
                    CanEdit = DetermineObjectAccess("0002", null, AppCode),
                    CanManageBudget = DetermineObjectAccess("0001", null, AppCode),
                    AccountNumber = item.AccountNumber,
                    EstimatedCost = item.EstimatedItemCost,
                    ItemDescription = item.ItemDescription,
                    OrderQuantity = item.OrderQuantity,
                    StockNumber = item.StockNumber,
                    Unit = item.Unit
                });
            }

            foreach (var doc in model.Documents)
            {
                viewModel.Documents.Add(new PurchaseOrderDocumentViewModel
                {
                    Id = doc.Id,
                    FileName = doc.FileName,
                    ContentType = doc.ContentType
                });
            }
        }

        private void SetApprovalPermission(PurchaseOrderViewModel viewModel)
        {
            var adName = CurrentDirectoryUserName;
            var approver = Context.PurchaseOrderApprovers.SingleOrDefault(a => a.Name == adName);

            if (approver == null)
            {
                return;
            }
            if (approver.ApprovalLevel > 1)
            {
                viewModel.IsLevel2ApprovalAllowed = true;
                if (approver.ApprovalLevel > 2)
                {
                    viewModel.IsLevel3ApprovalAllowed = true;
                    if (approver.ApprovalLevel > 3)
                    {
                        viewModel.IsLevel4ApprovalAllowed = true;
                    }
                }
            }
        }

        private void ApplyApprovals(PurchaseOrderViewModel viewModel, PurchaseOrder model)
        {
            // TODO: Validate approvals
            if (viewModel.IsLevel2MarkedForApproval)
            {
                model.Level2ApprovalDate = DateTime.Now;
                model.Level2ApproverName = UserPrincipal.Name;
                model.Level2ApproverADName = CurrentDirectoryUserName;
            }
            if (viewModel.IsLevel3MarkedForApproval)
            {
                model.Level3ApprovalDate = DateTime.Now;
                model.Level3ApproverName = UserPrincipal.Name;
                model.Level3ApproverADName = CurrentDirectoryUserName;
            }
            if (viewModel.IsLevel4MarkedForApproval)
            {
                model.Level4ApprovalDate = DateTime.Now;
                model.Level4ApproverName = UserPrincipal.Name;
                model.Level4ApproverADName = CurrentDirectoryUserName;
            }
            if (viewModel.IsMarkedForFinalApproval)
            {
                model.FinalApprovalDate = DateTime.Now;
                model.FinalApproverName = UserPrincipal.Name;
                model.FinalApproverADName = CurrentDirectoryUserName;
            }
            if (viewModel.IsMarkedForDenial)
            {
                model.DenialDate = DateTime.Now;
                model.DenierName = UserPrincipal.Name;
                model.DenierADName = CurrentDirectoryUserName;
                model.DenialComments = viewModel.DenialComments;
            }
        }

        private string CurrentDirectoryUserName
        {
            get
            {
                string userName = HttpContext.User.Identity.Name;
                if (userName == null)
                {
                    userName = WindowsIdentity.GetCurrent().Name;
                }
                return userName.Substring(userName.IndexOf(@"\") + 1);
            }
        }

        private PurchaseOrderVendor FindVendor(int? vendorId)
        {
            if (vendorId.HasValue)
            {
                return Context.PurchaseOrderVendors.Include("VendorClass").Where(v => v.Id == vendorId.Value).SingleOrDefault();
            }
            else
            {
                return null;
            }
        }

        private PurchaseOrder FindPurchaseOrder(int id)
        {
            return Context.PurchaseOrders
                .Include(p => p.Items)
                .Include(p => p.Documents)
                .SingleOrDefault(p => p.Id == id);
        }
        public ActionResult GetVendor(int vendorId)
        {
            var rec = Context.PurchaseOrderVendors.Include("VendorClass").Where(v => v.Id == vendorId).SingleOrDefault();
            if (rec == null)
            {
                return NotFound(string.Format("Vendor Id: '{0}' not found!", vendorId));
            }
            return Json(rec);
        }
    }
}