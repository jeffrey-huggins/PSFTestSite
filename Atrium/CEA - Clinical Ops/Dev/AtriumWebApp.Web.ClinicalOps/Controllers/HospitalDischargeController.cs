using System;
using System.Collections.Generic;
using System.Linq;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.ClinicalOps.Models.ViewModel;
using System.Data.Entity;
using AtriumWebApp.Web.Base.Controllers;
using AtriumWebApp.Web.Base.Library;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.ClinicalOps.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "HOD")]
    public class HospitalDischargeController : BaseHospitalDischargeController
    {
        public HospitalDischargeController(IOptions<AppSettingsConfig> config, HospitalDischargeContext context) : base(config, context)
        {
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
            ViewData["Communities"] = new SelectList(FacilityList[AppCode], "CommunityId", "CommunityShortName");
            return HODEventCurrentFacility();
        }

        public ActionResult Edit(DateTime censusDate, int id = 0)
        {
            HospitalDischarge hospitalDischarge = Context.Discharges.FirstOrDefault(d => d.PatientId == id && d.CensusDate == censusDate);
            if (hospitalDischarge == null)
            {
                return NotFound();
            }

            var selectedERDischargeReason = Context.DischargeReasons.FirstOrDefault(r => r.DischargeReasonId == hospitalDischarge.ERDischargeReasonId);
            var selectedHospitalDischargeReason = Context.DischargeReasons.FirstOrDefault(r => r.DischargeReasonId == hospitalDischarge.HospitalDischargeReasonId);
            var patient = Context.Residents.FirstOrDefault(r => r.PatientId == hospitalDischarge.PatientId);
            ViewData["ResidentName"] = patient.LastName + ", " + patient.FirstName;
            List<DischargeReason> erDischargeReasons = ERDischargeReasons.ToList();
            List<DischargeReason> hospitalDischargeReasons = HospitalDischargeReasons.ToList();
            if (!erDischargeReasons.Contains(selectedERDischargeReason))
            {
                erDischargeReasons.Add(selectedERDischargeReason);
            }
            if (!hospitalDischargeReasons.Contains(selectedHospitalDischargeReason))
            {
                hospitalDischargeReasons.Add(selectedHospitalDischargeReason);
            }
            ViewData["ERDischargeReasons"] = new SelectList(erDischargeReasons.Select(dr => new SelectListItem()
            {
                Text = dr.DischargeReasonDesc,
                Value = dr.DischargeReasonId.ToString()
            }), "Value", "Text");
            ViewData["HospitalDischargeReasons"] = new SelectList(hospitalDischargeReasons.Select(dr => new SelectListItem()
            {
                Text = dr.DischargeReasonDesc,
                Value = dr.DischargeReasonId.ToString()
            }), "Value", "Text");
            ViewData["Hospitals"] = new SelectList(Hospitals.Select(h => new SelectListItem()
            {
                Text = h.Name,
                Value = h.Id.ToString()
            }), "Value", "Text");
            ViewData["DNRR"] = new SelectList(Context.DidNotReturnReasons.ToList().Select(dnrr => new SelectListItem()
            {
                Text = dnrr.DidNotReturnReasonDesc,
                Value = dnrr.DidNotReturnReasonId.ToString()
            }), "Value", "Text");
            return View(hospitalDischarge);
        }

        [HttpPost]
        public ActionResult Edit(HospitalDischarge hospitalDischarge)
        {
            if (ModelState.IsValid)
            {
                Context.Entry(hospitalDischarge).State = EntityState.Modified;
                Context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(hospitalDischarge);
        }

        #region Private Helper Functions
        private ActionResult HODEventCurrentFacility()
        {
            int community;
            if (CurrentFacility.TryGet(AppCode, out community))
            {
                var occurredToDate = DateTime.Parse(OccurredRangeTo[AppCode]);
                var occurredFromDate = DateTime.Parse(OccurredRangeFrom[AppCode]);
                var patientsForCommunity = (from pat in Context.Residents
                                            where pat.CommunityId == community
                                            select pat);
                //Combine Discharge information with Patient information
                var dischargeForCommunity = from dis in Context.Discharges.ToList()
                                            join p in patientsForCommunity.ToList() on dis.PatientId equals p.PatientId
                                            join cp in Context.CommunityPayers.ToList() on dis.PayerId equals cp.PayerId
                                            where
                                                occurredFromDate.CompareTo(dis.CensusDate) <= 0 &&
                                                occurredToDate.CompareTo(dis.CensusDate) >= 0 &&
                                                cp.AtriumPayerTypeCode != "RST" && cp.AtriumPayerTypeCode != "IL"
                                            select new HospitalDischargePTO()
                                            {
                                                PatientId = p.PatientId,
                                                FirstName = p.FirstName,
                                                LastName = p.LastName,
                                                CensusDate = dis.CensusDate,
                                                DischargeType = dis.DischargeType,
                                                PatientStatus = dis.PatientStatus,
                                                ERDischargeReasonId = dis.ERDischargeReasonId,
                                                ERDischargeReasonIsIncluded = ERDischargeReasons
                                                    .Any(r => r.DischargeReasonId == dis.ERDischargeReasonId),
                                                HospitalDischargeReasonId = dis.HospitalDischargeReasonId,
                                                HospitalDischargeReasonIsIncluded = HospitalDischargeReasons
                                                    .Any(r => r.DischargeReasonId == dis.HospitalDischargeReasonId),
                                                DischargeReasonFlg = dis.DischargeReasonFlg,
                                                PlannedFlg = dis.PlannedFlg,
                                                DidNotReturnReasonId = dis.DidNotReturnReasonId,
                                                HospitalId = dis.HospitalId,
                                                HospitalIsIncluded = Hospitals.Any(h => h.Id == dis.HospitalId),
                                                AdmitSrc = (from ad in Context.PatientAdmits
                                                            where dis.AdmitDate.CompareTo(ad.AdmitDateTime) <= 0 && p.PatientId == ad.PatientId
                                                            orderby ad.AdmitDateTime descending
                                                            select ad.AdmitSrc).FirstOrDefault()
                                            };
                
                return View(new HospitalDischargeViewModel()
                {
                    DischargesForFacility = dischargeForCommunity.ToList(), //dischargeForCommunityList, //
                    DischargeReasons = DischargeReasons, 
                    ERDischargeReasons = ERDischargeReasons,
                    HospitalDischargeReasons = HospitalDischargeReasons,
                    DidNotReturnReasons = Context.DidNotReturnReasons.ToList(),
                    AllHospitals = Context.Hospitals.ToList(),
                    Hospitals = Hospitals
                });
            }
            return View(new HospitalDischargeViewModel());
        }
        #endregion

        #region Update Post Backs
        [HttpPost] //Side Drop Down List update
        public ActionResult SideDDL(string Communities, string returnUrl)
        {
            var facilityId = 0;
            if (!Int32.TryParse(Communities, out facilityId))
            {
                return Redirect(returnUrl);
            }
            CurrentFacility[AppCode] = facilityId;
            var facility = (from fac in Context.Facilities
                            where fac.CommunityId == facilityId
                            select fac).Single();
            CurrentFacilityName[AppCode] = facility.CommunityShortName;
            Hospitals = null;
            return Redirect(returnUrl);
        }

        [HttpPost]
        public ActionResult UpdateRange(string occurredRangeFrom, string occurredRangeTo, string returnUrl)
        {
            return UpdateTableRange(occurredRangeFrom, occurredRangeTo, returnUrl, AppCode);
        }
        #endregion

        #region Ajax Calls
        public JsonResult EditRow(int rowId, DateTime cDate, bool Planned, int ERDischarge, int HDischarge, int DNRReason, int Hospital)
        {
            var hosp = Context.Discharges.Find(rowId, cDate);
            if (hosp == null)
            {
                return Json(new { Success = false, Reason = 1 });
            }
            hosp.ERDischargeReasonId = ERDischarge;
            hosp.HospitalDischargeReasonId = HDischarge;
            hosp.DidNotReturnReasonId = DNRReason;
            hosp.HospitalId = Hospital;
            hosp.PlannedFlg = Planned;
            var ERDis = Context.DischargeReasons.Find(hosp.ERDischargeReasonId);
            var HDis = Context.DischargeReasons.Find(hosp.HospitalDischargeReasonId);
            var DNRR = Context.DidNotReturnReasons.Find(hosp.DidNotReturnReasonId);
            var HOSP = Context.Hospitals.Find(hosp.HospitalId);
            try
            {
                Context.SaveChanges();
            }
            catch
            {
                return Json(new { Success = false, Reason = 0 });
            }
            return Json(new
            {
                Success = true,
                ERReason = ERDis.DischargeReasonDesc,
                HDReason = HDis.DischargeReasonDesc,
                DNRReason = DNRR != null ? DNRR.DidNotReturnReasonDesc : "",
                Hospital = HOSP.Name,
                ERId = ERDischarge,
                HDId = HDischarge,
                HospitalId = Hospital,
                DNRRId = DNRR != null ? DNRR.DidNotReturnReasonId.ToString() : ""
            });
        }
        #endregion
    }
}