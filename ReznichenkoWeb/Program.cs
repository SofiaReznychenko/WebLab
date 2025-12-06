using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;

using Serilog;
using ReznichenkoWeb;
using ReznichenkoWeb.Repositories;
using ReznichenkoWeb.Models;
using ReznichenkoWeb.DTOs;
using ReznichenkoWeb.Validators;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Спортивний зал API",
        Version = "v1",
        Description = "REST API для управління членами спортивного залу та тренуваннями"
    });
});

// Configure Database
builder.Services.AddDbContext<GymContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Settings
builder.Services.Configure<GymSettings>(builder.Configuration.GetSection("GymSettings"));

// Add Memory Cache
builder.Services.AddMemoryCache();

// Register Repositories
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();
builder.Services.AddScoped<ITrainerRepository, TrainerRepository>();

// Register Validators
builder.Services.AddScoped<IValidator<CreateMemberDto>, CreateMemberDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateMemberDto>, UpdateMemberDtoValidator>();
builder.Services.AddScoped<IValidator<CreateWorkoutDto>, CreateWorkoutDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateWorkoutDto>, UpdateWorkoutDtoValidator>();
builder.Services.AddScoped<IValidator<CreateTrainerDto>, CreateTrainerDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateTrainerDto>, UpdateTrainerDtoValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Спортивний зал API V1");
    c.RoutePrefix = "swagger"; // Makes Swagger UI available at /swagger
});

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}