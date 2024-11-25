using Newtonsoft.Json;

namespace SalesContractApplication.Models
{
    public class OldSalesModel
    {
        [JsonProperty("CRDiamonds")]
        public double? CRDiamonds { get; set; }

        [JsonProperty("CRFindings")]
        public double? CRFindings { get; set; }

        [JsonProperty("CRMetal")]
        public double? CRMetal { get; set; }

        [JsonProperty("CRStones")]
        public double? CRStones { get; set; }

        [JsonProperty("CRTimes")]
        public double? CRTimes { get; set; }

        [JsonProperty("Client Code")]
        public string? ClientCode { get; set; }

        [JsonProperty("Client Name")]
        public string? ClientName { get; set; }

        [JsonProperty("CustRef")]
        public string? CustomerReference { get; set; }

        [JsonProperty("Customer")]
        public int? CustomerId { get; set; }

        [JsonProperty("CustomerOrderNo")]
        public string? CustomerOrderNumber { get; set; }

        public double? Diamonds { get; set; }

        public double? DiscountOrMarkupPriceRate { get; set; }

        [JsonProperty("Gold price")]
        public double? GoldPriceH { get; set; }
        [JsonProperty("GoldPrice")]
        public double? GoldPriceD { get; set; }

        [JsonProperty("Silver price")]
        public double? SilverPriceH { get; set; }

        [JsonProperty("SilverPrice")]
        public double? SilverPriceD { get; set; }

        public string? ITDS { get; set; }

        [JsonProperty("Invoice Date")]
        public DateTime? InvoiceDate { get; set; }

        [JsonProperty("Invoice Description")]
        public string? InvoiceDescription { get; set; }

        [JsonProperty("Invoice Number")]
        public string? InvoiceNumber { get; set; }

        [JsonProperty("Invoice To Address")]
        public string? InvoiceToAddress { get; set; }

        [JsonProperty("Invoice Type")]
        public string? InvoiceType { get; set; }

        public string? LOP { get; set; }

        public double? Metal { get; set; }

        public string? OrderID { get; set; }

        public double? OtherCost { get; set; }

        [JsonProperty("P/O")]
        public string? PO { get; set; }

        [JsonProperty("Procedure Out Number")]
        public int? PON { get; set; }

        public double? Qty { get; set; }

        public string? Reference { get; set; }

        public string? SalesGrouping { get; set; }

        [JsonProperty("Ship To Address")]
        public string? ShipToAddress { get; set; }

        public double? Stones { get; set; }

        [JsonProperty("Tax invoice Number")]
        public string? TaxInvoiceNumber { get; set; }

        [JsonProperty("Unit Price")]
        public decimal? UnitPrice { get; set; }

        public double? Weight { get; set; }

        [JsonProperty("unit")]
        public string? Unit { get; set; }
    }
}
