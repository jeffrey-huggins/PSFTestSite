using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AtriumWebApp.Web.Maintenance.Models.ViewModels
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

        public static implicit operator DocumentViewModel(EquipmentInspectionDocument document)
        {
            var vm = new DocumentViewModel
            {
                Id = document.EquipmentInspectionDocumentId,
                ParentId = document.EquipmentInspectionId,
                DocumentFileName = document.DocumentFileName,
                ContentType = document.DocumentFileName,
                Type = "inspection"
            };
            return vm;
        }

        public static implicit operator DocumentViewModel(EquipmentRepairDocument document)
        {
            var vm = new DocumentViewModel
            {
                Id = document.EquipmentRepairDocumentId,
                ParentId = document.EquipmentRepairId,
                DocumentFileName = document.DocumentFileName,
                ContentType = document.ContentType,
                Type = "repair"
            };
            return vm;
        }

        public static implicit operator EquipmentInspectionDocument(DocumentViewModel vm)
        {
            var docName = vm.Document.FileName.Split('\\');
            var document = new EquipmentInspectionDocument()
            {
                EquipmentInspectionDocumentId = vm.Id,
                EquipmentInspectionId = vm.ParentId,
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
        public static implicit operator EquipmentRepairDocument(DocumentViewModel vm)
        {
            var docName = vm.Document.FileName.Split('\\');
            var document = new EquipmentRepairDocument()
            {
                EquipmentRepairDocumentId = vm.Id,
                EquipmentRepairId = vm.ParentId,
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
