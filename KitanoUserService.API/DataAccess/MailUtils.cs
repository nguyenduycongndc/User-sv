using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using KitanoUserService.API.Models.ExecuteModels;
using Microsoft.AspNetCore.Hosting;
using StackExchange.Redis;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Net;

namespace KitanoUserService.API.DataAccess
{
    public static class MailUtils
    {
        private static IDatabase _iDb;
        private static IWebHostEnvironment _webHostEnvironment;
        private static IHttpContextAccessor context;
        public static void SetRedisDB(IDatabase iDb)
        {
            _iDb = iDb;
        }
        public static void SetEnvironment(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor accessor)
        {
            _webHostEnvironment = webHostEnvironment;
            context = accessor;
        }      
        public static bool SentMail(string subject, UsersModifyModels userInfo,string tempplate_path)
        {
            var iLogger = (ILoggerManager)context.HttpContext.RequestServices.GetService(typeof(ILoggerManager));
            var _config = (IConfiguration)context.HttpContext.RequestServices
                   .GetService(typeof(IConfiguration));
            var _paramInfoPrefix = "Param.SystemInfo";
            try
            {
                var pathTemplate = _config["Template:MailTemplate"];
                var mail_from = "";
                var host = "";
                var port = "";
                var mail_pass = "";
                var system_url = "";
                var value_get = _iDb.StringGet(_paramInfoPrefix);
                if (value_get.HasValue)
                {
                    var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
                    mail_from = list_param.FirstOrDefault(a => a.name == "MAIL_FROM")?.value;
                    host = list_param.FirstOrDefault(a => a.name == "MAIL_SERVER")?.value;
                    port = list_param.FirstOrDefault(a => a.name == "MAIL_PORT")?.value;
                    mail_pass = list_param.FirstOrDefault(a => a.name == "MAIL_PASSWORD")?.value;
                    system_url = list_param.FirstOrDefault(a => a.name == "SYSTEM_URL")?.value;
                }
                var message = new MimeMessage();
                var from = new MailboxAddress("Kitano System", mail_from);
                var to = new MailboxAddress(userInfo.FullName, userInfo.Email);
                message.From.Add(from);
                message.To.Add(to);
                message.Subject = subject;
                var bodyBuilder = new BodyBuilder();
                object[] infoModel = new object[] {userInfo.FullName, userInfo.UserName, userInfo.Password, system_url};
                var  contentRootPath  = Path.Combine(pathTemplate, tempplate_path);
                bodyBuilder.HtmlBody = GetEmailBody(contentRootPath, infoModel);
                message.Body = bodyBuilder.ToMessageBody();
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;// SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                var client = new SmtpClient();
                client.Connect(host, string.IsNullOrEmpty(port) ? 587 : int.Parse(port), true);
                client.Authenticate(mail_from, mail_pass);
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
                iLogger.LogError($"CHANGE_PASS_USER - {userInfo.UserName} : successfully!");
                return true;
                
            }
            catch (Exception ex)
            {
                iLogger.LogError($"CHANGE_PASS_USER - {userInfo.UserName} : {ex.Message}!");
                return false;
            }
        }
        public static string GetEmailBody(string templatePath, dynamic model)
        {
            try
            {
                var template = File.ReadAllText(templatePath);
                var body = string.Format(template, model);
                return body;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
