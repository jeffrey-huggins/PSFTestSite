using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AtriumWebApp.Models;
using AtriumWebApp.Models.Budget;
using AtriumWebApp.Models.Budget.ViewModels;
using AtriumWebApp.Web.Budget.Library;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using OfficeOpenXml;

namespace AtriumWebApp.Web.Budget.Controllers
{
    public class AdminExportController : Controller
    {
        BudgetMasterIntactEntities BudgetContext { get; set; }
        delegate string ProcessTask(string id, List<int> facilities);
        ExportIntactToExcel Export = new ExportIntactToExcel();

        public AdminExportController(BudgetMasterIntactEntities budgetContext)
        {
            BudgetContext = budgetContext;
        }

        public IActionResult RebuildImportFile()
        {
            BudgetContext.spDropCreateTbl_IntactImportFile_Base1();
            BudgetContext.spDropCreateTbl_IntactImportFile_TEMP();
            BudgetContext.spIntactImportFile_Base1_Load();
            BudgetContext.spIntactImportFile_Temp_Load();
            return Json(new { Success = true });
        }

        public IActionResult ExportFacilitiesToTempTable(string facilityList)
        {

            BudgetContext.IntactImport_ExportSelectedFacilities(facilityList);
            DataTable table = new DataTable();

            DbProviderFactory dbFactory = DbProviderFactories.GetFactory(BudgetContext.Database.Connection);
            using (var cmd = dbFactory.CreateCommand())
            {
                cmd.Connection = BudgetContext.Database.Connection;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "SELECT * FROM ExportedFacilities_Results";
                using (DbDataAdapter adapter = dbFactory.CreateDataAdapter())
                {
                    adapter.SelectCommand = cmd;
                    adapter.Fill(table);
                }
            }


            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = table.Columns.Cast<DataColumn>().Select(a => a.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));
            foreach (DataRow row in table.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(a => string.Concat("\"", a.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(",", fields));
            }
            FileContentResult result = new FileContentResult(Encoding.ASCII.GetBytes(sb.ToString()), "application/octet-stream");

            result.FileDownloadName = "IntactExport_" + DateTime.Now.ToString("yyyy-M-d") + ".csv";

            return result;
        }

        public IActionResult ExportCompanyToExcel()
        {
            string id = HttpContext.Session.Id;
            Export.Add(id);
            ProcessTask processTask = new ProcessTask(Export.CreateCompanyFiles);
            processTask.BeginInvoke(id, null, EndExport, processTask);
            return Json(new { Success = true, Id = id });
        }

        public IActionResult ExportRegionsToExcel(int[] regions)
        {
            List<int> selectedRegions = regions.ToList();
            if (regions.Length == 0)
            {
                selectedRegions = BudgetContext.Regions.Select(a => a.RegionID).ToList();
            }

            string id = HttpContext.Session.Id;
            Export.Add(id);
            ProcessTask processTask = new ProcessTask(Export.CreateRegionFiles);
            processTask.BeginInvoke(id, selectedRegions, EndExport, processTask);
            return Json(new { Success = true, Id = id });

        }

        public IActionResult ExportFacilitiesToExcel(int[] facilities)
        {
            List<int> selectedFacilities = facilities.ToList();
            if(facilities.Length == 0)
            {
                selectedFacilities = BudgetContext.Facilities.Select(a => a.FacilityID).ToList();
            }
            string id = HttpContext.Session.Id;
            Export.Add(id);
            ProcessTask processTask = new ProcessTask(Export.CreateFacilityFiles);
            processTask.BeginInvoke(id, selectedFacilities, EndExport, processTask);
            return Json(new { Success = true, Id = id });
        }

        public void EndExport(IAsyncResult result)
        {
            ProcessTask processTask = (ProcessTask)result.AsyncState;
            processTask.EndInvoke(result);
        }

        public IActionResult GetExportFileStatus(string id)
        {
            ExcelFileExportStatus status = Export.GetStatus(id);
            
            return Json(new { Status = status.ProgressStatus, Progress = status.Progress });
        }

        public IActionResult GetExportFile(string id)
        {
            var result = Export.GetResult(id);
            if (result != null)
            {
                Export.Remove(id);
                return result;
            }
            return Json(new { Status = "Error" });
        }

        public IActionResult CSVExport()
        {
            return PartialView(BudgetContext.Facilities.ToList());
        }
        public IActionResult ExcelExport()
        {
            ExcelExportViewModel viewModel = new ExcelExportViewModel()
            {
                Facilities = BudgetContext.Facilities.ToList(),
                Regions = BudgetContext.Regions.ToList()
            };
            return PartialView(viewModel);
        }
    }
}