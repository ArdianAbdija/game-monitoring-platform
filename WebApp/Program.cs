using WebApp.Services;

// Program.cs is the startup file for the MVC WebApp project. It registers MVC, sessions,
// the API client service, middleware, and the default MVC route.

// Creates the WebApplication builder used to register all required MVC services.
var builder = WebApplication.CreateBuilder(args);

// Adds MVC controller and Razor view support.
builder.Services.AddControllersWithViews();

// Allows services to access the current HTTP context, which is needed for reading session data inside the API client.
builder.Services.AddHttpContextAccessor();

// Enables session storage so login details such as UserId, Username, Role, and Token can be remembered between requests.
builder.Services.AddSession();

// Registers the API client service that MVC controllers use to call the WebAPI.
builder.Services.AddHttpClient<IGameMonitoringApiClient, GameMonitoringApiClient>();

// Builds the MVC application after all services have been registered.
var app = builder.Build();

// Uses the production error handler and HSTS when the app is not running in development mode.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


// Enables serving static files from wwwroot, such as CSS, JavaScript, images, and uploaded review pictures.
app.UseStaticFiles();

// Enables routing so MVC can match URLs to controllers and actions.
app.UseRouting();

// Enables session before controllers run so controllers can read login/session values.
app.UseSession();

// Enables authorization middleware. The project mainly uses manual session checks, but this keeps standard MVC authorization available.
app.UseAuthorization();

// Defines the default MVC route. If no controller/action is supplied, the user goes to Home/Index.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Starts the MVC WebApp application.
app.Run();
