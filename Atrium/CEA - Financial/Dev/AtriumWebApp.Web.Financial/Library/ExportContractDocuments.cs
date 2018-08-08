using AtriumWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Financial.Library
{
    public class ExportContractDocuments
    {
        private static object syncRoot = new object();
        public static Dictionary<string, ExportContractDocumentsStatus> ProcessStatus { get; set; }

        public ExportContractDocuments()
        {
            if (ProcessStatus == null)
            {
                ProcessStatus = new Dictionary<string, ExportContractDocumentsStatus>();
            }
        }

        public void Remove(string id)
        {
            lock (syncRoot)
            {
                if (ProcessStatus.ContainsKey(id))
                {
                    ProcessStatus.Remove(id);
                }
            }
        }

        public void Add(string id)
        {
            lock (syncRoot)
            {
                if (ProcessStatus.ContainsKey(id))
                {
                    ProcessStatus.Remove(id);
                }
                ExportContractDocumentsStatus status = new ExportContractDocumentsStatus()
                {
                    StartTime = DateTime.Now,
                    ProgressStatus = "Initiating",
                    Progress = 0
                };
                ProcessStatus.Add(id, status);
            }
        }

        public string CreateFiles(string id, List<int> facilityIds)
        {
            var Context = new ContractManagementContext(ContractManagementContext.connectionString);

            var facilities = Context.Facilities.Where(a => facilityIds.Contains(a.CommunityId)).ToList();
            int documentCount = Context.Contracts.Where(a => facilityIds.Intersect(a.ContractCommunities.Select(b => b.CommunityId)).Any() && !a.ArchiveFlg).Sum(a => a.ContractDocuments.Count);
            int completedDocuments = 0;
            string path = Path.GetTempPath() + Path.GetRandomFileName() + "/";

            lock (syncRoot)
            {
                ProcessStatus[id].ProgressStatus = "Obtaining Data";
                ProcessStatus[id].Progress = 0;
            }
            foreach (var facility in facilities)
            {
                Context.Dispose();
                Context = new ContractManagementContext(ContractManagementContext.connectionString);
                var contracts = Context.Contracts.Include("Provider").Include("ContractCommunities").Include("ContractDocuments")
                    .Where(a => a.ContractCommunities.Any(b => b.CommunityId == facility.CommunityId) && !a.ArchiveFlg).ToList();
                foreach (var contract in contracts)
                {
                    var contractDocuments = Context.ContractDocuments.Where(a => a.ContractId == contract.Id && !a.ArchiveFlg).ToList();
                    foreach (var contractDocument in contractDocuments)
                    {
                        var directoryPath = path + facility.CommunityShortName + "/" + contract.Provider.Name + "/";
                        Directory.CreateDirectory(directoryPath);
                        string documentName = directoryPath + contractDocument.FileName;
                        if (File.Exists(documentName))
                        {
                            var fileSplit = contractDocument.FileName.Split('.');
                            fileSplit[0] += contractDocument.Id.ToString();
                            documentName = directoryPath + string.Join(".", fileSplit);
                        }
                        using (FileStream stream = new FileStream(documentName, FileMode.CreateNew))
                        {
                            stream.Write(contractDocument.Document, 0, contractDocument.Document.Length);
                        }
                        completedDocuments++;
                        lock (syncRoot)
                        {
                            ProcessStatus[id].ProgressStatus = "Retrieving documents for " + facility.CommunityShortName + "(" + completedDocuments + " of " + documentCount + ")";
                            ProcessStatus[id].Progress = (int)(((float)completedDocuments / (float)documentCount) * 100);
                        }
                    }

                }

            }
            lock (syncRoot)
            {
                ProcessStatus[id].Progress = 99;
                ProcessStatus[id].ProgressStatus = "Compressing";
            }
            var fileName = Path.GetTempPath() + Path.GetRandomFileName();
            ZipFile.CreateFromDirectory(path, fileName);

            lock (syncRoot)
            {
                ProcessStatus[id].Progress = 100;
                ProcessStatus[id].ProgressStatus = "Completed";
                ProcessStatus[id].ResultFileName = fileName;
            }

            Context.Dispose();
            return id;

        }

        public ExportContractDocumentsStatus GetStatus(string id)
        {
            lock (syncRoot)
            {
                if (ProcessStatus.ContainsKey(id))
                {
                    return ProcessStatus[id];
                }
                else
                {
                    return new ExportContractDocumentsStatus() { Progress = -1, ProgressStatus = "Error" };
                }
            }
        }

        public string GetResult(string id)
        {
            lock (syncRoot)
            {
                if (ProcessStatus.ContainsKey(id))
                {
                    return ProcessStatus[id].ResultFileName;
                }
                else
                {
                    return null;
                }
            }
        }

    }

    public class ExportContractDocumentsStatus
    {
        public DateTime StartTime { get; set; }
        public int Progress { get; set; }
        public string ProgressStatus { get; set; }
        public string ResultFileName { get; set; }
    }

}
