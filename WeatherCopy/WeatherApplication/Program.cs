using Serilog;
using WeatherApplication;

var builder = WebApplication.CreateBuilder(args);

// Build configuration from appsettings.json and environment variables
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory()) // Set the base path to the current directory
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) // Load appsettings.json
    .AddEnvironmentVariables(); // Optionally add environment variables

// Initialize Serilog from the configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Read Serilog configuration from appsettings.json
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Create a Startup instance
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services); // Call to configure services

var app = builder.Build();

// Configure the HTTP request pipeline using Startup
startup.Configure(app, app.Environment); // Call to configure the HTTP request pipeline

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
