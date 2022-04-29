using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.MigrationsModels
{
    [Table("SYSTEMLOG")]
    public class SystemLog
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int id { get; set; }
        [Column("module")]
        [JsonPropertyName("module")]
        public string module { get; set; }
        [Column("name")]
        [JsonPropertyName("name")]
        public string name { get; set; }
        [Column("perform_task")]
        [JsonPropertyName("perform_task")]
        public string perform_task { get; set; }
        [Column("datetime")]
        [JsonPropertyName("datetime")]
        public DateTime? datetime { get; set; }
    }
}