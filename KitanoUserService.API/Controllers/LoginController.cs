using KitanoUserService.API.DataAccess;
using KitanoUserService.API.LdapService.Entity;
using KitanoUserService.API.LdapService.Interface;
using KitanoUserService.API.Models.ExecuteModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace KitanoUserService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        private readonly IConfiguration _config;
        private readonly KitanoSqlContext _context;
        private readonly IDatabase _iDb;
        protected readonly ILoggerManager _logger;
        protected readonly ILdapService _ldapservice;
        protected readonly string sServerType;
        protected readonly string sdomain;
        public LoginController(IConfiguration config, KitanoSqlContext context, IDatabase redisDb, ILoggerManager logger, ILdapService ldapservice)
        {
            _config = config;
            _context = context;
            _iDb = redisDb;
            _logger = logger;
            sServerType = _config["LDAPinfo:ADPath"];
            sdomain = _config["LDAPinfo:ADDomain"];
            _ldapservice = ldapservice;
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            IActionResult response;
            var iCache = (IConnectionMultiplexer)HttpContext.RequestServices.GetService(typeof(IConnectionMultiplexer));
            if (!iCache.IsConnected)
            {
                return Ok(new AutResult()
                {
                    Result = false,
                    Token = "",
                    currentUsers = new CurrentUserModel(),
                    SystemParam = new List<SystemParam>(),
                });
            }
            if (login != null && login.user_name != null && login.password != null)
            {
                var user = await AuthenticateUser(login);
                if (user != null)
                {
                    var tokenString = GenerateJSONWebToken(user);
                    _iDb.StringSet($"Auth.UserInfo.{user.UserName}", JsonSerializer.Serialize(user));

                    var list_param = GetParamSystem();
                    _iDb.StringSet("Param.SystemInfo", JsonSerializer.Serialize(list_param));

                    var list_status_approval = GetApprovalStatus();
                    _iDb.StringSet("Approval.StatusConfig", JsonSerializer.Serialize(list_status_approval)); 
                    response = Ok(new AutResult()
                    {
                        Result = true,
                        Token = tokenString,
                        currentUsers = user,
                        SystemParam = list_param,
                        approvalstatus = list_status_approval,
                    });
                }
                else
                {
                    response = Ok(new AutResult()
                    {
                        Result = false,
                        Token = "",
                        currentUsers = user,
                        SystemParam = new List<SystemParam>(),
                    });
                }
            }
            else
            {
                response = Ok(new AutResult()
                {
                    Result = false,
                    Token = "",
                    currentUsers = new CurrentUserModel(),
                    SystemParam = new List<SystemParam>(),
                });
            }
            return response;
        }

        private string GenerateJSONWebToken(CurrentUserModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                 new Claim("id", userInfo.UserName),
                new Claim("username", userInfo.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("user_id", userInfo.Id + ""),
                new Claim("Role", userInfo.RoleId + ""),
                
            };
            var token = new JwtSecurityToken(null,
                null,
                claims,
                expires: DateTime.Now.AddHours(6),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<CurrentUserModel> AuthenticateUser(LoginModel login)
        {
            CurrentUserModel user = null;
            try
            {
                var userlogin = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == login.user_name.ToLower() && u.IsDeleted != true && u.IsActive == true);
                if (userlogin != null)
                {
                    var allow_login = false;
                    if (login.typelogin == 1) //xác thực LDAP
                    {
                        allow_login = CheckLDAP(login.user_name, login.password);
                    }
                    else
                    {
                        allow_login = VerifyPassword(login.password, userlogin.SaltKey, userlogin.Password);
                    }
                    if (allow_login == true)
                    {
                        var userRoles = _context.UsersRoles.Where(u => u.users_id == userlogin.Id).Select(a => a.roles_id);
                        var group = _context.UsersGroupMapping.Where(u => u.users_id == userlogin.Id).Select(a => a.group_id).ToArray();
                        var groupRoles = _context.UsersGroupRoles.Where(u => group.Contains(u.group_id)).Select(a => a.roles_id);
                        var list_roles = userRoles.Concat(groupRoles).Distinct().ToArray();
                        //var test = _context.RolePermissionMenu.Include(a => a.Permission).Include(a => a.Menu).Where(r => list_roles.Contains(r.role_id));
                        var current_permission = _context.RolePermissionMenu.Include(a => a.Permission).Include(a => a.Menu).Where(r => list_roles.Contains(r.role_id)).Select(r => new RolesFunctionModel()
                        {
                            RolesId = r.role_id,
                            Permission = r.Menu.CodeName + "_" + r.Permission.CodeName,
                            MenuId = r.menu_id,
                        }).ToList();
                        user = new CurrentUserModel()
                        {
                            Id = userlogin.Id,
                            UserName = userlogin.UserName,
                            FullName = userlogin.FullName,
                            RoleId = userlogin.RoleId,
                            DomainId = 1,
                            RoleList = current_permission,
                            UsersType = userlogin.UsersType,
                            DepartmentId = userlogin.DepartmentId,
                        };
                        userlogin.LastOnline = DateTime.Now;
                        _context.SaveChanges();
                    }
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError($"USER-LOGIN - {login.user_name} : {ex.Message}!");
                return user;
            }
        }
        public static bool VerifyPassword(string pass, string salt, string passhash)
        {
            var PlainPass = pass + salt; // append salt key
            return Crypto.VerifyHashedPassword(passhash, PlainPass); //verify password
        }
        protected List<SystemParam> GetParamSystem()
        {
            var results = new List<SystemParam>();
            try
            {
                var list_param = _context.SystemParameter.ToArray();
              
                if (list_param.Length > 0)
                {
                    for (int i = 0; i < list_param.Length; i++)
                    {
                        var param = new SystemParam()
                        {
                            id = list_param[i].Id,
                            name = list_param[i].Parameter_Name,
                            value = list_param[i].Value,
                        };
                        results.Add(param);
                    }
                    
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET-SYSTEMPARAM : {ex.Message}!");
                return results;
            }
        }
        protected List<ApprovalStatusFunction> GetApprovalStatus()
        {
            var results = new List<ApprovalStatusFunction>();
            try
            {
                var list_config = _context.ApprovalConfig.Where(a=>a.IsShow == true).ToArray();

                if (list_config.Length > 0)
                {
                    for (int i = 0; i < list_config.Length; i++)
                    {
                        var param = new ApprovalStatusFunction()
                        {
                            function_code = list_config[i].item_code,
                            status_code = list_config[i].StatusCode,
                            status_name = list_config[i].StatusName,
                            level = list_config[i].ApprovalLevel,
                            outside = list_config[i].IsOutside == true ? 1 : 0,
                        };
                        results.Add(param);
                    }
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET-APPROVAL_CONFIG : {ex.Message}!");
                return results;
            }
        }
        [AllowAnonymous]
        [HttpPost("logintest")]
        public IActionResult LoginTest([FromBody] LoginModel login)
        {
            try
            {
                var result = _ldapservice.Login(login.user_name, login.password);
                return Ok(new {data = result });
            }
            catch (Exception ex)
            {
                return Ok(new { data = ex.Message });
            }
        }
        protected bool CheckLDAP(string sTenDangNhap, string sMatKhau)
        {
            bool bolLDAP = false;
            try
            {
                //LdapAuthentication ldap = new LdapAuthentication();
                //bolLDAP = ldap.IsAuthenticated(sdomain, sServerType, sTenDangNhap, sMatKhau);
                var result = _ldapservice.Login(sTenDangNhap, sMatKhau);
                if (result != null)
                {
                    bolLDAP = !string.IsNullOrEmpty(result.UserName);
                }
            }
            catch (Exception)
            {
                bolLDAP = false;
            }

            return bolLDAP;
        }
        protected string CheckLDAPTest(string sTenDangNhap, string sMatKhau)
        {
            string result = "";
            bool bolLDAP = false;
            try
            {
                LdapAuthentication ldap = new LdapAuthentication();
                bolLDAP = ldap.IsAuthenticated(sdomain, sServerType, sTenDangNhap, sMatKhau);
                result = bolLDAP + "";
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }

            return result;
        }
        
        protected LdapUser CheckLDAPTestMore(string sTenDangNhap, string sMatKhau)
        {
            var LdapUser = new LdapUser();
            try
            {
                LdapUser = _ldapservice.Login(sTenDangNhap, sMatKhau);
            }
            catch (Exception)
            {
            }
            return LdapUser;

        }
    }
}