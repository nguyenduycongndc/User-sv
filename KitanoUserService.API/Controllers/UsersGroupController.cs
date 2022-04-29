using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Models;
using KitanoUserService.API.Models.ExecuteModels;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

namespace KitanoUserService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersGroupController : BaseController
    {
        protected readonly IConfiguration _config;
        public UsersGroupController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc , IConfiguration config , IDatabase iDb) : base(logger, uow, dbc , config , iDb)
        {
            _config = config;
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
                string KeyWord = q;
                Expression<Func<UsersGroup, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Name.Contains(q)) && c.IsActive == true && c.IsDeleted != true;
                var list_group = _uow.Repository<UsersGroup>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<UsersGroup> data = list_group;
                var group = data.Select(a => new UsersGroupModels()
                {
                    Id = a.Id,
                    Name = a.Name,
                });
                return Ok(new { code = "1", msg = "success", data = group });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersGroupModels() });
            }
        }
        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<ModelSearch>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.Status);
                Expression<Func<UsersGroup, bool>> filter = c => (string.IsNullOrEmpty(obj.FullName) || c.Name.Contains(obj.FullName.Trim()))
                                               && (string.IsNullOrEmpty(obj.Description) || c.Description.Contains(obj.Description.Trim()))
                                               && (status == -1 || (status == 1 ? c.IsActive == true : c.IsActive != true)) && c.IsDeleted != true;
                var list_group = _uow.Repository<UsersGroup>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<UsersGroup> data = list_group;
                var count = list_group.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.IsDeleted != true).Select(a => new UsersGroupModels()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    IsActive = a.IsActive,
                    Status = a.IsActive == true ? 1 : 0,
                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] UsersGroupModels userInfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _allgroupuser = _uow.Repository<UsersGroup>().Find(a => a.IsDeleted != true && a.Name.Equals(userInfo.Name)).ToArray();
                if (_allgroupuser.Length > 0)
                {
                    return BadRequest();
                }
               
                var usersgroup = new UsersGroup();
                usersgroup.Name = userInfo.Name.Trim();
                usersgroup.Description = userInfo.Description;
                var status = userInfo.Status;
                usersgroup.IsActive = status == 1;
                usersgroup.IsDeleted = false;
                usersgroup.CreatedAt = DateTime.Now;
                usersgroup.CreatedBy = _userInfo.Id;
                List<UsersGroupMapping> _listMapping = new List<UsersGroupMapping>();
                if (userInfo.ListUsersID.Count() > 0)
                {
                    foreach (var item in userInfo.ListUsersID)
                    {
                        var mapping = new UsersGroupMapping
                        {
                            users_id = item,
                            UsersGroup = usersgroup
                        };
                        _listMapping.Add(mapping);
                    }
                }
                List<UsersGroupRoles> _listroles = new List<UsersGroupRoles>();
                if (userInfo.ListRolesID.Count() > 0)
                {
                    foreach (var item in userInfo.ListRolesID)
                    {
                        var _userrole = new UsersGroupRoles
                        {
                            roles_id = item,
                            UsersGroup = usersgroup
                        };
                        _listroles.Add(_userrole);
                    }
                }
                _uow.Repository<UsersGroupRoles>().Insert(_listroles);
                _uow.Repository<UsersGroupMapping>().Insert(_listMapping);
                _uow.Repository<UsersGroup>().AddWithoutSave(usersgroup);
                _uow.SaveChanges();

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CREATE USER GROUP - {userInfo.Name} : {ex.Message}!");
                return BadRequest();
            }
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
                var usergroup = _uow.Repository<UsersGroup>().FirstOrDefault(a => a.Id == id);
                var usergroup_mapping = _uow.Repository<UsersGroupMapping>().Find(a => a.group_id == id ).Select(a=>a.Users).ToArray();
                var userroles = _uow.Repository<UsersGroupRoles>().Find(a => a.group_id == id && a.Roles.IsDeleted != true).Select(a => a.Roles).ToArray();
                var list_user = _uow.Repository<Users>().GetAll().ToArray();
                if (usergroup != null)
                {
                    var userinfo = new UsersGroupModels()
                    {
                        Id = usergroup.Id,
                        Name = usergroup.Name,
                        Description = usergroup.Description,
                        IsActive = usergroup.IsActive,
                        Status = usergroup.IsActive == true ? 1 : 0,
                        CreatedBy = list_user.FirstOrDefault(a => a.Id == usergroup.CreatedBy)?.FullName,
                        CreatedAt = usergroup.CreatedAt,
                        ModifiedBy = list_user.FirstOrDefault(a => a.Id == usergroup.ModifiedBy)?.FullName,
                        ModifiedAt = usergroup.ModifiedAt,
                        ListUsers = usergroup_mapping.Select(a=>a.Id + ":" + a.FullName),
                        ListRoles = userroles.Select(a => a.Id + ":" + a.Name)
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
                _logger.LogError($"GET_DEITAIL_USER_GROUP - {id} : {ex.Message}!");
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Edit([FromBody] UsersGroupModels userInfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var usersgroup = _uow.Repository<UsersGroup>().FirstOrDefault(a => a.Id == userInfo.Id);
                if (usersgroup == null)
                {
                    return NotFound();
                }
                var status = userInfo.Status;
                usersgroup.IsActive = status == 1;
                usersgroup.Name = userInfo.Name.Trim();
                usersgroup.Description = userInfo.Description;
                usersgroup.ModifiedAt = DateTime.Now;
                usersgroup.ModifiedBy = _userInfo.Id;
                var usergroup_mapping = _uow.Repository<UsersGroupMapping>().Find(a => a.group_id == userInfo.Id);
                if(usergroup_mapping.Count() > 0)
                {
                    _uow.Repository<UsersGroupMapping>().Delete(usergroup_mapping);
                    _uow.SaveChanges();
                }
                List<UsersGroupMapping> _listMapping = new List<UsersGroupMapping>();
                if (userInfo.ListUsersID.Count() > 0)
                {
                    foreach (var item in userInfo.ListUsersID)
                    {
                        var mapping = new UsersGroupMapping();
                        mapping.users_id = item;
                        mapping.UsersGroup = usersgroup;
                        _listMapping.Add(mapping);
                    }
                }
                var usergroup_roles = _uow.Repository<UsersGroupRoles>().Find(a => a.group_id == userInfo.Id);
                if (usergroup_roles.Count() > 0)
                {
                    _uow.Repository<UsersGroupRoles>().Delete(usergroup_roles);
                    _uow.SaveChanges();
                }
                List<UsersGroupRoles> _listroles = new List<UsersGroupRoles>();
                if (userInfo.ListRolesID.Count() > 0)
                {
                    foreach (var item in userInfo.ListRolesID)
                    {
                        var _userrole = new UsersGroupRoles
                        {
                            roles_id = item,
                            UsersGroup = usersgroup
                        };
                        _listroles.Add(_userrole);
                    }
                }
                _uow.Repository<UsersGroupRoles>().Insert(_listroles);
                _uow.Repository<UsersGroupMapping>().Insert(_listMapping);
                _uow.Repository<UsersGroup>().UpdateWithoutSave(usersgroup);
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"EDIT_USER_GROUP - {userInfo.Name} : {ex.Message}!");
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
                var usersgroup = _uow.Repository<UsersGroup>().Include(a=>a.UsersGroupMappings).FirstOrDefault(a => a.Id == id);
                if (usersgroup == null)
                {
                    return NotFound();
                }
                if (!usersgroup.UsersGroupMappings.Any())
                {
                    usersgroup.IsDeleted = true;
                    usersgroup.DeletedAt = DateTime.Now;
                    usersgroup.DeletedBy = userInfo.Id;
                    _uow.Repository<UsersGroup>().Update(usersgroup);
                    return Ok(new { code = "1", msg = "success" });
                }
                else
                {
                    return Ok(new { code = "0", msg = "fail" });
                }               
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE_USER_GROUP - {id} : {ex.Message}!");
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
                var usersgroup = _uow.Repository<UsersGroup>().Find(a => list.Contains(a.Id.ToString()) && !a.UsersGroupMappings.Any() );
                List<UsersGroup> _user_temp = new List<UsersGroup>();
                foreach (var item in usersgroup)
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
                _logger.LogError($"DELETE_USER_GROUP - {obj.listID} : {ex.Message}!");
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
                var usersgroup = _uow.Repository<UsersGroup>().FirstOrDefault(a => a.Id == obj.id);
                if (usersgroup == null)
                {
                    return NotFound();
                }
                if (obj.status == 1)
                {
                    usersgroup.IsActive = true;
                }
                else
                {
                    usersgroup.IsActive = false;
                }
                usersgroup.ModifiedBy = userInfo.Id;
                _uow.Repository<UsersGroup>().Update(usersgroup);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"ACTIVE_USER_GROUP - {obj.id} : {ex.Message}!");
                return BadRequest();
            }
        }
    }
}
