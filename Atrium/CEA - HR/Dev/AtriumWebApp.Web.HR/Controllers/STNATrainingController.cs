using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AtriumWebApp.Web.HR.Models;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Models;
using AtriumWebApp.Web.HR.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AtriumWebApp.Web.HR.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "STNA")]
    public class STNATrainingController : BaseController
    {
        //private STNATrainingContext Context = new STNATrainingContext();
        protected const string AppCode = "STNA";

        public STNATrainingController(IOptions<AppSettingsConfig> config, STNATrainingContext context) : base(config, context)
        {
        }

        protected new STNATrainingContext Context
        {
            get { return (STNATrainingContext)base.Context; }
        }

        //
        // GET: /STNATraining/

        public ActionResult Index()
        {
            // Record user access
            LogSession(AppCode);

            // Set initial Lookback Date
            InitializeLookbackDate();

            //Set initial date range values
            SetDateRangeErrorValues();
            SetInitialTableRange(AppCode);

            // Set Community Drop Down based on user access privileges
            if (FacilityList[AppCode] == null)
            {
                LoadFacilityList();
            }

            // Default selected facility to first in list
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

            if (!Session.Contains(AppCode + "CurrentTab"))
            {
                Session.SetItem(AppCode + "CurrentTab", 0);
            }
            Session.TryGetObject(AppCode + "LookbackDate", out string lookbackDateString);
            // Load Training Facility List
            var lookbackDate = DateTime.Parse(lookbackDateString);
            LoadTrainingFacilityList(com.CommunityId);

            // Build page ViewModel
            var viewModel = new STNATrainingIndexViewModel();
            Session.TryGetObject(AppCode + "CurrentTab", out int currentTab);
            viewModel.CurrentTab = currentTab;

            if (Session.Contains(AppCode + "CurrentTrainingFacilityId"))
            {
                Session.TryGetObject(AppCode + "CurrentTrainingFacilityId", out int currentTrainingFacilityId);

                //var actionItems = Context.STNATrainingActionItems.ToList();

                //viewModel.TrainingFacilityInteractions = Context.STNATrainingFacilityInteractions
                //    .Where(i => i.STNATrainingFacilityId == currentTrainingFacilityId)
                //    .ToList().Select(i => new STNATrainingFacilityInteractionViewModel {
                //        STNATrainingFacilityInteractionId = i.STNATrainingFacilityInteractionId
                //        , STNATrainingFacilityId = i.STNATrainingFacilityId
                //        , InteractionDate = i.InteractionDate
                //        , STNATrainingActionItemId = i.STNATrainingActionItemId
                //        , STNATrainingActionItemDesc = 
                //        actionItems.First(d => d.STNATrainingActionItemId == i.STNATrainingActionItemId).ActionItemDesc
                //    });

                viewModel.TrainingClasses = Context.STNATrainingClasses
                    .Where(c => c.STNATrainingFacilityId == currentTrainingFacilityId)
                    .ToList();

                viewModel.TrainingFacilityNotes = Context.STNATrainingFacilityNotes
                    .Where(c => c.STNATrainingFacilityId == currentTrainingFacilityId)
                    .ToList();
                
            }

            return View(viewModel);
        }

        ////
        //// GET: /STNATraining/Details/5

        //public ActionResult Details(int id = 0)
        //{
        //    STNATrainingFacilityInteraction stnatrainingfacilityinteraction = Context.STNATrainingFacilityInteractions.Find(id);
        //    if (stnatrainingfacilityinteraction == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(stnatrainingfacilityinteraction);
        //}

        ////
        //// GET: /STNATraining/Create

        //public ActionResult Create()
        //{
        //    Session[AppCode + "CurrentTab"] = 0;
        //    LoadTrainingActionItemList();
        //    LoadTrainingFacilityList(CurrentFacility[AppCode]);

        //    return View(new STNATrainingFacilityInteraction { 
        //        STNATrainingFacilityId = (int)Session[AppCode + "CurrentTrainingFacilityId"], 
        //        InteractionDate = DateTime.Now
        //    });
        //}

        ////
        //// POST: /STNATraining/Create

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(STNATrainingFacilityInteraction stnatrainingfacilityinteraction)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Context.STNATrainingFacilityInteractions.Add(stnatrainingfacilityinteraction);
        //        Context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(stnatrainingfacilityinteraction);
        //}



        ////
        //// GET: /STNATraining/Edit/5

        //public ActionResult Edit(int id = 0)
        //{
        //    Session[AppCode + "CurrentTab"] = 0;

        //    STNATrainingFacilityInteraction stnatrainingfacilityinteraction = Context.STNATrainingFacilityInteractions.Find(id);
        //    if (stnatrainingfacilityinteraction == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    LoadTrainingActionItemList();
        //    LoadTrainingFacilityList(CurrentFacility[AppCode]);

        //    return View(stnatrainingfacilityinteraction);
        //}

        ////
        //// POST: /STNATraining/Edit/5

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(STNATrainingFacilityInteraction stnatrainingfacilityinteraction)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Context.Entry(stnatrainingfacilityinteraction).State = EntityState.Modified;
        //        Context.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(stnatrainingfacilityinteraction);
        //}


        ////
        //// GET: /STNATraining/Delete/5

        //public ActionResult Delete(int id = 0)
        //{
        //    Session[AppCode + "CurrentTab"] = 0;
        //    STNATrainingFacilityInteraction stnatrainingfacilityinteraction = Context.STNATrainingFacilityInteractions.Find(id);
        //    if (stnatrainingfacilityinteraction == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    LoadTrainingActionItemList();
        //    LoadTrainingFacilityList(CurrentFacility[AppCode]);

        //    return View(stnatrainingfacilityinteraction);
        //}

        ////
        //// POST: /STNATraining/Delete/5

        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    STNATrainingFacilityInteraction stnatrainingfacilityinteraction = Context.STNATrainingFacilityInteractions.Find(id);
        //    Context.STNATrainingFacilityInteractions.Remove(stnatrainingfacilityinteraction);
        //    Context.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        #region Training Facility Classes

        public ActionResult CreateTrainingClass()
        {
            Session.SetItem(AppCode + "CurrentTab", 0);
            //LoadTrainingActionItemList();
            LoadTrainingFacilityList(CurrentFacility[AppCode]);
            Session.TryGetObject(AppCode + "CurrentTrainingFacilityId", out int currentTrainingFacilityId);
            return View(new STNATrainingClass
            {
                STNATrainingFacilityId = currentTrainingFacilityId,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTrainingClass(STNATrainingClass stnaTrainingClass)
        {
            if (ModelState.IsValid)
            {
                Context.STNATrainingClasses.Add(stnaTrainingClass);
                Context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stnaTrainingClass);
        }



        //
        // GET: /STNATraining/Edit/5

        public ActionResult EditTrainingClass(int id = 0)
        {
            Session.SetItem(AppCode + "CurrentTab", 0);
            STNATrainingClass stnaTrainingClass = Context.STNATrainingClasses.Find(id);
            if (stnaTrainingClass == null)
            {
                return NotFound();
            }
            //LoadTrainingActionItemList();
            LoadTrainingFacilityList(CurrentFacility[AppCode]);

            return View(stnaTrainingClass);
        }

        //
        // POST: /STNATraining/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTrainingClass(STNATrainingClass stnaTrainingClass)
        {
            if (ModelState.IsValid)
            {
                Context.Entry(stnaTrainingClass).State = EntityState.Modified;
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stnaTrainingClass);
        }



        public ActionResult DeleteTrainingClass(int id = 0)
        {
            Session.SetItem(AppCode + "CurrentTab", 0);
            STNATrainingClass stnaTrainingClass = Context.STNATrainingClasses.Find(id);
            if (stnaTrainingClass == null)
            {
                return NotFound();
            }
            LoadTrainingFacilityList(CurrentFacility[AppCode]);

            return View(stnaTrainingClass);
        }

        //
        // POST: /STNATraining/Delete/5

        [HttpPost, ActionName("DeleteTrainingClass")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTrainingClassConfirmed(int id)
        {
            STNATrainingClass stnaTrainingClass = Context.STNATrainingClasses.Find(id);
            Context.STNATrainingClasses.Remove(stnaTrainingClass);
            Context.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion


        #region Training Facility Notes

        public ActionResult CreateTrainingNote()
        {
            Session.SetItem(AppCode + "CurrentTab", 1);
            LoadTrainingFacilityList(CurrentFacility[AppCode]);
            Session.TryGetObject(AppCode + "CurrentTrainingFacilityId", out int currentTrainingFacilityId);
            return View(new STNATrainingFacilityNote
            {
                STNATrainingFacilityId = currentTrainingFacilityId,
                NoteDate = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateTrainingNote(STNATrainingFacilityNote stnaTrainingNote)
        {
            if (ModelState.IsValid)
            {
                stnaTrainingNote.InsertedDate = DateTime.Now;
                Context.STNATrainingFacilityNotes.Add(stnaTrainingNote);
                Context.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(stnaTrainingNote);
        }



        //
        // GET: /STNATraining/Edit/5

        public ActionResult EditTrainingNote(int id = 0)
        {
            Session.SetItem(AppCode + "CurrentTab", 1);
            STNATrainingFacilityNote stnaTrainingNote = Context.STNATrainingFacilityNotes.Find(id);
            if (stnaTrainingNote == null)
            {
                return NotFound();
            }
            LoadTrainingFacilityList(CurrentFacility[AppCode]);

            return View(stnaTrainingNote);
        }

        //
        // POST: /STNATraining/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditTrainingNote(STNATrainingFacilityNote stnaTrainingNote)
        {
            if (ModelState.IsValid)
            {
                Context.Entry(stnaTrainingNote).State = EntityState.Modified;
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(stnaTrainingNote);
        }



        public ActionResult DeleteTrainingNote(int id = 0)
        {
            Session.SetItem(AppCode + "CurrentTab", 1);
            STNATrainingFacilityNote stnaTrainingNote = Context.STNATrainingFacilityNotes.Find(id);
            if (stnaTrainingNote == null)
            {
                return NotFound();
            }
            LoadTrainingFacilityList(CurrentFacility[AppCode]);

            return View(stnaTrainingNote);
        }

        //
        // POST: /STNATraining/Delete/5

        [HttpPost, ActionName("DeleteTrainingNote")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteTrainingNoteConfirmed(int id)
        {
            STNATrainingFacilityNote stnaTrainingNote = Context.STNATrainingFacilityNotes.Find(id);
            Context.STNATrainingFacilityNotes.Remove(stnaTrainingNote);
            Context.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion


        protected override void Dispose(bool disposing)
        {
            Context.Dispose();
            base.Dispose(disposing);
        }



        #region Update Post Backs
        [HttpPost] //Side Drop Down List update
        public ActionResult SideDDL(string TrainingFacilities, string FullEmployees, int CurrentCommunity, string returnUrl, string TerminatedShown)
        {
            int trainingFacilityId;
            if (!Int32.TryParse(TrainingFacilities, out trainingFacilityId))
            {
                return Redirect(returnUrl);
            }

            CurrentFacility[AppCode] = CurrentCommunity;
            LoadTrainingFacilityInformation(trainingFacilityId);

            return Redirect(returnUrl);
        }

        private void LoadTrainingFacilityInformation(int trainingFacilityId)
        {
            var facility = Context.STNATrainingFacilities.Single(f => f.STNATrainingFacilityId == trainingFacilityId);
            Session.SetItem(AppCode + "CurrentTrainingFacilityId", trainingFacilityId);
            Session.SetItem(AppCode + "CurrentTrainingFacilityName", facility.TrainingFacilityName);

        }

        [HttpPost]
        public ActionResult Lookback(string lookbackDate, string returnUrl, string CurrentCommunity)
        {
            Session.SetItem(AppCode + "TerminatedShown", "true");
            return SaveLookbackDate(lookbackDate, returnUrl, CurrentCommunity, AppCode);
        }

        [HttpPost]
        public ActionResult UpdateRange(string occurredRangeFrom, string occurredRangeTo, string returnUrl)
        {
            Session.Remove(AppCode + "CurrentInfectionId");
            return UpdateTableRange(occurredRangeFrom, occurredRangeTo, returnUrl, AppCode);
        }
        #endregion

        #region Ajax Calls
        
        public void ChangeCommunity(int communityId)
        {
            Session.TryGetObject(AppCode + "LookbackDate", out string lookbackDateString);
            var lookbackDate = DateTime.Parse(lookbackDateString);

            LoadTrainingFacilityList(communityId);

            CurrentFacility[AppCode] = communityId;

            Session.Remove(AppCode + "CurrentTrainingFacilityId");
            Session.SetItem(AppCode + "CurrentTab", 0);
        }
        #endregion

        #region Private Helper Functions
        private void InitializeLookbackDate()
        {
            // Set initial Lookback Date
            SetLookbackDays(HttpContext, AppCode);

            // Initialize "Census Date" session values
            if (!Session.Contains("CensusDateChanged"))
            {
                Session.SetItem("CensusDateChanged", false);
            }
            if (!Session.Contains("CensusDateChangedUpdate"))
            {
                Session.SetItem("CensusDateChangedUpdate", false);
            }
            if (!Session.Contains("CensusDateInvalid"))
            {
                Session.SetItem("CensusDateInvalid", "0");
            }
            if (!Session.Contains("CensusDateInFuture"))
            {
                Session.SetItem("CensusDateInFuture", "0");
            }
        }

        private void LoadFacilityList()
        {
            FacilityList[AppCode] = FilterCommunitiesByADGroups(AppCode);
        }


        private void LoadTrainingFacilityList(int communityId)
        {
            var trainingFacilities = Context.STNATrainingFacilities
                .Where(t => t.Communities.Any(c => c.CommunityId == communityId))
                .OrderBy(t => t.TrainingFacilityName);

            ViewData[AppCode + "TrainingFacilityList"] = GetTrainingFacilitySelectList(trainingFacilities);
        }

        //private void LoadTrainingActionItemList()
        //{
        //    var actionItems = Context.STNATrainingActionItems.ToList();

        //    ViewData[AppCode + "TrainingActionItemList"] = GetTrainingActionItemSelectList(actionItems);
        //}

        
        private SelectList GetTrainingFacilitySelectList(IEnumerable<STNATrainingFacility> trainingFacilities)
        {
            return new SelectList(trainingFacilities.Select(f => new SelectListItem()
            {
                Text = f.TrainingFacilityName,
                Value = f.STNATrainingFacilityId.ToString()
            }), "Value", "Text");
        }

        private SelectList GetTrainingActionItemSelectList(IEnumerable<STNATrainingActionItem> actionItems)
        {
            return new SelectList(actionItems.Select(a => new SelectListItem()
            {
                Text = a.ActionItemDesc,
                Value = a.STNATrainingActionItemId.ToString()
            }), "Value", "Text");
        }


        #endregion
    }
}