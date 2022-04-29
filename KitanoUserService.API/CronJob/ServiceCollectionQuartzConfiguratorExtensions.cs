using KitanoUserService.API.Models.ExecuteModels;
using Microsoft.Extensions.Configuration;
using Quartz;
using Quartz.Impl.Calendar;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KitanoUserService.API.CronJob
{
    public static class ServiceCollectionQuartzConfiguratorExtensions
    {
        private static IDatabase _iDb;
        public static void SetRedisDB(IDatabase iDb)
        {
            _iDb = iDb;
        }
        public static void AddJobAndTrigger<T>(this IServiceCollectionQuartzConfigurator quartz,IConfiguration config) where T : IJob
        {
            string jobName = typeof(T).Name;
            var configKey = $"Quartz:{jobName}";          
          
            var cronOn = config[configKey + ":On"];
            var cronTimeType = config[configKey + ":TimeType"];
            if (cronOn == "1" && !string.IsNullOrEmpty(cronTimeType))
            { 
                var cronTimeValue = config[configKey + ":Duration"];
                var _value = string.IsNullOrEmpty(cronTimeValue) ? 60 : Convert.ToInt32(cronTimeValue);
                var jobKey = new JobKey(jobName);
                quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));

                switch (cronTimeType)
                {
                    case "h":
                        quartz.AddTrigger(opts => opts
                          .ForJob(jobKey)
                          .WithIdentity(jobName + "-trigger")
                          .WithSimpleSchedule(x => x
                              .WithIntervalInHours(_value)
                              .RepeatForever()
                          ));
                        break;
                    case "m":
                        quartz.AddTrigger(opts => opts
                          .ForJob(jobKey)
                          .WithIdentity(jobName + "-trigger")
                          .WithSimpleSchedule(x => x
                              .WithIntervalInMinutes(_value)
                              .RepeatForever()
                          ));
                        break;
                    case "s":
                        quartz.AddTrigger(opts => opts
                          .ForJob(jobKey)
                          .WithIdentity(jobName + "-trigger")
                          .WithSimpleSchedule(x => x
                              .WithIntervalInSeconds(_value)
                              .RepeatForever()
                          ));
                        break;
                    case "d":
                        quartz.AddTrigger(opts => opts
                          .ForJob(jobKey)
                          .WithIdentity(jobName + "-trigger")
                         .WithDailyTimeIntervalSchedule
                          (s =>
                             s.WithIntervalInHours(24)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(16, 05))
                          ));
                        break;
                }
                //throw new Exception($"No Quartz.NET Cron schedule found for job in configuration at {configKey}");
            }

        }

        public static void AddJobAndTriggerRemind<T>(this IServiceCollectionQuartzConfigurator quartz, IConfiguration config) where T : IJob
        {
            string jobName = typeof(T).Name;
            var configKey = $"Quartz:{jobName}";

            var cronOn = config[configKey + ":On"];
            if (cronOn == "1" )
            {
                var _paramInfoPrefix = "Param.SystemInfo";
                var value_get = _iDb.StringGet(_paramInfoPrefix);
                var cronTimeValue = "";
                if (value_get.HasValue)
                {
                    var list_param = JsonSerializer.Deserialize<List<SystemParam>>(value_get);
                    cronTimeValue = list_param.FirstOrDefault(a => a.name.Contains("DAY_SENT_MAIL"))?.value;
                }
                var _value = string.IsNullOrEmpty(cronTimeValue) ? 1 : Convert.ToInt32(cronTimeValue);
                var now = DateTime.Now;
                var date_start = new DateTime(now.Year, now.Month, _value);
                var now_compare = now.Date;
                var date_start_compare = date_start.Date;
                //HolidayCalendar cal = new HolidayCalendar();
                //cal.AddExcludedDate(new DateTime(now.Year, now.Month, _value));
                //quartz.AddCalendar("myHolidays", cal, false, false);
                var jobKey = new JobKey(jobName);
                quartz.AddJob<T>(opts => opts.WithIdentity(jobKey));
                quartz.AddTrigger(opts => opts
                  .ForJob(jobKey)
                  .WithIdentity(jobName + "-trigger")
                  .WithSchedule(CronScheduleBuilder
                      .MonthlyOnDayAndHourAndMinute(_value, 08, 00))
                  );
            }
        }
    }
}
