﻿using System;
using System.Text;
using KiMa_API.Data;
using KiMa_API.Models;
using KiMa_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Azure.Communication.Email;
using KiMa_API.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// 1. Datenbank-Konfiguration
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
    );
}
else
{
    var mysqlConn = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Missing DefaultConnection");
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseMySql(mysqlConn, ServerVersion.AutoDetect(mysqlConn))
    );
}

// 2. Azure Communication EmailClient
builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connStr = config["CommunicationServices:EmailConnectionString"]
                  ?? throw new InvalidOperationException("Missing CommunicationServices:EmailConnectionString");
    return new EmailClient(connStr);
});

// 3. API-Versionierung
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(2, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// 4. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy => policy
        .WithOrigins(
            "http://localhost:4200",
            "https://kima-gui-efcbhkgkdkf8a9ap.westeurope-01.azurewebsites.net",
            "https://kimafo.info",
            "https://kimafo.de"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
    );
});

// 5. Identity & JWT-Konfiguration
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("Missing Jwt:Key");
var key = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    // Passwort-Einstellungen
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    // Benutzer-Einstellungen
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+!#$%^&*()äöüÄÖÜß ";
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddDefaultTokenProviders();

// DataProtectionTokenProvider (z. B. E-Mail-Confirm, PW-Reset)
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    options.TokenLifespan = TimeSpan.FromDays(1);
});

// JWT Bearer Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

// 6. Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Bitte 'Bearer {token}' eingeben",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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

// 7. Controllers & JSON-Optionen
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
});

// 8. Applikations-Services
builder.Services.AddScoped<IProjectResponseService, ProjectResponseService>();
builder.Services.AddScoped<IEmailCampaignService, EmailCampaignService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFAQService, FAQService>();
builder.Services.AddScoped<JwtService>();


var app = builder.Build();

// 9. Middleware-Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
