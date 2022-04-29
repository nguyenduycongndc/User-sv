using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class SystemLogMd
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("module")]
        public string module { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("perform_tasks")]
        public string perform_tasks { get; set; }
        [JsonPropertyName("datetime")]
        public DateTime datetime { get; set; }
    }

    public class SystemLogDataMd
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("module")]
        public string module { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("perform_tasks")]
        public string perform_tasks { get; set; }
        [JsonPropertyName("datetime")]
        public string datetime { get; set; }
    }

    public class SystemLogSearchMd
    {
        [JsonPropertyName("id")]
        public int id { get; set; }
        [JsonPropertyName("module")]
        public string module { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("perform_tasks")]
        public string perform_tasks { get; set; }
        [JsonPropertyName("datetime")]
        public DateTime? datetime { get; set; }
        [JsonPropertyName("start_date")]
        public string start_date { get; set; }
        [JsonPropertyName("end_date")]
        public string end_date { get; set; }
        [JsonPropertyName("start_number")]
        public int start_number { get; set; }
        [JsonPropertyName("page_size")]
        public int page_size { get; set; }
    }
    public class SystemLogCreateMd
    {
        [JsonPropertyName("module")]
        public string module { get; set; }
        [JsonPropertyName("perform_tasks")]
        public string perform_tasks { get; set; }
    }
    public class MonngoLogSearchMd
    {
        [JsonPropertyName("module")]
        public string id { get; set; }
        [JsonPropertyName("auditplan_id")]
        public int auditplan_id { get; set; }
        [JsonPropertyName("module")]
        public string module { get; set; }
        [JsonPropertyName("name")]
        public string name { get; set; }
        [JsonPropertyName("perform_tasks")]
        public string perform_tasks { get; set; }
        [JsonPropertyName("datetime")]
        public DateTime? datetime { get; set; }
        [JsonPropertyName("start_date")]
        public string start_date { get; set; }
        [JsonPropertyName("end_date")]
        public string end_date { get; set; }
        [JsonPropertyName("start_number")]
        public int start_number { get; set; }
        [JsonPropertyName("page_size")]
        public int page_size { get; set; }
    }
}
