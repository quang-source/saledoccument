using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using SalesContractApplication.API;

var builder = WebApplication.CreateBuilder(args);

// Read authentication settings from appsettings.json
var authSettings = builder.Configuration.GetSection("Authentication");

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = authSettings["DefaultScheme"];
})
.AddCookie(options =>
{
    options.LoginPath = authSettings["LoginUrl"];
    options.ExpireTimeSpan = TimeSpan.FromMinutes(authSettings.GetValue<int>("Timeout"));
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(authSettings.GetValue<int>("Timeout"));
});

builder.Services.AddHttpClient<APIServices>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
