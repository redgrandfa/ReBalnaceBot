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


//�O�o���U Invocable����@
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

    //�L�O�G13:30~20:00 
    //�V�O�G14:30~21:00
    //=> �p��  �x�W�ߤW 9�I�b~���5�I: UTC 13~21
    //=> �涰: �x�W�ߤW10�I�b~���4�I: UTC 15~20

    //UTC+0  13:30 ~ 20:00

    //25K �H�W����day trade�v�T
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