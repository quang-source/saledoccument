using Newtonsoft.Json;

namespace SalesContractApplication.Models
{
    public class InvoiceDataModel
    {
        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("cust_code")]
        public string CustomerCode { get; set; }

        [JsonProperty("cust_name")]
        public string CustomerName { get; set; }

        [JsonProperty("cust_ref")]
        public string CustomerReference { get; set; }

        [JsonProperty("cust_order")]
        public string CustomerOrder { get; set; }

        [JsonProperty("itds")]
        public string ITDS { get; set; }

        [JsonProperty("division")]
        public string Division { get; set; }

        [JsonProperty("mo")]
        public string MO { get; set; }

        [JsonProperty("order_id")]
        public int OrderID { get; set; }

        [JsonProperty("line")]
        public int Line { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("drawing")]
        public string Drawing { get; set; }

        [JsonProperty("l_o_p")]
        public string LOP { get; set; }

        [JsonProperty("invoice_date")]
        public string InvoiceDate { get; set; }

        [JsonProperty("packing_date")]
        public string PackingDate { get; set; }

        [JsonProperty("qty")]
        public int Quantity { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("unit_price")]
        public float UnitPrice { get; set; }

        [JsonProperty("amount")]
        public float Amount { get; set; }

        [JsonProperty("item_type")]
        public string ItemType { get; set; }

        [JsonProperty("p_o_n")]
        public string PON { get; set; }

        [JsonProperty("invoice_id")]
        public string InvoiceID { get; set; }

        [JsonProperty("tax_no")]
        public string TaxNo { get; set; }

        [JsonProperty("metal_weight")]
        public float MetalWeight { get; set; }

        [JsonProperty("unit_metal")]
        public float UnitMetal { get; set; }

        [JsonProperty("net_weight")]
        public float NetWeight { get; set; }

        [JsonProperty("diamond_weight")]
        public float DiamondWeightCT { get; set; }

        [JsonProperty("diamond_weight_gram")]
        public float DiamondWeightG { get; set; }

        [JsonProperty("stone_weight")]
        public float StoneWeightCT { get; set; }

        [JsonProperty("stone_weight_gram")]
        public float StoneWeightG { get; set; }

        [JsonProperty("brand_code")]
        public string BrandCode { get; set; }

        [JsonProperty("brand_name")]
        public string BrandName { get; set; }

        [JsonProperty("planning_remark")]
        public string PlanningRemark { get; set; }

        [JsonProperty("mo_remark")]
        public string MORemark { get; set; }

        [JsonProperty("receiveddate")]
        public int ReceivedDate { get; set; }

        [JsonProperty("launchingdate")]
        public int LaunchingDate { get; set; }

        [JsonProperty("priority")]
        public string Priority { get; set; }

        [JsonProperty("gold_price")]
        public float GoldPrice { get; set; }

        [JsonProperty("silver_price")]
        public float SilverPrice { get; set; }

        [JsonProperty("_other_nonmetal")]
        public float OtherNonMetal { get; set; }

        [JsonProperty("std_metal_weight")]
        public string StdMetalWeight { get; set; }

        [JsonProperty("discrepency")]
        public string Discrepency { get; set; }


    }
}
