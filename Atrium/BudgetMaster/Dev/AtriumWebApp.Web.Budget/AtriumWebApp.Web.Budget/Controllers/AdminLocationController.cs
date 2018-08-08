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
    public class AdminLocationController : Controller
    {
        BudgetMasterIntactEntities BudgetContext { get; set; }
        delegate string ProcessTask(string id, List<int> facilities);
        ExportIntactToExcel Export = new ExportIntactToExcel();

        public AdminLocationController(BudgetMasterIntactEntities budgetContext)
        {
            BudgetContext = budgetContext;
        }

        public IActionResult States()
        {
            return PartialView(BudgetContext.States.ToList());
        }

        public IActionResult EditState(int StateId)
        {
            var states = new SelectList(BudgetContext.States, "StateID", "StateCd");
            ViewBag.States = states;
            if (StateId == 0)
            {
                return PartialView(new State());
            }
            var state = BudgetContext.States.Find(StateId);
            if (state == null)
            {
                return NotFound();
            }
            return PartialView(state);
        }

        public IActionResult SaveState(State state)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid for this state." });
            }
            string type = "existing";
            var oldState = BudgetContext.States.Find(state.StateID);
            if (oldState == null)
            {

                oldState = state;
                BudgetContext.States.Add(oldState);
                type = "new";
            }
            else
            {
                BudgetContext.Entry(oldState).CurrentValues.SetValues(state);
            }

            BudgetContext.SaveChanges();
            return Json(new { Success = true, Type = type });
        }

        public IActionResult DeleteState(int StateId)
        {
            var oldState = BudgetContext.States.Find(StateId);
            if (oldState == null)
                return Json(new { Success = false, Message = "Unable to locate a state with id " + StateId });
            BudgetContext.States.Remove(oldState);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });

        }

        public IActionResult Regions()
        {
            return PartialView(BudgetContext.Regions.ToList());
        }

        public IActionResult EditRegion(int RegionId)
        {
            var Regions = new SelectList(BudgetContext.Regions, "RegionID", "RegionNm");
            ViewBag.Regions = Regions;
            if (RegionId == 0)
            {
                return PartialView(new Region());
            }
            var Region = BudgetContext.Regions.Find(RegionId);
            if (Region == null)
            {
                return NotFound();
            }
            return PartialView(Region);
        }

        public IActionResult SaveRegion(Region Region)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Success = false, Message = "Data may be missing or invalid for this Region." });
            }
            string type = "existing";
            var oldRegion = BudgetContext.Regions.Find(Region.RegionID);
            if (oldRegion == null)
            {
                oldRegion = Region;
                BudgetContext.Regions.Add(oldRegion);
                type = "new";
            }
            else
            {
                BudgetContext.Entry(oldRegion).CurrentValues.SetValues(Region);
            }

            BudgetContext.SaveChanges();
            return Json(new { Success = true, Type = type });
        }

        public IActionResult DeleteRegion(int RegionId)
        {
            var oldRegion = BudgetContext.Regions.Find(RegionId);
            if (oldRegion == null)
                return Json(new { Success = false, Message = "Unable to locate a Region with id " + RegionId });
            BudgetContext.Regions.Remove(oldRegion);
            BudgetContext.SaveChanges();
            return Json(new { Success = true });

        }

    }
}