using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AtriumWebApp.Models.Budget
{
    [ModelMetadataType(typeof(GeneralLedgerMetaData))]
    public partial class GeneralLedger
    {
        [NotMapped]
        public string DisplayName
        {
            get {
                if (string.IsNullOrEmpty(GLAccountNb))
                {
                    return GLAccountNb;
                }

                return (GLAccountNb.PadRight(8,' ') + " | " + GLAccountNm); }
       
        }

    }

    public class GeneralLedgerMetaData
    {
        [NotMapped]
        public string DisplayName;
        [Required]
        [MaxLength(20)]
        public string GLAccountNb { get; set; }
        [Required]
        [MaxLength(150)]
        public string GLAccountNm { get; set; }
        [Required]
        public int GLAccountTypeID { get; set; }
        [MaxLength(150)]
        public string GLAccountSimpleNm { get; set; }
    }

}
