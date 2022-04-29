using KitanoUserService.API.Attributes;
using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace KitanoUserService.API.Controllers
{

    [BaseAuthorize]
    public class BaseController : ControllerBase
    {
        protected readonly ILoggerManager _logger;
        protected readonly IUnitOfWork _uow;
        protected readonly AppDbContext _dbc;
        //protected readonly IMapper _mapper;
        protected readonly IConfiguration _config;
        protected readonly IDatabase _redis;


        public BaseController(ILoggerManager logger,IUnitOfWork uow, AppDbContext dbc , IConfiguration config , IDatabase iDb) : base()
        {
            _uow = uow;
            _logger = logger;
            _dbc = dbc;
            _config = config;
            _redis = iDb;

        }

        protected string GenCodeDownload(string fileName)
        {
            var pathToSave = _config["Upload:BaseTemplates"];
            var fullPath = Path.Combine(pathToSave, fileName);

            if (!System.IO.File.Exists(fullPath))
                return "";

            var code = Guid.NewGuid().ToString().Replace("-", "");

            _redis.StringSet(code, fileName);
            return code;
        }

        protected string GetTempPath(string code, out string fileName)
        {
            fileName = _redis.StringGet(code);

            if (string.IsNullOrEmpty(fileName))
                return "";

            var pathToSave = _config["Upload:BaseTemplates"];
            var fullPath = Path.Combine(pathToSave, fileName);

            if (!System.IO.File.Exists(fullPath))
                return "";

            return fullPath;
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
        protected string CreateUploadURL(IFormFile imageFile, string folder = "")
        {
            var pathSave = "";
            var pathconfig = _config["Upload:ApprovalOutSideDocPath"];
            if (imageFile != null)
            {
                if (string.IsNullOrEmpty(folder)) folder = "Public";
                var pathToSave = Path.Combine(pathconfig, folder);
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(imageFile.FileName)?.Trim();
                var extension = Path.GetExtension(imageFile.FileName);
                var new_folder = DateTime.Now.ToString("yyyyMM");
                var fileName = fileNameWithoutExtension + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                var fullPathroot = Path.Combine(pathToSave, new_folder);
                if (!Directory.Exists(fullPathroot))
                {
                    Directory.CreateDirectory(fullPathroot);
                }
                pathSave = Path.Combine(folder, new_folder, fileName);
                var filePath = Path.Combine(fullPathroot, fileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    imageFile.CopyTo(fileStream);
                }
            }
            return pathSave;

        }
    }
}
