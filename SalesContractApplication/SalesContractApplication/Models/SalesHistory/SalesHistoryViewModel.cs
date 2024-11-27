using Microsoft.AspNetCore.Mvc.Rendering;

namespace SalesContractApplication.Models
{
    public class SalesHistoryViewModel
    {
        public List<InvoiceDataModel> InvoiceDataList { get; set; }
        public List<OldSalesModel> OldSalesList { get; set; }

        public List<SelectListItem> Customers { get; set; }
        public List<SelectListItem> BrandCodes { get; set; }
        public List<SelectListItem> BrandNames { get; set; }

        public SalesHistoryViewModel()
        {
            InvoiceDataList = [];
            OldSalesList = [];
            Customers = [];
            BrandCodes = [];
            BrandNames = [];
        }
    }
}
