using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using ResolveAI.Application.Interfaces;
using ResolveAI.Domain.Entities;
using ResolveAI.Infrastructure.Identity;
using ResolveAI.Infrastructure.Notifications;
using ResolveAI.Infrastructure.Notifications.SignalR;
using ResolveAI.Infrastructure.Persistence;
using ResolveAI.Infrastructure.Repositories;

using System.Text;

var builder = WebApplication.CreateBuilder(args);


// =========================================================
// BASIC SERVICES
// =========================================================

builder.Services.AddControllers();
builder.Services.AddOpenApi();


// =========================================================
// DATABASE - SQL SERVER
// =========================================================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));


// =========================================================
// ASP.NET IDENTITY
// =========================================================

builder.Services.AddIdentity<User, Role>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// =========================================================
// APPLICATION SERVICES
// =========================================================

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddScoped<INotificationService, NotificationService>();


// =========================================================
// SIGNALR
// =========================================================

builder.Services.AddSignalR();


// =========================================================
// JWT AUTHENTICATION
// =========================================================

var jwtSettings = builder.Configuration.GetSection("Jwt");

var key = Encoding.UTF8.GetBytes(
    jwtSettings["Key"]!
);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme =
        JwtBearerDefaults.AuthenticationScheme;

    options.DefaultChallengeScheme =
        JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],

        IssuerSigningKey =
            new SymmetricSecurityKey(key)
    };
});


// =========================================================
// BUILD APP
// =========================================================

var app = builder.Build();


// =========================================================
// OPEN API
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// =========================================================
// MIDDLEWARE
// =========================================================

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();


// =========================================================
// SIGNALR HUB
// =========================================================

app.MapHub<NotificationHub>("/notificationHub");


// =========================================================
// API CONTROLLERS
// =========================================================

app.MapControllers();


// =========================================================
// ROLE SEEDING
// =========================================================

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();

    string[] roles =
    {
        "Admin",
        "Agent",
        "Employee",
        "Manager"
    };

    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(
                new Role
                {
                    Name = roleName
                }
            );
        }
    }
}


// =========================================================
// RUN APPLICATION
// =========================================================

app.Run();