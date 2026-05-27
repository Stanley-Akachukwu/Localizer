

using Microsoft.Extensions.Hosting;

try
{
    var builder = DistributedApplication.CreateBuilder(args);

    // PostgreSQL
    var postgres = builder.AddAzurePostgresFlexibleServer("postgres");

    // Database
    var localizedb = postgres.AddDatabase("localizedb");

    // Blob Storage
    var storage = builder.AddAzureStorage("storage");

    var blobs = storage.AddBlobs("blobs");

    // API
    var localizerWebApi = builder
        .AddProject<Projects.Lacalizer_WebAPI>("lacalizer-webapi")
        .WithExternalHttpEndpoints()
        .WithReference(localizedb)
        .WithReference(blobs);

    // Development-only resources
    if (builder.Environment.IsDevelopment())
    {
        postgres.RunAsContainer(postgres =>
        {
            postgres.WithDataVolume();
        });

        storage.RunAsEmulator();

        builder.AddContainer("pgadmin", "dpage/pgadmin4")
            .WithEnvironment("PGADMIN_DEFAULT_EMAIL", "admin@admin.com")
            .WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "AdminPass123")
            .WithEnvironment("PGADMIN_CONFIG_SERVER_MODE", "False")
            .WithEnvironment("PGADMIN_CONFIG_MASTER_PASSWORD_REQUIRED", "False")
            .WithBindMount("./pgadmin-data", "/var/lib/pgadmin")
            .WithEndpoint(name: "pgadmin-http", port: 8090, targetPort: 80)
            .WithReference(localizedb);

        builder.AddDevTunnel("localizer-api")
            .WithReference(localizerWebApi)
            .WithAnonymousAccess();
    }

    builder.Build().Run();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;

    Console.WriteLine("An error occurred while starting the Aspire application.");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"StackTrace: {ex.StackTrace}");

    Console.ResetColor();

    throw;
}
 





