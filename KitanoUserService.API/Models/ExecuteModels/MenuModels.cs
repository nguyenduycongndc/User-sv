using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class MenuModels
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("parent_id")]
        public int? ParentID { get; set; }

        [JsonPropertyName("code_name")]
        public string CodeName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("controller")]
        public string Controller { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; }

        [JsonPropertyName("default_url")]
        public string DefaultUrl { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("ancestor")]
        public string Ancestor { get; set; }

        [JsonPropertyName("sort_order")]
        public int? SortOrder { get; set; }

        [JsonPropertyName("is_menu")]
        public bool? IsMenu { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }
    }
    public class PermissionModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("code_name")]
        public string CodeName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("type")]
        public int? Type { get; set; }

        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }
    }
    public class RenderPermission
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("state")]
        public StateModel State { get; set; }

        [JsonPropertyName("parent_id")]
        public string ParentId { get; set; }

        [JsonPropertyName("children")]
        public List<RenderPermission> Children { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }
    public class StateModel
    {
        [JsonPropertyName("opened")]
        public bool Opened { get; set; }

        [JsonPropertyName("selected")]
        public bool Selected { get; set; }

        [JsonPropertyName("disabled")]
        public bool Disabled { get; set; }
    }
    public class RoleMenuPermissionModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("permission_id")]
        public int? PermissionId { get; set; }

        [JsonPropertyName("menu_id")]
        public int? MenuId { get; set; }
    }
}
