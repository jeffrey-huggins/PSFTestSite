using AtriumWebApp.Web.Base.Library;
using LZ4;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.RiskManagement.Models.ViewModel
{
    public class DocumentViewModel
    {
        public int Id { get; set; }
        public string Requestid { get; set; }
        public string DocumentFileName { get; set; }
        public string ContentType { get; set; }
        public List<IFormFile> Document { get; set; }
        public DateTime SentDate { get; set; }
        public bool Delete { get; set; }

        public static implicit operator DocumentViewModel(MedicalRecordsRequestDocument document)
        {
            var vm = new DocumentViewModel
            {
                Id = document.MedicalRecordsRequestDocumentId,
                Requestid = document.Requestid,
                DocumentFileName = document.DocumentFileName,
                ContentType = document.DocumentFileName,
                SentDate = document.SentDate
            };
            return vm;
        }

        //public static implicit operator MedicalRecordsRequestDocument(DocumentViewModel vm)
        //{
        //    var docName = string.Empty;
        //    var contentType = string.Empty;
        //    if (vm.Document != null)
        //    {
        //        var path = vm.Document.FileName.Split('\\');
        //        docName = path[path.Length - 1];
        //        contentType = vm.Document.ContentType;
        //    }
        //    var document = new MedicalRecordsRequestDocument()
        //    {
        //        MedicalRecordsRequestDocumentId = vm.Id,
        //        Requestid = vm.Requestid,
        //        ContentType = contentType,
        //        DocumentFileName = docName,
        //        SentDate = vm.SentDate
        //    };
        //    if (vm.Document != null)
        //    {
        //        using (var stream = vm.Document.OpenReadStream())
        //        using (var memStream = new MemoryStream())
        //        {
        //            stream.CopyTo(memStream);
        //            document.Document = LZ4Codec.Wrap(memStream.ToArray());
        //        }
        //    }
        //    return document;
        //}

    }
}
