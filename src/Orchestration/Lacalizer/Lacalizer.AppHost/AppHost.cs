
try
{
    var builder = DistributedApplication.CreateBuilder(args);

    builder.AddProject<Projects.Lacalizer_WebAPI>("lacalizer-webapi")
       .WithExternalHttpEndpoints();

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
 





