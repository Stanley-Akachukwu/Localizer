using Asp.Versioning;
using FluentValidation;
using Lacalizer.WebAPI.Apis;
using Lacalizer.WebAPI.Application.Commands.Videos;
using Lacalizer.WebAPI.Application.Queries;
using Lacalizer.WebAPI.Auth;
using Lacalizer.WebAPI.Entites.Users;
using Lacalizer.WebAPI.Extensions;
using Lacalizer.WebAPI.Infrastructure;
using Lacalizer.WebAPI.Models;
using Lacalizer.WebAPI.Services.Validations;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

//builder.AddServiceDefaults();
//builder.AddNpgsqlDbContext<LocalizeDbContext>("localizedb");
builder.Services.AddDbContext<LocalizeDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("localizedb")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<LocalizeDbContext>()
    .AddDefaultTokenProviders();
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddValidatorsFromAssemblyContaining(typeof(CreateVideoItemCommandHandler));
builder.Services.AddHealthChecks();
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateVideoItemCommandHandler).Assembly);
});
builder.Services.AddRouting(options =>
{
    options.LowercaseUrls = true;
    options.LowercaseQueryStrings = true;  
});

builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IVideoItemQueries, VideoItemQueries>();
builder.Services.AddScoped<ICommentQueries, CommentQueries>();
builder.Services.AddScoped<IContextQueries, ContextQueries>();

builder.Services.AddScoped<JwtTokenGenerator>();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddAzureClients(clientBuilder =>
    {
        clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnection:blobServiceUri"]!).WithName("StorageConnection");
        clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnection:queueServiceUri"]!).WithName("StorageConnection");
        clientBuilder.AddTableServiceClient(builder.Configuration["StorageConnection:tableServiceUri"]!).WithName("StorageConnection");
    });
}
else
{
    builder.Services.AddAzureClients(clientBuilder =>
    {
        clientBuilder.AddBlobServiceClient(
            builder.Configuration.GetConnectionString("blobs")!);
    });
}


var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Localizer API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapAuthAPI();
app.MapVideoItemAPI();
app.MapCommentAPI();
app.MapVideoContextAPI();
app.MapDefaultEndpoints();

app.CreateLocalizeDbIfNotExists();

app.Run();





