using BlazorAuthenticationDemoApp.Data;
using BlazorAuthenticationDemoApp.Data.Users;
using BlazorAuthenticationDemoApp.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// Add Db Context
var cs = builder.Configuration.GetConnectionString("default");
builder.Services.AddDbContextFactory<DataContext>(options => options.UseSqlServer(cs));
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(cs));

// Configure our identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 5;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
})
    .AddRoles<IdentityRole>() // Add Roles to support role based authorization
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddAuthorization( options => {
    options.AddPolicy("SupportedCityOnly", policy => policy.RequireClaim("city", "ames"));
});

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<WeatherForecastService>();

builder.Services.AddScoped<AuthenticationStateProvider, IdentityValidationProvider<IdentityUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication(); // Add Authentication Middleware
app.UseAuthorization(); // Add Authorization Middleware

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
