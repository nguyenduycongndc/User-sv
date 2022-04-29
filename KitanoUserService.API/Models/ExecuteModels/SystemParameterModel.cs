using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class SystemParameterModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("sub_system")]
        public string Sub_System { get; set; }
        [JsonPropertyName("parameter_name")]
        public string Parameter_Name { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }
        [JsonPropertyName("modified_at")]
        public DateTime? Modified_At { get; set; }
        [JsonPropertyName("modified_by")]
        public int? Modified_By { get; set; }
        [JsonPropertyName("reset_at")]
        public DateTime? Reset_At { get; set; }
        [JsonPropertyName("default_value")]
        public string Default_Value { get; set; }
        [JsonPropertyName("default_note")]
        public string Default_Note { get; set; }
    }
    public class SystemParameterSearchModel
    {
        [JsonPropertyName("sub_system")]
        public string Sub_System { get; set; }
        [JsonPropertyName("parameter_name")]
        public string Parameter_Name { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
    public class SystemParameterModifyModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("sub_system")]
        public string Sub_System { get; set; }
        [JsonPropertyName("parameter_name")]
        public string Parameter_Name { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }
        [JsonPropertyName("note")]
        public string Note { get; set; }
    }
    public class SystemParam
    {
        public int? id { get; set; }
        public string name { get; set; }
        public string value { get; set; }
    }
}
