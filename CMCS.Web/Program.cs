using CMCS.Web.Data;
using CMCS.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.InMemory.Storage.Internal;
using InMemoryStore = CMCS.Web.Data.InMemoryStore;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
// Cookie auth for fake-login
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// In-memory store as singleton
builder.Services.AddSingleton<InMemoryStore>();


var app = builder.Build();


// Ensure uploads folder exists
var uploads = Path.Combine(app.Environment.WebRootPath ?? "wwwroot", "uploads");
Directory.CreateDirectory(uploads);


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();








