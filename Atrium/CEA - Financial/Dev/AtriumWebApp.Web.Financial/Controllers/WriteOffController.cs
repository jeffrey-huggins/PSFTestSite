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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Financial.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "WO")]
    public class WriteOffController : BaseController
    {
        public WriteOffController(IOptions<AppSettingsConfig> config, WriteOffContext context) : base(config, context)
        {
        }

        private const string AppCode = "WO";

        private IDictionary<string, bool> _AdminAccess;
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

        protected new WriteOffContext Context
        {
            get { return (WriteOffContext)base.Context; }
        }

        //private WriteOffContext Context = new WriteOffContext();

        //
        // GET: /WriteOff/

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
            IList<Community> communities;
            FacilityList.TryGet(AppCode, out communities);
            var idList = communities.Select(a => a.CommunityId);
            var items = Context.WriteOffs.Where(a => idList.Any( b => b == a.CommunityId)).ToList(); //.Where(wo => wo.TransferDate >= fromDate
            //&& wo.TransferDate <= toDate
            //&& wo.DeletedFlg != true).ToList();
            
            var viewModel = new WriteOffIndexViewModel
            {
                //IsAdministrator = this.IsAdministrator,
                Items = items.Select(wo =>
                {
                    var vm = new WriteOffViewModel();
                    CopyModelToViewModel(wo, vm);
                    return vm;
                }).ToList(),
                CurrentCommunity = currentCommunity,
                Communities = FacilityList[AppCode],
                AppCode = AppCode,
                DateRangeFrom = fromDate,
                DateRangeTo = toDate
            };
            string lookbackString;
            Session.TryGetObject(AppCode + "LookbackDate", out lookbackString);
            var lookbackDate = DateTime.Parse(lookbackString);
            return View(viewModel);
        }

        private void CopyModelToViewModel(WriteOff wo, WriteOffViewModel vm)
        {
            vm.WriteOffId = wo.WriteOffId;
            vm.CommunityId = wo.CommunityId;
            vm.PatientId = wo.PatientId;
            vm.PayerId = wo.PayerId;
            vm.WriteOffAmt = wo.WriteOffAmt;
            vm.DOSYear = wo.DOSYear;
            vm.DOSMonth = wo.DOSMonth;
            vm.OurFaultFlg = wo.OurFaultFlg;
            vm.Notes = wo.Notes;

            vm.Communities = FacilityList[AppCode];
            vm.CommunityName = vm.Communities.FirstOrDefault(c => c.CommunityId == vm.CommunityId).CommunityShortName;
            vm.PatientName = GetPatientName(vm.PatientId);
            vm.PayerName = Context.CommunityPayers.First(p => p.PayerId == vm.PayerId).PayerName;
        }

        private string GetPatientName(int patientId)
        {
            var patient = Context.Residents.First(r => r.PatientId == patientId);
            return patient.LastName + ", " + patient.FirstName;
        }

        //
        // GET: /WriteOff/Details/5

        public ActionResult Details(int id = 0)
        {
            WriteOff writeoff = Context.WriteOffs.Find(id);
            if (writeoff == null)
            {
                return NotFound();
            }
            return View(writeoff);
        }

        //
        // GET: /WriteOff/Create

        public ActionResult Create()
        {
            WriteOffViewModel viewModel = new WriteOffViewModel
            {
                Communities = FacilityList[AppCode],
                //Patients = Context.Residents.ToList()
            };
            ViewData["ResidentList"] = new SelectList(new List<SelectListItem>());
            ViewData["PayerList"] = new SelectList(new List<SelectListItem>());
            ViewData["Years"] = GetYearsDropdownList();
            return View(viewModel);
        }

        private SelectList GetYearsDropdownList()
        {
            List<SelectListItem> years = new List<SelectListItem>();
            for (var x = DateTime.Today.Year; x >= DateTime.Today.Year - 3; x--)
            {
                var year = new SelectListItem() { Text = x.ToString(), Value = x.ToString() };
                years.Add(year);
            }
            return new SelectList(years, "Value", "Text");
        }

        //
        // POST: /WriteOff/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(WriteOff writeoff)
        {
            if (ModelState.IsValid)
            {
                Context.WriteOffs.Add(writeoff);
                Context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(writeoff);
        }

        //
        // GET: /WriteOff/Edit/5

        public ActionResult Edit(int id = 0)
        {
            WriteOff writeoff = Context.WriteOffs.Find(id);
            if (writeoff == null)
            {
                return NotFound();
            }
            WriteOffViewModel viewModel = new WriteOffViewModel();
            CopyModelToViewModel(writeoff, viewModel);


            ViewData["ResidentList"] = new SelectList(Context.Residents.Where(r => r.CommunityId == viewModel.CommunityId).ToList().Select(i => new
            SelectListItem()
            {
                Text = i.LastName + ", " + i.FirstName,
                Value = i.PatientId.ToString()
            }).ToList().OrderBy(x => x.Text), "Value", "Text");
            ViewData["PayerList"] = new SelectList(GetCommunityPayersForCommunity(viewModel.CommunityId)
                .Select(p => new SelectListItem()
                {
                    Text = p.PayerName,
                    Value = p.PayerId.ToString()
                }), "Value", "Text");
            ViewData["Years"] = GetYearsDropdownList();

            return View(viewModel);
        }

        //
        // POST: /WriteOff/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(WriteOff writeoff)
        {
            if (ModelState.IsValid)
            {
                Context.Entry(writeoff).State = EntityState.Modified;
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(writeoff);
        }

        //
        // GET: /WriteOff/Delete/5

        public ActionResult Delete(int id = 0)
        {
            WriteOff writeoff = Context.WriteOffs.Find(id);
            if (writeoff == null)
            {
                return NotFound();
            }
            return View(writeoff);
        }

        //
        // POST: /WriteOff/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            WriteOff writeoff = Context.WriteOffs.Find(id);
            Context.WriteOffs.Remove(writeoff);
            Context.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult GetCommunityPatientsAndPayers(int communityId)
        {
            var patients = Context.Residents.Where(r => r.CommunityId == communityId).ToList().Select(i => new {
                Text = i.LastName + ", " + i.FirstName,
                Value = i.PatientId
            }).ToList().OrderBy(x => x.Text);

            var communityPayers = GetCommunityPayersForCommunity(communityId);

            var payerList = communityPayers.Select(i => new
            {
                Text = i.PayerName,
                Value = i.PayerId
            }).OrderBy(x => x.Text);

            return Json(new
            {
                Patients = patients,
                Payers = payerList
            });
        }

        private IEnumerable<CommunityPayers> GetCommunityPayersForCommunity(int communityId)
        {
            var payers = Context.CommunityPayerRelationships.Where(r => r.CommunityId == communityId).ToList();

            return payers.Select(p => Context.CommunityPayers.FirstOrDefault(cp => cp.PayerId == p.PayerId));

        }
    }
}