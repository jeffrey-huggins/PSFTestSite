using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Base.Library
{
    public class AppSettingsConfig
    {
        public LDAPConfig LDAP { get; set; }
        public SMTPConfig SMTP { get; set; }
    }
}
