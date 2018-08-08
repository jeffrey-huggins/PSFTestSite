using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using AtriumWebApp.Models.Budget;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace AtriumWebApp.Web.Budget.Library
{
    public class ExportIntactToExcel
    {
        private static object syncRoot = new object();
        private static Dictionary<string, ExcelFileExportStatus> ProcessStatus { get; set; }

        public ExportIntactToExcel()
        {
            if (ProcessStatus == null)
            {
                ProcessStatus = new Dictionary<string, ExcelFileExportStatus>();
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
                ExcelFileExportStatus status = new ExcelFileExportStatus()
                {
                    StartTime = DateTime.Now
                };
                ProcessStatus.Add(id, status);
            }
        }
        public string CreateCompanyFiles(string id,List<int> companies)
        {
            using (var budgetContext = new BudgetMasterIntactEntities(BudgetMasterIntactEntities.connection))
            {
                using (var zipFile = new MemoryStream())
                {
                    using (var archive = new ZipArchive(zipFile, ZipArchiveMode.Create, true))
                    {
                        lock (syncRoot)
                        {
                            ProcessStatus[id].ProgressStatus = "Obtaining Data";
                            ProcessStatus[id].Progress = 0;
                        }
                        char[] monthColumnMapping = { '-', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N' };
                        var results = budgetContext.fnAtriumPayPeriodExport()
                            .OrderBy(a => a.LaborExpenseGLGrpNm).ThenBy(a => a.GLAccountNb).ThenBy(a => a.MonthNb).ToList();
                        var numGLAccounts = results.GroupBy(a => a.GLAccountNm).ToList().Select(a => a.FirstOrDefault())
                            .OrderBy(a => a.LaborExpenseGLGrpNm).ThenBy(a => a.GLAccountNb).ThenBy(a => a.MonthNb).ToList();
                        int numNurse = results.Where(a => a.LaborExpenseGLGrpNm == "Nursing").Select(a => a.GLAccountNm).Distinct().Count() / 4;
                        //new excel for each facility
                        using (ExcelPackage package = new ExcelPackage())
                        {
                            package.Workbook.Worksheets.Add("Total Budget");
                            package.Workbook.Worksheets.Add("SNF Budget");
                            package.Workbook.Worksheets.Add("IL Budget");
                            package.Workbook.Worksheets.Add("RST Budget");
                            package.Workbook.Worksheets.Add("MRD Budget");
                            lock (syncRoot)
                            {
                                ProcessStatus[id].ProgressStatus = "Writing Headers";
                                ProcessStatus[id].Progress = 25;
                            }
                            int rowSkip = 4;
                            WriteHeadersToExcel(1, numGLAccounts.Count() / 4, package.Workbook.Worksheets);
                            lock (syncRoot)
                            {
                                ProcessStatus[id].ProgressStatus = "Writing Totals";
                                ProcessStatus[id].Progress = 50;
                            }
                            for (var i = 2; i <= numGLAccounts.Count() / 4 + 1; i++)
                            {
                                for (var j = 0; j <= 2; j++)
                                {
                                    int row = (j * (numGLAccounts.Count() / 4 + rowSkip) + i);
                                    package.Workbook.Worksheets[1].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".XX";
                                    package.Workbook.Worksheets[2].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".00";
                                    package.Workbook.Worksheets[3].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".15";
                                    package.Workbook.Worksheets[4].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".70";
                                    package.Workbook.Worksheets[5].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".90";
                                    foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                                    {
                                        worksheet.Cells[row, 2].Value = numGLAccounts[(i - 2) * 4].GLAccountNm;
                                        if (worksheet.Index > 1)
                                        {
                                            for (var k = 1; k <= 12; k++)
                                            {
                                                string accountNum = worksheet.Cells[row, 1].Value.ToString();
                                                var record = results.FirstOrDefault(a => a.MonthNb == k && a.GLAccountNb == accountNum);
                                                if (j == 0)
                                                {
                                                    worksheet.Cells[row, 2 + k].Value = record.HoursPPD;
                                                }
                                                else if (j == 1)
                                                {
                                                    worksheet.Cells[row, 2 + k].Value = record.DollarPPD;
                                                    worksheet.Cells[row, 2 + k].Style.Numberformat.Format = "$0.00";
                                                }
                                                else
                                                {
                                                    worksheet.Cells[row, 2 + k].Value = record.AWR;
                                                    worksheet.Cells[row, 2 + k].Style.Numberformat.Format = "$0.00";
                                                }
                                            }
                                        }
                                        else
                                        {
                                            for (var k = 1; k <= 12; k++)
                                            {
                                                string cellLocation = monthColumnMapping[k] + row.ToString();
                                                var formula = String.Format("='SNF Budget'!{0} + 'IL Budget'!{0} + 'RST Budget'!{0} + 'MRD Budget'!{0}", cellLocation);
                                                worksheet.Cells[row, 2 + k].Formula = formula;
                                                if (j > 0)
                                                {
                                                    worksheet.Cells[row, 2 + k].Style.Numberformat.Format = "$0.00";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            int startRow = 2;
                            int rowLocation = numNurse + startRow;
                            int rowsAdded = 0;
                            NursingTotals(startRow, rowLocation, package.Workbook.Worksheets, monthColumnMapping, false);
                            rowsAdded++;
                            int totalRow = startRow + 1 + (numGLAccounts.Count() / 4);
                            OtherTotals(rowLocation + 1, totalRow, package.Workbook.Worksheets, monthColumnMapping, false, "Total Hours");

                            startRow = (numGLAccounts.Count() / 4) + rowSkip + rowsAdded + 2;
                            rowLocation = numNurse + startRow;
                            NursingTotals(startRow, rowLocation, package.Workbook.Worksheets, monthColumnMapping, true);
                            rowsAdded++;
                            totalRow = startRow + 1 + (numGLAccounts.Count() / 4);
                            OtherTotals(rowLocation + 1, totalRow, package.Workbook.Worksheets, monthColumnMapping, true, "Total Dollars");

                            startRow = ((numGLAccounts.Count() / 4) + rowSkip) * 2 + rowsAdded + 2;
                            rowLocation = numNurse + startRow;
                            NursingTotals(startRow, rowLocation, package.Workbook.Worksheets, monthColumnMapping, true);
                            rowsAdded++;
                            totalRow = startRow + 1 + (numGLAccounts.Count() / 4);
                            OtherTotals(rowLocation + 1, totalRow, package.Workbook.Worksheets, monthColumnMapping, true, "Total AWR");
                            lock (syncRoot)
                            {
                                ProcessStatus[id].ProgressStatus = "Completing";
                                ProcessStatus[id].Progress = 75;
                            }
                            var newFile = archive.CreateEntry("Atrium.xlsx");
                            var stream = newFile.Open();
                            package.SaveAs(stream);
                            stream.Dispose();
                        }

                        //add to zip
                    }



                    FileContentResult result = new FileContentResult(zipFile.ToArray(), "application/octet-stream")
                    {
                        FileDownloadName = "Intact.zip"
                    };
                    lock (syncRoot)
                    {
                        ProcessStatus[id].Progress = 100;
                        ProcessStatus[id].ProgressStatus = "Completed";
                        ProcessStatus[id].Result = result;
                    }
                }
            }
            return id;
        }
        public string CreateRegionFiles(string id, List<int> regions)
        {
            using (var budgetContext = new BudgetMasterIntactEntities(BudgetMasterIntactEntities.connection))
            {
                using (var zipFile = new MemoryStream())
                {
                    using (var archive = new ZipArchive(zipFile, ZipArchiveMode.Create, true))
                    {
                        int progress = 0;
                        int completedRegions = 0;
                        char[] monthColumnMapping = { '-', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N' };
                        foreach (var region in regions)
                        {
                            progress = (int)((completedRegions / (float)regions.Count) * 100);
                            lock (syncRoot)
                            {
                                ProcessStatus[id].ProgressStatus = String.Format("{0} of {1} facilities completed.", completedRegions, regions.Count);
                                ProcessStatus[id].Progress = progress;
                            }
                            var results = budgetContext.fnRegionsPayPeriodExport()
                                .Where(a => a.RegionID == region)
                                .OrderBy(a => a.RegionID).ThenBy(a => a.LaborExpenseGLGrpNm).ThenBy(a => a.GLAccountNb).ThenBy(a => a.MonthNb).ToList();
                            var numGLAccounts = results.GroupBy(a => a.GLAccountNm).ToList().Select(a => a.FirstOrDefault())
                                .OrderBy(a => a.RegionID).ThenBy(a => a.LaborExpenseGLGrpNm).ThenBy(a => a.GLAccountNb).ThenBy(a => a.MonthNb).ToList();
                            int numNurse = results.Where(a => a.LaborExpenseGLGrpNm == "Nursing").Select(a => a.GLAccountNm).Distinct().Count() / 4;
                            //new excel for each facility
                            using (ExcelPackage package = new ExcelPackage())
                            {
                                package.Workbook.Worksheets.Add("Total Budget");
                                package.Workbook.Worksheets.Add("SNF Budget");
                                package.Workbook.Worksheets.Add("IL Budget");
                                package.Workbook.Worksheets.Add("RST Budget");
                                package.Workbook.Worksheets.Add("MRD Budget");

                                int rowSkip = 4;
                                WriteHeadersToExcel(1, numGLAccounts.Count() / 4, package.Workbook.Worksheets);
                                var fileName = budgetContext.Regions.Find(region).RegionNm;
                                for (var i = 2; i <= numGLAccounts.Count() / 4 + 1; i++)
                                {
                                    for (var j = 0; j <= 2; j++)
                                    {
                                        int row = (j * (numGLAccounts.Count() / 4 + rowSkip) + i);
                                        package.Workbook.Worksheets[1].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".XX";
                                        package.Workbook.Worksheets[2].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".00";
                                        package.Workbook.Worksheets[3].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".15";
                                        package.Workbook.Worksheets[4].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".70";
                                        package.Workbook.Worksheets[5].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".90";
                                        foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                                        {
                                            worksheet.Cells[row, 2].Value = numGLAccounts[(i - 2) * 4].GLAccountNm;
                                            if (worksheet.Index > 1)
                                            {
                                                for (var k = 1; k <= 12; k++)
                                                {
                                                    string accountNum = worksheet.Cells[row, 1].Value.ToString();
                                                    var record = results.FirstOrDefault(a => a.MonthNb == k && a.GLAccountNb == accountNum && a.RegionID == region);
                                                    if (j == 0)
                                                    {
                                                        worksheet.Cells[row, 2 + k].Value = record.HoursPPD;
                                                    }
                                                    else if (j == 1)
                                                    {
                                                        worksheet.Cells[row, 2 + k].Value = record.DollarPPD;
                                                        worksheet.Cells[row, 2 + k].Style.Numberformat.Format = "$0.00";
                                                    }
                                                    else
                                                    {
                                                        worksheet.Cells[row, 2 + k].Value = record.AWR;
                                                        worksheet.Cells[row, 2 + k].Style.Numberformat.Format = "$0.00";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (var k = 1; k <= 12; k++)
                                                {
                                                    string cellLocation = monthColumnMapping[k] + row.ToString();
                                                    var formula = String.Format("='SNF Budget'!{0} + 'IL Budget'!{0} + 'RST Budget'!{0} + 'MRD Budget'!{0}", cellLocation);
                                                    worksheet.Cells[row, 2 + k].Formula = formula;
                                                    if (j > 0)
                                                    {
                                                        worksheet.Cells[row, 2 + k].Style.Numberformat.Format = "$0.00";
                                                    }
                                                }
                                            }
                                        }

                                    }

                                    //package.Workbook.Worksheets
                                }

                                int startRow = 2;
                                int rowLocation = numNurse + startRow;
                                int rowsAdded = 0;
                                NursingTotals(startRow, rowLocation, package.Workbook.Worksheets, monthColumnMapping, false);
                                rowsAdded++;
                                int totalRow = startRow + 1 + (numGLAccounts.Count() / 4);
                                OtherTotals(rowLocation + 1, totalRow, package.Workbook.Worksheets, monthColumnMapping, false, "Total Hours");

                                startRow = (numGLAccounts.Count() / 4) + rowSkip + rowsAdded + 2;
                                rowLocation = numNurse + startRow;
                                NursingTotals(startRow, rowLocation, package.Workbook.Worksheets, monthColumnMapping, true);
                                rowsAdded++;
                                totalRow = startRow + 1 + (numGLAccounts.Count() / 4);
                                OtherTotals(rowLocation + 1, totalRow, package.Workbook.Worksheets, monthColumnMapping, true, "Total Dollars");

                                startRow = ((numGLAccounts.Count() / 4) + rowSkip) * 2 + rowsAdded + 2;
                                rowLocation = numNurse + startRow;
                                NursingTotals(startRow, rowLocation, package.Workbook.Worksheets, monthColumnMapping, true);
                                rowsAdded++;
                                totalRow = startRow + 1 + (numGLAccounts.Count() / 4);
                                OtherTotals(rowLocation + 1, totalRow, package.Workbook.Worksheets, monthColumnMapping, true, "Total AWR");
                                //Add totals
                                var newFile = archive.CreateEntry(budgetContext.Regions.Find(region).RegionNm + ".xlsx");
                                var stream = newFile.Open();
                                package.SaveAs(stream);
                                stream.Dispose();
                                completedRegions++;
                            }
                        }

                        //add to zip
                    }



                    FileContentResult result = new FileContentResult(zipFile.ToArray(), "application/octet-stream")
                    {
                        FileDownloadName = "Intact.zip"
                    };
                    lock (syncRoot)
                    {
                        ProcessStatus[id].Progress = 100;
                        ProcessStatus[id].ProgressStatus = "Completed";
                        ProcessStatus[id].Result = result;
                    }
                }
            }
            return id;
        }

        public string CreateFacilityFiles(string id, List<int> facilities)
        {
            using (var budgetContext = new BudgetMasterIntactEntities(BudgetMasterIntactEntities.connection))
            {
                using (var zipFile = new MemoryStream())
                {
                    using (var archive = new ZipArchive(zipFile, ZipArchiveMode.Create, true))
                    {
                        int progress = 0;
                        int completedFacilities = 0;
                        var facilityList = facilities;
                        char[] monthColumnMapping = { '-', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N' };
                        
                        foreach (var facility in facilityList)
                        {
                            progress = (int)((completedFacilities / (float)facilityList.Count) * 100);
                            lock (syncRoot)
                            {
                                ProcessStatus[id].ProgressStatus = String.Format("{0} of {1} facilities completed.", completedFacilities, facilityList.Count);
                                ProcessStatus[id].Progress = progress;
                            }
                            var results = budgetContext.fnFacilitiesPayPeriodExport()
                                .Where(a => a.FacilityID.Value == facility)
                                .OrderBy(a => a.FacilityID).ThenBy(a => a.LaborExpenseGLGrpNm).ThenBy(a => a.GLAccountNb).ThenBy(a => a.MonthNb).ToList();
                            var numGLAccounts = results.GroupBy(a => a.GLAccountNm).ToList().Select(a => a.FirstOrDefault())
                                .OrderBy(a => a.FacilityID).ThenBy(a => a.LaborExpenseGLGrpNm).ThenBy(a => a.GLAccountNb).ThenBy(a => a.MonthNb).ToList();
                            int numNurse = results.Where(a => a.LaborExpenseGLGrpNm == "Nursing").Select(a => a.GLAccountNm).Distinct().Count() / 4;
                            //new excel for each facility
                            using (ExcelPackage package = new ExcelPackage())
                            {
                                package.Workbook.Worksheets.Add("Total Budget");
                                package.Workbook.Worksheets.Add("SNF Budget");
                                package.Workbook.Worksheets.Add("IL Budget");
                                package.Workbook.Worksheets.Add("RST Budget");
                                package.Workbook.Worksheets.Add("MRD Budget");

                                int rowSkip = 4;
                                WriteHeadersToExcel(1, numGLAccounts.Count() / 4, package.Workbook.Worksheets);
                                var fileName = budgetContext.Facilities.Find(facility).FacilityShortNm;
                                for (var i = 2; i <= numGLAccounts.Count() / 4 + 1; i++)
                                {
                                    for (var j = 0; j <= 2; j++)
                                    {
                                        int row = (j * (numGLAccounts.Count() / 4 + rowSkip) + i);
                                        package.Workbook.Worksheets[1].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".XX";
                                        package.Workbook.Worksheets[2].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".00";
                                        package.Workbook.Worksheets[3].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".15";
                                        package.Workbook.Worksheets[4].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".70";
                                        package.Workbook.Worksheets[5].Cells[row, 1].Value = numGLAccounts[(i - 2) * 4].GLAccountNb.Substring(0, 4) + ".90";
                                        foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
                                        {
                                            worksheet.Cells[row, 2].Value = numGLAccounts[(i - 2) * 4].GLAccountNm;
                                            if (worksheet.Index > 1)
                                            {
                                                for (var k = 1; k <= 12; k++)
                                                {
                                                    string accountNum = worksheet.Cells[row, 1].Value.ToString();
                                                    var record = results.FirstOrDefault(a => a.MonthNb == k && a.GLAccountNb == accountNum && a.FacilityID == facility);
                                                    if (j == 0)
                                                    {
                                                        worksheet.Cells[row, 2 + k].Value = record.HoursPPD;
                                                    }
                                                    else if (j == 1)
                                                    {
                                                        worksheet.Cells[row, 2 + k].Value = record.DollarPPD;
                                                        worksheet.Cells[row, 2 + k].Style.Numberformat.Format = "$0.00";
                                                    }
                                                    else
                                                    {
                                                        worksheet.Cells[row, 2 + k].Value = record.AWR;
                                                        worksheet.Cells[row, 2 + k].Style.Numberformat.Format = "$0.00";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                for (var k = 1; k <= 12; k++)
                                                {
                                                    string cellLocation = monthColumnMapping[k] + row.ToString();
                                                    var formula = String.Format("='SNF Budget'!{0} + 'IL Budget'!{0} + 'RST Budget'!{0} + 'MRD Budget'!{0}", cellLocation);
                                                    worksheet.Cells[row, 2 + k].Formula = formula;
                                                    if (j > 0)
                                                    {
                                                        worksheet.Cells[row, 2 + k].Style.Numberformat.Format = "$0.00";
                                                    }
                                                }
                                            }
                                        }

                                    }

                                    //package.Workbook.Worksheets
                                }

                                int startRow = 2;
                                int rowLocation = numNurse + startRow;
                                int rowsAdded = 0;
                                NursingTotals(startRow, rowLocation, package.Workbook.Worksheets, monthColumnMapping, false);
                                rowsAdded++;
                                int totalRow = startRow + 1 + (numGLAccounts.Count() / 4);
                                OtherTotals(rowLocation + 1, totalRow, package.Workbook.Worksheets, monthColumnMapping, false, "Total Hours");

                                startRow = (numGLAccounts.Count() / 4) + rowSkip + rowsAdded + 2;
                                rowLocation = numNurse + startRow;
                                NursingTotals(startRow, rowLocation, package.Workbook.Worksheets, monthColumnMapping, true);
                                rowsAdded++;
                                totalRow = startRow + 1 + (numGLAccounts.Count() / 4);
                                OtherTotals(rowLocation + 1, totalRow, package.Workbook.Worksheets, monthColumnMapping, true, "Total Dollars");

                                startRow = ((numGLAccounts.Count() / 4) + rowSkip) * 2 + rowsAdded + 2;
                                rowLocation = numNurse + startRow;
                                NursingTotals(startRow, rowLocation, package.Workbook.Worksheets, monthColumnMapping, true);
                                rowsAdded++;
                                totalRow = startRow + 1 + (numGLAccounts.Count() / 4);
                                OtherTotals(rowLocation + 1, totalRow, package.Workbook.Worksheets, monthColumnMapping, true, "Total AWR");
                                //Add totals
                                var newFile = archive.CreateEntry(budgetContext.Facilities.Find(facility).FacilityShortNm + ".xlsx");
                                var stream = newFile.Open();
                                package.SaveAs(stream);
                                stream.Dispose();
                                completedFacilities++;
                            }
                        }

                        //add to zip
                    }



                    FileContentResult result = new FileContentResult(zipFile.ToArray(), "application/octet-stream")
                    {
                        FileDownloadName = "Intact.zip"
                    };
                    lock (syncRoot)
                    {
                        ProcessStatus[id].Progress = 100;
                        ProcessStatus[id].ProgressStatus = "Completed";
                        ProcessStatus[id].Result = result;
                    }
                }
            }
            return id;
        }

        public ExcelFileExportStatus GetStatus(string id)
        {

            lock (syncRoot)
            {
                if (ProcessStatus.ContainsKey(id))
                {
                    return ProcessStatus[id];
                }
                else
                {
                    return new ExcelFileExportStatus() { Progress = -1, ProgressStatus = "Error" };
                }
            }
        }

        public FileContentResult GetResult(string id)
        {
            lock (syncRoot)
            {
                if (ProcessStatus.ContainsKey(id))
                {
                    return ProcessStatus[id].Result;
                }
                else
                {
                    return null;
                }
            }
        }

        public void OtherTotals(int startRow, int rowNum, ExcelWorksheets worksheets, char[] monthColumnMapping, bool moneyFormat, string totalName)
        {
            foreach (ExcelWorksheet worksheet in worksheets)
            {
                worksheet.Cells[rowNum, 2].Value = "Other Totals";
                worksheet.Cells[rowNum + 1, 2].Value = totalName;
                for (var i = 1; i <= 12; i++)
                {
                    string startCellLocation = monthColumnMapping[i] + startRow.ToString();
                    string endCellLocation = monthColumnMapping[i] + (rowNum - 1).ToString();
                    worksheet.Cells[rowNum, i + 2].Formula = String.Format("=Sum({0}:{1})", startCellLocation, endCellLocation);
                    if (moneyFormat)
                    {
                        worksheet.Cells[rowNum, i + 2].Style.Numberformat.Format = "$0.00";
                    }
                    string nurseTotal = monthColumnMapping[i] + (startRow - 1).ToString();
                    string otherTotal = monthColumnMapping[i] + rowNum.ToString();
                    worksheet.Cells[rowNum + 1, i + 2].Formula = String.Format("=Sum({0},{1})", nurseTotal, otherTotal);
                    if (moneyFormat)
                    {
                        worksheet.Cells[rowNum + 1, i + 2].Style.Numberformat.Format = "$0.00";
                    }
                }
            }
        }

        public void NursingTotals(int startRow, int rowNum, ExcelWorksheets worksheets, char[] monthColumnMapping, bool moneyFormat)
        {
            foreach (ExcelWorksheet worksheet in worksheets)
            {
                worksheet.InsertRow(rowNum, 1);
                worksheet.Cells[rowNum, 2].Value = "Nursing Totals";
                for (var i = 1; i <= 12; i++)
                {
                    string startCellLocation = monthColumnMapping[i] + startRow.ToString();
                    string endCellLocation = monthColumnMapping[i] + (rowNum - 1).ToString();
                    worksheet.Cells[rowNum, i + 2].Formula = String.Format("=Sum({0}:{1})", startCellLocation, endCellLocation);
                    if (moneyFormat)
                    {
                        worksheet.Cells[rowNum, i + 2].Style.Numberformat.Format = "$0.00";
                    }
                }
            }
        }

        public void WriteHeadersToExcel(int rowNum, int numGLAccounts, ExcelWorksheets worksheets)
        {
            int skipRows = 4;
            for (int i = 1; i <= worksheets.Count; i++)
            {
                worksheets[i].Cells[rowNum, 1].Value = "HOURS PPD";
                worksheets[i].Cells[(numGLAccounts + skipRows) + rowNum, 1].Value = "DOLLARS PPD";
                worksheets[i].Cells[(2 * (numGLAccounts + skipRows)) + rowNum, 1].Value = "AWR";
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 3; k < 15; k++)
                    {
                        worksheets[i].Cells[(j * ((numGLAccounts + skipRows)) + rowNum), k].Value = k - 2;
                    }
                }

            }
        }


    }

    public class ExcelFileExportStatus
    {
        public DateTime StartTime { get; set; }
        public int Progress { get; set; }
        public string ProgressStatus { get; set; }
        public FileContentResult Result { get; set; }
    }

}
