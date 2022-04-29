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
    public class ApprovalConfigController : BaseController
    {
        public ApprovalConfigController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc , IConfiguration config , IDatabase iDb) : base(logger, uow, dbc , config , iDb)
        {
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
        public IActionResult Search()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var list_menu = _uow.Repository<Menu>().Include(a=>a.ApprovalConfig).Where(a => a.IsConfigApproval == true && a.IsDeleted != true).ToArray();
                var list_config = list_menu.Select(a => new ApprovalConfigListModel()
                {
                    item_id = a.Id,                    
                    item_code = a.CodeName,
                    item_name = a.Description,
                    ApprovalLevel = a.ApprovalConfig.FirstOrDefault()?.ApprovalLevel,
                    IsOutside = a.ApprovalConfig.FirstOrDefault()?.IsOutside,
                    ListStatus = a.ApprovalConfig.Where(a=>a.IsShow == true).OrderBy(a=>a.StatusCode).Select(x => new ApprovalConfigStatusModel() { 
                        id = x.id,
                        StatusCode = x.StatusCode,
                        StatusDescription = x.StatusDescription,
                        StatusName = x.StatusName
                    }).ToList()
                });
                return Ok(new { code = "1", msg = "success", data = list_config });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPut]
        public IActionResult Edit([FromBody] ApprovalConfigModifyModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var list_config = new List<ApprovalConfig>();
                var list_config_update = new List<ApprovalConfig>();
                foreach (var item in model.ListConfig)
                {
                    var menu_config = _uow.Repository<ApprovalConfig>().Find(a => a.item_code == item.item_code).ToArray();
                    if (menu_config.Length == 0)
                    {
                        foreach (var _status_item in item.ListStatus)
                        {
                            var approval_config = new ApprovalConfig()
                            {
                                item_id = item.item_id,
                                item_code = item.item_code,
                                item_name = item.item_name,
                                ApprovalLevel = item.ApprovalLevel,
                                IsOutside = item.IsOutside == 1,
                                StatusCode = _status_item.StatusCode,
                                StatusDescription = _status_item.StatusDescription,
                                StatusName = _status_item.StatusName,
                                IsShow = true,
                            };
                            list_config.Add(approval_config);
                        }

                    }
                    else
                    {
                        foreach (var _status_item in item.ListStatus)
                        {
                            var status = menu_config.FirstOrDefault(a => a.StatusCode == _status_item.StatusCode);
                            if (status == null)
                            {
                                var approval_config = new ApprovalConfig()
                                {
                                    item_id = item.item_id,
                                    item_code = item.item_code,
                                    item_name = item.item_name,
                                    ApprovalLevel = item.ApprovalLevel,
                                    IsOutside = item.IsOutside == 1,
                                    StatusCode = _status_item.StatusCode,
                                    StatusDescription = _status_item.StatusDescription,
                                    StatusName = _status_item.StatusName,
                                    IsShow = true,
                                };
                                list_config.Add(approval_config);
                            }
                            else
                            {
                                status.ApprovalLevel = item.ApprovalLevel;
                                status.IsOutside = item.IsOutside == 1;
                                status.StatusCode = _status_item.StatusCode;
                                status.StatusDescription = _status_item.StatusDescription;
                                status.StatusName = _status_item.StatusName;
                                status.IsShow = true;
                                list_config_update.Add(status);
                            }
                        }

                        var status_old = menu_config.Where(a => !item.ListStatus.Any(x => x.StatusCode == a.StatusCode)).ToArray();
                        foreach (var _status_item in status_old)
                        {
                            _status_item.IsShow = false;
                            list_config_update.Add(_status_item);
                        }
                    }
                }
                foreach (var item in list_config_update)
                {
                    _uow.Repository<ApprovalConfig>().UpdateWithoutSave(item);
                }
                foreach (var item in list_config)
                {
                    _uow.Repository<ApprovalConfig>().AddWithoutSave(item);
                }
                _uow.SaveChanges();
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception ex)
            {
                _logger.LogError($"APPROVAL_CONFIG: {ex.Message}!");
                return BadRequest();
            }
        }
    }
}
