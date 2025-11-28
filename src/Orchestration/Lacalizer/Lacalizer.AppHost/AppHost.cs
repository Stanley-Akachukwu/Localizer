
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddAzurePostgresFlexibleServer("postgres")
    .RunAsContainer(postgres =>
    {
        postgres.WithDataVolume();
    });
var localizedb = db.AddDatabase("localizedb");

builder.AddContainer("pgadmin", "dpage/pgadmin4")
    .WithEnvironment("PGADMIN_DEFAULT_EMAIL", "admin@admin.com")
    .WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "AdminPass123")
    .WithEnvironment("PGADMIN_CONFIG_SERVER_MODE", "False")
    .WithEnvironment("PGADMIN_CONFIG_MASTER_PASSWORD_REQUIRED", "False")
    .WithBindMount("./pgadmin-data", "/var/lib/pgadmin")
    .WithEndpoint(name: "pgadmin-http", port: 8090, targetPort: 80)
    .WithReference(localizedb); // http://localhost:8090/browser/  

var lacalizerWebapi =builder.AddProject<Projects.Lacalizer_WebAPI>("lacalizer-webapi")
    .WithExternalHttpEndpoints()
    .WithReference(localizedb);

builder.AddDevTunnel("tunnel")
       .WithReference(lacalizerWebapi)
       .WithAnonymousAccess(); 
 
builder.Build().Run();
