using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.HR.Models.ViewModel
{
	public class DocumentViewModel
	{
		public int Id { get; set; }
		public int ParentId { get; set; }
		public string DocumentFileName { get; set; }
		public string ContentType { get; set; }
		public IFormFile Document { get; set; }
		public string Type { get; set; }
		public bool Delete { get; set; }

		public static implicit operator DocumentViewModel(EmployeeNewHireChecklistDocument document)
		{
			var vm = new DocumentViewModel
			{
				Id = document.EmployeeNewHireChecklistDocumentId,
				ParentId = document.EmployeeNewHireChecklistId,
				DocumentFileName = document.DocumentFileName,
				ContentType = document.DocumentFileName,
				Type = "inspection"
			};
			return vm;
		}

		public static implicit operator EmployeeNewHireChecklistDocument(DocumentViewModel vm)
		{
			var docName = vm.Document.FileName.Split('\\');
			var document = new EmployeeNewHireChecklistDocument()
			{
				EmployeeNewHireChecklistDocumentId = vm.Id,
				EmployeeNewHireChecklistId = vm.ParentId,
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
