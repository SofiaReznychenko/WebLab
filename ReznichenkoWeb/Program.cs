using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
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

// Register Repositories
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IWorkoutRepository, WorkoutRepository>();

// Register Validators
builder.Services.AddScoped<IValidator<CreateMemberDto>, CreateMemberDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateMemberDto>, UpdateMemberDtoValidator>();
builder.Services.AddScoped<IValidator<CreateWorkoutDto>, CreateWorkoutDtoValidator>();
builder.Services.AddScoped<IValidator<UpdateWorkoutDto>, UpdateWorkoutDtoValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Спортивний зал API V1");
    c.RoutePrefix = "swagger"; // Makes Swagger UI available at /swagger
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();