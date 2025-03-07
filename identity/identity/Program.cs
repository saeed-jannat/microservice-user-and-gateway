using identity.Core.Application.DTOs;
using identity.Core.Application.Interfaces;
using identity.Core.Application.Services;
using identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<SilbargBaseIdentityContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IToken, Token>();
builder.Services.AddSingleton<IDateService, DateService>();

builder.Services.AddHttpContextAccessor();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    StaticData.ApiGateWayBaseUrl = builder.Configuration.GetSection("ApiGateWayBaseUrl").GetValue<string>("Development");
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
    StaticData.ApiGateWayBaseUrl = builder.Configuration.GetSection("ApiGateWayBaseUrl").GetValue<string>("Production");



app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
