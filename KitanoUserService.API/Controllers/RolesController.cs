using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Models;
using KitanoUserService.API.Models.ExecuteModels;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace KitanoUserService.API.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class RolesController : BaseController
    {
        private readonly IDatabase _iDb;
        private string _menuInfoPrefix = "Auth.MenuInfo.{0}";
        public RolesController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc, IDatabase redisDb , IConfiguration config, IDatabase iDb) : base(logger, uow, dbc , config , iDb)
        {
            _iDb = redisDb;
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
                Expression<Func<Roles, bool>> filter = c => (string.IsNullOrEmpty(q) || c.Name.Contains(q)) && c.IsActive == true && c.IsDeleted != true;
                var list_group = _uow.Repository<Roles>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<Roles> data = list_group;
                var _roles = data.Select(a => new RolesModels()
                {
                    Id = a.Id,
                    Name = a.Name,
                });
                return Ok(new { code = "1", msg = "success", data = _roles });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new RolesModels() });
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
                Expression<Func<Roles, bool>> filter = c => (string.IsNullOrEmpty(obj.FullName) || c.Name.ToLower().Contains(obj.FullName.Trim().ToLower()))
                                               && (string.IsNullOrEmpty(obj.Description) || c.Description.ToLower().Contains(obj.Description.Trim().ToLower()))
                                               && (status == -1 || (status == 1 ? c.IsActive == true : c.IsActive != true)) && c.IsDeleted != true;
                var list_roles = _uow.Repository<Roles>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<Roles> data = list_roles;
                var count = list_roles.Count();

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.IsDeleted != true).Select(a => new RolesModels()
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

        [HttpGet("TreePermission")]
        public IActionResult GetTreePermission(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var list_assign = _uow.Repository<RolePermissionMenu>().Find(a => a.role_id == id).Select(a=> a.menu_id+"_"+a.permission_id).ToArray();
                var list_permission = _uow.Repository<Permission>().Find(a => a.IsDeleted != true && a.IsActive == true).OrderBy(a => a.Type).ToArray();
                var per_view = list_permission.FirstOrDefault(a => a.Type == 1);
                var list_menu = _uow.Repository<Menu>().Find(a=>a.IsDeleted != true && a.IsActive == true).OrderBy(a=>a.SortOrder).ToArray();
                var list_parent = list_menu.Where(a => a.ParentID == 0);
                var _listRender = new List<RenderPermission>();
                foreach (var item in list_parent)
                {
                    var _itemRender = new RenderPermission
                    {
                        Id = item.Id + "_" + per_view?.Id,
                        Name = item.Description,
                        Text = "<b style='color: blue'>" + item.Description + "</b>",
                        State = new StateModel()
                        {
                            Opened = true,
                            Selected = item.CodeName == "M_H" && list_assign.Contains(item.Id + "_" + per_view.Id)
                        },
                        ParentId = "0"
                    };
                    var checkExistChild = list_menu.Count(a => a.ParentID == item.Id);
                    if(checkExistChild > 0)
                    {
                        _itemRender.Children = LoopGetChild(item.Id, list_menu, list_permission, per_view?.Id, list_assign);
                    }
                    else
                    {
                        if (item.CodeName != "M_H")
                        {
                            var _listChild = new List<RenderPermission>();
                            for (int j = 0; j < list_permission.Length; j++)
                            {
                                var itemPer = list_permission[j];
                                var _idTemp = item.Id + "_" + itemPer.Id;
                                var _id = item.Id + "_" + itemPer.Id + "_" + per_view?.Id;
                                var _itemPerRender = new RenderPermission
                                {
                                    Id = _id,
                                    Name = itemPer.Description,
                                    Type = "file",
                                    Text = "<b style='color: blue'>" + itemPer.Description + "</b>",
                                    ParentId = item.Id + "_" + per_view?.Id
                                };
                                if (list_assign.Contains(_idTemp))
                                {
                                    _itemPerRender.State = new StateModel()
                                    {
                                        Selected = true,
                                    };
                                }
                                _listChild.Add(_itemPerRender);
                            }
                            _itemRender.Children = _listChild;
                        }
                    }
                    _listRender.Add(_itemRender);
                }
                return Ok(new { code = "1", msg = "success", data = _listRender, total = 0 });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        public static List<RenderPermission> LoopGetChild(int parentId,[FromBody] Menu[] menus,[FromBody] Permission[] permissions,int? per_view_id, [FromBody] string[] list_asign)
        {
            var rs = new List<RenderPermission>();        
            var _listClosestByParent = menus.Where(a => a.ParentID == parentId).ToArray();
            for (int i = 0; i < _listClosestByParent.Length; i++)
            {
                var item = _listClosestByParent[i];
                var _itemRender = new RenderPermission
                {
                    Id = item.Id + "_" + per_view_id,
                    Name = item.Description,
                    Text = "<b style='color: blue'>" + item.Description + "</b>",
                    ParentId = parentId + "_" + per_view_id
                };
                var checkExistChild = menus.Count(a => a.ParentID == item.Id);
                var _listChild = new List<RenderPermission>();
                if (checkExistChild > 0)
                {
                    _listChild = LoopGetChild(item.Id, menus, permissions, per_view_id, list_asign);
                }
                else
                {
                    for (int j = 0; j < permissions.Length; j++)
                    {
                        var itemPer = permissions[j];
                        var _idTemp = item.Id + "_" + itemPer.Id;
                        var _id = item.Id + "_" + itemPer.Id + "_" + per_view_id;
                        var _itemPerRender = new RenderPermission
                        {
                            Id = _id,
                            Name = itemPer.Description,
                            Type = "file",
                            Text = "<b style='color: blue'>" + itemPer.Description + "</b>",
                            ParentId = item.Id + "_" + per_view_id
                        };
                        if (list_asign.Contains(_idTemp))
                        {
                            _itemPerRender.State = new StateModel()
                            {
                                Selected = true,
                            };
                        }
                        _listChild.Add(_itemPerRender);
                    }
                }
                _itemRender.Children = _listChild;
                rs.Add(_itemRender);
            }
            return rs;
        }

        [HttpGet("Menu")]
        public IActionResult GetTreeMenu()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var value_get = _iDb.StringGet(string.Format(_menuInfoPrefix, userInfo.UserName));
                if (value_get.HasValue)
                {
                    var list_menu = JsonSerializer.Deserialize<List<MenuModels>>(value_get);
                    return Ok(list_menu);
                }
                else
                {
                    var list_role_assign = userInfo.RoleList.Select(a => a.RolesId).ToArray();
                    var list_menu_assign = _uow.Repository<RolePermissionMenu>().Find(a => list_role_assign.Contains(a.role_id)).Select(a => a.menu_id).ToArray();
                    var list_menu = _uow.Repository<Menu>().Find(a => a.IsDeleted != true && a.IsActive == true && list_menu_assign.Contains(a.Id)).Select(b => new MenuModels()
                    {
                        Id = b.Id,
                        CodeName = b.CodeName,
                        DefaultUrl = b.DefaultUrl,
                        Description = b.Description,
                        Icon = b.Icon,
                        SortOrder = b.SortOrder,
                        ParentID = b.ParentID,
                        Ancestor = b.Ancestor
                    }).ToArray();
                    var now = DateTime.Now;
                    var date = now.AddMinutes(30);
                    var expiryTimeSpan = date.Subtract(now);
                    _iDb.StringSet(string.Format(_menuInfoPrefix, userInfo.UserName), JsonSerializer.Serialize(list_menu), expiryTimeSpan);
                    return Ok(list_menu);
                }
               
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] RolesModels rolesInfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _allroles = _uow.Repository<Roles>().Find(a => a.IsDeleted != true && a.Name.Equals(rolesInfo.Name.Trim())).ToArray();
                if (_allroles.Length > 0)
                {
                    return BadRequest();
                }

                var _roles = new Roles
                {
                    Name = rolesInfo.Name?.Trim(),
                    Description = rolesInfo.Description?.Trim()
                };
                var status = rolesInfo.Status;
                _roles.IsActive = status == 1;
                _roles.IsDeleted = false;
                _roles.CreatedAt = DateTime.Now;
                _roles.CreatedBy = _userInfo.Id;               
                _uow.Repository<Roles>().Add(_roles);

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"CREATE ROLES - {rolesInfo.Name} : {ex.Message}!");
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
                var roles = _uow.Repository<Roles>().FirstOrDefault(a => a.Id == id);
                var list_user = _uow.Repository<Users>().GetAll().ToArray();
                if (roles != null)
                {
                    var userinfo = new RolesModels()
                    {
                        Id = roles.Id,
                        Name = roles.Name,
                        Description = roles.Description,
                        IsActive = roles.IsActive,
                        Status = roles.IsActive == true ? 1 : 0,
                        CreatedBy = list_user.FirstOrDefault(a => a.Id == roles.CreatedBy)?.FullName,
                        CreatedAt = roles.CreatedAt,
                        ModifiedBy = list_user.FirstOrDefault(a => a.Id == roles.ModifiedBy)?.FullName,
                        ModifiedAt = roles.ModifiedAt,
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
                _logger.LogError($"GET_DEITAIL_ROLES - {id} : {ex.Message}!");
                return BadRequest();
            }
        }

        [HttpPut]
        public IActionResult Edit([FromBody] RolesModels rolesInfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _roles = _uow.Repository<Roles>().FirstOrDefault(a => a.Id == rolesInfo.Id);
                if (_roles == null)
                {
                    return NotFound();
                }
                var status = rolesInfo.Status;
                _roles.IsActive = status == 1;
                _roles.Name = rolesInfo.Name.Trim();
                _roles.Description = rolesInfo.Description?.Trim();
                _roles.ModifiedAt = DateTime.Now;
                _roles.ModifiedBy = _userInfo.Id;
                _uow.Repository<Roles>().Update(_roles);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"EDIT_USER_ROLES - {rolesInfo.Name} : {ex.Message}!");
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
                var _roles = _uow.Repository<Roles>().Include(a=>a.UsersRoles, a=>a.UsersGroupRoles).FirstOrDefault(a => a.Id == id);
                if (_roles == null)
                {
                    return NotFound();
                }
                if (!_roles.UsersRoles.Any() && !_roles.UsersGroupRoles.Any())
                {
                    _roles.IsDeleted = true;
                    _roles.DeletedAt = DateTime.Now;
                    _roles.DeletedBy = userInfo.Id;
                    _uow.Repository<Roles>().Update(_roles);
                    return Ok(new { code = "1", msg = "success" });
                }
                else
                {
                    return Ok(new { code = "0", msg = "fail" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE_ROLES - {id} : {ex.Message}!");
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
                var usersgroup = _uow.Repository<Roles>().Find(a => list.Contains(a.Id.ToString()) && !a.UsersRoles.Any() && !a.UsersGroupRoles.Any());
                List<Roles> _user_temp = new();
                foreach (var item in usersgroup)
                {
                    item.IsDeleted = true;
                    item.DeletedAt = DateTime.Now;
                    _user_temp.Add(item);
                }
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"DELETE_ROLES - {obj.listID} : {ex.Message}!");
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
                var _roles = _uow.Repository<Roles>().FirstOrDefault(a => a.Id == obj.id);
                if (_roles == null)
                {
                    return NotFound();
                }
                if (obj.status == 1)
                {
                    _roles.IsActive = true;
                }
                else
                {
                    _roles.IsActive = false;
                }
                _roles.ModifiedBy = userInfo.Id;
                _uow.Repository<Roles>().Update(_roles);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"ACTIVE_ROLES - {obj.id} : {ex.Message}!");
                return BadRequest();
            }
        }
        [HttpPost("UpdatePermission")]
        public IActionResult UpdatePermission([FromBody] UpdateRolesPermissionModels rolesInfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _allrolespermission = _uow.Repository<RolePermissionMenu>().Find(a => a.role_id == rolesInfo.Id).ToArray();
                if (_allrolespermission.Length > 0)
                {
                    _uow.Repository<RolePermissionMenu>().Delete(_allrolespermission);
                    _uow.SaveChanges();
                }
                var list_add = new List<RolePermissionMenu>();
                if (rolesInfo.ListPermission.Count > 0)
                { 
                    foreach (var item in rolesInfo.ListPermission)
                    {
                        var _rolepermission = new RolePermissionMenu();
                        var menu_per = item.Split('_');
                        _rolepermission.role_id = rolesInfo.Id;
                        _rolepermission.menu_id = Convert.ToInt32(menu_per[0]);
                        _rolepermission.permission_id = Convert.ToInt32(menu_per[1]);
                        list_add.Add(_rolepermission);
                    }
                }
                var list_user = _uow.Repository<UsersRoles>().Include(a => a.Users).Where(a => a.roles_id == rolesInfo.Id).Select(a => a.Users.UserName).Distinct().ToArray();
                for (int i = 0; i < list_user.Length; i++)
                {
                    var _userItem = list_user[i];
                    var key = string.Format(_menuInfoPrefix, _userItem);
                    var check_key = _iDb.KeyExists(key);
                    if (check_key == true)
                    {
                        _iDb.KeyDelete(key);
                    }
                }

                _uow.Repository<RolePermissionMenu>().Insert(list_add);
                _uow.SaveChanges();

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"UPDATE PERMISSION ROLE - {rolesInfo.Id} : {ex.Message}!");
                return BadRequest();
            }
        }
    }
}
