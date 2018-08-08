using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AtriumWebApp.Models.Budget
{
    [ModelMetadataType(typeof(OtherExpensesGLCalcMetadata))]
    public partial class OtherExpensesGLCalc
    {
    }

    public class OtherExpensesGLCalcMetadata
    {
        [Required]
        public string CalcNm { get; set; }
    }
}
