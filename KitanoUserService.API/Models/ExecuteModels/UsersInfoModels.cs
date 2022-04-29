using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class UsersInfoModels
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        [JsonPropertyName("status")]
        public int? IsActive { get; set; }

        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("department_id")]
        public int? DepartmentId { get; set; }

        [JsonPropertyName("department_name")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("date_of_joining")]
        public DateTime? DateOfJoining { get; set; }

        [JsonPropertyName("avartar")]
        public string Avartar { get; set; }

        [JsonPropertyName("last_online_at")]
        public DateTime? LastOnline { get; set; }

        [JsonPropertyName("roleId")]
        public int? RoleId { get; set; }

        [JsonPropertyName("users_type")]
        public int? UsersType { get; set; }
        [JsonPropertyName("list_group")]
        public IEnumerable<string> ListGroup { get; set; }
        [JsonPropertyName("list_roles")]
        public IEnumerable<string> ListRoles { get; set; }

        [JsonPropertyName("created_by")]
        public string CreatedBy { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("modified_by")]
        public string ModifiedBy { get; set; }

        [JsonPropertyName("modified_at")]
        public DateTime? ModifiedAt { get; set; }

        [JsonPropertyName("data_source")]
        public int? DataSource { get; set; }
    }

    public class UsersModifyModels
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        [JsonPropertyName("status")]
        public int? IsActive { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("department_id")]
        public int? DepartmentId { get; set; }

        [JsonPropertyName("date_of_joining")]
        public string DateOfJoining { get; set; }

        [JsonPropertyName("avartar")]
        public string Avartar { get; set; }

        [JsonPropertyName("roleId")]
        public int? RoleId { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("users_type")]
        public int? UsersType { get; set; }

        [JsonPropertyName("list_group_id")]
        public List<int?> ListGroupID { get; set; }

        [JsonPropertyName("list_group")]
        public string ListGroup { get; set; }

        [JsonPropertyName("data_source")]
        public int? DataSource { get; set; }

        [JsonPropertyName("list_role_id")]
        public List<int?> ListRoleID { get; set; }

        [JsonPropertyName("list_role")]
        public string ListRole { get; set; }
    }

    public class UsersHistoryModels
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; }

        [JsonPropertyName("user_name")]
        public string UserName { get; set; }

        [JsonPropertyName("department_id")]
        public int? DepartmentId { get; set; }

        [JsonPropertyName("department_name")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("date_of_joining")]
        public string DateOfJoining { get; set; }
    }
}
