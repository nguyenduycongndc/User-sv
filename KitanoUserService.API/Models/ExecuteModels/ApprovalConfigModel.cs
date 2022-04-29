using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using User_service.DataAccess;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class ApprovalConfigListModel
    {
        [JsonPropertyName("item_id")]
        public int? item_id { get; set; }

        [JsonPropertyName("item_name")]
        public string item_name { get; set; }

        [JsonPropertyName("item_code")]
        public string item_code { get; set; }

        [JsonPropertyName("approval_level")]
        public int? ApprovalLevel { get; set; }

        [JsonPropertyName("is_outside")]
        public bool? IsOutside { get; set; }

        [JsonPropertyName("list_status")]
        public List<ApprovalConfigStatusModel> ListStatus { get; set; }

    }
    public class ApprovalConfigStatusModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }

        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }

        [JsonPropertyName("status_description")]
        public string StatusDescription { get; set; }

        [JsonPropertyName("status_name")]
        public string StatusName { get; set; }

    }

    public class ApprovalConfigModifyModel
    {
        [JsonPropertyName("list_config")]
        public List<ApprovalConfigStatusListModel> ListConfig { get; set; }
    }
    public class ApprovalConfigStatusListModel
    {
        [JsonPropertyName("item_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? item_id { get; set; }

        [JsonPropertyName("item_name")]
        public string item_name { get; set; }

        [JsonPropertyName("item_code")]
        public string item_code { get; set; }

        [JsonPropertyName("approval_level")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? ApprovalLevel { get; set; }

        [JsonPropertyName("is_outside")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? IsOutside { get; set; }

        [JsonPropertyName("list_status")]
        public List<ApprovalConfigStatusModel> ListStatus { get; set; }

    }
    public class ApprovalStatusFunction
    {
        public string function_code { get; set; }
        public string status_code { get; set; }
        public string status_name { get; set; }
        public int? level { get; set; }
        public int? outside { get; set; }
    }
}
