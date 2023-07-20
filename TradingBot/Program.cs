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
builder.Services.AddTransient<TestInvocable>();
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
    //scheduler.Schedule<TestInvocable>()
    //scheduler.ScheduleWithParams<BackupDatabaseTableInvocable>("[dbo].[Users]") //DI以外的引數

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

    scheduler.ScheduleAsync(async () =>
    {
        var envClient = new EnvClient();
        var client = envClient.Client;
        var account = envClient.Account;

        var a = new ReBalanceOperation(client, account);
        await a.TryReBalanceAll();
    })
    //UTC+0  08:00 ~ 13:30 ~ 20:00
    //.Cron("* 08-20 * * 1-5")
    .Cron("* 13-20 * * 1-5")
    //不能搭配 Weekday()
    ;

}).OnError((exception) =>
{
    Console.WriteLine(exception.Message);
    Console.WriteLine(exception);
});



//app.MapGet("/weatherforecast", () =>
//{
//    return 1;
//})
//.WithName("GetWeatherForecast");

app.Run();