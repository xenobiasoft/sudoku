using Azure.Identity;
using Microsoft.OpenApi;
using Serilog;
using Sudoku.Api;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, services, config) =>
    config
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithMachineName()
        .Enrich.WithThreadId()
        .Enrich.WithEnvironmentName(),
    writeToProviders: true);

builder.AddServiceDefaults();

builder.Configuration.AddUserSecrets(Assembly.GetAssembly(typeof(Program)));

if (!builder.Environment.IsDevelopment())
{
    var keyVaultUri = builder.Configuration["ConnectionStrings:AzureKeyVault"];
    if (!string.IsNullOrEmpty(keyVaultUri))
    {
        builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
    }
    else
    {
        throw new InvalidOperationException("Azure Key Vault connection string is not configured. Please set 'ConnectionStrings:AzureKeyVault' in your configuration.");
    }
}

builder.AddAzureCosmosClient("CosmosDb");

builder.Services.AddApiDefaults(builder.Configuration, builder.Environment);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sudoku API",
        Version = "v1",
        Description = "A Sudoku game API"
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("EnableSwagger"))
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sudoku API V1");
    });
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/health-check");

app.Run();
