using Newtonsoft.Json;

namespace SalesContractApplication.Models
{
    public class WIPReportModel
    {

        [JsonProperty("customer_group")]
        public string CustomerGroup { get; set; }

        [JsonProperty("customer_code")]
        public string CustomerCode { get; set; }

        [JsonProperty("order_type")]
        public string OrderType { get; set; }

        [JsonProperty("customer_order_number")]
        public string CustomerOrderNumber { get; set; }

        [JsonProperty("order_id")]
        public string OrderId { get; set; }

        [JsonProperty("lop")]
        public string LOP { get; set; }



        [JsonProperty("packing")]
        public int? Packing { get; set; }

        [JsonProperty("non_moving")]
        public int? NonMoving { get; set; }

        [JsonProperty("repairing")]
        public int? Repairing { get; set; }

        [JsonProperty("finishing")]
        public int? Finishing { get; set; }

        [JsonProperty("pre_finishing")]
        public int? PreFinishing { get; set; }

        [JsonProperty("setting")]
        public int? Setting { get; set; }

        [JsonProperty("missing_component")]
        public int? MissingComponent { get; set; }

        [JsonProperty("stones")]
        public int? Stones { get; set; }

        [JsonProperty("post_preparation")]
        public int? PostPreparation { get; set; }

        [JsonProperty("preparation")]
        public int? Preparation { get; set; }

        [JsonProperty("casting")]
        public int? Casting { get; set; }

        [JsonProperty("wax")]
        public int? Wax { get; set; }

        [JsonProperty("wax_setting")]
        public int? WaxSetting { get; set; }

        [JsonProperty("planning")]
        public int? Planning { get; set; }

        [JsonProperty("others")]
        public int? Others { get; set; }

        [JsonProperty("total")]
        public int? Total { get; set; }
    }
}