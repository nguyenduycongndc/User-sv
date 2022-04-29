using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class LoginModel
    {
        [JsonPropertyName("user_name")]
        public string user_name { get; set; }
        [JsonPropertyName("password")]
        public string password { get; set; }
        [JsonPropertyName("host")]
        public string host { get; set; }
        [JsonPropertyName("type_login")]
        public int typelogin { get; set; }
    }
    public class AutResult
    {
        public string Token { get; set; }
        public bool Result { get; set; }
        public List<string> Errors { get; set; }
        public CurrentUserModel currentUsers { get; set; }
        public List<SystemParam> SystemParam { get; set; }
        public List<ApprovalStatusFunction> approvalstatus { get; set; }
    }
    public class CurrentUserModel
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
        [JsonPropertyName("role_id")]
        public int? RoleId { get; set; }
        [JsonPropertyName("domain_id")]
        public int? DomainId { get; set; }
        [JsonPropertyName("department_id")]
        public int? DepartmentId { get; set; }
        [JsonPropertyName("users_type")]
        public int? UsersType { get; set; }
        [JsonPropertyName("role_list")]
        public List<RolesFunctionModel> RoleList { get; set; }
    }
    public class RolesFunctionModel 
    {
        [JsonPropertyName("role_id")]
        public int? RolesId { get; set; }       
        [JsonPropertyName("menu_id")]
        public int? MenuId { get; set; }
        [JsonPropertyName("permission")]
        public string Permission { get; set; }
        //[JsonPropertyName("controller")]
        //public string Controller { get; set; }
        //[JsonPropertyName("action")]
        //public string Action { get; set; }

    }
}
