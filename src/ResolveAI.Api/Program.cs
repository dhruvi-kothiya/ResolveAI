using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using ResolveAI.Application.Interfaces;
using ResolveAI.Domain.Entities;
using ResolveAI.Infrastructure.BackgroundJobs;
using ResolveAI.Infrastructure.Identity;
using ResolveAI.Infrastructure.Middleware;
using ResolveAI.Infrastructure.Notifications;
using ResolveAI.Infrastructure.Notifications.SignalR;
using ResolveAI.Infrastructure.Persistence;
using ResolveAI.Infrastructure.Repositories;

using System.Text;

var builder = WebApplication.CreateBuilder(args);


// =========================================================
// BASIC SERVICES & ERROR HANDLING (Section 34)
// =========================================================

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddProblemDetails();

builder.Services.AddMemoryCache();


// =========================================================
// DATABASE - SQL SERVER (Section 2025)
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
// APPLICATION SERVICES (Dependencies - Section 4)
// =========================================================

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services.AddScoped<ITicketRepository, TicketRepository>();

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IAuditService, AuditService>();


// =========================================================
// SIGNALR & HANGFIRE (Section 16 & 43)
// =========================================================

builder.Services.AddSignalR();

builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

builder.Services.AddHangfireServer();


// =========================================================
// JWT AUTHENTICATION (Section 1)
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
    options.TokenValidationParameters =
        new TokenValidationParameters
        {
            ValidateIssuer = true,

            ValidateAudience = true,

            ValidateLifetime = true,

            ValidateIssuerSigningKey = true,

            ValidIssuer =
                jwtSettings["Issuer"],

            ValidAudience =
                jwtSettings["Audience"],

            IssuerSigningKey =
                new SymmetricSecurityKey(key)
        };
});


var app = builder.Build();


// =========================================================
// MIDDLEWARE & DASHBOARDS
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


// =========================================================
// EXCEPTION HANDLING
// =========================================================

app.UseExceptionHandler();


// =========================================================
// CORRELATION ID MIDDLEWARE
// =========================================================

app.UseMiddleware<CorrelationIdMiddleware>();


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseHangfireDashboard();


// =========================================================
// HEALTH CHECK (Section 79)
// =========================================================

app.MapGet(
    "/health",
    () => Results.Ok(
        new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow
        }
    )
);


// =========================================================
// SIGNALR
// =========================================================

app.MapHub<NotificationHub>("/notificationHub");


// =========================================================
// CONTROLLERS
// =========================================================

app.MapControllers();


// =========================================================
// ROLE SEEDING (Section 8)
// =========================================================

using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<RoleManager<Role>>();

    string[] roles =
    {
        "Admin",
        "Agent",
        "Employee",
        "Manager",
        "TeamLead",
        "KnowledgeManager"
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
// SLA BACKGROUND JOB
// =========================================================

RecurringJob.AddOrUpdate<SlaMonitorJob>(
    "CheckSlaBreach",
    job => job.CheckSlaBreaches(),
    Cron.Minutely
);


app.Run();