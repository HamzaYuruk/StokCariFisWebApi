using CariWebApi.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using CariWebApi.Application.Interfaces;
using CariWebApi.Infrastructure.Repositories;
using CariWebApi.Application.Services;
using CariWebApi.Application.Mapping;
using CariWebApi.Middleware;
using Serilog;
using CariWebApi.Application.Constants;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using CariWebApi.Domain.Entities;


// logları dosyaya kaydetmek için
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// jwt ayaları
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.AddSingleton(jwtSettings);



builder.Host.UseSerilog();

// interface isteği görünce şunları ver kısımları
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReceiptService, ReceiptService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();

// mapping için ayar
builder.Services.AddAutoMapper(cfg => { }, typeof(MappingProfile));

// Appdbcontext isteyene bunu ver 
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// jwt
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization();




// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// swaggerde jwt tokeni test etmek için 
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT token'ı 'Bearer {token}' formatında girin"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


var app = builder.Build();

// seed
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!context.Roles.Any())
    {
        context.Roles.AddRange(
            new Role { Name = "Owner" },
            new Role { Name = "User" },
            new Role { Name = "Customer" },
            new Role { Name = "Admin" }
        );
        context.SaveChanges();
    }
}

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();