using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalesContractApplication.Models
{
    public class SalesDocumentViewModel
    {
        public SalesDocumentModel? SalesDocument { get; set; }

        public List<SalesDocumentLineModel> SalesDocumentLines { get; set; }
        public int FirstLineIndex { get; set; } = 0;

        public List<SelectListItem> Currencies { get; set; }
        public List<SelectListItem> UOMs { get; set; }
        public List<SelectListItem> Customers { get; set; }
        public List<SelectListItem> Divisions { get; set; }
        public List<SelectListItem> MetalPricingMethods { get; set; }
        public List<SelectListItem> PartialShipmentLevels { get; set; }
        public List<SelectListItem> LabelOrderStatuses { get; set; }
        public List<SelectListItem> InvoiceToAddresses { get; set; }
        public List<SelectListItem> ShipToAddresses { get; set; }
        public List<SelectListItem> OrderTypes { get; set; }

        public SalesDocumentViewModel()
        {
            SalesDocumentLines = new List<SalesDocumentLineModel>();

            Currencies = new List<SelectListItem>();
            UOMs = new List<SelectListItem>();
            Customers = new List<SelectListItem>();
            Divisions = new List<SelectListItem>();
            InvoiceToAddresses = new List<SelectListItem>();
            ShipToAddresses = new List<SelectListItem>();

            MetalPricingMethods = new List<SelectListItem> {
                new SelectListItem { Value = "standard", Text = "Standard", Selected = true },
                new SelectListItem { Value = "real metal price", Text = "Real Metal Price"  }
            };

            PartialShipmentLevels = new List<SelectListItem>{
                new SelectListItem { Value = "full PO", Text = "Full PO", Selected = true },
                new SelectListItem { Value = "full SO", Text = "Full SO" },
                new SelectListItem { Value = "full SOD", Text = "Full SO Line" },
                new SelectListItem { Value = "partial", Text = "Partial" },
            };

            LabelOrderStatuses = new List<SelectListItem>(){
                new SelectListItem { Value = "0", Text = "Nothing done", Selected = true },
                new SelectListItem { Value = "50", Text = "Ordered" },
                new SelectListItem { Value = "100", Text = "Received" },
            };

            OrderTypes = new List<SelectListItem>()
            {
                new SelectListItem { Value = "MP", Text = "MP", Selected = true },
                new SelectListItem { Value = "NP", Text = "NP" },
                new SelectListItem { Value = "SP", Text = "SP" },
            };
        }
    }
}
