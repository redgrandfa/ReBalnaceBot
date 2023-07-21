using Alpaca.Markets;
using Coravel;
using MyAlpacaStrategyLib;
using TradingBot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//builder.Services.AddSingleton<IAlpacaTradingClient, >();


//記得註冊 Invocable的實作
builder.Services.AddTransient<ReBalanceInvocable>();
builder.Services.AddScheduler();

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
    //scheduler.Schedule(() =>
    //{
    //    int i = 0;
    //    Console.WriteLine($"{i++} : {DateTime.UtcNow}");
    //})
    //.Cron("* 13-20 * * 1-5")
    //.Weekday()
    //.RunOnceAtStart()
    //.Zoned(TimeZoneInfo.Local)
    ;

    //夏令：13:30~20:00 
    //冬令：14:30~21:00
    //=> 聯集  台灣晚上9點半~凌晨5點: UTC 13~21
    //=> 希望訂單馬上成交: 交集: UTC 15~20

    //UTC+0  13:30 ~ 20:00

    scheduler.Schedule<ReBalanceInvocable>()
    .Cron("*/1 13-20 * * 1-5")
    //不能搭配 Weekday()
    ;
}).OnError((exception) =>
{
    Console.WriteLine(exception);
});



//app.MapGet("/weatherforecast", () =>
//{
//    return 1;
//})
//.WithName("GetWeatherForecast");

app.Run();