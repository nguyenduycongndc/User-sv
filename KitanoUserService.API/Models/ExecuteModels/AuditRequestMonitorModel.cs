using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class AuditRequestMonitorDetailModel
    {
        public string fullname { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string tbody { get; set; }
    }
}
