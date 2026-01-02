using Asp.Versioning;
using FluentValidation;
using Lacalizer.WebAPI.Apis;
using Lacalizer.WebAPI.Application.Commands.Videos;
using Lacalizer.WebAPI.Application.Queries;
using Lacalizer.WebAPI.Infrastructure;
using Lacalizer.WebAPI.Services.Validations;
using Microsoft.Extensions.Azure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddNpgsqlDbContext<LocalizeContext>("localizedb");
builder.Services.AddMemoryCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IVideoItemQueries, VideoItemQueries>();
builder.Services.AddScoped<ICommentQueries, CommentQueries>();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["StorageConnection:blobServiceUri"]!).WithName("StorageConnection");
    clientBuilder.AddQueueServiceClient(builder.Configuration["StorageConnection:queueServiceUri"]!).WithName("StorageConnection");
    clientBuilder.AddTableServiceClient(builder.Configuration["StorageConnection:tableServiceUri"]!).WithName("StorageConnection");
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Localizer API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapVideoItemAPI();
app.MapCommentAPI();
app.MapDefaultEndpoints();

app.CreateLocalizeDbIfNotExists();

app.Run();





