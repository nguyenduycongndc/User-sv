using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace KitanoUserService.API.Models.ExecuteModels.Category
{
    public class CatAuditRequestModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("status")]
        public bool? Status { get; set; }
        [JsonPropertyName("createdate")]
        public DateTime CreateDate { get; set; }
        [JsonPropertyName("createdat")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("modifiedat")]
        public string ModifiedAt { get; set; }
        [JsonPropertyName("deletedat")]
        public DateTime? DeletedAt { get; set; }
        [JsonPropertyName("createdby")]
        public int? CreatedBy { get; set; }
        [JsonPropertyName("modifiedby")]
        public int? ModifiedBy { get; set; }
        [JsonPropertyName("deletedby")]
        public int? DeletedBy { get; set; }
        [JsonPropertyName("is_active")]
        public bool? IsActive { get; set; }
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }
        public string CreatorName { get; set; }
        [JsonPropertyName("editorName")]
        public string EditorName { get; set; }
    }

    public class CatAuditRequestSearchModel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }
    }

    public class CatAuditRequestListModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string TypeName { get; set; }
        [JsonPropertyName("status")]
        public bool? Status { get; set; }
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }
    }

    public class CatAuditRequestCreateModel
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("status")]
        public bool? Status { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class CatAuditRequestActiveModel
    {
        public int id { get; set; }
        public int status { get; set; }
    }

    public class CatAuditRequestModifyModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("status")]
        public bool? Status { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public class CatAuditRequestDetailModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("is_deleted")]
        public bool? IsDeleted { get; set; }
        [JsonPropertyName("status")]
        public bool? Status { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; }
        [JsonPropertyName("modifiedAt")]
        public string ModifiedAt { get; set; }
        [JsonPropertyName("deletedAt")]
        public DateTime? DeletedAt { get; set; }
        [JsonPropertyName("createdBy")]
        public int? CreatedBy { get; set; }
        [JsonPropertyName("modifiedBy")]
        public int? ModifiedBy { get; set; }
        [JsonPropertyName("deletedBy")]
        public int? DeletedBy { get; set; }
        [JsonPropertyName("creatorName")]
        public string CreatorName { get; set; }
        [JsonPropertyName("editorName")]
        public string EditorName { get; set; }
    }
}
