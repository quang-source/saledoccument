using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace SalesContractApplication.Models
{
    public class SalesDocumentLineModel
    {
        [JsonProperty("sales_document_line_id")]
        public int? SalesDocumentLineId { get; set; }

        [JsonProperty("sales_document_id")]
        public int? SalesDocumentId { get; set; }

        [Required]
        [JsonProperty("customer_reference")]
        [Display(Name = "Customer Reference")]
        public string CustomerReference { get; set; }

        [Required]
        [JsonProperty("product_id")]
        [Display(Name = "Product ID")]
        public string ProductID
        {
            get => _productID;
            set
            {
                _productID = value;
                UpdateProductIDSize();
            }
        }
        private string _productID;

        [JsonProperty("size")]
        public int? Size
        {
            get => _size;
            set
            {
                _size = value;
                UpdateProductIDSize();
            }
        }
        private int? _size;

        [Required]
        [JsonProperty("product_material")]
        [Display(Name = "Product Material")]
        public string ProductMaterial
        {
            get => _productMaterial;
            set
            {
                _productMaterial = value;
                UpdateLOP();
            }
        }
        private string _productMaterial;

        [JsonProperty("metal_fineness")]
        [Display(Name = "Metal Fineness")]
        public int? MetalFineness
        {
            get => _metalFineness;
            set
            {
                _metalFineness = value;
                UpdateLOP();
            }
        }
        private int? _metalFineness;

        [JsonProperty("quantity")]
        public int? Quantity { get; set; }

        [JsonProperty("quantity_cancelled")]
        public int? QuantityCancelled { get; set; }

        [JsonProperty("quantity_shipped")]
        public int? QuantityShipped { get; set; }

        [Required]
        [JsonProperty("uom")]
        [Display(Name = "Unit of Measurement")]
        public string? UOM { get; set; }

        [Required]
        [JsonProperty("unit_price")]
        [Display(Name = "Unit Price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than or equal to {1}.")]
        public decimal? UnitPrice { get; set; }

        [Required]
        [JsonProperty("currency")]
        public string? Currency { get; set; }

        [Required]
        [JsonProperty("order_type")]
        [Display(Name = "Order Type")]
        public string? OrderType { get; set; }

        [JsonProperty("status")]
        public int? Status { get; set; }

        [JsonProperty("status_description")]
        public string? StatusDescription { get; set; } 

        [JsonProperty("amount")]
        public string? Amount { get; set; }
        [JsonProperty("stamp")]
        public string? Stamp { get; set; }

        [JsonProperty("remark")]
        public string? Remark { get; set; }

        [JsonProperty("ship_to_address")]
        [Display(Name = "Ship To")]
        public int? ShipToAddress { get; set; }

        [JsonProperty("invoice_to_address")]
        [Display(Name = "Invoice To")]
        public int? InvoiceToAddress { get; set; }

        [JsonProperty("created_date")]
        public DateTime? CreatedDate { get; set; }

        [JsonProperty("created_by")]
        public string? CreatedBy { get; set; }

        [JsonProperty("modified_date")]
        public DateTime? ModifiedDate { get; set; }

        [JsonProperty("modified_by")]
        public string? ModifiedBy { get; set; }

        public virtual SalesDocumentModel? SalesDocument { get; set; }


        [JsonProperty("lop")]
        public string? LOP
        {
            get => _lop;
            set
            {
                _lop = value;
                SplitLOP();
            }
        }
        private string? _lop;
        private void SplitLOP()
        {
            if (!string.IsNullOrWhiteSpace(LOP))
            {
                var match = System.Text.RegularExpressions.Regex.Match(LOP, @"([A-Za-z]+)(\d+)?");
                if (match.Success)
                {
                    ProductMaterial = match.Groups[1].Value.ToString().Trim().ToUpper(); // Assign characters to ProductMaterial

                    if (match.Groups[2].Success)
                    {
                        MetalFineness = int.Parse(match.Groups[2].Value);
                    }
                    else
                    {
                        MetalFineness = null;
                    }
                }
            }
        }
        private void UpdateLOP()
        {
            if (!string.IsNullOrEmpty(_productMaterial) && _metalFineness.HasValue)
            {
                _lop = $"{_productMaterial}{_metalFineness.Value}";
            }
            else if (!string.IsNullOrEmpty(_productMaterial))
            {
                _lop = _productMaterial; // If MetalFineness is null, use only ProductMaterial
            }
            else
            {
                _lop = null; // If both are null or empty, set LOP to null
            }
        }

        [JsonProperty("product_id_size")]
        public string? ProductIDSize
        {
            get => _productIDSize;
            set
            {
                _productIDSize = value;
                SplitProductIDSize();
            }
        }
        private string? _productIDSize;
        private void SplitProductIDSize()
        {
            if (!string.IsNullOrWhiteSpace(_productIDSize))
            {
                var productIDParts = _productIDSize.Split('-');
                var productID = productIDParts.Length > 0 ? productIDParts[0] : string.Empty;
                var size = productIDParts.Length > 1 ? productIDParts[1] : string.Empty;

                int sizeValue = int.TryParse(size, out var parsedSize) ? parsedSize : 0;

                ProductID = productID;
                Size = sizeValue;
            }
        }
        private void UpdateProductIDSize()
        {
            if (!string.IsNullOrEmpty(_productID) && _size.HasValue)
            {
                _productIDSize = $"{_productID}-{_size.Value}";
            }
        }
    }

    //public class UnitPriceDetailModel
    //{
    //    public float Margin { get; set; }
    //    public float Discount { get; set; }
    //    public List<ComponentDetailModel> Components { get; set; }
    //}

    //public class ComponentDetailModel
    //{
    //    public float Margin { get; set; }
    //    public float Discount { get; set; }
    //    public float Cost { get; set; }
    //    public string Type { get; set; }
    //}
}