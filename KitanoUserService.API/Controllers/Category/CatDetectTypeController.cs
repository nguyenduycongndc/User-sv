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
    public class CatDetectTypeController : BaseController
    {
        public CatDetectTypeController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc ,  IConfiguration config , IDatabase iDb) : base(logger, uow, dbc , config , iDb)
        {
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<CatDetectTypeSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.Status);

                Expression<Func<CatDetectType, bool>> filter = c => (string.IsNullOrEmpty(obj.Name) || c.Name.ToLower().Contains(obj.Name.ToLower())) && (status == -1 || (status == 1 ? c.Status == true : c.Status != true)) && c.IsDeleted != true;
                var list_depart = _uow.Repository<CatDetectType>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<CatDetectType> data = list_depart;
                var count = list_depart.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.IsDeleted != true).Select(a => new CatDetectTypeModel()
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
                return Ok(new { code = "0", msg = "fail", data = new CatDetectTypeModel(), total = 0 });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CatDetectTypeModel catdetecttypeinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _allCatDetectType = _uow.Repository<CatDetectType>().Find(a => a.IsDeleted != true && a.Name.Equals(catdetecttypeinfo.Name)).ToArray();

                if (_allCatDetectType.Length > 0)
                {
                    return Ok(new { code = "-1", msg = "fail" });
                }
                var CatDetectType = new CatDetectType();
                CatDetectType.Name = catdetecttypeinfo.Name;
                CatDetectType.Code = catdetecttypeinfo.Code;
                CatDetectType.Description = catdetecttypeinfo.Description;
                CatDetectType.Status = catdetecttypeinfo.Status;
                CatDetectType.IsActive = true;
                CatDetectType.IsDeleted = false;
                CatDetectType.CreateDate = DateTime.Now;
                CatDetectType.CreatedAt = DateTime.Now;
                CatDetectType.CreatedBy = _userInfo.Id;

                if (CatDetectType.Name == "" || CatDetectType.Name == null)
                {
                    return Ok(new { code = "0", msg = "fail" });
                }
                _uow.Repository<CatDetectType>().Add(CatDetectType);
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
                var CatDetectType = _uow.Repository<CatDetectType>().FirstOrDefault(a => a.Id == id);
                var checkCreatedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatDetectType.CreatedBy);
                var checkModifiedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatDetectType.ModifiedBy);
                if (CatDetectType != null)
                {
                    var catdetecttypeinfo = new CatDetectTypeDetailModel()
                    {
                        Id = CatDetectType.Id,
                        Name = CatDetectType.Name,

                        Description = CatDetectType.Description,
                        Status = CatDetectType.Status,

                        CreatedAt = CatDetectType.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                        ModifiedAt = CatDetectType.ModifiedAt != null ? CatDetectType.ModifiedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                        DeletedAt = CatDetectType.DeletedAt,
                        CreatedBy = CatDetectType.CreatedBy,
                        ModifiedBy = CatDetectType.ModifiedBy,
                        DeletedBy = CatDetectType.DeletedBy,
                        CreatorName = checkCreatedBy.UserName,
                        EditorName = checkModifiedBy != null ? checkModifiedBy.UserName : "",

                    };
                    return Ok(new { code = "1", msg = "success", data = catdetecttypeinfo });
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
        public IActionResult Edit([FromBody] CatDetectTypeModifyModel editcatdetecttype)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }

                var checkCatDetectType = _uow.Repository<CatDetectType>().FirstOrDefault(a => a.Id == editcatdetecttype.Id);
                if (checkCatDetectType == null) { return NotFound(); }
                if (editcatdetecttype.Name == "" || editcatdetecttype.Name == null) { return Ok(new { code = "0", msg = "fail" }); }
                var checkName = _uow.Repository<CatDetectType>().FirstOrDefault(a => a.Name == editcatdetecttype.Name);
                if (checkCatDetectType.Id != (checkName != null ? checkName.Id : null) && checkName != null) { return Ok(new { code = "-1", msg = "fail" }); }

                checkCatDetectType.Name = editcatdetecttype.Name;
                checkCatDetectType.Code = editcatdetecttype.Code;
                checkCatDetectType.Status = editcatdetecttype.Status;
                checkCatDetectType.Description = editcatdetecttype.Description;
                checkCatDetectType.ModifiedAt = DateTime.Now;
                checkCatDetectType.ModifiedBy = _userInfo.Id;

                if (editcatdetecttype.Name == "" || editcatdetecttype.Name == null) { return Ok(new { code = "0", msg = "fail" }); }
                _uow.Repository<CatDetectType>().Update(checkCatDetectType);
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
                var CatDetectType = _uow.Repository<CatDetectType>().FirstOrDefault(a => a.Id == id);
                if (CatDetectType == null)
                {
                    return NotFound();
                }

                CatDetectType.IsDeleted = true;
                CatDetectType.DeletedAt = DateTime.Now;
                CatDetectType.DeletedBy = userInfo.Id;
                _uow.Repository<CatDetectType>().Update(CatDetectType);
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
                var CatDetectType = _uow.Repository<CatDetectType>().FirstOrDefault(a => a.Id == obj.id);
                if (CatDetectType == null)
                {
                    return NotFound();
                }
                if (obj.status == 1)
                {
                    CatDetectType.Status = true;
                }
                else
                {
                    CatDetectType.Status = false;
                }
                CatDetectType.ModifiedBy = userInfo.Id;
                _uow.Repository<CatDetectType>().Update(CatDetectType);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}