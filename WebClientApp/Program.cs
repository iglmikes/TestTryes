using GrpcClient.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using WebClientApp.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


//// Конфигурация из appsettings.json
//builder.Services.Configure<GrpcClientOptions>(
//    builder.Configuration.GetSection("GrpcClient"));

// В Program.cs основного приложения
builder.Services.AddSingleton<IGrpcBusClient>(provider =>
{
    var options = provider.GetRequiredService<IOptions<GrpcConfig>>().Value;
    var logger = provider.GetRequiredService<ILogger<GrpcBusClient>>();
    return new GrpcBusClient(options.ServerUrl);
});


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

app.Run();
