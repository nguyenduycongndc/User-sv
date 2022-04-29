using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KitanoUserService.API.Models.ExecuteModels
{
    public class NguoiDungLDAP
    {
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string AdsPath { get; set; }
        public string Email { get; set; }
        public int ID { get; set; }
        /// <summary>
        /// Hàm khởi tạo lấy thông tin người dùng LDAP
        /// </summary>
        public NguoiDungLDAP()
        {
            ID = 0;
            FullName = string.Empty;
            UserName = string.Empty;
            AdsPath = string.Empty;
            Email = string.Empty;
        }

    }
}
