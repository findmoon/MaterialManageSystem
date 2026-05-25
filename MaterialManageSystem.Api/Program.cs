using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MaterialManageSystem.Core.Interfaces;
using MaterialManageSystem.Infrastructure.Data;
using MaterialManageSystem.Infrastructure.Repositories;
using MaterialManageSystem.Infrastructure.Services;
using MaterialManageSystem.Api.DTOs;
using MaterialManageSystem.Api.Hubs;
using MaterialManageSystem.Api.HostedServices;
using MaterialManageSystem.Api.Middleware;
using MaterialManageSystem.Api.Mapping;
using MaterialManageSystem.Api.Configuration;
using Serilog;

// 配置Serilog
SerilogConfiguration.ConfigureSerilog();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MaterialDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IRackRepository, RackRepository>();
builder.Services.AddScoped<ILayerRepository, LayerRepository>();
builder.Services.AddScoped<ICellRepository, CellRepository>();
builder.Services.AddScoped<IPartNoRepository, PartNoRepository>();
builder.Services.AddScoped<IPartNoCategoryRepository, PartNoCategoryRepository>();
builder.Services.AddScoped<IReelIdRepository, ReelIdRepository>();
builder.Services.AddScoped<IReelUsageLogRepository, ReelUsageLogRepository>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IWarningConfigRepository, WarningConfigRepository>();
builder.Services.AddScoped<IWarningRecordRepository, WarningRecordRepository>();
builder.Services.AddScoped<IOperationLogRepository, OperationLogRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IWarningDetectionService, WarningDetectionService>();
builder.Services.AddHostedService<WarningDetectionHostedService>();

var jwtKey = builder.Configuration["Jwt:Key"] 
    ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? throw new InvalidOperationException("JWT Key is not configured. Set Jwt:Key in configuration or JWT_SECRET_KEY environment variable.");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MaterialManageSystem";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtIssuer,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            var errorMessage = errors.Count > 0 ? string.Join("; ", errors) : "参数验证失败";

            var result = ApiResponse<object>.Fail(400, errorMessage);
            return new BadRequestObjectResult(result);
        };
    });

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
// .NET 10 内置的 OpenAPI 功能（ Microsoft.AspNetCore.OpenApi ）
// 提供 openapi/v1.json OpenAPI 规范（JSON 格式）
// builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<RateLimitMiddleware>();
app.UseMiddleware<OperationLogMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<WarningHub>("/hubs/warning");
app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MaterialDbContext>();
    dbContext.Database.EnsureCreated();
    DataSeeder.SeedData(dbContext);
}

app.Run();
