using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.ASAP.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace AtriumWebApp.Web.ASAP.Controllers
{
    [RestrictAccessWithApp( AppCode = "ASAP")]
    public class ASAPHotlineController : BaseController
    {
        private const string AppCode = "ASAP";

        public ASAPHotlineController(IOptions<AppSettingsConfig> config, ASAPHotlineContext context) : base(config, context)
        {

        }
        protected new ASAPHotlineContext Context
        {
            get { return (ASAPHotlineContext)base.Context; }
        }
        public ActionResult Index()
        {
            //Record user access
            LogSession(AppCode);
            //Set Census Date Information and Manipulate when changed
            InitializeCensusDateChangedSessionVariable();
            SetLookbackDays(HttpContext, AppCode);
            ManipulateCensusDate(AppCode);
            //Set initial date range values
            SetDateRangeErrorValues();
            SetInitialTableRangeLookback(AppCode);
            //Set Community Drop Down based on user access privileges
            GetCommunitiesDropDownWithFilter(AppCode);
            ViewData["Communities"] = new SelectList(FacilityList[AppCode], "CommunityId", "CommunityShortName");
            SetASAPDropDowns();
            Session.TryGetObject(AppCode + "ComplaintTypes", out List<ASAPComplaintType> complaints);
            ViewData["ComplaintTypes"] = new SelectList((List<ASAPComplaintType>)complaints, "ASAPComplaintTypeId", "ASAPComplaintTypeDesc");
            InitializeEmailContactDictionary();
            var currentCommunity = CurrentFacility[AppCode];
            //var contactList = (from con in Context.Contacts.AsEnumerable()
            //                   join emp in employeeContext.Employees.AsEnumerable() on con.EmployeeId equals emp.EmployeeId
            //                   select
            //                       new TextValue
            //                           {
            //                               Text = emp.LastName + ", " + emp.FirstName,
            //                               Value = emp.EmployeeId.ToString()
            //                           })
            //    .ToList();
            var contactList = (from con in Context.Contacts
                               select new SelectListItem
                               {
                                   Text = con.LastName + ", " + con.FirstName,
                                   Value = SqlFunctions.StringConvert((double)con.ASAPContactId)
                               }).ToList();
            var dateFrom = DateTime.Parse(OccurredRangeFrom[AppCode]);
            var dateTo = DateTime.Parse(OccurredRangeTo[AppCode]);
            var asapCalls = Context.AsapCalls.Where(
                        a => a.CommunityId == currentCommunity && a.CallDate <= dateTo && a.CallDate >= dateFrom)
                                    .ToList();
            return View(new ASAPHotlineViewModel
            {
                ASAPCalls = asapCalls,
                Contacts = contactList
                ,
                Documents = Context.ASAPCallDocuments.ToList()
            });
        }

        #region Private Helper Functions
        private void InitializeEmailContactDictionary()
        {
            Session.SetItem("Emails", new Dictionary<int, bool>());
        }

        private void SetASAPDropDowns()
        {
            if (!Session.Contains(AppCode + "ComplaintTypes"))
            {
                var complaintTypesContext = new ASAPComplaintTypeContext();
                var complaintTypes = (from comp in complaintTypesContext.AsapComplaintTypes
                                      where comp.DataEntryFlg
                                      orderby comp.SortOrder, comp.ASAPComplaintTypeDesc
                                      select comp).ToList();
                Session.SetItem(AppCode + "ComplaintTypes", complaintTypes);
            }
        }
        #endregion

        #region Save New
        [HttpPost]
        public ActionResult SaveASAP(IFormCollection form, IEnumerable<IFormFile> files)
        {
            var asap = String.IsNullOrEmpty(form["EditingId"])
                           ? new ASAPCall()
                           : Context.AsapCalls.Find(Int32.Parse(form["EditingId"]));
            asap.CallDate = DateTime.Parse(form["Date"]);
            asap.CommunityId = CurrentFacility[AppCode];
            asap.ASAPComplaintTypeId = Int32.Parse(form["ComplaintTypes"]);
            asap.CallerName = form["CallerName"];
            asap.CallerContactInfo = form["CallerNumber"];
            asap.ComplaintDesc = form["Complaint"];
            asap.InvestigationDesc = form["Investigation"];
            asap.ActionDesc = form["Action"];
            asap.SummaryDesc = form["Summary"];
            if (String.IsNullOrEmpty(form["EditingId"]))
            {
                Context.AsapCalls.Add(asap);
            }
            Context.SaveChanges();
            var communityContext = new SharedContext();
            var community = communityContext.Facilities.Find(CurrentFacility[AppCode]);
            //Send email on new Incident
            var subject = "ASAP";
            var body = "Please login to the ASAP Hotline Application to review and investigate the complaint per policy. <br /><br />"
                        + "Complaint Information: <br />"
                        + "Community:  " + community.CommunityShortName + "<br />"
                        + "Policy #:  " + asap.ASAPCallId + "<br />"
                        + "Call Date:  " + asap.CallDate.ToString("d");
            var emailList = new List<string>();
            var contacts = new Dictionary<int, bool>();
            Session.TryGetObject("Emails",out contacts);
            foreach (var contact in contacts)
            {
                if (contact.Value)
                {
                    emailList.Add(Context.Contacts.Find(contact.Key).eMail);
                }
            }

            foreach (var file in files)
            {
                if (file != null)
                {
                    byte[] docData;
                    using (var reader = new BinaryReader(file.OpenReadStream()))
                    {

                        docData = reader.ReadBytes((int)file.Length);
                    }


                    // NOTE: Compression as 7z file happens here.
                    docData = SevenZipHelper.CompressStreamLZMA(docData).ToArray();


                    //check if fileName is not in documents
                    var existingFile = Context.ASAPCallDocuments.Where(d => d.ASAPCallId == asap.ASAPCallId
                        && d.DocumentFileName == file.FileName).FirstOrDefault();
                    if (existingFile == null)
                    {
                        Context.ASAPCallDocuments.Add(new ASAPCallDocument
                        {
                            ASAPCallId = asap.ASAPCallId
                            ,
                            DocumentFileName = file.FileName
                            ,
                            ContentType = file.ContentType
                            ,
                            Document = docData
                        });
                    }
                }
            }
            Context.SaveChanges();

            var sendMail = new Thread(() => MailHelper.SendEmailToListOfEmails(emailList, subject, body, true));
            sendMail.Start();
            Session.SetItem("Emails", new Dictionary<int, bool>());
            return RedirectToAction("");
        }
        #endregion

        #region Other Postbacks
        [HttpPost]
        public ActionResult SideDDL(int Communities)
        {
            CurrentFacility[AppCode] = Communities;
            InitializeEmailContactDictionary();
            return RedirectToAction("");
        }

        [HttpPost]
        public ActionResult UpdateRange(string occurredRangeFrom, string occurredRangeTo, string returnUrl)
        {
            return UpdateTableRange(occurredRangeFrom, occurredRangeTo, returnUrl, AppCode);
        }
        #endregion

        #region Ajax calls
        public JsonResult DeleteRow(int rowId)
        {
            Context.AsapCalls.Remove(Context.AsapCalls.Find(rowId));
            Context.SaveChanges();
            return Json(new SaveResultViewModel { Success = true });
        }

        public void EmailContact(int rowId, bool check)
        {
            Session.TryGetObject("Emails", out Dictionary<int, bool> emails);
            if (emails.ContainsKey(rowId))
            {
                emails[rowId] = check;
            }
            else
            {
                emails.Add(rowId, check);
            }
            Session.SetItem("Emails", emails);
        }

        public void ClearEmailContacts()
        {
            InitializeEmailContactDictionary();
        }
        #endregion


        public ActionResult StreamASAPDocument(int id)
        {
            var document = Context.ASAPCallDocuments.SingleOrDefault(d => d.ASAPCallDocumentId == id);

            if (document == null)
                ModelState.AddModelError("DocumentMissing", "Document could not be found");

            document.Document = SevenZipHelper.DecompressStreamLZMA(document.Document).ToArray();
            //document.Document = SevenZipHelper.DECompressBytes(document.Document).ToArray();

            return File(document.Document, document.ContentType, document.DocumentFileName);
        }

        [HttpPost]
        public JsonResult GetASAPCallDocuments(int asapCallId)
        {
            var documents = Context.ASAPCallDocuments.Where(d => d.ASAPCallId == asapCallId).Select(i => new
            {
                ASAPCallDocumentId = i.ASAPCallDocumentId,
                ASAPCallId = i.ASAPCallId,
                ContentType = i.ContentType,
                DocumentFileName = i.DocumentFileName
            }).ToList();

            return Json(documents);
        }

        public ActionResult DeleteASAPCallDocument(int documentId)
        {

            Context.ASAPCallDocuments.Remove(Context.ASAPCallDocuments.Single(d => d.ASAPCallDocumentId == documentId));
            Context.SaveChanges();

            return Json(new SaveResultViewModel { Success = true });
        }
    }
}