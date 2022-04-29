using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Models.ExecuteModels;
using KitanoUserService.API.Repositories;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MimeKit;
using Quartz;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace KitanoUserService.API.CronJob
{
    public class BaseCronJob
    {
        protected static IServiceCollection _services;
        public static void SetService(IServiceCollection services)
        {
            _services = services;
        }

        public static bool SentMail_Reminder(string subject, UsersModifyModels userInfo, string tempplate_path, string mail_from, string host, string port, string mail_pass, string system_url, int? duration)
        {
            var iLogger = (ILoggerManager)_services.BuildServiceProvider().GetService(typeof(ILoggerManager));
            var _config = (IConfiguration)_services.BuildServiceProvider().GetService(typeof(IConfiguration));
            try
            {
                var pathTemplate = _config["Template:MailTemplate"];

                var message = new MimeMessage();
                var from = new MailboxAddress("Kitano System", mail_from);
                var to = new MailboxAddress(userInfo.FullName, userInfo.Email);
                message.From.Add(from);
                message.Subject = subject;
                message.To.Add(to);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;// SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                var client = new SmtpClient();
                client.Connect(host, string.IsNullOrEmpty(port) ? 587 : int.Parse(port), true);
                client.Authenticate(mail_from, mail_pass);
                var bodyBuilder = new BodyBuilder();
                object[] infoModel = new object[] { userInfo.FullName, duration, system_url };
                var contentRootPath = Path.Combine(pathTemplate, tempplate_path);
                bodyBuilder.HtmlBody = GetEmailBody(contentRootPath, infoModel);
                message.Body = bodyBuilder.ToMessageBody();
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
                iLogger.LogError($"REMIND_CHANGE_PASS_USER - {userInfo.UserName} : successfully!");
                return true;
            }
            catch (Exception ex)
            {
                iLogger.LogError($"REMIND_CHANGE_PASS_USER - {userInfo.UserName} : {ex.Message}!");
                return false;
            }
        }

        public static bool SentMail_Reminder_Audit(string subject, AuditRequestMonitorDetailModel model, string tempplate_path, string mail_from, string host, string port, string mail_pass, string system_url)
        {
            var iLogger = (ILoggerManager)_services.BuildServiceProvider().GetService(typeof(ILoggerManager));
            var _config = (IConfiguration)_services.BuildServiceProvider().GetService(typeof(IConfiguration));
            var Fullname = model.fullname;
            var UserName = model.username;
            var Email = model.email;
            try
            {
               
                var pathTemplate = _config["Template:MailTemplate"];

                var message = new MimeMessage();
                var from = new MailboxAddress("Kitano System", mail_from);
                var to = new MailboxAddress(Fullname, Email);
                message.From.Add(from);
                message.Subject = subject;
                message.To.Add(to);
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;// SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12;
                var client = new SmtpClient();
                client.Connect(host, string.IsNullOrEmpty(port) ? 587 : int.Parse(port), true);
                client.Authenticate(mail_from, mail_pass);
                var bodyBuilder = new BodyBuilder();
                var contentRootPath = Path.Combine(pathTemplate, tempplate_path);
                string contentBody = GetEmailBodyCustom(contentRootPath);
                bodyBuilder.HtmlBody =string.IsNullOrEmpty(contentBody) ? "" : contentBody.Replace("{Fullname}", Fullname).Replace("{tbody}", model.tbody);
                message.Body = bodyBuilder.ToMessageBody();
                client.Send(message);
                client.Disconnect(true);
                client.Dispose();
                iLogger.LogError($"REMIND_AUDIT_REQUEST_MONITOR - {UserName} : successfully!");
                return true;
            }
            catch (Exception ex)
            {
                iLogger.LogError($"REMIND_AUDIT_REQUEST_MONITOR - {UserName} : {ex.Message}!");
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
        public static string GetEmailBodyCustom(string templatePath)
        {
            try
            {
                var template = File.ReadAllText(templatePath);              
                return template;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
