using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Models.ExecuteModels;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class SystemParameterController : BaseController
    {
        public SystemParameterController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc , IConfiguration config , IDatabase iDb) : base(logger, uow, dbc , config , iDb)
        {
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
                var obj = JsonSerializer.Deserialize<SystemParameterSearchModel>(jsonData);
                Expression<Func<SystemParameter, bool>> filter = c => 
                (string.IsNullOrEmpty(obj.Parameter_Name) || c.Parameter_Name.ToLower().Contains(obj.Parameter_Name.ToLower()))
                && (string.IsNullOrEmpty(obj.Sub_System) || c.Sub_System.ToLower().Contains(obj.Sub_System.ToLower()));
                var List_SystemParameter = _uow.Repository<SystemParameter>().Find(filter).OrderBy(a => a.Id);
                IEnumerable<SystemParameter> data = List_SystemParameter;
                var count = List_SystemParameter.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var systemParameter = data.Select(sp => new SystemParameterModel()
                {
                    Id = sp.Id,
                    Sub_System = sp.Sub_System,
                    Parameter_Name = sp.Parameter_Name,
                    Value = (sp.Parameter_Name != "MAIL_PASSWORD" ? sp.Value : "************"),
                    Note = sp.Note,
                    Modified_At = sp.Modified_At,
                    Modified_By = sp.Modified_By,
                    Reset_At = sp.Reset_At,
                    Default_Value = sp.Default_Value,
                    Default_Note = sp.Default_Note,
                });
                return Ok(new { code = "1", msg = "success", data = systemParameter, total = count, record_number = obj.StartNumber });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels(), total = 0 });
            }
        }
        [HttpGet("{id}")]
        public IActionResult Detail(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkSystemParameter = _uow.Repository<SystemParameter>().FirstOrDefault(a => a.Id == id);

                if (checkSystemParameter != null)
                {
                    var systemParameter = new SystemParameterModel()
                    {
                        Id = checkSystemParameter.Id,
                        Sub_System = checkSystemParameter.Sub_System,
                        Parameter_Name = checkSystemParameter.Parameter_Name,
                        Value = checkSystemParameter.Value,
                        Note = checkSystemParameter.Note,
                        Modified_At = checkSystemParameter.Modified_At,
                        Modified_By = checkSystemParameter.Modified_By,
                        Reset_At = checkSystemParameter.Reset_At,
                        Default_Value = checkSystemParameter.Default_Value,
                        Default_Note = checkSystemParameter.Default_Note,
                    };
                    return Ok(new { code = "1", msg = "success", data = systemParameter });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPut]
        public IActionResult Edit([FromBody] SystemParameterModifyModel systemParameterModifyModel)
        {
            try
            {

                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkSystemParameter = _uow.Repository<SystemParameter>().FirstOrDefault(a => a.Id == systemParameterModifyModel.Id);
                if (checkSystemParameter == null)
                {
                    return NotFound();
                }
                checkSystemParameter.Value = systemParameterModifyModel.Value;
                checkSystemParameter.Note = systemParameterModifyModel.Note;
                checkSystemParameter.Modified_At = DateTime.Now;
                checkSystemParameter.Modified_By = _userInfo.Id;
                _uow.Repository<SystemParameter>().Update(checkSystemParameter);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("{id}")]
        public IActionResult Reset(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkSystemParameter = _uow.Repository<SystemParameter>().FirstOrDefault(a => a.Id == id);
                if (checkSystemParameter == null)
                {
                    return NotFound();
                }
                checkSystemParameter.Reset_At = DateTime.Now;
                checkSystemParameter.Value = checkSystemParameter.Default_Value;
                checkSystemParameter.Note = checkSystemParameter.Default_Note;
                checkSystemParameter.Modified_At = null;
                checkSystemParameter.Modified_By = null;
                _uow.Repository<SystemParameter>().Update(checkSystemParameter);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpGet("ListSystemParameter")]
        public IActionResult List()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var listSystemParameter = _uow.Repository<SystemParameter>().GetAll().OrderBy(a => a.Id);
                var systemParameter = listSystemParameter.Select(a => new SystemParameterModifyModel()
                {
                    Id = a.Id,
                    Sub_System = a.Sub_System,
                    Parameter_Name = a.Parameter_Name,
                    Value = a.Value,
                    Note = a.Note,
                });
                return Ok(new { code = "1", msg = "success", data = systemParameter });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels(), total = 0 });
            }
        }
    }
}
