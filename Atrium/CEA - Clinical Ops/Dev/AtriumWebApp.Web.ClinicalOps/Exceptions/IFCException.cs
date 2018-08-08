using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.ClinicalOps.Exceptions
{
    public class IFCException : Exception
    {
        public int TableMax { get; set; }

        public IFCException(int tableMax)
        {
            TableMax = tableMax;
        }
    }
}
