using BlazorApplicationInsights;

var builder = WebApplication.CreateBuilder(args);

try
{
    builder
        .Configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .AddUserSecrets<Program>();

    builder.Services.AddRazorPages();
    builder.Services.AddServerSideBlazor();
    builder.Services.AddHealthChecks();
    builder.Services
        .RegisterGameServices()
        .RegisterBlazorGameServices()
        .AddBlazorApplicationInsights(x =>
        {
            x.InstrumentationKey = builder.Configuration["ApplicationInsights:InstrumentationKey"];
        });

    var app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseRouting();

    app.MapBlazorHub();
    app.MapFallbackToPage("/_Host");
    app.MapHealthChecks("/health-check");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine(ex);
    throw;
}