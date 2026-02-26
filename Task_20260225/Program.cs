using Task_20260225.Common.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();

_AddServices(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();

var api = app.MapGroup("/api");
api.MapControllers();

if (_InitializeServices(app.Services) == false)
{
    // 바로 종료
    return;
}

app.Run();

void _AddServices(IServiceCollection services)
{
    services.AddSingleton<ContactCacheService>();
    services.AddSingleton<TaskServerServices>();
}

bool _InitializeServices(IServiceProvider serviceProvider)
{
    var configFiles = new List<string> { "appsettings.json" };
    var serverService = serviceProvider.GetService<TaskServerServices>();
    if (serverService == null)
        return false;
    
    serverService.Initialize(configFiles);
    serverService.LoggerService.Information(serverService, "Initializing services completed");
    
    var cacheService = serviceProvider.GetService<ContactCacheService>();
    if (cacheService == null)
        return false;

    cacheService.Initialize(serverService.LoggerService, serverService.GetValue("StartWithData", false));

    return true;
} 
