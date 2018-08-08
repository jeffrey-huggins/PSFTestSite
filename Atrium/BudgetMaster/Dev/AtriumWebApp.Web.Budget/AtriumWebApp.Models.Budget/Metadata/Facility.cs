using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AtriumWebApp.Models.Budget
{
    [ModelMetadataType(typeof(FacilityMetadata))]
    public partial class Facility
    {

    }
    public class FacilityMetadata
    {
        [Required]
        public string FacilityNb { get; set; }
        [Required]
        public string FacilityShortNm { get; set; }
        [Required]
        public string FacilityNm { get; set; }
        [Required]
        public int RegionID { get; set; }
        [Required]
        public int StateID { get; set; }
        [Required]
        public Nullable<float> WageIncreasePercentage { get; set; }
        [Required]
        public float MgtFeeCalcPercent { get; set; }
    }
}
