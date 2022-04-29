using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class DepartmentModels
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("full_name")]
        public string Name { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("is_unit")]
        public bool? IsUnit { get; set; }

        [JsonPropertyName("parent_id")]
        public int? PareantId { get; set; }

        [JsonPropertyName("department_type_id")]
        public int? DepartmentTypeID { get; set; }
    }
}
