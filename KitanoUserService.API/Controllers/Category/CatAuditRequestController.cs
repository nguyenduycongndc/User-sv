using System;
using System.Collections.Generic;
using System.Linq;
using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Models.ExecuteModels;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using System.Text.Json;
using KitanoUserService.API.Models.MigrationsModels.Category;
using KitanoUserService.API.Models.ExecuteModels.Category;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;

namespace KitanoUserService.API.Controllers.Category
{
    [Route("[controller]")]
    [ApiController]
    public class CatAuditRequestController : BaseController
    {
        public CatAuditRequestController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc, IConfiguration config , IDatabase iDb) : base(logger, uow, dbc , config , iDb)
        {
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<CatAuditRequestSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.Status);
                
                Expression<Func<CatAuditRequest, bool>> filter = c => (string.IsNullOrEmpty(obj.Name) || c.Name.ToLower().Contains(obj.Name.ToLower())) && (status == -1 || (status == 1 ? c.Status == true : c.Status != true)) && c.IsDeleted != true;
                var list_depart = _uow.Repository<CatAuditRequest>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<CatAuditRequest> data = list_depart;
                var count = list_depart.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.IsDeleted != true).Select(a => new CatAuditRequestModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Code = a.Code,
                    Status = a.Status,
                    Description = a.Description,
                    IsActive = a.IsActive,
                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new CatAuditRequestModel(), total = 0 });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CatAuditRequestModel catauditrequestinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _allCatAuditRequest = _uow.Repository<CatAuditRequest>().Find(a => a.IsDeleted != true && a.Name.Equals(catauditrequestinfo.Name)).ToArray();
                
                if (_allCatAuditRequest.Length > 0)
                {
                    return Ok(new { code = "2", msg = "fail" });
                }
                var CatAuditRequest = new CatAuditRequest();
                CatAuditRequest.Name = catauditrequestinfo.Name;
                CatAuditRequest.Code = catauditrequestinfo.Code;
                CatAuditRequest.Description = catauditrequestinfo.Description;
                CatAuditRequest.Status = catauditrequestinfo.Status;
                CatAuditRequest.IsActive = true;
                CatAuditRequest.IsDeleted = false;
                CatAuditRequest.CreateDate = DateTime.Now;
                CatAuditRequest.CreatedAt = DateTime.Now;
                CatAuditRequest.CreatedBy = _userInfo.Id;

                if (CatAuditRequest.Name == "" || CatAuditRequest.Name == null) 
                { 
                    return Ok(new { code = "0", msg = "fail" }); 
                }
                _uow.Repository<CatAuditRequest>().Add(CatAuditRequest);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
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
                var CatAuditRequest = _uow.Repository<CatAuditRequest>().FirstOrDefault(a => a.Id == id);
                var checkCreatedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatAuditRequest.CreatedBy);
                var checkModifiedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatAuditRequest.ModifiedBy);
                if (CatAuditRequest != null)
                {
                    var catauditrequestinfo = new CatAuditRequestDetailModel()
                    {
                        Id = CatAuditRequest.Id,
                        Name = CatAuditRequest.Name,

                        Description = CatAuditRequest.Description,
                        Status = CatAuditRequest.Status,

                        CreatedAt = CatAuditRequest.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                        ModifiedAt = CatAuditRequest.ModifiedAt != null ? CatAuditRequest.ModifiedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                        DeletedAt = CatAuditRequest.DeletedAt,
                        CreatedBy = CatAuditRequest.CreatedBy,
                        ModifiedBy = CatAuditRequest.ModifiedBy,
                        DeletedBy = CatAuditRequest.DeletedBy,
                        CreatorName = checkCreatedBy.UserName,
                        EditorName = checkModifiedBy != null ? checkModifiedBy.UserName : "",

                    };
                    return Ok(new { code = "1", msg = "success", data = catauditrequestinfo });
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
        public IActionResult Edit([FromBody] CatAuditRequestModifyModel editcatauditrequest)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }

                var checkCatAuditRequest = _uow.Repository<CatAuditRequest>().FirstOrDefault(a => a.Id == editcatauditrequest.Id);
                if (checkCatAuditRequest == null) { return NotFound(); }
                if (editcatauditrequest.Name == "" || editcatauditrequest.Name == null) { return Ok(new { code = "0", msg = "fail" }); }
                var checkName = _uow.Repository<CatAuditRequest>().FirstOrDefault(a => a.Name == editcatauditrequest.Name);
                if (checkCatAuditRequest.Id != (checkName != null ? checkName.Id : null) && checkName != null) 
                { 
                    return Ok(new { code = "2", msg = "fail" });
                }

                checkCatAuditRequest.Name = editcatauditrequest.Name;
                checkCatAuditRequest.Code = editcatauditrequest.Code;
                checkCatAuditRequest.Status = editcatauditrequest.Status;
                checkCatAuditRequest.Description = editcatauditrequest.Description;
                checkCatAuditRequest.ModifiedAt = DateTime.Now;
                checkCatAuditRequest.ModifiedBy = _userInfo.Id;

                if (editcatauditrequest.Name == "" || editcatauditrequest.Name == null) { return Ok(new { code = "0", msg = "fail" }); }
                _uow.Repository<CatAuditRequest>().Update(checkCatAuditRequest);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var CatAuditRequest = _uow.Repository<CatAuditRequest>().FirstOrDefault(a => a.Id == id);
                if (CatAuditRequest == null)
                {
                    return NotFound();
                }

                CatAuditRequest.IsDeleted = true;
                CatAuditRequest.DeletedAt = DateTime.Now;
                CatAuditRequest.DeletedBy = userInfo.Id;
                _uow.Repository<CatAuditRequest>().Update(CatAuditRequest);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
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
                var CatAuditRequest = _uow.Repository<CatAuditRequest>().FirstOrDefault(a => a.Id == obj.id);
                if (CatAuditRequest == null)
                {
                    return NotFound();
                }
                if (obj.status == 1)
                {
                    CatAuditRequest.Status = true;
                }
                else
                {
                    CatAuditRequest.Status = false;
                }
                CatAuditRequest.ModifiedBy = userInfo.Id;
                _uow.Repository<CatAuditRequest>().Update(CatAuditRequest);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}