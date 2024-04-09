using System.Security.Claims;
using AspNetCoreRateLimit;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Test;
using FinanceManagement.Api.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using FinanceManagement.Api.utils;
using FinanceManagement.Api.Repositories;
using FinanceManagement.Api.Controllers.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add user secrets
// Used Secret Manager to hide secrets, this is for development only.
// For Production, use Azure Key Vault or other secure methods.
if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();

    //Setting secret for IdentityServer for dev only.
    HashedClientSecret secret = new HashedClientSecret();
    secret.Write("Secret123"); // <-- Replace with your own secret
}

//Add Repos
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IIncomeRepository, IncomeRepository>();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();

// Add rate limiting services
builder.Services.AddOptions();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();

// Configure OAuth server (Authorization Server)
var identityServerClients = builder
    .Configuration.GetSection("IdentityServer:Clients")
    .Get<List<Client>>();
var identityServerApiScopes = builder
    .Configuration.GetSection("IdentityServer:ApiScopes")
    .Get<List<ApiScope>>();

builder
    .Services.AddIdentityServer(options => options.EmitStaticAudienceClaim = true)
    .AddInMemoryClients(identityServerClients)
    .AddInMemoryApiScopes(identityServerApiScopes)
    .AddDeveloperSigningCredential();

builder.Services.AddDbContext<FinanceManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("AssessmentDB"))
);

// Configure OAuth client
var jwtBearerOptions = builder.Configuration.GetSection("JwtBearer").Get<JwtBearerOptions>();

builder
    .Services.AddAuthentication("Bearer")
    .AddJwtBearer(
        "Bearer",
        options =>
        {
            options.Authority = jwtBearerOptions.Authority;
            options.RequireHttpsMetadata = jwtBearerOptions.RequireHttpsMetadata;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = jwtBearerOptions.Audience,
            };
            options.Audience = jwtBearerOptions.Audience;
        }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseIpRateLimiting();
app.UseIdentityServer();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

CreateDatabase(app);
app.Run();

void CreateDatabase(WebApplication app)
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<FinanceManagementContext>();
        context.Database.EnsureCreated();
    }
}
