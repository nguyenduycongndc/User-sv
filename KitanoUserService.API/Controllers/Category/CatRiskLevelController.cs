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
    public class CatRiskLevelController : BaseController
    {
        public CatRiskLevelController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc , IConfiguration config , IDatabase iDb) : base(logger, uow, dbc , config , iDb)
        {
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<CatRiskLevelSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.Status);

                Expression<Func<CatRiskLevel, bool>> filter = c => (string.IsNullOrEmpty(obj.Name) || c.Name.ToLower().Contains(obj.Name.ToLower())) && (status == -1 || (status == 1 ? c.Status == true : c.Status != true)) && c.IsDeleted != true;
                var list_depart = _uow.Repository<CatRiskLevel>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<CatRiskLevel> data = list_depart;
                var count = list_depart.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.IsDeleted != true).Select(a => new CatRiskLevelModel()
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
                return Ok(new { code = "0", msg = "fail", data = new CatRiskLevelModel(), total = 0 });
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] CatRiskLevelModel catrisklevelinfo)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var _allCatRiskLevel = _uow.Repository<CatRiskLevel>().Find(a => a.IsDeleted != true && a.Name.Equals(catrisklevelinfo.Name)).ToArray();

                if (_allCatRiskLevel.Length > 0)
                {
                    return Ok(new { code = "-1", msg = "fail" });
                }
                var CatRiskLevel = new CatRiskLevel();
                CatRiskLevel.Name = catrisklevelinfo.Name;
                CatRiskLevel.Code = catrisklevelinfo.Code;
                CatRiskLevel.Description = catrisklevelinfo.Description;
                CatRiskLevel.Status = catrisklevelinfo.Status;
                CatRiskLevel.IsActive = true;
                CatRiskLevel.IsDeleted = false;
                CatRiskLevel.CreateDate = DateTime.Now;
                CatRiskLevel.CreatedAt = DateTime.Now;
                CatRiskLevel.CreatedBy = _userInfo.Id;

                if (CatRiskLevel.Name == "" || CatRiskLevel.Name == null)
                {
                    return Ok(new { code = "0", msg = "fail" });
                }
                _uow.Repository<CatRiskLevel>().Add(CatRiskLevel);
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
                var CatRiskLevel = _uow.Repository<CatRiskLevel>().FirstOrDefault(a => a.Id == id);
                var checkCreatedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatRiskLevel.CreatedBy);
                var checkModifiedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == CatRiskLevel.ModifiedBy);
                if (CatRiskLevel != null)
                {
                    var catrisklevelinfo = new CatRiskLevelDetailModel()
                    {
                        Id = CatRiskLevel.Id,
                        Name = CatRiskLevel.Name,

                        Description = CatRiskLevel.Description,
                        Status = CatRiskLevel.Status,

                        CreatedAt = CatRiskLevel.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm:ss"),
                        ModifiedAt = CatRiskLevel.ModifiedAt != null ? CatRiskLevel.ModifiedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                        DeletedAt = CatRiskLevel.DeletedAt,
                        CreatedBy = CatRiskLevel.CreatedBy,
                        ModifiedBy = CatRiskLevel.ModifiedBy,
                        DeletedBy = CatRiskLevel.DeletedBy,
                        CreatorName = checkCreatedBy.UserName,
                        EditorName = checkModifiedBy != null ? checkModifiedBy.UserName : "",

                    };
                    return Ok(new { code = "1", msg = "success", data = catrisklevelinfo });
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
        public IActionResult Edit([FromBody] CatRiskLevelModifyModel editcatrisklevel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }

                var checkCatRiskLevel = _uow.Repository<CatRiskLevel>().FirstOrDefault(a => a.Id == editcatrisklevel.Id);
                if (checkCatRiskLevel == null) { return NotFound(); }
                if (editcatrisklevel.Name == "" || editcatrisklevel.Name == null) { return Ok(new { code = "0", msg = "fail" }); }
                var checkName = _uow.Repository<CatRiskLevel>().FirstOrDefault(a => a.Name == editcatrisklevel.Name);
                if (checkCatRiskLevel.Id != (checkName != null ? checkName.Id : null) && checkName != null) { return Ok(new { code = "-1", msg = "fail" }); }

                checkCatRiskLevel.Name = editcatrisklevel.Name;
                checkCatRiskLevel.Code = editcatrisklevel.Code;
                checkCatRiskLevel.Status = editcatrisklevel.Status;
                checkCatRiskLevel.Description = editcatrisklevel.Description;
                checkCatRiskLevel.ModifiedAt = DateTime.Now;
                checkCatRiskLevel.ModifiedBy = _userInfo.Id;

                if (editcatrisklevel.Name == "" || editcatrisklevel.Name == null) { return Ok(new { code = "0", msg = "fail" }); }
                _uow.Repository<CatRiskLevel>().Update(checkCatRiskLevel);
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
                var CatRiskLevel = _uow.Repository<CatRiskLevel>().FirstOrDefault(a => a.Id == id);
                if (CatRiskLevel == null)
                {
                    return NotFound();
                }
                var _AuditStrategyRisk = _uow.Repository<AuditStrategyRisk>().FirstOrDefault(a => a.risk_level == id);
                var _AuditCycle = _uow.Repository<AuditCycle>().FirstOrDefault(a => a.RatingPoint == id);
                var _RatingScale = _uow.Repository<RatingScale>().FirstOrDefault(a => a.RiskLevel == id);

                if (_AuditStrategyRisk == null && _RatingScale == null && _AuditCycle == null)
                {
                    CatRiskLevel.IsDeleted = true;
                    CatRiskLevel.DeletedAt = DateTime.Now;
                    CatRiskLevel.DeletedBy = userInfo.Id;
                    _uow.Repository<CatRiskLevel>().Update(CatRiskLevel);
                    return Ok(new { code = "1", msg = "success" });
                }
                else
                {
                    return Ok(new { code = "2", msg = "error" });
                } 
            }
            catch (Exception e)
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
                var CatRiskLevel = _uow.Repository<CatRiskLevel>().FirstOrDefault(a => a.Id == obj.id);
                if (CatRiskLevel == null)
                {
                    return NotFound();
                }
                if (obj.status == 1)
                {
                    CatRiskLevel.Status = true;
                }
                else
                {
                    CatRiskLevel.Status = false;
                }
                CatRiskLevel.ModifiedBy = userInfo.Id;
                _uow.Repository<CatRiskLevel>().Update(CatRiskLevel);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}