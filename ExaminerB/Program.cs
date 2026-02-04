using ExaminerB.Components;
using ExaminerB.Service;
using ExaminerB.Services2Backend;
using ExaminerS.Models;
using MudBlazor.Services;
var builder = WebApplication.CreateBuilder (args);
// Add services to the container.
builder.Services.AddRazorComponents ().AddInteractiveServerComponents ();
builder.Services.AddScoped<AppState> ();
builder.Services.AddMudServices ();
//add backend services
builder.Services.AddScoped<BeIService, BeService> ();
// Configure HttpClient with base address from configuration or environment
builder.Services.AddHttpClient<FeService> (client =>
    {
        //var baseAddress = builder.Configuration["BaseAddress"] ?? "http://localhost:5232";  //local server - Linux (http ONLY)
        var baseAddress = builder.Configuration["BaseAddress"] ?? "https://localhost:7139";  //local server - Windows
        //var baseAddress = builder.Configuration["BaseAddress"] ?? "https://x.msht.ir";      //remote server: [x].msht.ir
        //var baseAddress = builder.Configuration["BaseAddress"] ?? "https://www.msht.ir";      //remote server: wwww.msht.ir
        client.BaseAddress = new Uri (baseAddress);
        client.Timeout = TimeSpan.FromSeconds (120);
    });
builder.Services.AddServerSideBlazor ();
builder.Services.AddControllers ();
var app = builder.Build ();
if (!app.Environment.IsDevelopment ())
    {
    app.UseExceptionHandler ("/Error", createScopeForErrors: true);
    app.UseHsts ();
    }
// app.UseHttpsRedirection();
app.UseStaticFiles ();
app.UseAntiforgery ();
app.MapRazorComponents<App> ().AddInteractiveServerRenderMode ();
app.MapControllers ();
app.Run ();
