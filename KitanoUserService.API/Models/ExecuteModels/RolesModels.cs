using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class RolesModels
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("full_name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }

        [JsonPropertyName("status")]
        public int? Status { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("list_users_id")]
        public List<int?> ListUsersID{ get; set; }

        [JsonPropertyName("list_users")]
        public IEnumerable<string> ListUsers { get; set; }

        [JsonPropertyName("list_permission")]
        public List<string> list_permission { get; set; }

        [JsonPropertyName("created_by")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("modified_by")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime? ModifiedAt { get; set; }
    }
    public class UpdateRolesPermissionModels
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("list_permission")]
        public List<string> ListPermission { get; set; }
    }
}
