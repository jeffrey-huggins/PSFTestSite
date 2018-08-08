using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.Budget;
using AtriumWebApp.Models.Budget.ViewModels;
using AtriumWebApp.Web.Budget.Library;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using OfficeOpenXml;

namespace AtriumWebApp.Web.Budget.Controllers
{
    public class AdminController : Controller
    {
        BudgetMasterIntactEntities BudgetContext { get; set; }


        public AdminController(BudgetMasterIntactEntities budgetContext)
        {
            BudgetContext = budgetContext;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        public IActionResult Facilities()
        {
            FacilityViewModel viewModel = new FacilityViewModel()
            {
                Facilities = BudgetContext.Facilities.ToList(),
                States = BudgetContext.States.ToList(),
                Regions = BudgetContext.Regions.ToList()
            };
            return PartialView(viewModel);
        }

        public IActionResult DeleteFacility(int id)
        {
            BudgetContext.Facility_DeleteByFacilityID(id);
            return Json(new { Success = true });
        }

        public IActionResult AddFacility(Facility newFacility, int CopyFrom)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid for this facility" });
            }
            var existingFacility = BudgetContext.Facilities.FirstOrDefault(a => a.FacilityNb == newFacility.FacilityNb);
            if (existingFacility != null)
            {
                return Json(new { Success = false, Message = "Facility number is already in use by " + existingFacility.FacilityNm + "." });
            }
            BudgetContext.Facility_Clone(newFacility.FacilityNb, newFacility.FacilityShortNm,
                            newFacility.FacilityNm, newFacility.RegionID, newFacility.StateID, newFacility.WageIncreasePercentage,
                            newFacility.MgtFeeCalcPercent, newFacility.RestrictFromDwFeed, CopyFrom);

            return Json(new { Success = true });

        }

        public IActionResult UpdateFacility(Facility facility)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid for this facility" });
            }
            var currentFacility = BudgetContext.Facilities.Find(facility.FacilityID);
            BudgetContext.Entry(currentFacility).CurrentValues.SetValues(facility);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult GeneralLedger()
        {
            GeneralLedgerAdminViewModel viewModel = new GeneralLedgerAdminViewModel()
            {
                GeneralLedgers = BudgetContext.GeneralLedgers.ToList(),
                AccountTypes = BudgetContext.GLAccountTypes.ToList(),
                LaborGLGroups = BudgetContext.LaborExpenseGLGrps.ToList(),
                PayGrpCds = BudgetContext.PayGrps.ToList()
            };
            return PartialView(viewModel);
        }

        public IActionResult EditGL(int? GLId)
        {
            var laborGLGroups = new SelectList(BudgetContext.LaborExpenseGLGrps, "LaborExpenseGLGrpID", "LaborExpenseGLGrpNm");
            var accountTypes = new SelectList(BudgetContext.GLAccountTypes, "GLAccountTypeID", "GLAccountTypeNm");
            var payGrpCds = new SelectList(BudgetContext.PayGrps, "PayGrpCd", "PayGrpNm");

            ViewBag.LaborGlGroups = laborGLGroups;
            ViewBag.AccountTypes = accountTypes;
            ViewBag.PayGrpCds = payGrpCds;

            if (!GLId.HasValue)
            {
                return PartialView(new GeneralLedger());
            }
            var gl = BudgetContext.GeneralLedgers.Find(GLId.Value);
            if (gl == null)
            {
                return NotFound();
            }
            return PartialView(gl);
        }

        public IActionResult SaveGL(GeneralLedger ledger)
        {

            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid for this general ledger." });
            }
            if (ledger.GLAccountIndex == 0)
            {
                BudgetContext.GeneralLedgers.Add(ledger);

            }
            else
            {
                var oldGl = BudgetContext.GeneralLedgers.FirstOrDefault(a => a.GLAccountIndex == ledger.GLAccountIndex);
                if (oldGl == null)
                {
                    return Json(new { Success = false, Message = "Unable to Update General Ledger, invalid Index" });
                }
                BudgetContext.Entry(oldGl).CurrentValues.SetValues(ledger);
            }
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult DeleteGL(int id)
        {
            var oldGl = BudgetContext.GeneralLedgers.Find(id);
            if (oldGl == null)
            {
                return Json(new { Success = false, Message = "Unable to Update General Ledger, invalid Index" });
            }
            BudgetContext.GeneralLedgers.Remove(oldGl);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });
        }

        public IActionResult PayerTypes()
        {
            return PartialView(BudgetContext.PayTypes.ToList());
        }

        public IActionResult EditPT(string PTId)
        {
            var payGrpCds = new SelectList(BudgetContext.PayGrps, "PayGrpCd", "PayGrpNm");
            ViewBag.PayGrpCds = payGrpCds;

            if (string.IsNullOrEmpty(PTId))
            {
                return PartialView(new PayType());
            }
            var pt = BudgetContext.PayTypes.FirstOrDefault(a => a.PayTypeCd == PTId);
            if (pt == null)
            {
                return NotFound();
            }
            return PartialView(pt);
        }

        public IActionResult SavePT(PayType payType)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid for this general ledger." });
            }
            string type = "existing";
            var oldPT = BudgetContext.PayTypes.FirstOrDefault(a => a.PayTypeCd == payType.PayTypeCd);
            if (oldPT == null)
            {
                oldPT = payType;
                BudgetContext.PayTypes.Add(oldPT);
                type = "new";
            }
            else
            {
                BudgetContext.Entry(oldPT).CurrentValues.SetValues(payType);
            }

            BudgetContext.SaveChanges();
            return Json(new { Success = true, Type = type });
        }

        public IActionResult DeletePT(string PTId)
        {
            var oldPT = BudgetContext.PayTypes.FirstOrDefault(a => a.PayTypeCd == PTId);
            if (oldPT == null)
                return Json(new { Success = false, Message = "Unable to locate a Pay Type with code " + PTId });
            BudgetContext.PayTypes.Remove(oldPT);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });

        }


    }
}