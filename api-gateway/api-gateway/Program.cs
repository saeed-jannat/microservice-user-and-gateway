using api_gateway.Core.Application.Interfaces;
using api_gateway.Core.Application.Services;
using api_gateway.Infrastructure.Ocelot.Aggegators;
using api_gateway.Infrastructure.Ocelot.DelegatingHandlers;
using api_gateway.Presentation.Middleware;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;



var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile($"ocelot.{environment}.json", optional: true, reloadOnChange: true); // فایل محیطی

builder.Services.AddOcelot()
    .AddSingletonDefinedAggregator<AdminPageAggregator>()
    .AddSingletonDefinedAggregator<BuildingPageAggregator>()
    .AddDelegatingHandler<RoleHandler>(false);
    
builder.Services.AddScoped<ICookie, api_gateway.Core.Application.Services.Cookie>();
builder.Services.AddHttpClient<IStaticDataService, staticDataService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

api_gateway.Core.Common.Utilities.Authorization.Initialize(app.Services);

app.UseCors("AllowAll");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.UseSwaggerUI(c =>
    //{
    //    foreach (var swaggerEndpoint in builder.Configuration.GetSection("SwaggerEndPoints").GetChildren())
    //    {
    //        var key = swaggerEndpoint["Key"];
    //        var url = swaggerEndpoint["Config:0:Url"];
    //        var name = swaggerEndpoint["Config:0:Name"];

    //        // اضافه کردن Swagger endpoint برای هر سرویس
    //        c.SwaggerEndpoint(url, name);
    //    }

    //    c.RoutePrefix = string.Empty; // این خط برای نمایش Swagger UI در روت
    //});
}

//app.UseHttpsRedirection();

using (var scope = app.Services.CreateScope())
{
    var AuthorizationDataService = scope.ServiceProvider.GetRequiredService<IStaticDataService>();

    try
    {
        // فراخوانی متد از سرویس
        await AuthorizationDataService.InitializeStaticDataService();
        //AuthorizationDataService.GetAllControllersAndActions();
        await AuthorizationDataService.GetAllTheClaims();

    }
    catch (Exception ex)
    {
        // مدیریت خطا
        Console.WriteLine($"Error initializing roles: {ex.Message}");
    }
}

app.UseAuthorization();


app.UseWhen(context =>
    !context.Request.Path.ToString().ToLower().Contains("roles") &&
    !context.Request.Path.ToString().ToLower().Contains("login") &&
    !context.Request.Path.ToString().ToLower().Contains("register") &&
    !context.Request.Path.ToString().ToLower().Contains("set-claims") &&
    context.Request.Path.StartsWithSegments("/api")
    , appBuilder =>
    {
        appBuilder.UseAuthMiddleware();
    });

app.UseOcelot().Wait();
app.MapControllers();

app.Run();
