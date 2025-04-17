using DBAbstractions.Interfaces;
using EFPostgreDataProvider.Core;
using EFMySqlDataProvider.Core;
using GrpcBus.Core;
using GrpcServer.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


var builder = WebApplication.CreateBuilder(args);


// ������������ Kestrel ��� GRPC
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5557, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps(); // ��� HTTPS
    });

    // ������������� ��� HTTP:
    // options.ListenAnyIP(5000, listenOptions => 
    // {
    //     listenOptions.Protocols = HttpProtocols.Http2;
    // });
});


// Add services to the container.
builder.Services.AddControllersWithViews();
// ��������� ������� GRPC
// ���������� ��������
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 16 * 1024 * 1024; // 16MB
    options.MaxSendMessageSize = 16 * 1024 * 1024; // 16MB
});
// ������������ ��� ���������� ���������
builder.Services.AddSingleton<IMessageProcessor, DefaultMessageProcessor>();

// �������� ������������
var configuration = builder.Configuration;

var hz = configuration["Database:Type"];


IDatabaseProvider databaseProvider = configuration["Database:Type"] switch
{
    "MySql" => new MySqlProvider(),
    "Postgre" => new PostgreProvider(),
    null => throw new Exception("not founded settings for a database provider"),
    _ => throw new Exception("Unsupported database provider")
};

// ������������� �������
databaseProvider.ConfigureDbContext(
builder.Services,
    configuration.GetConnectionString(configuration["Database:Type"]));

databaseProvider.RegisterUnitOfWork(builder.Services);
//databaseProvider.ConfigureRepositories(builder.Services); // ����������� - not ready yet



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


// ����������� pipeline
app.MapGrpcBusService();
// �������������� endpoint'� ��� �������� ��������
app.MapGet("/", () => "GRPC Bus Service is running");
app.MapGet("/health", () => "Healthy");

app.Run();
