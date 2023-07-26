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
builder.Services.AddTransient<ReBalanceDayStartInvocable>();
builder.Services.AddTransient<ReBalanceDayRepeatInvocable>();
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
    //var exec = () =>
    //{
    //    int i = 0;
    //    Console.WriteLine($"{i++} : {DateTime.UtcNow}");
    //};

    //scheduler.Schedule(exec).Cron("30-59 13 * * 1-5");
    //scheduler.Schedule(exec).Cron("* 14-20 * * 1-5");

    //scheduler.Schedule(exec)
    //.Cron("30/1 13-20 * * 1-5")
    //.Weekday()
    //.RunOnceAtStart()
    //.Zoned(TimeZoneInfo.Local)
    ;

    //夏令：13:30~20:00 
    //冬令：14:30~21:00
    //=> 聯集  台灣晚上 9點半~凌晨5點: UTC 13~21
    //=> 交集: 台灣晚上10點半~凌晨4點: UTC 15~20

    //UTC+0  13:30 ~ 20:00

    //25K 以上不受day trade影響
    scheduler.Schedule<ReBalanceDayStartInvocable>().Cron("56 13 * * 1-5");
    //scheduler.Schedule<ReBalanceDayRepeatInvocable>().Cron("54-59 13 * * 1-5");
    scheduler.Schedule<ReBalanceDayRepeatInvocable>().Cron("* 14-20 * * 1-5");
    //scheduler.Schedule<ReBalanceDayRepeatInvocable>().Cron("0 14-20 * * 1-5").RunOnceAtStart();
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