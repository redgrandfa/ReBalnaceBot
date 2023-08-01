using Alpaca.Markets;
using Coravel;
using MyAlpacaStrategyLib;
using TradingBot;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
IConfiguration config = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.Services.UseScheduler(scheduler =>
{
    //夏令：13:30~20:00 
    //冬令：14:30~21:00
    //=> 聯集  台灣晚上 9點半~凌晨5點: UTC 13~21
    //=> 交集: 台灣晚上10點半~凌晨4點: UTC 15~20

    //UTC+0  13:30 ~ 20:00

    //25K 以上不受day trade影響
    //scheduler.Schedule<ReBalanceDayStartInvocable>().Cron("* * * * *");

    scheduler.Schedule<ReBalanceDayStartInvocable>().Cron("53 13 * * 1-5")
    .RunOnceAtStart()
    ;
    scheduler.Schedule<ReBalanceDayRepeatInvocable>().Cron("54-59 13 * * 1-5");
    scheduler.Schedule<ReBalanceDayRepeatInvocable>().Cron("* 14-18 * * 1-5");
    scheduler.Schedule<ReBalanceDayRepeatInvocable>().Cron("1-41 19 * * 1-5") ;
}).OnError((exception) =>
{
    Console.WriteLine(exception);
});



app.MapGet("/x", () =>
{
    return 1;
})
.WithName("X");

app.Run();