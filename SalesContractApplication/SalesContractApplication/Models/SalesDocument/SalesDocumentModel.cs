using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace SalesContractApplication.Models
{
    public class SalesDocumentModel
    {
        [JsonProperty("sales_document_id")]
        public int? SalesDocumentId { get; set; }

        [JsonProperty("document_code")]
        [Display(Name = "Document Code")]
        public string? DocumentCode { get; set; }

        [JsonProperty("version")]
        public string? Version { get; set; }

        [JsonProperty("parent_id")]
        public string? ParentId { get; set; }

        [JsonProperty("purchase_order_number")]
        [Display(Name = "Purchase Order Number")]
        public string? PurchaseOrderNumber { get; set; }

        [JsonProperty("division")]
        public string? Division { get; set; }

        [JsonProperty("customer_id")]
        [Required(ErrorMessage = "Customer ID is required.")]
        public float? CustomerId { get; set; }

        [Required]
        [JsonProperty("document_valid_start")]
        //[Display(Name = "Contract Valid From")]
        [DataType(DataType.Date)]
        public DateTime? DocumentValidStart { get; set; } = DateTime.Now;

        [Required]
        [JsonProperty("document_valid_end")]
        //[Display(Name = "Contract Valid To")]
        [DataType(DataType.Date)]
        public DateTime? DocumentValidEnd { get; set; } = new DateTime(DateTime.Now.Year, 12, 31);

        [JsonProperty("metal_pricing_method")]
        [Display(Name = "Metal Pricing Method")]
        public string MetalPricingMethod { get; set; }

        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("file_path")]
        public string? FilePath { get; set; }

        [JsonProperty("expected_invoice_date")]
        [Display(Name = "Expected Invoice Date")]
        [DataType(DataType.Date)]
        public DateTime? ExpectedInvoiceDate { get; set; } = DateTime.Now.AddDays(14);

        [JsonProperty("partial_shipment_level")]
        [Display(Name = "Partial Shipment Level")]
        public string PartialShipmentLevel { get; set; }

        [JsonProperty("gold_price")]
        [Range(0, Double.MaxValue, ErrorMessage = "Value cannot be negative.")]
        [Display(Name = "Gold Price")]
        public float? GoldPrice { get; set; } = 0.00f;

        [JsonProperty("silver_price")]
        [Range(0, Double.MaxValue, ErrorMessage = "Value cannot be negative.")]
        [Display(Name = "Silver Price")]
        public float? SilverPrice { get; set; } = 0.00f;

        [JsonProperty("margin")]
        public float? Margin { get; set; } = 0.00f;

        [JsonProperty("discount")]
        public float? Discount { get; set; } = 0.00f;

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("exchange_rate")]
        [Display(Name = "Exchange Rate")]
        public float? ExchangeRate { get; set; } = -1.00f;

        [JsonProperty("label_order_status")]
        [Display(Name = "Label Order Status")]
        public int? LabelOrderStatus { get; set; }

        [JsonProperty("other_information")]
        [Display(Name = "Other Information")]
        public string? OtherInformation { get; set; }

        [JsonProperty("created_date")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("created_by")]
        public string? CreatedBy { get; set; }

        [JsonProperty("modified_date")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty("modified_by")]
        public string? ModifiedBy { get; set; }

        [JsonProperty("lines")]
        public List<SalesDocumentLineModel>? Lines { get; set; }
    }
}
