using System.Collections.Generic;
using System.Linq;
using AtriumWebApp.Web.ClinicalOps.Models;
using AtriumWebApp.Web.Base.Library;
using AtriumWebApp.Web.Controllers;
using Microsoft.Extensions.Options;
using AtriumWebApp.Models;

namespace AtriumWebApp.Web.Base.Controllers
{
    public class BaseHospitalDischargeController : BaseController
    {
        protected const string AppCode = "HOD";
        protected new HospitalDischargeContext Context
        {
            get { return (HospitalDischargeContext)base.Context; }
        }

        public BaseHospitalDischargeController(IOptions<AppSettingsConfig> config, HospitalDischargeContext context) : 
            base(config, context)
        {
        }


        public IList<DischargeReason> ERDischargeReasons
        {
            get
            {
                IList<DischargeReason> value;
                if (!Session.TryGetObject("ERDischargeReasons", out value))
                {
                    value = (from discharge in Context.DischargeReasons
                             where discharge.ERDataEntryFlg
                             orderby discharge.SortOrder, discharge.DischargeReasonDesc
                             select discharge).ToList();
                    Session.SetItem("ERDischargeReasons", value);
                }
                return value;
            }
        }

        public IList<DischargeReason> DischargeReasons
        {
            get
            {
                IList<DischargeReason> value;
                if (!Session.TryGetObject("DischargeReasons", out value))
                {
                    value = (from discharge in Context.DischargeReasons
                             orderby discharge.SortOrder, discharge.DischargeReasonDesc
                             select discharge).ToList();
                    Session.SetItem("DischargeReasons", value);
                }
                return value;
            }
        }

        public IList<DischargeReason> HospitalDischargeReasons
        {
            get
            {
                IList<DischargeReason> value;
                if (!Session.TryGetObject("HospitalDischargeReasons", out value))
                {
                    value = (from discharge in Context.DischargeReasons
                             where discharge.HospitalDataEntryFlg
                             orderby discharge.SortOrder, discharge.DischargeReasonDesc
                             select discharge).ToList();
                    Session.SetItem("HospitalDischargeReasons", value);
                }
                return value;
            }
        }

        public IList<DidNotReturnReason> DidNotReturnReasons
        {
            get
            {
                IList<DidNotReturnReason> value;
               
                if (!Session.TryGetObject("DNRR",out value))
                {
                    value = (from dnrr in Context.DidNotReturnReasons
                             where dnrr.DataEntryFlg
                             orderby dnrr.SortOrder, dnrr.DidNotReturnReasonDesc
                             select dnrr).ToList();
                    Session.SetItem("DNRR", value);
                }
                return value;
            }
        }

        public IList<Hospital> Hospitals
        {
            get
            {
                if (!Session.TryGetObject("CommunityHospitals", out IList<Hospital> value))
                {
                    if (CurrentFacility.TryGet(AppCode, out int currentCommunity))
                    {
                        value = (from h in Context.Hospitals
                                 where h.Communities.Select(c => c.CommunityId).Contains(currentCommunity)
                                 orderby h.SortOrder, h.Name
                                 select h).ToList();
                        Session.SetItem("CommunityHospitals", value);
                    }
                }
                return value;
            }
            set
            {
                Session.SetItem("CommunityHospitals",value);
            }
        }

        public IList<Hospital> AllHospitals
        {
            get
            {
                if (!Session.TryGetObject("AllHospitals", out IList<Hospital> value))
                {
                    if (CurrentFacility.TryGet(AppCode, out int currentCommunity))
                    {
                        value = (from h in Context.Hospitals
                                 where h.Communities.Select(c => c.CommunityId).Contains(currentCommunity)
                                 orderby h.SortOrder, h.Name
                                 select h).ToList();
                        Session.SetItem("AllHospitals", value);
                    }
                }
                return value;
            }
            set
            {
                Session.SetItem("AllHospitals", value);
            }
        }
    }
}