using Azure.Identity;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.OpenApi;
using Sudoku.Api;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// Add Azure App Configuration
builder.AddAzureAppConfigurationAdvanced();

var keyVaultUri = builder.Configuration["ConnectionStrings:AzureKeyVault"];
if (!string.IsNullOrEmpty(keyVaultUri))
{
    builder.Configuration.AddAzureKeyVault(new Uri(keyVaultUri), new DefaultAzureCredential());
}

builder.Services.AddApiDefaults(builder.Configuration, builder.Environment);

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
});

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

if (true)//app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sudoku API V1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.UseHttpLogging();

app.MapHealthChecks("/health-check");

app.Run();
