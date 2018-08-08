using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Models.ViewModel
{
    public class ASAPHotlineAdminViewModel
    {
        public List<ASAPComplaintType> ComplaintTypes { get; set; }
        public List<Employee> ActiveEmployees { get; set; }
        public AdminViewModel AdminViewModel { get; set; }
        public List<ASAPContact> ContactList { get; set; }
        public ASAPContact NewContact
        {
            get { return new ASAPContact(); }
            set { NewContact = value; }
        }
        public ASAPComplaintType NewComplaintType
        {
            get { return new ASAPComplaintType(); }
            set { NewComplaintType = value; }
        }
    }
}
