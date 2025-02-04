using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PensionSystem.Application.Extensions;
using PensionSystem.Application.Services;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.BackgroundJobs;
using PensionSystem.Infrastructure.DBContext;
using PensionSystem.Infrastructure.Helpers;
using PensionSystem.Infrastructure.Services;
using PensionSystem.Infrastructure.Validation;
using PensionSystem.Presentation.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Register JwtSettings in DI container
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>(); //come bk
Console.WriteLine($"Issuer: {jwtSettings.Issuer}, Audience: {jwtSettings.Audience}, Key: {jwtSettings.Key}"); //come bk

// Add Hangfire services with custom configuration
builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.FromSeconds(15),
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    })
);

builder.Services.AddControllers()
     .AddJsonOptions(options =>
     {
          // Use JsonStringEnumConverter to serialize enums as strings
          options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
     });

builder.Services.AddDbContext<PensionSystemContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
//Enables integration between FluentValidation and ASP.NET Core automatic validation pipeline.
builder.Services.AddFluentValidationAutoValidation();

// Register FluentValidation validator from the current assembly
builder.Services.AddValidatorsFromAssemblyContaining<MemberValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterMemberDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateMemberDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<EmployerValidator>();
builder.Services.AddValidatorsFromAssemblyContaining< BenefitValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<TransactionHistoryValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddContributionRequestDtoValidator>();

builder.Services.AddScoped<IValidator<Contribution>, ContributionValidator>();

builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IContributionServices, ContributionService>();

builder.Services.AddHangfireServer(); // Adds Hangfire server
// Register your services
builder.Services.AddScoped<PensionSystemJobService>();
builder.Services.AddScoped<JobScheduler>();

// Add Authentication and Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
    // Add JWT authentication
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        // var jwtSettings = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<JwtSettings>>().Value;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,  // Optionally validate token expiration
            ClockSkew = TimeSpan.Zero, // Optional: eliminate clock skew
            ValidIssuer =jwtSettings.Issuer,//builder.Configuration["Jwt:Issuer"],
            ValidAudience = jwtSettings.Audience,//builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key))
        };
    });


builder.Services.AddAuthorization();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "JWT Token Authentication API",
        Description = ".NET 8 Web API"
    });

    // Add the JWT Bearer Security definition
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by a space and your token."
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
    c.SchemaFilter<EnumSchemaFilter>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});


var app = builder.Build();

// Use Hangfire Dashboard
app.UseHangfireDashboard("/hangfire");


// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");
app.MapControllers();

// Register the recurring jobs to be scheduled when the app starts
app.Lifetime.ApplicationStarted.Register(() =>
{
    using var scope = app.Services.CreateScope();
    var jobScheduler = scope.ServiceProvider.GetRequiredService<JobScheduler>();
    jobScheduler.ScheduleRecurringJobs();
});

app.Run();
