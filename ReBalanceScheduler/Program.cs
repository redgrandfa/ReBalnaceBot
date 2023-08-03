using Coravel;
using MyAlpacaStrategyLib;

namespace ReBalanceScheduler
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            IServiceCollection services = builder.Services;
            IConfiguration config = builder.Configuration;

            services.AddStackExchangeRedisCache(options =>
                options.Configuration = config["RedisConfig:reid12"]
            );
            services.AddSingleton<IMemoryCacheRepository, MemoryCacheRepository>();
            services.AddSingleton<Logger>();
            services.AddTransient<ReBalanceOperation>();

            services.AddScheduler();
            services.AddTransient<ReBalanceDayStartInvocable>();
            services.AddTransient<ReBalanceDayRepeatInvocable>();


            var app = builder.Build();

            app.Services.UseScheduler(scheduler =>
            {
                //�L�O�G13:30~20:00 
                //�V�O�G14:30~21:00
                //=> �p��  �x�W�ߤW 9�I�b~���5�I: UTC 13~21
                //=> �涰: �x�W�ߤW10�I�b~���4�I: UTC 15~20

                //UTC+0  13:30 ~ 20:00

                //25K �H�W����day trade�v�T
                //scheduler.Schedule<ReBalanceDayStartInvocable>().Cron("* * * * *");

                scheduler.Schedule<ReBalanceDayStartInvocable>().Cron("53 13 * * 1-5");

                scheduler.Schedule<ReBalanceDayRepeatInvocable>().Cron("54-59 13 * * 1-5");
                scheduler.Schedule<ReBalanceDayRepeatInvocable>().Cron("* 14-18 * * 1-5");
                scheduler.Schedule<ReBalanceDayRepeatInvocable>().Cron("1-41 19 * * 1-5");

                scheduler.Schedule<ReBalanceDayEndInvocable>().Cron("1 21 * * 1-5");

            }).OnError((exception) =>
            {
                Console.WriteLine(exception);
            });


            app.MapGet("/", () => "Hello World!");

            app.Run();
        }
    }
}