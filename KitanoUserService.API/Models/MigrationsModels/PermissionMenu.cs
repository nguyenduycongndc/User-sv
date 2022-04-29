using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.MigrationsModels
{
    [Table("PERMISSION_MENU")]
    public class PermissionMenu
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("menuid")]
        public int menuid { get; set; }

        [ForeignKey("menuid")]
        public virtual Menu Menu { get; set; }

        [JsonPropertyName("permissionid")]
        public int permissionid { get; set; }

        [ForeignKey("permissionid")]
        public virtual Permission Permission { get; set; }
    }
}