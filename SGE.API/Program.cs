using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SGE.API.Middleware;
using SGE.Application.DTOs.Employees;
using SGE.Application.Interfaces.Repositories;
using SGE.Application.Interfaces.Services;
using SGE.Application.Mappings;
using SGE.Application.Services;
using SGE.Application.Validators;
using SGE.Infrastructure.Data;
using SGE.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Récupérer la chaine de connexion
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection");
// Ajouter DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile).Assembly);
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Services
builder.Services.AddScoped<IExcelService, ExcelService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

// Validators
builder.Services.AddScoped<IValidator<EmployeeImportDto>, EmployeeImportValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EmployeeImportValidator>();

builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();

builder.Services.AddScoped<ILeaveRequestRepository, LeaveRequestRepository>();
builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();