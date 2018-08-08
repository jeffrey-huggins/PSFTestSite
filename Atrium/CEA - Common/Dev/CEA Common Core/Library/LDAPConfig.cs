using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Base.Library
{
    public class LDAPConfig
    {
        private string adDomain;
        public string ADDomain
        {
            get
            {
                return adDomain;
            }
            set
            {
                adDomain = value;
                Domain = value;
            }
        }
        private string adUser;
        public string ADUser
        {
            get
            {
                return adUser;
            }
            set
            {
                adUser = value;
                User = value;
            }
        }
        private string adPassword;
        public string ADPassword
        {
            get
            {
                return adPassword;
            }
            set
            {
                adPassword = value;
                Password = value;
            }
        }
        public static string Domain { get; set; }
        public static string User { get; set; }
        public static string Password { get; set; }

    }
}
