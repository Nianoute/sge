using Microsoft.EntityFrameworkCore;
using SGE.Infrastructure.Data;
using SGE.Application.Mappings;
using SGE.Application.Interfaces.Repositories;
using SGE.Infrastructure.Repositories;
using SGE.Application.Services;
using SGE.Application.Interfaces.Services;
using SGE.API;

var builder = WebApplication.CreateBuilder(args);
// Récupérer la chaine de connexion
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");
// Ajouter DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

//injection de dépendance des services
builder.Services.AddScoped<IDepartmentRepository,
    DepartmentRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();

builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


namespace SGE.API
{
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}