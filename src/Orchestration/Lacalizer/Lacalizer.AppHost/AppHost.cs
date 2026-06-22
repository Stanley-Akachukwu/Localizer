using Microsoft.Extensions.Configuration;

try
{
    var builder = DistributedApplication.CreateBuilder(args);

    var postgres = builder.AddAzurePostgresFlexibleServer("postgres");

    var localizedb = postgres.AddDatabase("localizedb");

    var connectionString = builder.Configuration.GetConnectionString("localizedb");
    Console.WriteLine("Postgres Connection: " + connectionString);

    var storage = builder.AddAzureStorage("storage");

    var blobs = storage.AddBlobs("blobs");

    var localizerWebApi = builder
        .AddProject<Projects.Lacalizer_WebAPI>("lacalizer-webapi")
        .WithExternalHttpEndpoints()
        .WithReference(localizedb)
        .WithReference(blobs);

    //if (builder.Environment.IsDevelopment())
    //{
    //    postgres.RunAsContainer(postgres =>
    //    {
    //        postgres.WithDataVolume();
    //    });

    //    storage.RunAsEmulator();

    //    builder.AddContainer("pgadmin", "dpage/pgadmin4")
    //        .WithEnvironment("PGADMIN_DEFAULT_EMAIL", "admin@admin.com")
    //        .WithEnvironment("PGADMIN_DEFAULT_PASSWORD", "AdminPass123")
    //        .WithEnvironment("PGADMIN_CONFIG_SERVER_MODE", "False")
    //        .WithEnvironment("PGADMIN_CONFIG_MASTER_PASSWORD_REQUIRED", "False")
    //        .WithBindMount("./pgadmin-data", "/var/lib/pgadmin")
    //        .WithEndpoint(name: "pgadmin-http", port: 8090, targetPort: 80)
    //        .WithReference(localizedb);// http://localhost:8090/browser/  
    //}

    builder.Build().Run();
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;

    Console.WriteLine("An error occurred while starting the Aspire application.");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"StackTrace: {ex.StackTrace}");
    Console.ResetColor();
}
 





