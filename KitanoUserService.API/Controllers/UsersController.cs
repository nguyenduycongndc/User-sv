using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Models.ExecuteModels;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace KitanoUserService.API.Controllers
{
    [Route("Users")]
    [ApiController]
    //[Authorize]
    public class UsersController : BaseController
    {
        private readonly IDatabase _iDb;
        private string _menuInfoPrefix = "Auth.MenuInfo.{0}";
        public UsersController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc, IConfiguration config, IDatabase iDb) : base(logger, uow, dbc, config, iDb)
        {
            _iDb = iDb;
        }
        [HttpGet("Select")]
        public IActionResult Select(string q)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = string.IsNullOrEmpty(q) ? q : q.ToLower().Trim();
                var department = _uow.Repository<Department>().GetAll(a => a.IsActive != false).AsEnumerable();
                Expression<Func<Users, bool>> filter = c => (string.IsNullOrEmpty(KeyWord) || c.FullName.ToLower().Contains(KeyWord) || c.UserName.ToLower().Contains(KeyWord) || department.Any(x => x.Id == c.DepartmentId && x.Name.ToLower().Contains(KeyWord))) && c.IsActive == true && c.IsDeleted != true;
               
                var list_user = _uow.Repository<Users>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<Users> data = list_user;
                var user = data.Select(a => new UsersInfoModels()
                {
                    Id = a.Id,
                    FullName = a.FullName,
                });
                return Ok(new { code = "1", msg = "success", data = user });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels() });
            }
        }
        [HttpGet("Select_AudiWork")]
        public IActionResult Select_AudiWork(string q)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = string.IsNullOrEmpty(q) ? q : q.ToLower().Trim();
                Expression<Func<Users, bool>> filter = c => (string.IsNullOrEmpty(KeyWord) || c.FullName.ToLower().Contains(KeyWord) || c.UserName.ToLower().Contains(KeyWord)) && c.IsActive == true && c.IsDeleted != true && c.UsersType == 1;
                var list_user = _uow.Repository<Users>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<Users> data = list_user;
                var user = data.Select(a => new UsersInfoModels()
                {
                    Id = a.Id,
                    FullName = a.FullName,
                });
                return Ok(new { code = "1", msg = "success", data = user });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels() });
            }
        }
        [HttpGet("Select_Auditor")]
        public IActionResult Select_Auditor(string q,string list_auditor)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                string KeyWord = string.IsNullOrEmpty(q) ? q : q.ToLower().Trim();
                var listAuditor = new List<string>();
                if (!string.IsNullOrEmpty(list_auditor))
                {
                    listAuditor = list_auditor.Split(',').ToList();
                }
                Expression<Func<Users, bool>> filter = c => (string.IsNullOrEmpty(KeyWord) || c.FullName.ToLower().Contains(KeyWord) || c.UserName.ToLower().Contains(KeyWord)) && c.IsActive == true && c.IsDeleted != true && c.UsersType == 1 && (listAuditor.Count == 0 || !listAuditor.Contains(c.Id.ToString()));
                var list_user = _uow.Repository<Users>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<Users> data = list_user;
                var user = data.Select(a => new UsersInfoModels()
                {
                    Id = a.Id,
                    FullName = a.FullName,
                });
                return Ok(new { code = "1", msg = "success", data = user });
            }
            catch (Exception ex)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels() });
            }
        }
        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var obj = JsonSerializer.Deserialize<ModelSearch>(jsonData);
                var status = Convert.ToInt32(obj.Status);
                var user_type = string.IsNullOrEmpty(obj.UsersType) ? -1 : Convert.ToInt32(obj.UsersType);
                var deparmentid = string.IsNullOrEmpty(obj.DepartmentId) ? -1 : Convert.ToInt32(obj.DepartmentId);
                Expression<Func<Users, bool>> filter = c => (string.IsNullOrEmpty(obj.FullName) || c.FullName.ToLower().Contains(obj.FullName.Trim().ToLower()))
                                                && (string.IsNullOrEmpty(obj.UserName) || c.UserName.ToLower().Contains(obj.UserName.Trim().ToLower()))
                                                && (user_type == -1 || c.UsersType == user_type)
                                                && (deparmentid == -1 || c.DepartmentId == deparmentid)
                                                && (status == -1 || (status == 1 ? c.IsActive == true : c.IsActive != true)) && c.IsDeleted != true;
                var list_user = _uow.Repository<Users>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<Users> data = list_user;
                var count = list_user.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var department = _uow.Repository<Department>().GetAll(a => a.IsActive != false).ToDictionary(a => a.Id, a => a.Name);
                var user = data.Select(a => new UsersInfoModels()
                {
                    Id = a.Id,
                    UserName = a.UserName,
                    FullName = a.FullName,
                    IsActive = a.IsActive == true ? 1 : 0,
                    UsersType = a.UsersType,
                    RoleId = a.RoleId,
                    DepartmentId = a.DepartmentId,
                    DepartmentName = a.DepartmentId.HasValue ? (department.FirstOrDefault(x => x.Key == a.DepartmentId).Value ?? "") : "",
                    DataSource = a.DataSource ?? 0,
                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels(), total = 0 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UsersModifyModels userInfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _alluser = _uow.Repository<Users>().Find(a => a.IsDeleted != true && (a.UserName.ToLower().Equals(userInfo.UserName.ToLower()) || a.Email.ToLower().Equals(userInfo.Email.ToLower()))).ToArray();
                if (_alluser.Length > 0)
                {
                    if (_alluser.Any(a => a.UserName.ToLower().Equals(userInfo.UserName.ToLower())))
                    {
                        return Ok(new { code = "2", msg = "fail" });
                    }
                    if (_alluser.Any(a=> a.Email.ToLower().Equals(userInfo.Email.ToLower())))
                    {
                        return Ok(new { code = "3", msg = "fail" });
                    }
                }
                string salt = "";
                string hashedPassword = "";
                if (userInfo.DataSource != 1)
                {
                    var pass = userInfo.Password;
                    salt = Crypto.GenerateSalt(); // salt key
                    var password = pass + salt;
                    hashedPassword = Crypto.HashPassword(password);
                }
                var users = new Users
                {
                    FullName = userInfo.FullName.Trim(),
                    UserName = userInfo.UserName.ToLower(),
                    Password = hashedPassword,
                    SaltKey = salt,
                    LastPassChange = DateTime.Now,
                };

                users.UsersType = userInfo.UsersType;
                users.Email = userInfo.Email.ToLower().Trim();
                var status = userInfo.IsActive;
                users.IsActive = status == 1;
                users.IsDeleted = false;
                users.DataSource = userInfo.DataSource ?? 0;
                users.CreatedAt = DateTime.Now;
                users.CreatedBy = _userInfo.Id;
                List<UsersGroupMapping> _listMapping = new();
                if (userInfo.ListGroupID.Count > 0)
                {
                    foreach (var item in userInfo.ListGroupID)
                    {
                        var mapping = new UsersGroupMapping
                        {
                            group_id = item,
                            Users = users
                        };
                        _listMapping.Add(mapping);
                    }
                }
                List<UsersRoles> _listroles = new();
                if (userInfo.ListRoleID.Count > 0)
                {
                    foreach (var item in userInfo.ListRoleID)
                    {
                        var _userrole = new UsersRoles
                        {
                            roles_id = item,
                            Users = users
                        };
                        _listroles.Add(_userrole);
                    }
                }
                _uow.Repository<UsersGroupMapping>().Insert(_listMapping);
                _uow.Repository<UsersRoles>().Insert(_listroles);
                _uow.Repository<Users>().AddWithoutSave(users);
                _uow.SaveChanges();

                if (userInfo.DataSource != 1)
                {
                    await Task.Run(() =>
                    {
                        var subject = "Thông tin đăng nhập";
                        var template_path = "UserNew.html";
                        MailUtils.SentMail(subject, userInfo, template_path);
                    }).ConfigureAwait(false);
                }
               
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CREATE USER - {userInfo.UserName} : {ex.Message}!");
                return BadRequest();
            }
        }

        public static bool VerifyPassword(string pass, string salt, string passhash)
        {
            var PlainPass = pass + salt; // append salt key
            return Crypto.VerifyHashedPassword(passhash, PlainPass); //verify password
        }

        public static string HashPassword_other(string password)
        {
            int SaltSize = 128 / 8;
            int Pbkdf2SubkeyLength = 256 / 8;
            byte[] salt = new byte[SaltSize];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            var subkey = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256, //KeyDerivationPrf.HMACSHA1
                iterationCount: 100000,
                numBytesRequested: Pbkdf2SubkeyLength);
            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            outputBytes[0] = 0x01; // format marker
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
            string hashed = Convert.ToBase64String(outputBytes);
            return hashed;
        }
        public static bool VerifyPassword_other(UsersModifyModels users, string hashpass, string pass)
        {
            var PasswordHasher = new PasswordHasher<UsersModifyModels>();
            var result = PasswordHasher.VerifyHashedPassword(users, hashpass, pass);
            return result == PasswordVerificationResult.Success;
        }
        [HttpGet("{id}")]
        public IActionResult Details(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var list_user = _uow.Repository<Users>().Find(a=>a.IsDeleted != true).ToArray();
                var user = list_user.FirstOrDefault(a => a.Id == id);
                var usergroup_mapping = _uow.Repository<UsersGroupMapping>().Find(a => a.users_id == id && a.UsersGroup.IsDeleted != true).Select(a => a.UsersGroup).ToArray();
                var userroles = _uow.Repository<UsersRoles>().Find(a => a.users_id == id && a.Roles.IsDeleted != true).Select(a => a.Roles).ToArray();
                if (user != null)
                {
                    var userinfo = new UsersInfoModels()
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Email = user.Email,
                        LastOnline = user.LastOnline,
                        IsDeleted = user.IsDeleted,
                        IsActive = user.IsActive == true ? 1 : 0,
                        RoleId = user.RoleId,
                        UsersType = user.UsersType,
                        DepartmentId = user.DepartmentId,
                        DateOfJoining = user.DateOfJoining,
                        CreatedBy = list_user.FirstOrDefault(a => a.Id == user.CreatedBy)?.FullName,
                        CreatedAt = user.CreatedAt,
                        ModifiedBy = list_user.FirstOrDefault(a => a.Id == user.ModifiedBy)?.FullName,
                        ModifiedAt = user.ModifiedAt,
                        ListGroup = usergroup_mapping.Select(a => a.Id + ":" + a.Name),
                        ListRoles = userroles.Select(a => a.Id + ":" + a.Name),
                        DataSource = user.DataSource ?? 0,
                    };
                    return Ok(new { code = "1", msg = "success", data = userinfo });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"GET_DEITAIL_USER - {id} : {ex.Message}!");
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Edit([FromBody] UsersModifyModels userInfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkusers = _uow.Repository<Users>().Find(a => ((a.Id == userInfo.Id) || (a.Id != userInfo.Id && a.Email.ToLower().Equals(userInfo.Email.ToLower()))) && a.IsDeleted != true);
                var users_check_mail = checkusers.FirstOrDefault(a => a.Id != userInfo.Id && a.Email.ToLower().Equals(userInfo.Email.ToLower()));
                if (users_check_mail != null)
                {
                    return Ok(new { code = "3", msg = "fail" });
                }
                var users = checkusers.FirstOrDefault(a => a.Id == userInfo.Id);
                if (users == null)
                {
                    return NotFound();
                }

                users.FullName = userInfo.FullName.Trim();
                users.UsersType = userInfo.UsersType;
                users.Email = userInfo.Email.ToLower().Trim();
                var status = userInfo.IsActive;
                users.IsActive = status == 1;
                users.ModifiedAt = DateTime.Now;
                users.ModifiedBy = _userInfo.Id;
                var usergroup_mapping = _uow.Repository<UsersGroupMapping>().Find(a => a.users_id == userInfo.Id);
                if (usergroup_mapping.Any())
                {
                    _uow.Repository<UsersGroupMapping>().Delete(usergroup_mapping);
                    _uow.SaveChanges();
                }
                List<UsersGroupMapping> _listMapping = new();
                if (userInfo.ListGroupID.Count > 0)
                {
                    foreach (var item in userInfo.ListGroupID)
                    {
                        var mapping = new UsersGroupMapping
                        {
                            group_id = item,
                            Users = users
                        };
                        _listMapping.Add(mapping);
                    }
                }

                var userroles = _uow.Repository<UsersRoles>().Find(a => a.users_id == userInfo.Id);
                if (userroles.Any())
                {
                    _uow.Repository<UsersRoles>().Delete(userroles);
                    _uow.SaveChanges();

                    var key = string.Format(_menuInfoPrefix, userInfo.UserName);
                    var check_key = _iDb.KeyExists(key);
                    if (check_key == true)
                    {
                        _iDb.KeyDelete(key);
                    }
                }
                List<UsersRoles> _listroles = new();
                if (userInfo.ListRoleID.Count > 0)
                {
                    foreach (var item in userInfo.ListRoleID)
                    {
                        var _userrole = new UsersRoles
                        {
                            roles_id = item,
                            Users = users
                        };
                        _listroles.Add(_userrole);
                    }
                }
                
                _uow.Repository<UsersGroupMapping>().Insert(_listMapping);
                _uow.Repository<UsersRoles>().Insert(_listroles);
                _uow.Repository<Users>().UpdateWithoutSave(users);
                _uow.SaveChanges();

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"EDIT_USER - {userInfo.UserName} : {ex.Message}!");
                return BadRequest();
            }
        }

        [HttpPost("{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var users = _uow.Repository<Users>().FirstOrDefault(a => a.Id == id);
                if (users == null)
                {
                    return NotFound();
                }
                var check_log_user = _uow.Repository<SystemLog>().Find(a => a.name == users.UserName).ToList();
                if (check_log_user.Count > 0)
                {
                    return Ok(new { code = "0", msg = "failed" });
                }
                else
                {
                    users.IsDeleted = true;
                    users.DeletedAt = DateTime.Now;
                    _uow.Repository<Users>().Update(users);
                    return Ok(new { code = "1", msg = "success" });
                }


            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE_USER - {id} : {ex.Message}!");
                return BadRequest();
            }
        }

        [HttpPost("DeleteAll")]
        public IActionResult DeleteAllConfirmed(DeleteAll obj)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                if (string.IsNullOrEmpty(obj.listID))
                {
                    return NotFound();
                }
                var list = obj.listID.Split(",").ToList();
                var users = _uow.Repository<Users>().Find(a => list.Contains(a.Id.ToString()) && a.UserName != "admin");
                List<Users> _user_temp = new();
                foreach (var item in users)
                {
                    item.IsDeleted = true;
                    item.DeletedAt = DateTime.Now;
                    _user_temp.Add(item);
                }

                //_uow.Repository<Users>().Update(_user_temp);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE_USER - {obj.listID} : {ex.Message}!");
                return BadRequest();
            }
        }
        [HttpPost("Active")]
        public IActionResult ActiveConfirmed(ActiveClass obj)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var users = _uow.Repository<Users>().FirstOrDefault(a => a.Id == obj.id);
                if (users == null)
                {
                    return NotFound();
                }
                if (obj.status == 1)
                {
                    users.IsActive = true;
                }
                else
                {
                    users.IsActive = false;
                }
                users.ModifiedAt = DateTime.Now;
                users.ModifiedBy = userInfo.Id;
                _uow.Repository<Users>().Update(users);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"ACTIVE_USER - {obj.id} : {ex.Message}!");
                return BadRequest();
            }
        }

        [HttpPost("ChangePass")]
        public async Task<IActionResult> ChangePassConfirmed(ChangePass obj)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var users = _uow.Repository<Users>().FirstOrDefault(a => a.Id == obj.id);
                if (users == null)
                {
                    return NotFound();
                }
                var pass = obj.password;
                string salt = Crypto.GenerateSalt(); // salt key
                var password = pass + salt;
                string hashedPassword = Crypto.HashPassword(password);
                users.Password = hashedPassword;
                users.SaltKey = salt;
                users.ModifiedAt = DateTime.Now;
                users.ModifiedBy = userInfo.Id;
                users.LastPassChange = DateTime.Now;
                _uow.Repository<Users>().Update(users);

                var model = new UsersModifyModels()
                {
                    FullName = users.FullName,
                    UserName = users.UserName,
                    Password = obj.password,
                    Email = users.Email
                };
                await Task.Run(() =>
                {
                    var subject = "Thay đổi thông tin đăng nhập";
                    var template_path = "ChangePassword.html";
                    MailUtils.SentMail(subject, model, template_path);
                }).ConfigureAwait(false);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CHANGE_PASS_USER - {obj.id} : {ex.Message}!");
                return BadRequest();
            }
        }
        protected async Task<IActionResult> ChangePass(Users users, ChangePass obj)
        {
            await Task.Delay(100);
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                if (users == null)
                {
                    return NotFound();
                }
                var pass = obj.password;
                string salt = Crypto.GenerateSalt(); // salt key
                var password = pass + salt;
                string hashedPassword = Crypto.HashPassword(password);
                users.Password = hashedPassword;
                users.SaltKey = salt;
                users.ModifiedAt = DateTime.Now;
                users.ModifiedBy = userInfo.Id;
                _uow.Repository<Users>().Update(users);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CHANGE_PASS_USER - {obj.id} : {ex.Message}!");
                return BadRequest();
            }
        }
        protected async Task<bool> Sentmail(Users users, ChangePass obj, string subject, string template_path)
        {
            await Task.Delay(100);
            if (users != null)
            {
                var model = new UsersModifyModels()
                {
                    FullName = users.FullName,
                    UserName = users.UserName,
                    Password = obj.password,
                    Email = users.Email
                };

                var result = MailUtils.SentMail(subject, model, template_path);
                return result;
            }
            else
            {
                return false;
            }
        }

        [HttpPost("ChangePassUser")]
        public async Task<IActionResult> ChangePassUserConfirmed(ChangePassUser obj)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var users = _uow.Repository<Users>().FirstOrDefault(a => a.Id == obj.id);
                if (users == null)
                {
                    return NotFound();
                }
                if (!VerifyPassword(obj.old_password, users.SaltKey, users.Password))
                {
                    return Ok(new { code = "0", msg = "fail" });
                }
                var pass = obj.new_password;
                string salt = Crypto.GenerateSalt(); // salt key
                var password = pass + salt;
                string hashedPassword = Crypto.HashPassword(password);
                users.Password = hashedPassword;
                users.SaltKey = salt;
                users.ModifiedAt = DateTime.Now;
                users.ModifiedBy = userInfo.Id;
                users.LastPassChange = DateTime.Now;
                _uow.Repository<Users>().Update(users);

                var model = new UsersModifyModels()
                {
                    FullName = users.FullName,
                    UserName = users.UserName,
                    Password = obj.new_password,
                    Email = users.Email
                };
                await Task.Run(() =>
                {
                    var subject = "Thay đổi thông tin đăng nhập";
                    var template_path = "ChangePassword.html";
                    MailUtils.SentMail(subject, model, template_path);
                }).ConfigureAwait(false);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CHANGE_PASS_USER - {obj.id} : {ex.Message}!");
                return BadRequest();
            }
        }

        [HttpPost("ChangeWorkplace")]
        public IActionResult ChangeWorkplaceConfirmed(ChangeWorkplace obj)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var users = _uow.Repository<Users>().FirstOrDefault(a => a.Id == obj.id);
                if (users == null)
                {
                    return NotFound();
                }
                users.DepartmentId = obj.departmentId;
                if (!string.IsNullOrEmpty(obj.dateofjoining))
                {
                    users.DateOfJoining = DateTime.ParseExact(obj.dateofjoining, "dd/MM/yyyy", null);
                }
                users.ModifiedAt = DateTime.Now;
                var user_working_history = new UsersWorkHistory
                {
                    DepartmentID = obj.departmentId
                };
                if (!string.IsNullOrEmpty(obj.dateofjoining))
                {
                    user_working_history.DateOfJoining = DateTime.ParseExact(obj.dateofjoining, "dd/MM/yyyy", null);
                }
                user_working_history.Users = users;
                user_working_history.CreatedAt = DateTime.Now;
                user_working_history.CreatedBy = userInfo.Id;
                _uow.Repository<UsersWorkHistory>().Insert(user_working_history);
                _uow.Repository<Users>().Update(users);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CHANGE_WORKPLACE_USER - {obj.id} : {ex.Message}!");
                return BadRequest();
            }
        }
        [HttpGet("SearchHistory")]
        public IActionResult SearchHistory(string jsonData)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var obj = JsonSerializer.Deserialize<ModelSearch>(jsonData);
                var user_type = string.IsNullOrEmpty(obj.UsersType) ? -1 : Convert.ToInt32(obj.UsersType);
                var deparmentid = string.IsNullOrEmpty(obj.DepartmentId) ? -1 : Convert.ToInt32(obj.DepartmentId);
                Expression<Func<UsersWorkHistory, bool>> filter = c =>
                                                (string.IsNullOrEmpty(obj.UserName) || c.Users.UserName.ToLower().Contains(obj.UserName.ToLower()))
                                                && (deparmentid == -1 || c.DepartmentID == deparmentid);
                var list_history = _uow.Repository<UsersWorkHistory>().Include(a => a.Users).Where(filter).OrderByDescending(a => a.DateOfJoining);
                IEnumerable<UsersWorkHistory> data = list_history;
                var count = list_history.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var department = _uow.Repository<Department>().GetAll(a => a.Deleted != true).ToDictionary(a => a.Id, a => a.Name);
                var user_history = data.Select(a => new UsersHistoryModels()
                {
                    Id = a.Id,
                    UserName = a.Users?.UserName,
                    FullName = a.Users?.FullName,
                    DateOfJoining = a.DateOfJoining?.ToString("dd/MM/yyyy"),
                    DepartmentId = a.DepartmentID,
                    DepartmentName = a.DepartmentID.HasValue ? (department.FirstOrDefault(x => x.Key == a.DepartmentID).Value ?? "") : ""
                });
                return Ok(new { code = "1", msg = "success", data = user_history, total = count });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersHistoryModels(), total = 0 });
            }
        }

        [HttpGet("DetailUserAudiWork/{id}")]
        public IActionResult DetailUserAudiWork(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkUsers = _uow.Repository<Users>().FirstOrDefault(a => a.Id == id);
                if (checkUsers != null)
                {
                    var userAudit = new UsersInfoModels()
                    {
                        Id = checkUsers.Id,
                        FullName = checkUsers.FullName,
                        Email = checkUsers.Email,

                    };
                    return Ok(new { code = "1", msg = "success", data = userAudit });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels() });
            }
        }
    }
}
