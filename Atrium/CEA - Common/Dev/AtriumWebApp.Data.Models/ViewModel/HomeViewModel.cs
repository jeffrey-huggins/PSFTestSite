using System.Collections.Generic;

namespace AtriumWebApp.Models.ViewModel
{
    public class HomeViewModel
    {
        public IDictionary<string, bool> AccessByAppCode { get; set; }

        public bool HasAccessTo(string appCode)
        {
            return AccessByAppCode.ContainsKey(appCode) && AccessByAppCode[appCode];
        }
    }
}