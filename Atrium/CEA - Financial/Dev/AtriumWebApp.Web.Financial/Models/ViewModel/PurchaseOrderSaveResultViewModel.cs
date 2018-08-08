using AtriumWebApp.Models.ViewModel;

namespace AtriumWebApp.Web.Financial.Models.ViewModel
{
    public class PurchaseOrderSaveResultViewModel : SaveResultViewModel
    {
        public PurchaseOrderViewModel Result { get; set; }
    }
}