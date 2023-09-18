using Sudoku.Web.Server.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.RegisterGameServices();
builder.Services.AddSingleton<ICellFocusedNotificationService, CellFocusedNotificationService>();
builder.Services.AddSingleton<IInvalidCellNotificationService, InvalidCellNotificationService>();

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

app.Run();
