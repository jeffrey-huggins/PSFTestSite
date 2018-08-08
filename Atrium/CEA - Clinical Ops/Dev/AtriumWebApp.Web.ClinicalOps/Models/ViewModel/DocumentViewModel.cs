using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Web.ClinicalOps.Models;
using System.IO;

namespace AtriumWebApp.Web.ClinicalOps.Models.ViewModel
{
	public class DocumentViewModel
	{
		public int Id { get; set; }
		public int ParentId { get; set; }
		public string DocumentFileName { get; set; }
		public string ContentType { get; set; }
		public IFormFile Document { get; set; }
		public bool Delete { get; set; }

		public static implicit operator DocumentViewModel(SOCPressureWoundDocument document)
		{
			var vm = new DocumentViewModel
			{
				Id = document.SOCPressureWoundDocumentId,
				ParentId = document.SOCEventId,
				DocumentFileName = document.DocumentFileName,
				ContentType = document.DocumentFileName
			};
			return vm;
		}
		public static implicit operator SOCPressureWoundDocument(DocumentViewModel vm)
		{
			var docName = vm.Document.FileName.Split('\\');
			var document = new SOCPressureWoundDocument()
			{
				SOCPressureWoundDocumentId = vm.Id,
				SOCEventId = vm.ParentId,
				ContentType = vm.Document.ContentType,
				DocumentFileName = docName[docName.Length - 1]
			};
			using (var stream = vm.Document.OpenReadStream())
			{
				using (var memoryStream = new MemoryStream())
				{
					stream.CopyTo(memoryStream);
					document.Document = memoryStream.ToArray();
				}
			}
			return document;
		}

        public static implicit operator DocumentViewModel(PatientIncidentEventDocument document)
        {
            var vm = new DocumentViewModel
            {
                Id = document.PatientIncidentEventDocumentId,
                ParentId = document.PatientIncidentEventId,
                DocumentFileName = document.DocumentFileName,
                ContentType = document.DocumentFileName
            };
            return vm;
        }
        public static implicit operator PatientIncidentEventDocument(DocumentViewModel vm)
        {
            var docName = vm.Document.FileName.Split('\\');
            var document = new PatientIncidentEventDocument()
            {
                PatientIncidentEventDocumentId = vm.Id,
                PatientIncidentEventId = vm.ParentId,
                ContentType = vm.Document.ContentType,
                DocumentFileName = docName[docName.Length - 1]
            };
            using (var stream = vm.Document.OpenReadStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
                    document.Document = memoryStream.ToArray();
                }
            }
            return document;
        }

    }
}
