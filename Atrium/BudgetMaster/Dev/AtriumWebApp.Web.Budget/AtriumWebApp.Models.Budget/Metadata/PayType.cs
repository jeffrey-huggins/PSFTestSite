using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AtriumWebApp.Models.Budget
{
    [ModelMetadataType(typeof(PayTypeMetadata))]
    public partial class PayType
    {

    }
    public class PayTypeMetadata
    {
        [Required]
        [MaxLength(3)]
        public string PayTypeCd { get; set; }
        [MaxLength(150)]
        public string PayTypeNm { get; set; }
        [Required]
        [MaxLength(3)]
        public string PayGrpCd { get; set; }

    }
}
