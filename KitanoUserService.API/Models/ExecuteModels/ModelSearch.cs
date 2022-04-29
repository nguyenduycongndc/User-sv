using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class ModelSearch
    {
        [JsonPropertyName("full_name")]
        public string FullName { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("user_name")]
        public string UserName { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("department_id")]
        public string DepartmentId { get; set; }
        [JsonPropertyName("users_type")]
        public string UsersType { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }
    public class ActiveClass
    {
        public int id { get; set; }
        public int status { get; set; }
    }
    public class ChangePass
    {
        public int id { get; set; }
        public string password { get; set; }
    }
    public class ChangePassUser
    {
        public int id { get; set; }
        public string new_password { get; set; }
        public string old_password { get; set; }
    }
    public class ChangeWorkplace
    {
        public int id { get; set; }
        public int departmentId { get; set; }
        public string dateofjoining { get; set; }
    }
    public class DeleteAll
    {
        public string listID { get; set; }
    }
    public class MonngoLogSearchModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; }
        public string auditplan_id { get; set; }
        public string module { get; set; }
        public string name { get; set; }
        public string perform_tasks { get; set; }
        public string version { get; set; }
        public DateTime? datetime { get; set; }
        public string start_date { get; set; }
        public string end_date { get; set; }
        public int start_number { get; set; }
        public int page_size { get; set; }
    }
}
