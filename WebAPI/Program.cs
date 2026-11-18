using System.Text;
using GameMonitoring.Business.Mapping;
using GameMonitoring.Data.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Program.cs is the startup file for the WebAPI project. It configures services, authentication,
// Swagger, middleware, and API routing before the application starts.

// Creates the WebApplication builder used to register all required API services.
var builder = WebApplication.CreateBuilder(args);

// Adds controller support and configures JSON serialization so object reference cycles do not crash responses.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Registers the shared EF Core database context so API controllers can access the SQL Server database.
builder.Services.AddDbContext<GameMonitoringContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registers AutoMapper mappings from the Business project so entities can be converted to DTOs.
builder.Services.AddAutoMapper(typeof(GameMappingProfile));

// Configures JWT bearer authentication so protected API calls can identify the logged-in user.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
    };
});

// Adds authorization services used by controllers/endpoints that require logged-in users or roles.
builder.Services.AddAuthorization();

// Adds Swagger/OpenAPI services so the API can be tested from the browser during development.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Game Monitoring API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
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

// Builds the API application after all services have been registered.
var app = builder.Build();

// Enables Swagger only in development so the API can be explored and tested more easily.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Allows the API to serve static files from wwwroot, such as the simple test HTML pages.
app.UseStaticFiles();

// Redirects HTTP requests to HTTPS when HTTPS is configured.
//app.UseHttpsRedirection();


// Runs authentication before authorization so user identity and roles are available to protected endpoints.
app.UseAuthentication();
app.UseAuthorization();

// Maps controller routes such as /api/games and /api/auth/login.
app.MapControllers();

// Starts the WebAPI application.
app.Run();
