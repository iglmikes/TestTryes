using GrpcBus.Core;
using GrpcServer.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;


var builder = WebApplication.CreateBuilder(args);


// Конфигурация Kestrel для GRPC
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5557, listenOptions =>
    {
        listenOptions.Protocols = HttpProtocols.Http2;
        listenOptions.UseHttps(); // Для HTTPS
    });

    // Альтернативно для HTTP:
    // options.ListenAnyIP(5000, listenOptions => 
    // {
    //     listenOptions.Protocols = HttpProtocols.Http2;
    // });
});


// Add services to the container.
builder.Services.AddControllersWithViews();
// Добавляем сервисы GRPC
// Добавление сервисов
builder.Services.AddGrpc(options =>
{
    options.EnableDetailedErrors = true;
    options.MaxReceiveMessageSize = 16 * 1024 * 1024; // 16MB
    options.MaxSendMessageSize = 16 * 1024 * 1024; // 16MB
});
// Регистрируем наш обработчик сообщений
builder.Services.AddSingleton<IMessageProcessor, DefaultMessageProcessor>();



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


// Настраиваем pipeline
app.MapGrpcBusService();
// Дополнительные endpoint'ы для проверки здоровья
app.MapGet("/", () => "GRPC Bus Service is running");
app.MapGet("/health", () => "Healthy");

app.Run();
