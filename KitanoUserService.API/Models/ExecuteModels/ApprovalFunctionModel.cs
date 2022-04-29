using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using User_service.DataAccess;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class ApprovalFunctionModifyModel
    {
        [JsonPropertyName("item_id")]
        public int? item_id { get; set; }

        [JsonPropertyName("function_name")]
        public string function_name { get; set; } // Tên function lấy từ menu_name

        [JsonPropertyName("function_code")]
        public string function_code { get; set; } // Mã function lấy từ menu_code

        [JsonPropertyName("approver")]
        public int? approver { get; set; } // người duyệt

        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }
    }
    public class RequestApprovalModel
    {
        [JsonPropertyName("item_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? item_id { get; set; } //id chức năng đang thao tác

        [JsonPropertyName("function_name")]
        public string function_name { get; set; } // Tên function lấy từ menu_name

        [JsonPropertyName("function_code")]
        public string function_code { get; set; } // Mã function lấy từ menu_code

        [JsonPropertyName("approvaluser")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? approvaluser { get; set; }

        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; } //mã request 1.1
    }
    public class ApprovalModel
    {
        [JsonPropertyName("item_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? id { get; set; } //id chức năng đang thao tác

        [JsonPropertyName("function_name")]
        public string function_name { get; set; } // Tên function lấy từ menu_name

        [JsonPropertyName("function_code")]
        public string function_code { get; set; } // Mã function lấy từ menu_code

        [JsonPropertyName("reason_note")]
        public string reason_note { get; set; }

        [JsonPropertyName("approvaluser")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? approvaluser { get; set; }

        [JsonPropertyName("level_approval")]
        public int? LevelApproval { get; set; } // cấp duyệt của chức năng

        [JsonPropertyName("year")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Year { get; set; } //chức năng kế hoạch năm cần dùng đến Year
    }
    public class UpdateStatusModel
    {
        [JsonPropertyName("item_id")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? id { get; set; } //id chức năng đang thao tác

        [JsonPropertyName("function_name")]
        public string function_name { get; set; } // Tên function lấy từ menu_name

        [JsonPropertyName("function_code")]
        public string function_code { get; set; } // Mã function lấy từ menu_code

        [JsonPropertyName("status_code")]
        public string StatusCode { get; set; }
        [JsonPropertyName("reason_note")]
        public string reason_note { get; set; }
        //Browsedate là ngày phê duyệt/ ngày từ chối
        [JsonPropertyName("browsedate")]
        public DateTime? Browsedate { get; set; }

        [JsonPropertyName("year")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? Year { get; set; } //chức năng kế hoạch năm cần dùng đến Year
        ////file
        //[JsonPropertyName("path")]
        //public string Path { get; set; }
    }
    public class ApprovalFunctionFileModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("item_id")]
        public int? item_id { get; set; } // id chức năng đang xử lý
        [JsonPropertyName("function_name")]
        public string function_name { get; set; }
        [JsonPropertyName("function_code")]
        public string function_code { get; set; } // mã của chức năng lấy theo menu
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("file_type")]
        public string FileType { get; set; }
    }
}
