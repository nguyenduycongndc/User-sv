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
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text.Json;

namespace KitanoUserService.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ApprovalFunctionController : BaseController
    {
        public ApprovalFunctionController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc , IConfiguration config , IDatabase iDb) : base(logger, uow, dbc , config , iDb)
        {
        }
   
        [HttpPost("RequestApproval")]
        public IActionResult RequestApproval(RequestApprovalModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }

                var checkFunction = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.item_id == model.item_id && a.function_code == model.function_code);
                if (checkFunction == null)
                {
                    var function = new ApprovalFunction
                    {
                        item_id = model.item_id,
                        function_code = model.function_code,
                        function_name = model.function_name,
                        approver = model.approvaluser,
                        StatusCode = "1.1",
                    };
                    _uow.Repository<ApprovalFunction>().Add(function);
                }
                else
                {
                    checkFunction.approver = model.approvaluser;
                    checkFunction.StatusCode = "1.1";
                    _uow.Repository<ApprovalFunction>().Update(checkFunction);
                }
                if (model.function_code == "M_AP") //nếu là chức năng kế hoạch năm thì xử lý tiếp
                {
                    var checkAuditPlan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == model.item_id);

                    var result = new
                    {
                        item_id = model.item_id,
                        function_code = model.function_code,
                        function_name = model.function_name,
                        version = checkAuditPlan.Version,
                    };
                    return Ok(new { code = "1", data = result, msg = "success" });
                }
                else
                {
                    var result = new
                    {
                        function_code = model.function_code,
                        function_name = model.function_name,
                    };
                    return Ok(new { code = "1", data = result, msg = "success" });
                }

            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("SubmitApproval")]
        public IActionResult SubmitApproval(ApprovalModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var level_approval = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == model.function_code);
                if (level_approval == null)
                {
                    return NotFound();
                }
                var level = level_approval.ApprovalLevel;
               
                var checkFunction = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.item_id == model.id && a.function_code == model.function_code);
                if (checkFunction == null)
                {
                    return NotFound();
                }
                var StatusCode = checkFunction.StatusCode;
                if (StatusCode == "1.1")
                {
                    if (level == 2 )
                    {
                        checkFunction.StatusCode = "2.1";
                        if (model.approvaluser.HasValue)
                        {
                            checkFunction.approver_last = model.approvaluser;
                        }
                    }
                    else
                    {
                        checkFunction.StatusCode = "3.1";
                    }
                }
                else if (StatusCode == "2.1")
                {
                    checkFunction.StatusCode = "3.1";
                }
                checkFunction.ApprovalDate = DateTime.Now;
                _uow.Repository<ApprovalFunction>().Update(checkFunction);
                if (model.function_code == "M_AP") //nếu là chức năng kế hoạch năm thì xử lý tiếp
                {
					if(checkFunction.StatusCode == "3.1"){
						var list_update = new List<ApprovalFunction>();
						var list_all = _uow.Repository<AuditPlan>().Find(a => a.Year == model.Year && a.IsDelete != true).ToArray();
						var list_old = list_all.Where(a => a.Id != model.id).Select(x => x.Id).ToList();
						if (list_old.Count > 0)
						{
							var checkFunction_old = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => list_old.Contains(a.item_id.Value) && a.function_code == model.function_code && a.StatusCode == "3.1");
							if (checkFunction_old != null)
							{
								checkFunction_old.StatusCode = "0.0"; //Hết hiệu lực
								list_update.Add(checkFunction_old);
							}
						}
						var checkAuditPlan = list_all.FirstOrDefault(a => a.Id == model.id);
						var check_count = list_all.Count();
						if (check_count == 1)
						{
							checkAuditPlan.Version = 1;
						}
						else
						{
							var ver = list_all.Max(a => a.Version);
							checkAuditPlan.Version = !ver.HasValue ? 1 : ver + 1;
						}
						foreach (var item in list_update)
						{
							_uow.Repository<ApprovalFunction>().UpdateWithoutSave(item);
						}
						_uow.Repository<AuditPlan>().UpdateWithoutSave(checkAuditPlan);
						_uow.SaveChanges();
					}
                    
                    var result = new
                    {
                        function_code = model.function_code,
                        function_name = model.function_name,
                    };
                    return Ok(new { code = "1", data = result, msg = "success" });
                }
                else
                {
                    var result = new
                    {
                        function_code = model.function_code,
                        function_name = model.function_name,
                    };
                    return Ok(new { code = "1", data = result, msg = "success" });
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("RejectApproval")]
        public IActionResult RejectApproval(ApprovalModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var level_approval = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == model.function_code);
                if (level_approval == null)
                {
                    return NotFound();
                }
                var level = level_approval.ApprovalLevel;
                var checkFunction = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.item_id == model.id && a.function_code == model.function_code);
                if (checkFunction == null)
                {
                    return NotFound();
                }
                var StatusCode = checkFunction.StatusCode;
                if (StatusCode == "1.1")
                {
                    if (level == 2)
                    {
                        checkFunction.StatusCode = "2.2";
                    }
                    else
                    {
                        checkFunction.StatusCode = "3.2";
                    }
                }
                else if (StatusCode == "2.1")
                {
                    checkFunction.StatusCode = "3.2";
                }
                checkFunction.ApprovalDate = DateTime.Now;
                checkFunction.Reason = model.reason_note;
                _uow.Repository<ApprovalFunction>().Update(checkFunction);
                if (model.function_code == "M_AP") //nếu là chức năng kế hoạch năm thì xử lý tiếp
                {
                    var checkAuditPlan = _uow.Repository<AuditPlan>().FirstOrDefault(a => a.Id == model.id);

                    var result = new
                    {
                        item_id = model.id,
                        function_code = model.function_code,
                        function_name = model.function_name,
                        version = checkAuditPlan.Version,
                    };
                    return Ok(new { code = "1", data = result, msg = "success" });
                }
                else
                {
                    var result = new
                    {
                        function_code = model.function_code,
                        function_name = model.function_name,
                    };
                    return Ok(new { code = "1", data = result, msg = "success" });
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("UpdateStatus")]
        public IActionResult updateStatus()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var model = new UpdateStatusModel();
                var data = Request.Form["data"];

                if (!string.IsNullOrEmpty(data))
                {
                    model = JsonSerializer.Deserialize<UpdateStatusModel>(data);
                }
                else
                {
                    return BadRequest();
                }
                var level_approval = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == model.function_code);
                if (level_approval == null)
                {
                    return NotFound();
                }
                var checkFunction = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.item_id == model.id && a.function_code == model.function_code);
                if (checkFunction == null)
                {
                    return NotFound();
                }
                var level = level_approval.ApprovalLevel;
                // người cập nhật trạng thái là người duyệt cuối
                if (level == 1)
                {
                    checkFunction.approver = _userInfo.Id;
                }
                else if (level > 1)
                {
                    checkFunction.approver_last = _userInfo.Id; 
                }
                checkFunction.StatusCode = model.StatusCode;
                checkFunction.ApprovalDate = model.Browsedate;
                checkFunction.Reason = model.reason_note;
                _uow.Repository<ApprovalFunction>().Update(checkFunction);
                var file = Request.Form.Files;
                var _listFile = new List<ApprovalFunctionFile>();
                foreach (var item in file)
                {
                    var file_type = item.ContentType;
                    var folder = model.function_code + "_" + "ApprvalOutside";
                    var pathSave = CreateUploadURL(item, folder);
                    var function_file = new ApprovalFunctionFile()
                    {
                        item_id = model.id,
                        function_code = model.function_code,
                        function_name = model.function_name,
                        CreatedAt = DateTime.Now,
                        IsDeleted = false,
                        FileType = file_type,
                        Path = pathSave,
                        
                    };
                    _listFile.Add(function_file);
                }
                foreach (var item in _listFile)
                {
                    _uow.Repository<ApprovalFunctionFile>().AddWithoutSave(item);
                }
                _uow.SaveChanges();

                if (model.function_code == "M_AP" && model.StatusCode == "3.1") //nếu là chức năng kế hoạch năm thì xử lý tiếp
                {
                    var list_update = new List<ApprovalFunction>();
                    var list_all = _uow.Repository<AuditPlan>().Find(a => a.Year == model.Year && a.IsDelete != true).ToArray();
                    var list_old = list_all.Where(a => a.Id != model.id).Select(x => x.Id).ToList();
                    if (list_old.Count > 0)
                    {
                        var checkFunction_old = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => list_old.Contains(a.item_id.Value) && a.function_code == model.function_code && a.StatusCode == "3.1");
                        if (checkFunction_old != null)
                        {
                            checkFunction_old.StatusCode = "0.0"; //Hết hiệu lực
                            list_update.Add(checkFunction_old);
                        }
                    }
                    var checkAuditPlan = list_all.FirstOrDefault(a => a.Id == model.id);
                    var check_count = list_all.Count();
                    if (check_count == 1)
                    {
                        checkAuditPlan.Version = 1;
                    }
                    else
                    {
                        var ver = list_all.Max(a => a.Version);
                        checkAuditPlan.Version = !ver.HasValue ? 1 : ver + 1;
                    }
                    foreach (var item in list_update)
                    {
                        _uow.Repository<ApprovalFunction>().UpdateWithoutSave(item);
                    }
                    _uow.Repository<AuditPlan>().UpdateWithoutSave(checkAuditPlan);
                    _uow.SaveChanges();
                    var result = new
                    {
                        item_id = model.id,
                        function_code = model.function_code,
                        function_name = model.function_name,
                        version = checkAuditPlan.Version+"",
                    };
                    return Ok(new { code = "1", data = result, msg = "success" });
                }
                else
                {
                    var result = new
                    {
                        item_id = model.id,
                        function_code = model.function_code,
                        function_name = model.function_name,
                        version = "",
                    };
                    return Ok(new { code = "1", data = result, msg = "success" });
                }
                //var result = new
                //{
                //    function_code = model.function_code,
                //    function_name = model.function_name,
                //};
                //return Ok(new { code = "1", data = result, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
        [HttpPost("DownloadAttachApproval/{id}")]
        public IActionResult DonwloadFile(int id)
        {
            try
            {
                var self = _uow.Repository<ApprovalFunctionFile>().FirstOrDefault(o => o.id == id);
                if (self == null)
                {
                    return NotFound();
                }
                var fullPath = Path.Combine(_config["Upload:ApprovalOutSideDocPath"], self.Path);
                var name = "DownLoadFile";
                if (!string.IsNullOrEmpty(self.Path))
                {
                    var _array = self.Path.Replace("/", "\\").Split("\\");
                    name = _array[_array.Length - 1];
                }
                var fs = new FileStream(fullPath, FileMode.Open);

                return File(fs, self.FileType, name);
            }
            catch (Exception)
            {

                return NotFound();
            }
        }
        [HttpGet("DeleteAttachApproval/{id}")]
        public IActionResult DeleteFile(int id)
        {
            try
            {
                var self = _uow.Repository<ApprovalFunctionFile>().FirstOrDefault(o => o.id == id);
                if (self == null)
                {
                    return NotFound();
                }
                var check = false;
                if (!string.IsNullOrEmpty(self.Path))
                {
                    var fullPath = Path.Combine(_config["Upload:ApprovalOutSideDocPath"], self.Path);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                        check = true;
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                if (check == true)
                {
                    self.IsDeleted = true;
                    _uow.Repository<ApprovalFunctionFile>().Update(self);
                }

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPost("CancelApproval")]
        public IActionResult CancelApproval(ApprovalModel model)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                //var level_approval = _uow.Repository<ApprovalConfig>().FirstOrDefault(a => a.item_code == model.function_code);
                //if (level_approval == null)
                //{
                //    return NotFound();
                //}
                var checkFunction = _uow.Repository<ApprovalFunction>().FirstOrDefault(a => a.item_id == model.id && a.function_code == model.function_code);
                if (checkFunction == null)
                {
                    return NotFound();
                }
                checkFunction.approver = null;
                checkFunction.approver_last = null;
                var status = checkFunction.StatusCode;
                checkFunction.StatusCode = "4.1";
                //checkFunction.ApprovalDate = DateTime.Now;
                checkFunction.Reason = model.reason_note;
                _uow.Repository<ApprovalFunction>().Update(checkFunction);
                var result = new
                {
                    function_code = model.function_code,
                    function_name = model.function_name,
                };
                return Ok(new { code = "1", data = result, msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
