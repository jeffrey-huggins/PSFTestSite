using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Base.Library
{
    public class SMTPConfig
    {
        public string SMTPServer { get; set; }
        public int SMTPort { get; set; }
        public string ApplicationEmailAddress { get; set; }
        public string SMTPLogin { get; set; }
        public string SMTPassword { get; set; }
    }
}
