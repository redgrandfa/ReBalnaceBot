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

    //�L�O�G13:30~20:00 
    //�V�O�G14:30~21:00
    //=> �p��  �x�W�ߤW9�I�b~���5�I: UTC 13~21
    //=> �Ʊ�q�氨�W����: �涰: UTC 15~20

    //UTC+0  13:30 ~ 20:00

    scheduler.Schedule<ReBalanceInvocable>()
    .Cron("*/1 13-20 * * 1-5")
    //����f�t Weekday()
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