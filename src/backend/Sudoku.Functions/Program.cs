using Microsoft.Extensions.Hosting;
using Sudoku.Infrastructure.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        services.AddInfrastructureServices(context.Configuration);
    })
    .Build();

host.Run();
