using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Models.ExecuteModels;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KitanoUserService.API.CronJob
{
    [DisallowConcurrentExecution]
    public class RemindChangePassJob : BaseCronJob, IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            try
            {
                var iCache = (IConnectionMultiplexer)_services.BuildServiceProvider().GetService(typeof(IConnectionMultiplexer));
                var _uow = (IUnitOfWork)_services.BuildServiceProvider().GetService(typeof(IUnitOfWork));
                var _iDb = iCache.GetDatabase();
                var _paramInfoPrefix = "Param.SystemInfo";
                var duration = 90;
                var mail_from = "";
                var host = "";
                var port = "";
                var mail_pass = "";
                var system_url = "";
                var value_get = _iDb.StringGet(_paramInfoPrefix);
                if (value_get.HasValue)
                {
                    var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
                    var reminder_time = list_param.FirstOrDefault(a => a.name == "REMINDER_TIME")?.value;
                    if (!string.IsNullOrEmpty(reminder_time))
                    {
                        duration = Convert.ToInt32(reminder_time);
                    }
                    mail_from = list_param.FirstOrDefault(a => a.name == "MAIL_FROM")?.value;
                    host = list_param.FirstOrDefault(a => a.name == "MAIL_SERVER")?.value;
                    port = list_param.FirstOrDefault(a => a.name == "MAIL_PORT")?.value;
                    mail_pass = list_param.FirstOrDefault(a => a.name == "MAIL_PASSWORD")?.value;
                    system_url = list_param.FirstOrDefault(a => a.name == "SYSTEM_URL")?.value;
                }
                var date = DateTime.Now.Date.AddDays(-duration);
                var list_user = _uow.Repository<Users>().Find(a => a.IsDeleted != true && a.DataSource != 1 && a.LastPassChange.HasValue && a.LastPassChange < date).ToArray();
                var subject = "Thay đổi thông tin tài khoản";
                var template_path = "RemindChangePassword.html";
                foreach (var user in list_user)
                {
                    Task.Delay(200);
                    var model = new UsersModifyModels()
                    {
                        FullName = user.FullName,
                        UserName = user.UserName,
                        Email = user.Email
                    };
                    SentMail_Reminder(subject, model, template_path, mail_from, host, port, mail_pass, system_url, duration);
                }
            }
            catch (Exception)
            {
            }
            return Task.CompletedTask;
        }
    }
}
