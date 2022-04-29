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
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using StackExchange.Redis;
using System.Net;

namespace KitanoUserService.API.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class DocumentController : BaseController
    {
        protected new readonly  IConfiguration _config;
        public  DocumentController(ILoggerManager logger, IUnitOfWork uow, AppDbContext dbc  , IConfiguration config , IDatabase iDb) : base(logger, uow, dbc , config , iDb)
        {
            _config = config;
        }

        [HttpGet("Search")]
        public IActionResult Search(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<DocumentSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.Status);

                var unitid = Convert.ToInt32(obj.unit_id);
                Expression<Func<Document, bool>> filter = c => (string.IsNullOrEmpty(obj.name) || c.name.ToLower().Contains(obj.name.ToLower())) && (status == -1 || (status == 1 ? c.status == true : c.status != true)) && (string.IsNullOrEmpty(obj.unit_id) || unitid == 0 || c.unit_id == unitid) && (string.IsNullOrEmpty(obj.code) || c.code == obj.code) && c.isdeleted != true;
                var list_depart = _uow.Repository<Document>().Find(filter).OrderByDescending(a => a.CreatedAt);
                var _unit = _uow.Repository<Department>().Find(a => a.IsActive == true).ToArray();
                IEnumerable<Document> data = list_depart;
                var count = list_depart.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.isdeleted != true).Select(a => new DocumentModel()
                {
                    Id = a.id,
                    name = a.name,
                    Code = a.code,
                    status = a.status,
                    description = a.description,
                    unit_id = a.unit_id,
                    publicdate = a.publicdate.HasValue ? a.publicdate.Value.ToString("MM/dd/yyyy") : "",
                    unitname = a.unit_id.HasValue ? _unit.FirstOrDefault(u => u.Id == a.unit_id)?.Name : "",
                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
            }
            catch (Exception)
            {
                return Ok(new { code = "0", msg = "fail", data = new DocumentModel(), total = 0 });
            }
        }

        [HttpGet("SearchEdit")]
        public IActionResult SearchEdit(string jsonData)
        {
            try
            {
                var obj = JsonSerializer.Deserialize<DocumentSearchModel>(jsonData);
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var status = Convert.ToInt32(obj.Status);

                var unitid = Convert.ToInt32(obj.unit_id);
                var controlId = Convert.ToInt32(obj.controlid);

                var approval_status = _uow.Repository<ControlDocument>().Find(a => a.controlid == controlId && a.isdeleted != true).ToArray();
                var _riskid = approval_status.Select(a => a.documentid).ToList();

                Expression<Func<Document, bool>> filter = c => (string.IsNullOrEmpty(obj.name) || c.name.ToLower().Contains(obj.name.ToLower())) && (status == -1 || (status == 1 ? c.status == true : c.status != true)) && (string.IsNullOrEmpty(obj.unit_id) || unitid == 0 || c.unit_id == unitid) && (string.IsNullOrEmpty(obj.code) || c.code == obj.code) && c.isdeleted != true && !_riskid.Contains(c.id);
                var list_depart = _uow.Repository<Document>().Find(filter).OrderByDescending(a => a.CreatedAt);
                var _unit = _uow.Repository<Department>().Find(a => a.IsActive == true).ToArray();
                IEnumerable<Document> data = list_depart;
                var count = list_depart.Count();
                if (count == 0)
                {
                    return Ok(new { code = "1", msg = "success", data = "", total = count });
                }
                if (obj.StartNumber >= 0 && obj.PageSize > 0)
                {
                    data = data.Skip(obj.StartNumber).Take(obj.PageSize);
                }
                var user = data.Where(a => a.isdeleted != true).Select(a => new DocumentModel()
                {
                    Id = a.id,
                    name = a.name,
                    Code = a.code,
                    status = a.status,
                    description = a.description,
                    unit_id = a.unit_id,
                    publicdate = a.publicdate.HasValue ? a.publicdate.Value.ToString("MM/dd/yyyy") : "",
                    unitname = a.unit_id.HasValue ? _unit.FirstOrDefault(u => u.Id == a.unit_id)?.Name : "",
                });
                return Ok(new { code = "1", msg = "success", data = user, total = count });
            }
            catch (Exception e)
            {
                return Ok(new { code = "0", msg = "fail", data = new DocumentModel(), total = 0 });
            }
        }

        [HttpPost]
        public IActionResult Create()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                var auditworkinfo = new CreateDocumentModel();

                var _allDocument = _uow.Repository<Document>().Find(a => a.isdeleted != true && a.name.Equals(auditworkinfo.name)).ToArray();

                if (_allDocument.Length > 0)
                {
                    return Ok(new { code = "-1", msg = "fail" });
                }
               
                var data = Request.Form["data"];
                var pathSave = "";
                var filetype = "";
                if (!string.IsNullOrEmpty(data))
                {
                    auditworkinfo = JsonSerializer.Deserialize<CreateDocumentModel>(data);                    
                }
                else
                {
                    return BadRequest();
                }

                var Document = new Document();
                Document.id = Guid.NewGuid();
                Document.name = auditworkinfo.name;
                Document.code = auditworkinfo.Code;
                Document.description = auditworkinfo.description;
                Document.status = auditworkinfo.status;
                Document.isdeleted = false;
                Document.unit_id = auditworkinfo.unit_id;
                Document.CreatedAt = DateTime.Now;
                Document.publicdate = DateTime.Parse(auditworkinfo.publicdate) ;

                var file = Request.Form.Files;
                var _listFile = new List<DocumentFile>();

                if (file.Count > 0)
                {
                    foreach (var item in file)
                    {
                        filetype = item.ContentType;                        
                        pathSave = CreateUploadURL(item, "Document/" + DateTime.Now.Year + DateTime.Now.Month);
                        var document_unit_provide_file = new DocumentFile()
                        {
                            Document = Document,
                            IsDelete = false,
                            FileType = filetype,
                            Path = pathSave,
                        };
                        _listFile.Add(document_unit_provide_file);                        
                    }

                    foreach (var item in _listFile)
                    {
                        _uow.Repository<DocumentFile>().AddWithoutSave(item);
                    }
                }

                if (Document.name == "" || Document.name == null)
                {
                    return Ok(new { code = "0", msg = "fail" });
                }
                _uow.Repository<Document>().Add(Document);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Details(Guid id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var Document = _uow.Repository<Document>().Include(a => a.DocumentFile).FirstOrDefault(a => a.id == id);


                if (Document != null)
                {
                    var Documentinfo = new DocumentModel()
                    {
                        Id = Document.id,
                        name = Document.name,
                        Code = Document.code,
                        description = Document.description,
                        unit_id = Document.unit_id,
                        status = Document.status,
                        publicdate = Document.publicdate.HasValue ? Document.publicdate.Value.ToString("yyyy-MM-dd") : null,
                        ListFile = Document.DocumentFile.Where(a => a.IsDelete != true).Select(x => new DocumentFileModel()
                        {
                            id = x.id,
                            path = x.Path,
                            file_type = x.FileType,
                        }).ToList(),
                    };
                    return Ok(new { code = "1", msg = "success", data = Documentinfo });
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        protected string CreateDownloadCode(string path, string name)
        {
            var fullPath = Path.Combine(_config["Upload:BaseTemplates"], path);

            if (!System.IO.File.Exists(fullPath))
                return "";            

            var code = Guid.NewGuid().ToString().Replace("-", "");
            _redis.StringSet(code, fullPath);
            _redis.StringSet(code + "_name", name);
            return code;
        }
        protected string GetPathDownload(string code, out string fileName)
        {
            fileName = _redis.StringGet(code + "_name");
            var fullPath = _redis.StringGet(code);

            if (string.IsNullOrEmpty(fileName))
                return "";

            if (!System.IO.File.Exists(fullPath))
                return "";

            return fullPath;
        }

        [AllowAnonymous]
        [HttpGet("DownloadAttach")]
        public IActionResult DonwloadFile(int id )
        {
            //var userInfo = HttpContext.Items["UserInfo"] as User;            
            var self = _uow.Repository<DocumentFile>().FirstOrDefault(o => o.id == id);
            var fullPath = Path.Combine(_config["Upload:BaseTemplates"], self.Path);

            string name  = self.Path.ToString().Replace("\\", "");
             name = name.ToString().Replace("UploadsDocument", "");
            var fs = new FileStream(fullPath, FileMode.Open);

            return File(fs, self.FileType, name);
        }

        [HttpPost("Update")]
        public IActionResult Edit()
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel _userInfo)
                {
                    return Unauthorized();
                }
                
                var auditworkinfo = new DocumentModifyModel();

                var data = Request.Form["data"];
                var pathSave = "";
                var filetype = "";
                if (!string.IsNullOrEmpty(data))
                {
                    auditworkinfo = JsonSerializer.Deserialize<DocumentModifyModel>(data);                    
                }
                else
                {
                    return BadRequest();
                }
                var checkDocument = _uow.Repository<Document>().FirstOrDefault(a => a.id == auditworkinfo.Id);
                if (checkDocument == null) { return NotFound(); }
                if (auditworkinfo.Name == "" || auditworkinfo.Name == null) 
                { 
                    return Ok(new { code = "0", msg = "fail" }); 
                }
                var checkName = _uow.Repository<Document>().FirstOrDefault(a => a.name == auditworkinfo.Name);
                if (checkDocument.id != (checkName != null ? checkName.id : null) && checkName != null) 
                { 
                    return Ok(new { code = "-1", msg = "fail" }); 
                }

                checkDocument.name = auditworkinfo.Name;
                checkDocument.code = auditworkinfo.Code;
                checkDocument.status = auditworkinfo.status;
                checkDocument.description = auditworkinfo.description;
                checkDocument.unit_id = auditworkinfo.unit_id;
                checkDocument.publicdate = auditworkinfo.public_date;
                if (auditworkinfo.Name == "" || auditworkinfo.Name == null) 
                { 
                    return Ok(new { code = "0", msg = "fail" }); 
                }

                var file = Request.Form.Files;
                var _listFile = new List<DocumentFile>();

                if (file.Count > 0)
                {
                    foreach (var item in file)
                    {
                        filetype = item.ContentType;
                        pathSave = CreateUploadURL(item, "Document/" + DateTime.Now.Year + DateTime.Now.Month);
                        var document_unit_provide_file = new DocumentFile()
                        {
                            Document = checkDocument,
                            IsDelete = false,
                            FileType = filetype,
                            Path = pathSave,
                        };
                        _listFile.Add(document_unit_provide_file);
                    }

                    foreach (var item in _listFile)
                    {
                        _uow.Repository<DocumentFile>().AddWithoutSave(item);
                    }
                }            

                _uow.Repository<Document>().Update(checkDocument);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception e)
            {
                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                if (HttpContext.Items["UserInfo"] is not CurrentUserModel userInfo)
                {
                    return Unauthorized();
                }
                var Document = _uow.Repository<Document>().FirstOrDefault(a => a.id == id);
                if (Document == null)
                {
                    return NotFound();
                }

                Document.isdeleted = true;
                
                _uow.Repository<Document>().Update(Document);
                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        protected  string CreateUploadURL(IFormFile imageFile, string folder = "")
        {
            var pathSave = "";
            var pathconfig = _config["Upload:BaseTemplates"];
            if (imageFile != null)
            {                
                if (string.IsNullOrEmpty(folder)) folder = "Public";
                var pathToSave = Path.Combine(pathconfig, "Uploads");
                var extension = Path.GetExtension(imageFile.FileName);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName)?.Trim();
                //var fileName = Path.GetFileName(imageFile.FileName)?.Trim() /*Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(imageFile.FileName)*/;
                var fileName = fileNameWithoutExtension + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                //var fileName = Path.GetFileName(imageFile.FileName)?.Trim() /*Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(imageFile.FileName)*/;
                var fullPathroot = Path.Combine(pathToSave, folder);
                if (!Directory.Exists(fullPathroot))
                {
                    Directory.CreateDirectory(fullPathroot);
                }
                pathSave = Path.Combine("Uploads", folder, fileName);
                var filePath = Path.Combine(fullPathroot, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }
            }
            return pathSave;
        }

        protected string CreateUploadFile(IFormFileCollection list_ImageFile, string folder = "")
        {
            var lstStr = new List<string>();
            foreach (var item in list_ImageFile)
            {
                string imgURL = CreateUploadURL(item, folder);
                if (!string.IsNullOrEmpty(imgURL)) lstStr.Add(imgURL);
            }
            return string.Join(",", lstStr);
        }

        [HttpGet("DeleteAttach/{id}")]
        public IActionResult DeleteFile(int id)
        {
            try
            {
                var self = _uow.Repository<DocumentFile>().FirstOrDefault(o => o.id == id);
                if (self == null)
                {
                    return NotFound();
                }

                var check = false;
                if (!string.IsNullOrEmpty(self.Path))
                {
                    var fullPath = Path.Combine(_config["Upload:BaseTemplates"], self.Path);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                        check = true;
                    }
                }
                if (check == true)
                {
                    self.IsDelete = true;
                    _uow.Repository<DocumentFile>().Update(self);
                }

                return Ok(new { code = "1", msg = "success" });
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}