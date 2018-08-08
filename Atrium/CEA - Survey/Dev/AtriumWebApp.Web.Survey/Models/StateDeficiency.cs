using System;
using System.ComponentModel.DataAnnotations;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Survey.Models
{
    public class StateDeficiency : BaseDeficiency
    {
        [StringLength(2)]
        public string StateCode { get; set; }
        public State State { get; set; }

        public override string Instructions
        {
            get { return null; }//throw new NotSupportedException("Instructions not supported for " + GetType().Name + " because it is not used by Mock Survey."); }
            set { throw new NotSupportedException("Instructions not supported for " + GetType().Name + " because it is not used by Mock Survey."); }
        }
    }
}