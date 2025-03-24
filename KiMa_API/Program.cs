using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KiMa_API.Data;
using KiMa_API.Models;
using Microsoft.OpenApi.Models;
using KiMa_API.Services;

var builder = WebApplication.CreateBuilder(args);

// 🔹 ConnectionString aus appsettings.json holen
var connectionString = builder.Configuration.GetConnectionString("SQLiteConnection");

// 🔹 SQLite als Datenbankprovider setzen
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString)
);

// 🔹 CORS-Richtlinie für Angular-App
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials());
});

// 🔹 Identity für Benutzerverwaltung
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    // 🔐 Passwortregeln anpassen
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;

    // 🆔 Benutzername darf Sonderzeichen enthalten
    options.User.AllowedUserNameCharacters =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+!#$%^&*()äöüÄÖÜß ";
    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AppDbContext>()
    .AddErrorDescriber<CustomIdentityErrorDescriber>()
    .AddDefaultTokenProviders();
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    // Setzt die Gültigkeitsdauer des Tokens auf 1 Tag (du kannst das natürlich anpassen)
    options.TokenLifespan = TimeSpan.FromDays(1);
});



// 🔹 JWT-Authentifizierung
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"] ?? "supergeheimeschluessel123!");

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
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Debug);


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
            new string[] {}
        }
    });
});


builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401; // Unauthorized statt Redirect
        return Task.CompletedTask;
    };
});



// 🔹 Controller & Swagger aktivieren
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<IMailService, MailService>();
builder.Services.AddScoped<IAuthService, AuthService>();



var app = builder.Build();

// 🔹 Middleware-Pipeline konfigurieren
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 🔹 CORS DIREKT HIER EINBINDEN
app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 🔹 Seed-Datenbank vor `app.Run()` ausführen
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

    await SeedDatabase(userManager, roleManager);
}

app.Run();

// 🔹 Seed-Methode
async Task SeedDatabase(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
{
    var roles = new[] { "Admin", "Proband" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<int> { Name = role });
        }
    }

    if (await userManager.FindByEmailAsync("admin@example.com") == null)
    {
        var adminUser = new User
        {            
            Email = "admin@example.com",
            Role = "Admin",
            FirstName = "Dieter",
            LastName = "Krebs"
        };

        var result = await userManager.CreateAsync(adminUser, "AdminPass123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    if (await userManager.FindByEmailAsync("proband@example.com") == null)
    {
        var probandUser = new User
        {            
            Email = "proband@example.com",
            Role = "Proband",
            FirstName = "Dieter",
            LastName = "Krebs"
        };

        var result = await userManager.CreateAsync(probandUser, "ProbandPass123!");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(probandUser, "Proband");
        }
    }
}
