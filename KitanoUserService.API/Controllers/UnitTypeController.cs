using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Models.ExecuteModels;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Repositories;
using Microsoft.AspNetCore.Authorization;
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
    //[Authorize]
    public class UnitTypeController : BaseController
    {

        public UnitTypeController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc , IConfiguration config , IDatabase iDb) : base(logger, uow, dbc , config , iDb)
        {
        }
        [HttpPost]
        public IActionResult Create([FromBody] UnitTypeCreateModel unitTypeCreateModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkUnitType = _uow.Repository<UnitType>().Find(a => a.IsDeleted != true && a.Name.Equals(unitTypeCreateModel.Name)).ToArray();
                if (checkUnitType.Length > 0)
                {
                    return Ok(new { code = "416", msg = "fail" });
                }
                var unitType = new UnitType
                {
                    Name = unitTypeCreateModel.Name,
                    Status = unitTypeCreateModel.Status,
                    Description = unitTypeCreateModel.Description,
                    IsDeleted = false,
                    CreatedAt = DateTime.Now,
                    CreatedBy = _userInfo.Id,
                };
                if (unitType.Name == "" || unitType.Name == null) { return Ok(new { code = "426", msg = "fail" }); }
                _uow.Repository<UnitType>().Add(unitType);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
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
                var checkUnitType = _uow.Repository<UnitType>().FirstOrDefault(a => a.Id == id && a.IsDeleted.Equals(false));
                var checkCreatedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == checkUnitType.CreatedBy);
                var checkModifiedBy = _uow.Repository<Users>().FirstOrDefault(a => a.Id == checkUnitType.ModifiedBy);

                if (checkUnitType != null)
                {
                    var unitType = new UnitTypeDetailModel()
                    {
                        Id = checkUnitType.Id,
                        Name = checkUnitType.Name,
                        IsDeleted = checkUnitType.IsDeleted,
                        Status = checkUnitType.Status,
                        Description = checkUnitType.Description,
                        CreatedAt = checkUnitType.CreatedAt.HasValue ? checkUnitType.CreatedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                        ModifiedAt = checkUnitType.ModifiedAt != null ? checkUnitType.ModifiedAt.Value.ToString("dd/MM/yyyy HH:mm:ss") : null,
                        DeletedAt = checkUnitType.DeletedAt,
                        CreatedBy = checkUnitType.CreatedBy,
                        ModifiedBy = checkUnitType.ModifiedBy,
                        DeletedBy = checkUnitType.DeletedBy,
                        CreatorName = checkCreatedBy.UserName,
                        EditorName = checkModifiedBy != null ? checkModifiedBy.UserName : "",
                    };
                    return Ok(new { code = "1", msg = "success", data = unitType });
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
        public IActionResult Edit([FromBody] UnitTypeModifyModel unitTypeModifyModel)
        {
            try
            {

                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var checkUnitType = _uow.Repository<UnitType>().FirstOrDefault(a => a.Id == unitTypeModifyModel.Id && a.IsDeleted.Equals(false));
                if (checkUnitType == null) { return NotFound(); }
                if (unitTypeModifyModel.Name == "" || unitTypeModifyModel.Name == null) { return Ok(new { code = "426", msg = "fail" }); }
                var checkName = _uow.Repository<UnitType>().FirstOrDefault(a => a.Name == unitTypeModifyModel.Name && a.IsDeleted != true);
                if (checkUnitType.Id != (checkName != null ? checkName.Id : null) && checkName != null) { return Ok(new { code = "416", msg = "fail" }); }

                checkUnitType.Name = unitTypeModifyModel.Name;
                checkUnitType.Status = unitTypeModifyModel.Status;
                checkUnitType.Description = unitTypeModifyModel.Description;
                checkUnitType.ModifiedAt = DateTime.Now;
                checkUnitType.ModifiedBy = _userInfo.Id;
                _uow.Repository<UnitType>().Update(checkUnitType);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkUnitType = _uow.Repository<UnitType>().FirstOrDefault(a => a.Id == id);
                if (checkUnitType == null)
                {
                    return NotFound();
                }
                var checkDeleteUnitType = _uow.Repository<Department>().Find(z => z.ObjectClassId.Equals(id)).ToArray();
                if (checkDeleteUnitType.Length > 0)
                {
                    return Ok(new { code = "407", msg = "Tồn tại đơn vị liên quan đến loại đơn vị này, bạn không thể xoá" });
                }
                checkUnitType.DeletedAt = DateTime.Now;
                checkUnitType.IsDeleted = true;
                checkUnitType.DeletedBy = userInfo.Id;
                _uow.Repository<UnitType>().Update(checkUnitType);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
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
                var obj = JsonSerializer.Deserialize<UnitTypeSearchModel>(jsonData);
                var status = Convert.ToInt32(obj.Status);
                Expression<Func<UnitType, bool>> filter = c => (string.IsNullOrEmpty(obj.Name) || c.Name.ToLower().Contains(obj.Name.ToLower()))
                                                && (status == -1 || (status == 1 ? c.Status == true : c.Status != true)) && c.IsDeleted != true;
                var list_unitType = _uow.Repository<UnitType>().Find(filter).OrderByDescending(a => a.CreatedAt);
                IEnumerable<UnitType> data = list_unitType;
                var count = list_unitType.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }

                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var unitType = data.Select(a => new UnitTypeModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    IsDeleted = a.IsDeleted,
                    Status = a.Status,
                    Description = a.Description,
                    CreatedAt = a.CreatedAt,
                    ModifiedAt = a.ModifiedAt,
                    DeletedAt = a.DeletedAt,
                    CreatedBy = a.CreatedBy,
                    ModifiedBy = a.ModifiedBy,
                    DeletedBy = a.DeletedBy,
                });
                return Ok(new { code = "1", msg = "success", data = unitType, total = count, record_number = obj.StartNumber });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels(), total = 0 });
            }
        }
        [HttpGet("ListUnitType")]
        public IActionResult List()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var listUnitType = _uow.Repository<UnitType>().GetAll(a => a.IsDeleted != true).OrderByDescending(a => a.Id);
                var unitType = listUnitType.Select(a => new UnitTypeListModel()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Status = a.Status,
                    IsDeleted = a.IsDeleted,
                });
                return Ok(new { code = "1", msg = "success", data = unitType });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new UsersInfoModels(), total = 0 });
            }
        }
        [HttpPost("Active")]
        public IActionResult Active(UnitTypeActiveModel unitTypeActiveModel)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var checkUnitType = _uow.Repository<UnitType>().FirstOrDefault(a => a.Id == unitTypeActiveModel.id);
                if (checkUnitType == null)
                {
                    return NotFound();
                }
                if (unitTypeActiveModel.status == 1)
                {
                    checkUnitType.Status = true;
                }
                else
                {
                    checkUnitType.Status = false;
                }
                checkUnitType.ModifiedAt = DateTime.Now;
                _uow.Repository<UnitType>().Update(checkUnitType);
                return Ok(new { code = "1", msg = "success", data = checkUnitType });
            }
            catch (Exception ex)
            {
                _logger.LogError($"ACTIVE_USER - {unitTypeActiveModel.id} : {ex.Message}!");
                return BadRequest();
            }
        }
    }
}
