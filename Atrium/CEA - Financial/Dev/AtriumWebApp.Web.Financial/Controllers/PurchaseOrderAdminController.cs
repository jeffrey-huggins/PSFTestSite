using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Financial.Models;
using AtriumWebApp.Web.Financial.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "POR", Admin = true)]
    public class PurchaseOrderAdminController : BaseAdminController
    {
        public PurchaseOrderAdminController(IOptions<AppSettingsConfig> config, PurchaseOrderContext context) : base(config, context)
        {
        }
        private const string AppCode = "POR";
        private IDictionary<string, bool> _AdminAccess;

        protected new PurchaseOrderContext Context
        {
            get { return (PurchaseOrderContext)base.Context; }
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

        public ActionResult Index()
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            return View(CreateViewModel());
        }

        [HttpPost]
        public ActionResult CreateType(PurchaseOrderType newType)
        {
            newType.AllowDataEntry = true;
            Context.PurchaseOrderTypes.Add(newType);
            Context.SaveChanges();
            return Json(new { Success = true, data = newType });
        }

        [HttpPost]
        public JsonResult DeleteType(int id)
        {
            Context.PurchaseOrderTypes.Remove(Context.PurchaseOrderTypes.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult EditType(int id, string name, int order, bool allowDataEntry)
        {
            var record = Context.PurchaseOrderTypes.Find(id);
            record.Name = name;
            record.SortOrder = order;
            record.AllowDataEntry = allowDataEntry;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public ActionResult CreateVendorClass(PurchaseOrderVendorClass newVendorClass)
        {
            newVendorClass.AllowDataEntry = true;
            Context.PurchaseOrderVendorClasses.Add(newVendorClass);
            Context.SaveChanges();
            return Json(new { Success = true, data = newVendorClass });
        }

        [HttpPost]
        public JsonResult DeleteVendorClass(int id)
        {
            Context.PurchaseOrderVendorClasses.Remove(Context.PurchaseOrderVendorClasses.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult EditVendorClass(int id, string name, int order, bool allowDataEntry)
        {
            var record = Context.PurchaseOrderVendorClasses.Find(id);
            record.Name = name;
            record.SortOrder = order;
            record.AllowDataEntry = allowDataEntry;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public ActionResult CreateAssetClass(PurchaseOrderAssetClass newAssetClass)
        {
            newAssetClass.AllowDataEntry = true;
            Context.PurchaseOrderAssetClasses.Add(newAssetClass);
            Context.SaveChanges();
            return Json(new { Success = true, data = newAssetClass });
        }

        [HttpPost]
        public JsonResult DeleteAssetClass(int id)
        {
            Context.PurchaseOrderAssetClasses.Remove(Context.PurchaseOrderAssetClasses.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult EditAssetClass(int id, string name, int order, bool allowDataEntry)
        {
            var record = Context.PurchaseOrderAssetClasses.Find(id);
            record.Name = name;
            record.SortOrder = order;
            record.AllowDataEntry = allowDataEntry;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public ActionResult CreateApprover(PurchaseOrderApprover newApprover)
        {
            Context.PurchaseOrderApprovers.Add(newApprover);
            Context.SaveChanges();
            return Json(new { Success = true, data = newApprover });
        }

        [HttpPost]
        public JsonResult DeleteApprover(int id)
        {
            Context.PurchaseOrderApprovers.Remove(Context.PurchaseOrderApprovers.Find(id));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        [HttpPost]
        public JsonResult EditApprover(int id, string name, int approvalLevel)
        {
            var record = Context.PurchaseOrderApprovers.Find(id);
            record.Name = name;
            record.ApprovalLevel = approvalLevel;
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        private PurchaseOrderAdminViewModel CreateViewModel()
        {
            return new PurchaseOrderAdminViewModel
            {
                AdminViewModel = CreateAdminViewModel(AppCode),
                PurcahseOrderTypes = Context.PurchaseOrderTypes.ToList(),
                Vendors = Context.PurchaseOrderVendors.ToList(),
                VendorClasses = Context.PurchaseOrderVendorClasses.ToList(),
                AssetClasses = Context.PurchaseOrderAssetClasses.ToList(),
                Approvers = Context.PurchaseOrderApprovers.ToList(),
                StateCodes = Context.States.OrderBy(s => s.StateCd).Select(s => s.StateCd).ToList()
            };
        }

        #region Vendor Admin

        public ActionResult Vendors()
        {
            var redirectToAction = DetermineWebpageAccess(AppCode);
            if (redirectToAction != null)
            {
                return redirectToAction;
            }
            return View(CreateViewModel());
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

        [HttpPost]
        public ActionResult SaveVendor(PurchaseOrderVendor model)
        {
            if (model.Id == 0) // New Vendor (Form POST)
            {
                Context.PurchaseOrderVendors.Add(model);
                Context.SaveChanges();
                model.VendorClass = Context.PurchaseOrderVendorClasses.Find(model.VendorClassId);
                return Json(model);
            }
            else // Existing Vendor (AJAX)
            {
                var rec = Context.PurchaseOrderVendors.Find(model.Id);
                if (rec == null)
                {
                    throw new ArgumentException(string.Format("Vendor Id: '{0}' not found!", model.Id));
                }

                rec.VendorName = model.VendorName;
                rec.VendorClassId = model.VendorClassId;
                rec.VendorClass = Context.PurchaseOrderVendorClasses.Find(rec.VendorClassId);
                rec.CorpVendorId = model.CorpVendorId;
                rec.Address1 = model.Address1;
                rec.Address2 = model.Address2;
                rec.City = model.City;
                rec.StateCd = model.StateCd;
                rec.ZipCode = model.ZipCode;
                rec.Phone = model.Phone;
                rec.Fax = model.Fax;
                rec.Email = model.Email;
                rec.SortOrder = model.SortOrder;
                rec.AllowDataEntry = model.AllowDataEntry;

                Context.SaveChanges();

                return Json(rec);
            }
        }

        [HttpPost]
        public ActionResult UpdateVendorDataEntryFlag(int vendorId, bool allowDataEntry)
        {
            var rec = Context.PurchaseOrderVendors.Find(vendorId);
            if (rec == null)
            {
                return NotFound(string.Format("Vendor Id: '{0}' not found!", vendorId));
            }

            rec.AllowDataEntry = allowDataEntry;
            Context.SaveChanges();

            return new StatusCodeResult(200);
        }

        #endregion
    }
}