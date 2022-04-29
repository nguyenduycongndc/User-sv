using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.MigrationsModels
{
    [Table("ROLE_PERMISSION_MENU")]
    public class RolePermissionMenu
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("role_id")]
        public int? role_id { get; set; }

        [ForeignKey("role_id")]
        public virtual Roles Roles { get; set; }

        [JsonPropertyName("menu_id")]
        public int? menu_id { get; set; }

        [ForeignKey("menu_id")]
        public virtual Menu Menu { get; set; }

        [JsonPropertyName("permission_id")]
        public int? permission_id { get; set; }

        [ForeignKey("permission_id")]
        public virtual Permission Permission { get; set; }


    }
}
