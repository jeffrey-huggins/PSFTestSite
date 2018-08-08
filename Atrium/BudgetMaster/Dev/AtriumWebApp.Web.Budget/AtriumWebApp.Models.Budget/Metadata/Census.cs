using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.Budget
{
    [MetadataType(typeof(CensusMetadata))]
    public partial class Census
    {

    }

    public class CensusMetadata
    {
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString = "{0:N1}")]
        public object AvgDailyCensus;
    }
}
