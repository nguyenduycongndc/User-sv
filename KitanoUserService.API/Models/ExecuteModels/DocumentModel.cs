//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using User_service.DataAccess;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class DocumentModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("unitid")]
        public int? unit_id { get; set; }

        [JsonPropertyName("status")]
        public bool? status { get; set; }

        [JsonPropertyName("public_date")]
        public string publicdate { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? isdeleted { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("unitname")]
        public string unitname { get; set; }
        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

        [JsonPropertyName("list_file")]
        public List<DocumentFileModel> ListFile { get; set; }
    }

    public class CreateDocumentModel
    {
        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("unitid")]
        [JsonConverter(typeof(IntNullableJsonConverter))]
        public int? unit_id { get; set; }

        [JsonPropertyName("status")]
        public bool? status { get; set; }

        [JsonPropertyName("public_date")]
        public string publicdate { get; set; }

        [JsonPropertyName("isdeleted")]
        public bool? isdeleted { get; set; }
        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

        [JsonPropertyName("list_file")]
        public List<DocumentFileModel> ListFile { get; set; }
    }
    public class DocumentSearchModel
    {
        [JsonPropertyName("code")]
        public string code { get; set; }

        [JsonPropertyName("unitid")]
        public string unit_id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("start_number")]
        public int StartNumber { get; set; }
        [JsonPropertyName("page_size")]
        public int PageSize { get; set; }

        [JsonPropertyName("controlid")]
        public string controlid { get; set; }
    }


    public class DocumentCreateModel
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("public_date")]
        public string publicdate { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("list_file")]
        public List<DocumentFileModel> ListFile { get; set; }

    }

    public class DocumentActiveModel
    {
        public int id { get; set; }
        public int status { get; set; }
    }

    public class DocumentModifyModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("status")]
        public bool? status { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("unitid")]
        public int? unit_id { get; set; }

        [JsonPropertyName("public_date")]
        public DateTime public_date { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("list_file")]
        public List<DocumentFileModel> ListFile { get; set; }
    }

    public class DocumentDetailModel
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("unitid")]
        public int? unit_id { get; set; }

        [JsonPropertyName("status")]
        public bool? status { get; set; }
        [JsonPropertyName("isdeleted")]
        public bool? isdeleted { get; set; }        

        [JsonPropertyName("public_date")]
        public string public_date { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("filetype")]
        public string filetype { get; set; }

        [JsonPropertyName("filename")]
        public string filename { get; set; }

        [JsonPropertyName("list_file")]
        public List<DocumentFileModel> ListFile { get; set; }
    }

    public class UploadFileModel
    {
        [JsonPropertyName("id")]        
        public Guid id { get; set; }
        [JsonPropertyName("brief_review")]
        public string brief_review { get; set; }
    }

    public class DocumentFileModel
    {
        [JsonPropertyName("id")]
        public int? id { get; set; }
        [JsonPropertyName("path")]
        public string path { get; set; }
        [JsonPropertyName("file_type")]
        public string file_type { get; set; }
    }
}
