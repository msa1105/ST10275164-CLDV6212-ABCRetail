using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection; // <-- ADD THIS
using Microsoft.EntityFrameworkCore;           // <-- ADD THIS
using ABCRetail.Functions;                     // <-- ADD THIS
using System;                                  // <-- ADD THIS

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        // Get the SQL connection string from local.settings.json or App Settings
        string? connectionString = Environment.GetEnvironmentVariable("SqlConnectionString");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "The SQL connection string ('SqlConnectionString') is not set.");
        }

        // Register your AppDbContext with the SQL Server provider
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString));
    })
    .Build();

host.Run();
