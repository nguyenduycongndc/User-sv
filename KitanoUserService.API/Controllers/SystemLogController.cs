using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Models.ExecuteModels;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;
using System.Threading.Tasks;

namespace KitanoUserService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SystemLogController : BaseController
    {
        public SystemLogController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc, IConfiguration config, IDatabase iDb) : base(logger, uow, dbc, config, iDb)
        {
        }
        [HttpGet("ListSystemLog")]
        public IActionResult List()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var listSystemLog = _uow.Repository<SystemLog>().Find(c => true).ToList();
                return Ok(new { code = "1", msg = "success", data = listSystemLog });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new SystemLogModel(), total = 0 });
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
                var obj = JsonSerializer.Deserialize<SystemLogSearchModel>(jsonData);
                DateTime? _start = !string.IsNullOrEmpty(obj.start_date) ? DateTime.Parse(obj.start_date) : null;
                DateTime? _end = !string.IsNullOrEmpty(obj.end_date) ? DateTime.Parse(obj.end_date).AddDays(1) : null;
                DateTime? _endfirst = !string.IsNullOrEmpty(obj.end_date) ? DateTime.Parse(obj.end_date) : null;

                if (_start > _endfirst) { return Ok(new { code = "-1", msg = "DateError" }); }

                Expression<Func<SystemLog, bool>> filter = c => (string.IsNullOrEmpty(obj.name) || c.name.ToLower().Contains(obj.name.ToLower()))
                                               && (string.IsNullOrEmpty(obj.module) || c.module.ToLower().Contains(obj.module.ToLower()))
                                                //&& (string.IsNullOrEmpty(obj.version) || c.version.ToLower().Contains(obj.version.ToLower()))
                                                && (string.IsNullOrEmpty(obj.start_date) || c.datetime >= _start)
                                                && (string.IsNullOrEmpty(obj.end_date) || c.datetime < _end);

                var list_systemLog = _uow.Repository<SystemLog>().Find(filter).OrderByDescending(a => a.datetime);
                IEnumerable<SystemLogDataMd> data = list_systemLog.Select(x => new SystemLogDataMd()
                {
                    id = x.id,
                    module = x.module,
                    datetime = x.datetime.HasValue ? x.datetime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    name = x.name,
                    perform_tasks = x.perform_task,
                }).ToList();
                var count = list_systemLog.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }

                if (obj.start_number >= 0 && obj.page_size > 0)
                {
                    data = data.Skip(obj.start_number).Take(obj.page_size);
                }
                return Ok(new { code = "1", msg = "success", data = data, total = count, record_number = obj.start_number });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new SystemLogMd(), total = 0 });
            }
        }

        [HttpGet("SearchAuditPlan")]
        public IActionResult SearchAuditPlan(string jsonData)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var obj = JsonSerializer.Deserialize<MonngoLogSearchMd>(jsonData);
                DateTime? _start = !string.IsNullOrEmpty(obj.start_date) ? DateTime.Parse(obj.start_date) : null;
                DateTime? _end = !string.IsNullOrEmpty(obj.end_date) ? DateTime.Parse(obj.end_date).AddDays(1) : null;
                DateTime? _endfirst = !string.IsNullOrEmpty(obj.end_date) ? DateTime.Parse(obj.end_date) : null;

                if (_start > _endfirst) { return Ok(new { code = "-1", msg = "DateError" }); }
                var audit_search = obj.auditplan_id + "_";
                Expression<Func<SystemLog, bool>> filter = c => (string.IsNullOrEmpty(obj.name) || c.name.ToLower().Contains(obj.name.ToLower()))
                                               && (string.IsNullOrEmpty(obj.module) || c.module.ToLower().Contains(obj.module.ToLower()))
                                                //&& (string.IsNullOrEmpty(obj.version) || c.version.ToLower().Contains(obj.version.ToLower()))
                                                && (string.IsNullOrEmpty(obj.start_date) || c.datetime >= _start)
                                                && (string.IsNullOrEmpty(obj.end_date) || c.datetime < _end);

                var list_systemLog = _uow.Repository<SystemLog>().Find(filter).ToList().OrderByDescending(a => a.datetime);
                IEnumerable<SystemLogDataMd> data = list_systemLog.Select(x => new SystemLogDataMd()
                {
                    id = x.id,
                    module = x.module,
                    datetime = x.datetime.HasValue ? x.datetime.Value.ToString("dd/MM/yyyy HH:mm:ss") : "",
                    name = x.name,
                    perform_tasks = x.perform_task,
                    //version = !string.IsNullOrEmpty(x.version) ? x.version.Split("_")[1] : ""
                }).ToList();
                var count = list_systemLog.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }
                return Ok(new { code = "1", msg = "success", data = data, total = count, record_number = obj.start_number });
            }
            catch (Exception ex)
            {
                return Ok(new { code = "0", msg = "fail", data = new SystemLog(), total = 0 });
            }
        }
        [HttpPost]
        public IActionResult Create([FromBody] SystemLogCreateMd systemLogCreateModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkSystemLog = _uow.Repository<SystemLog>().Find(a => a.module.Equals(systemLogCreateModel.module)
                                                            && a.name.Equals(_userInfo.UserName)
                                                            && a.perform_task.Equals(systemLogCreateModel.perform_tasks)).ToList();
                if (checkSystemLog.Count > 0)
                {
                    checkSystemLog[0].module = systemLogCreateModel.module;
                    checkSystemLog[0].name = _userInfo.UserName;
                    checkSystemLog[0].perform_task = systemLogCreateModel.perform_tasks;
                    checkSystemLog[0].datetime = DateTime.Now;
                    _uow.Repository<SystemLog>().Update(checkSystemLog);
                    return Ok(new { code = "1", msg = "success" });
                }
                SystemLog systemLog = new SystemLog()
                {
                    module = systemLogCreateModel.module,
                    name = _userInfo.UserName,
                    perform_task = systemLogCreateModel.perform_tasks,
                    datetime = DateTime.Now,
                };
                _uow.Repository<SystemLog>().Add(systemLog);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("CreateAuditPlan")]
        public IActionResult CreateAuditPlan([FromBody] SystemLogCreateMd systemLogCreateModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                SystemLog systemLog = new()
                {
                    module = systemLogCreateModel.module,
                    name = _userInfo.UserName,
                    perform_task = systemLogCreateModel.perform_tasks,
                    //version = systemLogCreateModel.version,
                    datetime = DateTime.Now,
                };
                _uow.Repository<SystemLog>().Insert(systemLog);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }


        //[HttpGet("ListSystemLog")]
        //public IActionResult List()
        //{
        //    try
        //    {
        //        if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
        //        {
        //            return Unauthorized();
        //        }
        //        var listSystemLog = _dbc.SystemLogs.Find(c => true).ToList();
        //        return Ok(new { code = "1", msg = "success", data = listSystemLog });
        //    }
        //    catch (Exception)
        //    {
        //        return Ok(new { code = "0", msg = "fail", data = new SystemLogModel(), total = 0 });
        //    }
        //}
        //[HttpGet("Search")]
        //public IActionResult Search(string jsonData)
        //{
        //    try
        //    {
        //        if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
        //        {
        //            return Unauthorized();
        //        }
        //        var obj = JsonSerializer.Deserialize<SystemLogSearchModel>(jsonData);
        //        DateTime? _start = !string.IsNullOrEmpty(obj.start_date) ? DateTime.Parse(obj.start_date) : null;
        //        DateTime? _end = !string.IsNullOrEmpty(obj.end_date) ? DateTime.Parse(obj.end_date).AddDays(1) : null;
        //        DateTime? _endfirst = !string.IsNullOrEmpty(obj.end_date) ? DateTime.Parse(obj.end_date) : null;

        //        if(_start > _endfirst) { return Ok(new { code = "-1", msg = "DateError" });}

        //        Expression<Func<SystemLogModel, bool>> filter = c => (string.IsNullOrEmpty(obj.name) || c.name.ToLower().Contains(obj.name.ToLower()))
        //                                       && (string.IsNullOrEmpty(obj.module) || c.module.ToLower().Contains(obj.module.ToLower()))
        //                                        //&& (string.IsNullOrEmpty(obj.version) || c.version.ToLower().Contains(obj.version.ToLower()))
        //                                        && (string.IsNullOrEmpty(obj.start_date) || c.datetime >= _start)
        //                                        && (string.IsNullOrEmpty(obj.end_date) || c.datetime < _end);

        //        var list_systemLog = _dbc.SystemLogs.Find(filter).ToList().OrderByDescending(a => a.datetime);
        //        IEnumerable<SystemLogDataModel> data = list_systemLog.Select(x => new SystemLogDataModel()
        //        {
        //            _id = x._id,
        //            module = x.module,
        //            datetime = x.datetime.ToString("dd/MM/yyyy HH:mm:ss"),
        //            name = x.name,
        //            perform_tasks = x.perform_tasks,
        //            version = x.version
        //        }).ToList();
        //        var count = list_systemLog.Count();
        //        if (count == 0)
        //        {
        //            return Ok(new { code = "1", msg = "success", data = "", total = count });
        //        }

        //        if (obj.start_number >= 0 && obj.page_size > 0)
        //        {
        //            data = data.Skip(obj.start_number).Take(obj.page_size);
        //        }
        //        return Ok(new { code = "1", msg = "success", data = data, total = count, record_number = obj.start_number });
        //    }
        //    catch (Exception)
        //    {
        //        return Ok(new { code = "0", msg = "fail", data = new SystemLogModel(), total = 0});
        //    }
        //}

        //[HttpGet("SearchAuditPlan")]
        //public IActionResult SearchAuditPlan(string jsonData)
        //{
        //    try
        //    {
        //        if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
        //        {
        //            return Unauthorized();
        //        }
        //        var obj = JsonSerializer.Deserialize<MonngoLogSearchModel>(jsonData);
        //        DateTime? _start = !string.IsNullOrEmpty(obj.start_date) ? DateTime.Parse(obj.start_date) : null;
        //        DateTime? _end = !string.IsNullOrEmpty(obj.end_date) ? DateTime.Parse(obj.end_date).AddDays(1) : null;
        //        DateTime? _endfirst = !string.IsNullOrEmpty(obj.end_date) ? DateTime.Parse(obj.end_date) : null;

        //        if (_start > _endfirst) { return Ok(new { code = "-1", msg = "DateError" }); }
        //        var audit_search = obj.auditplan_id + "_";
        //        Expression<Func<SystemLogModel, bool>> filter = c => (string.IsNullOrEmpty(obj.name) || c.name.ToLower().Contains(obj.name.ToLower()))
        //                                       && (string.IsNullOrEmpty(obj.module) || c.module.ToLower().Contains(obj.module.ToLower()))
        //                                        //&& (string.IsNullOrEmpty(obj.version) || c.version.ToLower().Contains(obj.version.ToLower()))
        //                                        && (string.IsNullOrEmpty(obj.start_date) || c.datetime >= _start)
        //                                        && (string.IsNullOrEmpty(obj.end_date) || c.datetime < _end)
        //                                        && (c.version.StartsWith(audit_search));

        //        var list_systemLog = _dbc.SystemLogs.Find(filter).ToList().OrderByDescending(a => a.datetime);
        //        IEnumerable<SystemLogDataModel> data = list_systemLog.Select(x => new SystemLogDataModel()
        //        {
        //            _id = x._id,
        //            module = x.module,
        //            datetime = x.datetime.ToString("dd/MM/yyyy HH:mm:ss"),
        //            name = x.name,
        //            perform_tasks = x.perform_tasks,
        //            version = !string.IsNullOrEmpty(x.version) ? x.version.Split("_")[1] : ""
        //        }).ToList();
        //        var count = list_systemLog.Count();
        //        if (count == 0)
        //        {
        //            return Ok(new { code = "1", msg = "success", data = "", total = count });
        //        }

        //        //if (obj.start_number >= 0 && obj.page_size > 0)
        //        //{
        //        //    data = data.Skip(obj.start_number).Take(obj.page_size);
        //        //}
        //        return Ok(new { code = "1", msg = "success", data = data, total = count, record_number = obj.start_number });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Ok(new { code = "0", msg = "fail", data = new SystemLogModel(), total = 0 });
        //    }
        //}
        //[HttpPost]
        //public IActionResult Create([FromBody] SystemLogCreateModel systemLogCreateModel)
        //{
        //    try
        //    {
        //        if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
        //        {
        //            return Unauthorized();
        //        }
        //        var checkSystemLog = _dbc.SystemLogs.Find(a => a.module.Equals(systemLogCreateModel.module)
        //                                                    && a.name.Equals(_userInfo.UserName) 
        //                                                    && a.perform_tasks.Equals(systemLogCreateModel.perform_tasks)).ToList();
        //        if (checkSystemLog.Count > 0)
        //        {
        //            var _systemLog = new SystemLogModel()
        //            {
        //                _id = checkSystemLog[0]._id,
        //                module = checkSystemLog[0].module,
        //                name = checkSystemLog[0].name,
        //                perform_tasks = checkSystemLog[0].perform_tasks,
        //                version = checkSystemLog[0].version,
        //                datetime = DateTime.Now,
        //            };
        //            _dbc.SystemLogs.ReplaceOne(a => a._id == checkSystemLog[0]._id, _systemLog);
        //            return Ok(new { code = "1", msg = "success" });
        //        }
        //        SystemLogModel systemLog = new SystemLogModel()
        //        {
        //            module = systemLogCreateModel.module,
        //            name = _userInfo.UserName,
        //            perform_tasks = systemLogCreateModel.perform_tasks,
        //            version = systemLogCreateModel.version,
        //            datetime = DateTime.Now,
        //        };
        //        _dbc.SystemLogs.InsertOne(systemLog);
        //        return Ok(new { code = "1", msg = "success" });
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}

        //[HttpPost("CreateAuditPlan")]
        //public IActionResult CreateAuditPlan([FromBody] SystemLogCreateModel systemLogCreateModel)
        //{
        //    try
        //    {
        //        if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
        //        {
        //            return Unauthorized();
        //        }
        //        SystemLogModel systemLog = new()
        //        {
        //            module = systemLogCreateModel.module,
        //            name = _userInfo.UserName,
        //            perform_tasks = systemLogCreateModel.perform_tasks,
        //            version = systemLogCreateModel.version,
        //            datetime = DateTime.Now,
        //        };
        //        _dbc.SystemLogs.InsertOne(systemLog);
        //        return Ok(new { code = "1", msg = "success" });
        //    }
        //    catch (Exception)
        //    {
        //        return BadRequest();
        //    }
        //}
    }
}
