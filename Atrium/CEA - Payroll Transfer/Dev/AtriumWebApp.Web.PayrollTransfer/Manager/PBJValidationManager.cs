using AtriumWebApp.Web.PayrollTransfer.Models.ViewModel;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.PayrollTransfer.Manager
{
    public class PBJValidationManager
    {
        List<string> glNumbers = new List<string>();
        List<string> communityCodes = new List<string>();
        List<PBJRow> pbjRows = new List<PBJRow>();
        public List<ValidationErrorsViewModel> errorList = new List<ValidationErrorsViewModel>();
        DateTime quarterStart = new DateTime();
        DateTime quarterEnd = new DateTime();

        public PBJValidationManager(PBJValidationViewModel validationFiles)
        {
            quarterStart = validationFiles.QuarterStart;
            quarterEnd = validationFiles.QuarterEnd;
            try
            {
                using (Stream lookupStream = validationFiles.LookupFile.OpenReadStream())
                {
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        lookupStream.CopyTo(memStream);
                        ExcelPackage lookupFile = new ExcelPackage(memStream);
                        PopulateLookupValues(lookupFile);
                    }
                }
            }
            catch (Exception ex)
            {
                errorList.Add(new ValidationErrorsViewModel()
                {
                    Error = ex.Message,
                    Row = -1
                });
                return;
            }
            
            try
            {
                using (Stream pbjStream = validationFiles.PBJFile.OpenReadStream())
                {
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        pbjStream.CopyTo(memStream);
                        ExcelPackage pbjFile = new ExcelPackage(memStream);
                        PopulatePBJValues(pbjFile);
                    }
                }


            }
            catch (Exception ex)
            {
                errorList.Add(new ValidationErrorsViewModel()
                {
                    Error = ex.Message,
                    Row = -1
                });
                return;
            }

            ValidateData();
        }

        private void ValidateData()
        {
            foreach(var row in pbjRows)
            {
                if (!communityCodes.Contains(row.CommunityCode))
                {
                    errorList.Add(new ValidationErrorsViewModel()
                    {
                        Error = "Invalid community code",
                        Row = row.RowNumber
                    });
                }
                if (!glNumbers.Contains(row.GLNumber))
                {
                    errorList.Add(new ValidationErrorsViewModel()
                    {
                        Error = "Invalid general ledger code",
                        Row = row.RowNumber
                    });
                }
                if(row.HireDate == DateTime.MinValue)
                {
                    errorList.Add(new ValidationErrorsViewModel()
                    {
                        Error = "Hire Date is in an invalid format",
                        Row = row.RowNumber
                    });
                }
                if(row.WorkDate < quarterStart)
                {
                    if (row.WorkDate == DateTime.MinValue)
                    {
                        errorList.Add(new ValidationErrorsViewModel()
                        {
                            Error = "Work Date is in an invalid format",
                            Row = row.RowNumber
                        });
                    }
                    else
                    {
                        errorList.Add(new ValidationErrorsViewModel()
                        {
                            Error = "Work Date is earlier than the start of the quarter",
                            Row = row.RowNumber
                        });
                    }

                }
                if (row.WorkDate > quarterEnd)
                {
                    errorList.Add(new ValidationErrorsViewModel()
                    {
                        Error = "Work Date is later than the end of the quarter",
                        Row = row.RowNumber
                    });
                }
                if(row.Hours < 0 || row.Hours > 24)
                {
                    errorList.Add(new ValidationErrorsViewModel()
                    {
                        Error = "Hours are not valid",
                        Row = row.RowNumber
                    });
                }
                float totalHours = pbjRows.Where(a => a.FirstName == row.FirstName && a.LastName == row.LastName && a.WorkDate == row.WorkDate).Sum(a => a.Hours);
                if(totalHours > 24)
                {
                    errorList.Add(new ValidationErrorsViewModel()
                    {
                        Error = "There are more than 24 hours for " + row.FirstName + " " + row.LastName + " on " + row.WorkDate.ToShortDateString() + " Total hours: " + totalHours,
                        Row = row.RowNumber
                    });
                }
            }
        }

        private void PopulateLookupValues(ExcelPackage lookupValues)
        {
            ExcelWorksheet glWs = lookupValues.Workbook.Worksheets["GL_NUMBER"];
            
            for (int row = glWs.Dimension.Start.Row + 1; row <= glWs.Dimension.End.Row; row++)
            {
                glNumbers.Add(glWs.Cells[row, 1].Value.ToString());
            }
            ExcelWorksheet comCodeWs = lookupValues.Workbook.Worksheets["ATRIUM_COMMUNITY_CODE"];
            for (int row = comCodeWs.Dimension.Start.Row + 1; row <= comCodeWs.Dimension.End.Row; row++)
            {
                communityCodes.Add(comCodeWs.Cells[row, 1].Value.ToString());
            }
        }

        private void PopulatePBJValues(ExcelPackage pbjValues)
        {
            ExcelWorksheet pbjWs = pbjValues.Workbook.Worksheets[1];
            if(pbjWs.Dimension.End.Column > 8)
            {
                errorList.Add(new ValidationErrorsViewModel()
                {
                    Error = "Extra columns detected " + pbjWs.Dimension.End.Column + " expected 8",
                    Row = -1
                });
            }
            
            if(pbjWs.Cells[1,1].Value.ToString() != "VENDOR_NAME")
            {
                throw new Exception("Column names are not valid.");
            }
            if (pbjWs.Cells[1, 2].Value.ToString() != "FIRST_NAME")
            {
                throw new Exception("Column names are not valid.");
            }
            if (pbjWs.Cells[1, 3].Value.ToString() != "LAST_NAME")
            {
                throw new Exception("Column names are not valid.");
            }
            if (pbjWs.Cells[1, 4].Value.ToString() != "HIRE_DATE")
            {
                throw new Exception("Column names are not valid.");
            }
            if (pbjWs.Cells[1, 5].Value.ToString() != "ATRIUM_COMMUNITY_CODE")
            {
                throw new Exception("Column names are not valid.");
            }
            if (pbjWs.Cells[1, 6].Value.ToString() != "WORK_DATE")
            {
                throw new Exception("Column names are not valid.");
            }
            if (pbjWs.Cells[1, 7].Value.ToString() != "GL_NUMBER")
            {
                throw new Exception("Column names are not valid.");
            }
            if (pbjWs.Cells[1, 8].Value.ToString() != "HOURS")
            {
                throw new Exception("Column names are not valid.");
            }
            for (int row = pbjWs.Dimension.Start.Row + 1; row <= pbjWs.Dimension.End.Row; row++)
            {
                bool missingValue = false;
                for(int cols = 1; cols <= 8; cols ++)
                {
                    if(pbjWs.Cells[row, cols].Value == null)
                    {
                        missingValue = true;
                        errorList.Add(new ValidationErrorsViewModel()
                        {
                            Error = "Missing value for column " + cols,
                            Row = row
                        });
                    }
                }

                if (missingValue)
                {
                    continue;
                }

                PBJRow newRow = new PBJRow()
                {
                    VendorName = pbjWs.Cells[row, 1].Value.ToString(),
                    FirstName = pbjWs.Cells[row, 2].Value.ToString(),
                    LastName = pbjWs.Cells[row, 3].Value.ToString(),
                    HireDate = pbjWs.Cells[row, 4].GetValue<DateTime>(),
                    CommunityCode = pbjWs.Cells[row, 5].Value.ToString(),
                    WorkDate = pbjWs.Cells[row, 6].GetValue<DateTime>(),
                    GLNumber = pbjWs.Cells[row, 7].Value.ToString(),
                    Hours = pbjWs.Cells[row, 8].GetValue<float>(),
                    RowNumber = row
                };

                var duplicates = pbjRows.Where(a => a.FirstName == newRow.FirstName &&
                a.LastName == newRow.LastName &&
                a.GLNumber == newRow.GLNumber &&
                a.CommunityCode == newRow.CommunityCode &&
                a.WorkDate == newRow.WorkDate);
                if (duplicates.Count() > 0)
                {
                    foreach(var duplicate in duplicates)
                    {
                        errorList.Add(new ValidationErrorsViewModel()
                        {
                            Error = "Duplicate entry found: " + duplicate.RowNumber,
                            Row = row
                        });

                    }
                }

                pbjRows.Add(newRow);
                //communityCodes.Add(pbjWs.Cells[row, 1].Value.ToString());
            }
        }

        private class PBJRow
        {
            public string VendorName { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime HireDate { get; set; }
            public string CommunityCode { get; set; }
            public DateTime WorkDate { get; set; }
            public string GLNumber { get; set; }
            public float Hours { get; set; }
            public int RowNumber { get; set; }
        }

    }


}
