using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Principal;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using AtriumWebApp.Web.Survey.Models;
using AtriumWebApp.Web.Survey.Models.Mappings;
using AtriumWebApp.Web.Survey.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.Survey.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "CSU")]
    public class SurveyController : BaseController
    {
        private const string AppCode = "CSU";

        public SurveyController(IOptions<AppSettingsConfig> config, SurveyContext context) : base(config, context)
        {
            ModelMapper = new SurveyMapper(context);
        }

        protected new SurveyContext Context
        {
            get { return (SurveyContext)base.Context; }
        }

        private SurveyMapper ModelMapper { get; set; }

        public string CurrentSurveyCycleId
        {
            get
            {
                Session.TryGetObject(AppCode + "CurrentSurveyCycleId", out string id);
                return id;
            }
            set
            {

                if (value != null)
                {
                    Session.SetItem(AppCode + "CurrentSurveyCycleId", value);
                }
                else
                {
                    Session.Remove(AppCode + "CurrentSurveyCycleId");
                }
            }
        }
        public int? CurrentSurveyId
        {
            get
            {
                Session.TryGetObject(AppCode + "CurrentSurveyId", out int? id);
                return id;
            }
            set
            {

                if (value != null)
                {
                    Session.SetItem(AppCode + "CurrentSurveyId", value);
                }
                else
                {
                    Session.Remove(AppCode + "CurrentSurveyId");
                }
            }
        }

        public ActionResult Index()
        {
            //Record user access
            LogSession(AppCode);
            //Set initial date range values
            SetDateRangeErrorValues();
            SetInitialTableRange(AppCode);
            //Set Community Drop Down based on user access privileges
            GetCommunitiesDropDownWithFilter(AppCode);
            var currentCom = CurrentFacility[AppCode];
            //Filter surveys by date range
            var fromDate = DateTime.Parse(OccurredRangeFrom[AppCode]);
            var toDate = DateTime.Parse(OccurredRangeTo[AppCode]);
            var surveysForCom = Context.CommunitySurveys.Include(s => s.AtriumPayerGroup).Where(s => s.CommunityId == currentCom && s.EnterDate >= fromDate && s.EnterDate <= toDate).ToList();
            IList<CommunitySurveyType> surveyTypes = Context.CommunitySurveyTypes.OrderBy(c => c.SurveyTypeDesc).ToList();
            var viewModel = new SurveyViewModel
            {
                SelectedCommunity = CurrentFacility[AppCode],
                Communities = new SelectList(FacilityList[AppCode], "CommunityId", "CommunityShortName"),
                Surveys = surveysForCom,
                SurveyTypes = new SelectList(surveyTypes, "SurveyTypeId", "SurveyTypeDesc"),
                SurveyTypesAll = Context.CommunitySurveyTypes.ToList(),
                FederalDeficiencies = Context.FederalDeficiencies.OrderBy(d => d.TagCode).ToList(),
                StateDeficiencies = Context.StateDeficiencies.OrderBy(d => d.TagCode).ToList(),
                SafetyDeficiencies = Context.SafetyDeficiencies.OrderBy(d => d.TagCode).ToList(),
                ScopeAndSeverity = Context.ScopeAndSeverities.ToList()
            };
            //If a survey is currently being edited
            if (CurrentSurveyCycleId != null)
            {
                //Get current survey
                var currentSurveyCycleId = CurrentSurveyCycleId;
                var currentSurveyId = CurrentSurveyId;
                var currentSurvey = Context.CommunitySurveys.Single(s => s.SurveyCycleId == currentSurveyCycleId && s.SurveyId == currentSurveyId);
                //Get Citations for current survey
                var currentFederalCitations = Context.FederalCitations.Where(f => f.SurveyCycleId == currentSurveyCycleId && f.SurveyId == currentSurveyId);
                var currentStateCitations = Context.StateCitations.Where(s => s.SurveyCycleId == currentSurveyCycleId && s.SurveyId == currentSurveyId);
                var currentSafetyCitations = Context.SafetyCitations.Where(s => s.SurveyCycleId == currentSurveyCycleId && s.SurveyId == currentSurveyId);
                //Get CMPs for current survey
                var currentCMP = Context.CivilMonetaryPenalties.Where(c => c.SurveyCycleId == currentSurveyCycleId && c.SurveyId == currentSurveyId).ToList();
                var cmpYTD = CalculateCMPYTD(currentCMP);
                //Get Deficiency Types
                viewModel.CurrentSurvey = currentSurvey;
                viewModel.CivilMonetaryPenalties = currentCMP.ToList();
                viewModel.CMPYTD = cmpYTD;
                viewModel.DaysOut = CalculateDaysOut(currentSurvey);
                viewModel.FederalCitations = currentFederalCitations.ToList();
                viewModel.StateCitations = currentStateCitations.ToList();
                viewModel.SafetyCitations = currentSafetyCitations.ToList();
                viewModel.SurveyPayerGroups = new SelectList(Context.AtriumPayerGroups.Where(e => e.IsCommunitySurveyEligible).ToList(), "AtriumPayerGroupCode", "AtriumPayerGroupName", currentSurvey.AtriumPayerGroupCode);

                IList<SurveyDocumentViewModel> surveyDocsVM = new List<SurveyDocumentViewModel>();
                IList<SurveyDocument> surveyDocs = ModelMapper.GetSurveyDocumentsByIds(currentSurveyId ?? 0, currentSurveyCycleId).SurveyDocuments.ToList();
                if (surveyDocs != null && surveyDocs.Count > 0)
                    foreach (SurveyDocument sd in surveyDocs)
                    {
                        //SurveyDocumentViewModel sdvm = new SurveyDocumentViewModel()
                        //{
                        //    ContentType = sd.ContentType,
                        //    //Document = 
                        //    FileName = sd.FileName,
                        //    Id = sd.Id,
                        //    SurveyCycleId = sd.SurveyCycleId,
                        //    SurveyId = sd.SurveyId
                        //};
                        surveyDocsVM.Add(
                            ModelMapper.MapSurveyDocument(sd)
                            );
                    }
                viewModel.Documents = surveyDocsVM;
            }
            else
            {
                //If survey is not a follow up, do not allow user to select Follow Up
                surveyTypes.Remove(Context.CommunitySurveyTypes.Single(s => s.SurveyTypeDesc == "Follow Up"));
                viewModel.SurveyPayerGroups = new SelectList(Context.AtriumPayerGroups.Where(e => e.IsCommunitySurveyEligible).ToList(), "AtriumPayerGroupCode", "AtriumPayerGroupName", "SNF");
            }
            return View(viewModel);
        }

        public PartialViewResult CreateSurveyDocument()
        {
            return PartialView(new SurveyDocumentViewModel());
        }

        public ActionResult StreamSurveyDocument(int id)
        {
            var document = Context.SurveyDocuments.SingleOrDefault(d => d.Id == id);

            if (document == null)
                ModelState.AddModelError("DocumentMissing", "Document could not be found");

            document.Document = SevenZipHelper.DecompressStreamLZMA(document.Document).ToArray();
            //document.Document = SevenZipHelper.DECompressBytes(document.Document).ToArray();

            return File(document.Document, document.ContentType, document.FileName);
        }

        #region Private Helper Functions
        private decimal CalculateCMPYTD(IEnumerable<CivilMonetaryPenalty> currentCMP)
        {
            //Calculate CMP YTD based on Instance and Daily Values
            var discountScalar = 0.65m;
            decimal valueYTD = 0;
            foreach (var cmp in currentCMP)
            {
                if (cmp.InstanceFlg)
                {
                    valueYTD += cmp.DiscountFlg ? cmp.CMPAmount * discountScalar : cmp.CMPAmount;
                }
                else if (cmp.DailyFlg)
                {
                    if (cmp.CMPEndDate != null)
                    {
                        var days = (((DateTime)cmp.CMPEndDate - (DateTime)cmp.CMPStartDate).Days + 1);
                        valueYTD += cmp.DiscountFlg ? days * cmp.CMPAmount * discountScalar : days * cmp.CMPAmount;
                    }
                }
            }
            return valueYTD;
        }

        /*Calculate Days out
            If Survey is open: Today minus exit of first survey in cycle
            If Survey is closed and is substantiated: Date certain minus exit date
            If Survey is closed and is UNsubstantiated: ALWAYS zero*/
        public int CalculateDaysOut(CommunitySurvey survey)
        {
            if (survey.UnsubstantiatedComplaintFlg)
            {
                return 0;
            }

            var firstSurveyInCycle = Context.CommunitySurveys.Where(s => s.SurveyCycleId == survey.SurveyCycleId).OrderBy(s => s.SurveyId).First();

            if (survey.ClosedFlg)
            {
                var lastSurveyInCycle = Context.CommunitySurveys.Where(s => s.SurveyCycleId == survey.SurveyCycleId).OrderByDescending(s => s.SurveyId).First();
                if (lastSurveyInCycle.CertainDate.HasValue)
                {
                    return ((DateTime)lastSurveyInCycle.CertainDate - firstSurveyInCycle.ExitDate).Days;
                }
                return 0;
            }
            return (DateTime.Today - firstSurveyInCycle.ExitDate).Days;
        }
        #endregion

        #region Save Post Backs
        [HttpPost]
        public ActionResult SaveMainSurvey(SurveyViewModel viewModel, bool? createFollowUp)
        {
            int currentCommunity;
            if (!CurrentFacility.TryGet(AppCode, out currentCommunity))
            {
                return RedirectToAction("");
            }
            //Get Current survey information, 0 if null
            var surveyCycleId = viewModel.CurrentSurvey.SurveyCycleId;
            var surveyId = viewModel.CurrentSurvey.SurveyId;
            //If Survey exists, get it, otherwise create a new survey
            var survey = (String.IsNullOrEmpty(surveyCycleId)
                              ? new CommunitySurvey()
                              : Context.CommunitySurveys.Single(s => s.SurveyCycleId == surveyCycleId && s.SurveyId == surveyId));
            //If Survey exists, add 1 to the latest in the cycle's SurveyId, otherwise start at 1
            survey.SurveyId = (surveyId == 0
                                   ? (String.IsNullOrEmpty(surveyCycleId)
                                          ? 1
                                          : Context.CommunitySurveys.Where(s => s.SurveyCycleId == survey.SurveyCycleId)
                                                                    .OrderByDescending(s => s.SurveyId)
                                                                    .First()
                                                                    .SurveyId + 1)
                                   : surveyId);
            survey.CommunityId = currentCommunity;
            survey.SurveyTypeId = viewModel.CurrentSurvey.SurveyTypeId;
            survey.AtriumPayerGroupCode = viewModel.CurrentSurvey.AtriumPayerGroupCode;
            survey.EnterDate = viewModel.CurrentSurvey.EnterDate;
            survey.ExitDate = viewModel.CurrentSurvey.ExitDate;
            survey.CertainDate = viewModel.CurrentSurvey.CertainDate;
            survey.ClosedFlg = viewModel.CurrentSurvey.ClosedFlg;
            survey.UnsubstantiatedComplaintFlg = viewModel.CurrentSurvey.UnsubstantiatedComplaintFlg || (survey.ClosedFlg && !survey.CertainDate.HasValue); // NOTE: If closed and there is no certain date, default to unsubstantiated
            survey.FollowUpDate = viewModel.CurrentSurvey.FollowUpDate;
            survey.Form2567ReceiveDate = viewModel.CurrentSurvey.Form2567ReceiveDate;
            survey.PotentialDOPDate = viewModel.CurrentSurvey.PotentialDOPDate;
            survey.PotentialTermDate = viewModel.CurrentSurvey.PotentialTermDate;
            survey.ImmediateJeopardyFlg = viewModel.CurrentSurvey.ImmediateJeopardyFlg;
            survey.SQCFlg = viewModel.CurrentSurvey.SQCFlg;
            survey.DidNotClearFollowUpFlg = viewModel.CurrentSurvey.DidNotClearFollowUpFlg;
            survey.SelfReportFlg = viewModel.CurrentSurvey.SelfReportFlg;
            survey.FederalMonitoringFlg = viewModel.CurrentSurvey.FederalMonitoringFlg;
            survey.DOPFlg = viewModel.CurrentSurvey.DOPFlg;
            survey.DOPStartDate = viewModel.CurrentSurvey.DOPStartDate;
            survey.DOPEndDate = viewModel.CurrentSurvey.DOPEndDate;
            survey.DOPDailyAmount = viewModel.CurrentSurvey.DOPDailyAmount;
            survey.StateFineFlg = viewModel.CurrentSurvey.StateFineFlg;
            survey.StateFineAmount = viewModel.CurrentSurvey.StateFineAmount;
            //If SurveyId is null, create a new cycle
            if (surveyId == 0)
            {
                
                survey.SurveyCycleId = User.Identity.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                Context.CommunitySurveys.Add(survey);
            }
            Context.SaveChanges();
            //If FollowUp flag has been raised, create a followup, otherwise set editing to current survey
            if (createFollowUp.HasValue && createFollowUp == true)
            {
                this.CreateFollowUp(survey.SurveyCycleId, survey.SurveyId);
            }
            else
            {
                CurrentSurveyCycleId = survey.SurveyCycleId;
                CurrentSurveyId = survey.SurveyId;
            }
            return RedirectToAction("");
        }

        [HttpPost]
        public ActionResult SaveDocumentsSurvey(ICollection<SurveyDocumentViewModel> Documents)
        {
            if (Documents == null)
                return RedirectToAction("");

            int currentCommunity;
            if (!CurrentFacility.TryGet(AppCode, out currentCommunity))
            {
                return RedirectToAction("");
            }
            // Already set in Edit
            string surveyCycleId = this.CurrentSurveyCycleId;
            int surveyId = this.CurrentSurveyId ?? 0;
            CommunitySurvey existingSurvey = ModelMapper.GetSurveyDocumentsByIds(surveyId, surveyCycleId);
            if (existingSurvey != null)
            {
                SurveyViewModel svm = new SurveyViewModel()
                {
                    CurrentSurvey = new CommunitySurvey()
                    {
                        SurveyId = surveyId,
                        SurveyCycleId = surveyCycleId
                    },
                    Documents = Documents
                };
                ModelMapper.MapSurveyDocuments(svm, existingSurvey);
            }
            else
            {
                IList<SurveyDocument> documents = new List<SurveyDocument>();
                ModelMapper.MapSurveyDocuments(Documents.ToList(), documents);
            }
            Context.SaveChanges();
            return RedirectToAction("");
        }

        [HttpPost]
        public ActionResult SaveCMP(IFormCollection form)
        {
            if (CurrentSurveyCycleId == null || CurrentSurveyId == null)
            {
                return RedirectToAction("");
            }
            var cmp = (String.IsNullOrEmpty(form["CMPEditingId"])
                           ? new CivilMonetaryPenalty()
                           : Context.CivilMonetaryPenalties.Find(Int32.Parse(form["CMPEditingId"])));
            cmp.DailyFlg = form["Daily"].Contains("true");
            cmp.InstanceFlg = form["Instance"].Contains("true");
            //Flag determines which dates to consider, all of them are Not Null, but some are required client side
            if (cmp.DailyFlg)
            {
                cmp.CMPStartDate = DateTime.Parse(form["DateCMPFrom"]);
                cmp.CMPEndDate = (String.IsNullOrEmpty(form["DateCMPTo"])
                                      ? (DateTime?)null
                                      : DateTime.Parse(form["DateCMPTo"]));
                cmp.CMPInstanceDate = null;
            }
            else
            {
                cmp.CMPInstanceDate = DateTime.Parse(form["DateInstance"]);
                cmp.CMPStartDate = null;
                cmp.CMPEndDate = null;
            }
            cmp.CMPAmount = decimal.Parse(form["CMPAmount"]);
            cmp.DiscountFlg = form["Discount"].Contains("true");
            if (String.IsNullOrEmpty(form["CMPEditingId"]))
            {
                cmp.SurveyCycleId = CurrentSurveyCycleId;
                cmp.SurveyId = CurrentSurveyId.Value;
                Context.CivilMonetaryPenalties.Add(cmp);
            }
            Context.SaveChanges();
            return Json(new SaveResultWithIdViewModel { Id = cmp.CMPId, Success = true });
        }

        [HttpPost]
        public ActionResult SaveCitation(IFormCollection form)
        {
            var editingId = (String.IsNullOrEmpty(form["EditingId"]) ? 0 : Int32.Parse(form["EditingId"]));
            BaseCitation citation = null;
            switch (form["CitationType"])
            {
                case "Fed":
                    var federalCitation = (editingId == 0
                                               ? new FederalCitation()
                                               : Context.FederalCitations.Find(editingId));
                    var survey = Context.CommunitySurveys.Single(e => e.SurveyId == CurrentSurveyId.Value && e.SurveyCycleId == CurrentSurveyCycleId);
                    citation = federalCitation;
                    federalCitation.FederalDeficiencyId = Int32.Parse(form["FedDef"]);
                    federalCitation.SASId = Int32.Parse(form["SAS"]);
                    if (editingId == 0)
                    {
                        Context.FederalCitations.Add(federalCitation);
                    }
                    var federalDeficiency = Context.FederalDeficiencies.Single(e => e.Id == federalCitation.FederalDeficiencyId);
                    survey.AtriumPayerGroupCode = federalDeficiency.AtriumPayerGroupCode;
                    break;
                case "State":
                    var stateCitation = (editingId == 0
                                             ? new StateCitation()
                                             : Context.StateCitations.Find(editingId));
                    citation = stateCitation;
                    stateCitation.StateDeficiencyId = Int32.Parse(form["StateDef"]);
                    stateCitation.SASId = (String.IsNullOrEmpty(form["SASNotRequired"])
                                               ? (int?)null
                                               : Int32.Parse(form["SASNotRequired"]));
                    if (editingId == 0)
                    {
                        Context.StateCitations.Add(stateCitation);
                    }
                    break;
                case "Safety":
                    var safetyCitation = (editingId == 0
                                              ? new SafetyCitation()
                                              : Context.SafetyCitations.Find(editingId));
                    citation = safetyCitation;
                    safetyCitation.SafetyDeficiencyId = Int32.Parse(form["SafetyDef"]);
                    safetyCitation.SASId = (String.IsNullOrEmpty(form["SASNotRequired"])
                                                ? (int?)null
                                                : Int32.Parse(form["SASNotRequired"]));
                    safetyCitation.WaiverFlg = form["Waiver"].Contains("true");
                    if (editingId == 0)
                    {
                        Context.SafetyCitations.Add(safetyCitation);
                    }
                    break;
            }
            if (citation == null)
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            if (editingId == 0)
            {
                citation.SurveyCycleId = CurrentSurveyCycleId;
                citation.SurveyId = CurrentSurveyId.Value;
            }
            citation.Comments = form["Comments"];
            Context.SaveChanges();
            return Json(new SaveResultWithIdViewModel { Id = citation.Id, Success = true });
        }
        #endregion

        #region Update Information Postbacks
        [HttpPost]
        public ActionResult SideDDL(int selectedCommunity)
        {
            //Change Community and clear Survey information
            CurrentFacilityName[AppCode] = Context.Facilities.Single(c => c.CommunityId == selectedCommunity).CommunityShortName;
            CurrentFacility[AppCode] = selectedCommunity;
            CurrentSurveyCycleId = null;
            CurrentSurveyId = null;
            return RedirectToAction("");
        }

        [HttpPost]
        public ActionResult UpdateRange(string occurredRangeFrom, string occurredRangeTo, string returnUrl)
        {
            return UpdateTableRange(occurredRangeFrom, occurredRangeTo, returnUrl, AppCode);
        }
        #endregion

        #region Ajax methods
        public void ChangeSurveyId(string cycleId, int surveyId)
        {
            CurrentSurveyCycleId = cycleId;
            CurrentSurveyId = surveyId;
        }

        public void DeleteSurvey(string cycleId, int surveyId)
        {
            Context.CommunitySurveys.Remove(Context.CommunitySurveys.Single(c => c.SurveyCycleId == cycleId && c.SurveyId == surveyId));
            Context.SaveChanges();
        }

        public void DeleteCitation(int citationId, string citationType)
        {
            switch (citationType)
            {
                case "Federal":
                    Context.FederalCitations.Remove(Context.FederalCitations.Find(citationId));
                    Context.SaveChanges();
                    break;
                case "State":
                    Context.StateCitations.Remove(Context.StateCitations.Find(citationId));
                    Context.SaveChanges();
                    break;
                case "Safety":
                    Context.SafetyCitations.Remove(Context.SafetyCitations.Find(citationId));
                    Context.SaveChanges();
                    break;
            }
        }

        public void DeleteCMP(int cmpId)
        {
            Context.CivilMonetaryPenalties.Remove(Context.CivilMonetaryPenalties.Find(cmpId));
            Context.SaveChanges();
        }

        public void ClearSurvey()
        {
            CurrentSurveyCycleId = null;
            CurrentSurveyId = null;
        }

        public void CreateFollowUp(string cycleId, int surveyId)
        {
            //Create new followup within same Cycle
            CurrentSurveyCycleId = cycleId;
            var surveyToBeFollowedUp = Context.CommunitySurveys.Where(c => c.SurveyCycleId == cycleId)
                                      .OrderByDescending(c => c.SurveyId)
                                      .First();
            var followUpId = Context.CommunitySurveyTypes.Single(s => s.SurveyTypeDesc == "Follow Up");
            //Must create a followup entry in order to edit
            var communitySurvey = new CommunitySurvey
            {
                SurveyTypeId = followUpId.SurveyTypeId,
                SurveyCycleId = cycleId,
                SurveyId = surveyToBeFollowedUp.SurveyId + 1,
                AtriumPayerGroupCode = surveyToBeFollowedUp.AtriumPayerGroupCode,
                CommunityId = surveyToBeFollowedUp.CommunityId,
                EnterDate = DateTime.Today,
                ExitDate = DateTime.Today.AddDays(3)
            };
            Context.CommunitySurveys.Add(communitySurvey);
            Context.SaveChanges();
            CurrentSurveyId = communitySurvey.SurveyId;
        }
        #endregion
    }
}