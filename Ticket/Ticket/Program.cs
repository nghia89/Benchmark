using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using StackExchange.Redis;
using Ticket.BackgroundService;
using Ticket.Helpers;
using Ticket.Modules.SeatChecking;
using Ticket.Modules.TicketBooking;
using Ticket.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();  
builder.Services.AddEndpointsApiExplorer();  
builder.Services.AddSwaggerGen( );


#region Mongodb
builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddScoped(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});


#endregion

#region Kafka
var kafkaProducerConfig = new ProducerConfig
{
    BootstrapServers = "localhost:9092",  
};

builder.Services.AddSingleton(kafkaProducerConfig);

builder.Services.AddSingleton<IKafkaHelper, KafkaHelper>();

builder.Services.AddSingleton<IKafkaConsumerService, BookingConsumerService>();
builder.Services.AddSingleton<ITicketBookingConsumerService, TicketBookingConsumerService>();
builder.Services.AddHostedService<KafkaConsumersHostedService>();

#endregion

#region Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddScoped<IRedisCacheHelper, RedisCacheHelper>();
#endregion

#region Add services

builder.Services.AddScoped<ISeatCheckingService, SeatCheckingService>();
builder.Services.AddScoped<ITicketBookingService, TicketBookingService>();
#endregion
 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{ 
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "My API v1");
    options.RoutePrefix = string.Empty;  
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllers();  
 
app.Run();