using KitanoUserService.API.DataAccess;
using KitanoUserService.API.Models.ExecuteModels;
using KitanoUserService.API.Models.MigrationsModels;
using KitanoUserService.API.Repositories;
using Microsoft.AspNetCore.Http;
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
    public class RemindAuditRequestMonitor : BaseCronJob,IJob
    {
        public RemindAuditRequestMonitor()
        {
        }

        public Task Execute(IJobExecutionContext context)
        {

            try
            {
                var iCache = (IConnectionMultiplexer)_services.BuildServiceProvider().GetService(typeof(IConnectionMultiplexer));
                var _uow = (IUnitOfWork)_services.BuildServiceProvider().GetService(typeof(IUnitOfWork));
                var _iDb = iCache.GetDatabase();
                var _paramInfoPrefix = "Param.SystemInfo";
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
                var approval_status = _uow.Repository<ApprovalFunction>().Find(a => a.function_code == "M_AD" && a.StatusCode == "3.1").Select(a=>a.item_id).ToList();
                var audit_detect = _uow.Repository<AuditDetect>().Find(a => approval_status.Contains(a.id) && a.IsDeleted != true).ToArray();
                var now = DateTime.Now.Date;

                //var audit_request = _uow.Repository<AuditRequestMonitor>().Include(c => c.AuditDetect, c => c.FacilityRequestMonitorMapping,c => c.Users).AsEnumerable().Where(a => audit_detect.Any(x=>x.id == a.detectid) && a.is_deleted != true && a.ProcessStatus != 3).GroupBy(a=>a.userid);
                var audit_request = _uow.Repository<AuditRequestMonitor>().Include(c => c.AuditDetect, c => c.FacilityRequestMonitorMapping, c => c.Users).AsEnumerable().Where(a => audit_detect.Any(x => x.id == a.detectid) && a.is_deleted != true && a.Conclusion != 2).GroupBy(a => a.userid);

                var subject = "Nhắc thực hiện kiến nghị kiểm toán tháng " + DateTime.Now.Month + " năm " + DateTime.Now.Year;
                var template_path = "RemindAuditRequestMonitor.html";
                foreach (var item_audit in audit_request)
                {
                    Task.Delay(50);
                    var tbodytable = "";
                    foreach (var item in item_audit)
                    {
                        tbodytable += "<tr>";
                        tbodytable += "<td>" + item.AuditDetect?.description + "</td>";
                        tbodytable += "<td>" + item.Content + "</td>";
                        tbodytable += "<td>" + item.Users?.FullName + "</td>";
                        tbodytable += "<td>" + string.Join(", ", item.FacilityRequestMonitorMapping.Where(p => p.type == 2).Select(a => a.audit_facility_name).Distinct()) + "</td>";
                        tbodytable += "<td>" + (item.extend_at.HasValue ? item.extend_at.Value.ToString("dd/MM/yyyy") : (item.CompleteAt.HasValue ? item.CompleteAt.Value.ToString("dd/MM/yyyy") : "")) + "</td>";
                        tbodytable += "</tr>";
                    }
                  
                    var model = new AuditRequestMonitorDetailModel()
                    {
                        fullname = item_audit.FirstOrDefault()?.Users?.FullName,
                        username = item_audit.FirstOrDefault()?.Users?.UserName,
                        email = item_audit.FirstOrDefault()?.Users?.Email,
                        tbody = tbodytable,
                    };
                   
                    SentMail_Reminder_Audit(subject, model, template_path, mail_from, host, port, mail_pass, system_url);
                }
            }
            catch (Exception)
            {
            }
            return Task.CompletedTask;
        }
    }
}
