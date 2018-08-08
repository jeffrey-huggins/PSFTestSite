using System;
using System.Collections.Generic;
using System.Linq;
using AtriumWebApp.Models.ViewModel;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.RiskManagement.Models;
using AtriumWebApp.Web.RiskManagement.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.IO;
using LZ4;

namespace AtriumWebApp.Web.RiskManagement.Controllers
{
    [RestrictAccessWithApp(RedirectUrl = "Unauthorized", AppCode = "RMM")]
    public class RMMedicalRecordsController : BaseRiskManagementController
    {
        private const string AppCode = "RMM";
        protected new MedicalRecordsContext Context
        {
            get { return (MedicalRecordsContext)base.Context; }
        }
        public RMMedicalRecordsController(IOptions<AppSettingsConfig> config, MedicalRecordsContext context) : base(config, context)
        {
        }

        public ActionResult Index()
        {
            //Record user access
            LogSession(AppCode);
            //Set Census Date Information and Manipulate when changed
            InitializeCensusDateChangedSessionVariable();
            SetLookbackDays(HttpContext, AppCode);
            ManipulateCensusDate(AppCode);
            if (!Session.TryGetObject("SideBar", out SideBarViewModel sideBar))
            {
                sideBar = SideBarService.InitSideBar(this, AppCode, Context);
            }
            return View(sideBar);
        }

        #region Record
        public IActionResult EditOrCreateRMMRecord(string recordId, int patientId)
        {
            RMMedicalRecordsViewModel vm = new RMMedicalRecordsViewModel
            {
                NewDocument = new DocumentViewModel() { Requestid = recordId },
                NewNote = new MedicalRecordsRequestNotes() { Requestid = recordId }
            };
            if (!string.IsNullOrEmpty(recordId))
            {
                var record = Context.RecordRequests.Find(recordId);
                vm.Request = record;
                vm.Notes = Context.MedicalRecordsRequestNotes.Where(a => a.Requestid == recordId).ToList() ;
                List<DocumentViewModel> documents = new List<DocumentViewModel>();
                //prevent retrieving the byte[] column, it is only needed when downloading.
                var requestDocuments = Context.MedicalRequestDocuments.Where(a => a.Requestid == recordId)
                    .Select(c => new { c.DocumentFileName, c.MedicalRecordsRequestDocumentId, c.Requestid, c.SentDate }).ToList();
                foreach (var document in requestDocuments)
                {
                    documents.Add(new DocumentViewModel()
                    {
                        Requestid = document.Requestid,
                        DocumentFileName = document.DocumentFileName,
                        SentDate = document.SentDate,
                        Id = document.MedicalRecordsRequestDocumentId
                    });
                }
                vm.Documents = documents;
            }
            else
            {
                MedicalRecordsRequest record = new MedicalRecordsRequest()
                {
                    PatientId = patientId,
                    LastUser = User.Identity.Name
                };
                vm.Request = record;
                vm.Notes = new List<MedicalRecordsRequestNotes>();
                vm.Documents = new List<DocumentViewModel>();
                
            }
            return EditorFor(vm);
        }

        public IActionResult SaveRMMRecord(RMMedicalRecordsViewModel vm)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var request = vm.Request;
                    if (!string.IsNullOrEmpty(request.Requestid))
                    {
                        var currentRecord = Context.RecordRequests.Find(request.Requestid);
                        Context.Entry(currentRecord).CurrentValues.SetValues(request);
                    }
                    else
                    {
                        request.Requestid = User.Identity.Name + DateTime.Now.ToString("yyyyMMddHHmmss");
                        Context.RecordRequests.Add(request);
                    }
                    Context.SaveChanges();
                }
                catch(Exception ex)
                {
                    return Json(new { success = false, data = ex.Message });
                }
                if (vm.Documents != null)
                {
                    foreach (var doc in vm.Documents)
                    {
                        var currentDoc = Context.MedicalRequestDocuments.Find(doc.Id);
                        currentDoc.SentDate = doc.SentDate;

                    }
                }
                Context.SaveChanges();
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, data = "Please double check that all fields are properly filled out." });
            }

        }

        public IActionResult RMMRecordList(int patientId)
        {
            var records = Context.RecordRequests.Where(a => a.PatientId == patientId).ToList();
            return PartialView(records);
        }

        public JsonResult DeleteRow(string rowId)
        {
            var recordRequest = Context.RecordRequests.Find(rowId);
            if (recordRequest == null)
            {
                return Json(new SaveResultViewModel { Success = false });
            }
            Context.RecordRequests.Remove(recordRequest);
            try
            {
                Context.SaveChanges();
            }
            catch
            {
                return Json(new SaveResultViewModel { Success = false });
            }

            return Json(new SaveResultViewModel { Success = true });
        }

        #endregion
        #region Documents

        [HttpPost]
        public IActionResult SaveDocument([FromForm]DocumentViewModel vm)
        {

            if (vm.Id == 0)
            {
                if (vm.Document != null)
                {
                    foreach (var file in vm.Document)
                    {
                        var docName = string.Empty;
                        var contentType = string.Empty;
                        var path = file.FileName.Split('\\');
                        docName = path[path.Length - 1];
                        contentType = file.ContentType;

                        var document = new MedicalRecordsRequestDocument()
                        {
                            MedicalRecordsRequestDocumentId = vm.Id,
                            Requestid = vm.Requestid,
                            ContentType = contentType,
                            DocumentFileName = docName,
                            SentDate = DateTime.Now
                        };
                        using (var stream = file.OpenReadStream())
                        using (var memStream = new MemoryStream())
                        {
                            stream.CopyTo(memStream);
                            document.Document = LZ4Codec.Wrap(memStream.ToArray());
                        }

                        Context.MedicalRequestDocuments.Add(document);
                        Context.SaveChanges();

                    }
                }

            }
            else
            {
                var currentDoc = Context.MedicalRequestDocuments.Find(vm.Id);
                currentDoc.SentDate = vm.SentDate;
                Context.SaveChanges();
            }
            return Content("{\"Success\":true}");

        }

        public IActionResult StreamDocument(int id)
        {

            var document = Context.MedicalRequestDocuments.SingleOrDefault(d => d.MedicalRecordsRequestDocumentId == id);

            if (document == null)
            {
                ModelState.AddModelError("DocumentMissing", "Document could not be found");
            }
            document.Document = LZ4Codec.Unwrap(document.Document);
            return File(document.Document, document.ContentType, document.DocumentFileName);

        }

        public IActionResult StreamRaw(int id)
        {
            var document = Context.MedicalRequestDocuments.SingleOrDefault(d => d.MedicalRecordsRequestDocumentId == id);

            if (document == null)
            {
                ModelState.AddModelError("DocumentMissing", "Document could not be found");
            }
            return File(document.Document, document.ContentType, document.DocumentFileName);

        }

        public IActionResult DeleteDocument(int documentId)
        {
            Context.MedicalRequestDocuments.Remove(Context.MedicalRequestDocuments.Single(d => d.MedicalRecordsRequestDocumentId == documentId));
            Context.SaveChanges();

            return Json(new { success = true });

        }

        public IActionResult DocumentList(string requestid)
        {

            List<DocumentViewModel> documents = new List<DocumentViewModel>();
            //prevent retrieving the byte[] column, it is only needed when downloading.
            var requestDocuments = Context.MedicalRequestDocuments.Where(a => a.Requestid == requestid)
                .Select(c => new { c.DocumentFileName, c.MedicalRecordsRequestDocumentId, c.Requestid, c.SentDate }).ToList();
            foreach (var document in requestDocuments)
            {
                documents.Add(new DocumentViewModel()
                {
                    Requestid = document.Requestid,
                    DocumentFileName = document.DocumentFileName,
                    SentDate = document.SentDate,
                    Id = document.MedicalRecordsRequestDocumentId
                });
            }
            return PartialView(documents);


        }

        #endregion
        #region Notes
        public IActionResult RMMRecordNotesList(string requestid)
        {
            return PartialView(Context.MedicalRecordsRequestNotes.Where(a => a.Requestid == requestid).ToList());
        }

        public IActionResult EditOrCreateRecordNote(string requestid, int? notesId)
        {
            if (notesId.HasValue)
            {
                var note = Context.MedicalRecordsRequestNotes.Find(notesId.Value);
                if (note != null)
                {
                    return EditorFor(note);
                }
            }
            MedicalRecordsRequestNotes newNote = new MedicalRecordsRequestNotes()
            {
                Requestid = requestid
            };
            return EditorFor(newNote);
        }

        public IActionResult SaveRecordNote(MedicalRecordsRequestNotes note)
        {
            if(note.RequestNoteId != 0)
            {
                var currentNote = Context.MedicalRecordsRequestNotes.Find(note.RequestNoteId);
                Context.Entry(currentNote).CurrentValues.SetValues(note);
            }
            else
            {
                note.InsertedDate = DateTime.Now;
                note.UserName = User.Identity.Name;
                Context.MedicalRecordsRequestNotes.Add(note);
            }
            Context.SaveChanges();
            return Json(new { success = true });
        }

        public IActionResult DeleteRecordNote(int noteId)
        {
            var note = Context.MedicalRecordsRequestNotes.Find(noteId);
            if(note == null)
            {
                return Json(new { success = false, data = "Unable to locate note with that id." });
            }
            Context.MedicalRecordsRequestNotes.Remove(note);
            Context.SaveChanges();
            return Json(new { success = true });
        }

        #endregion

    }
}