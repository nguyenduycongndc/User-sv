using KitanoUserService.API.Models.ExecuteModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class ReturnLoginModels
    {
        public bool? status { get; set; }
        public UsersInfoModels current_user { get; set; }
        public string token { get; set; }
       
        public string refreshtoken { get; set; }       
    }
}
