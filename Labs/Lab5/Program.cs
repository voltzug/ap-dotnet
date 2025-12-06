using Lab5.Data;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<Lab5.Data.MoviesDbContext>(
    options => options
        .UseSqlite(builder.Configuration.GetConnectionString("Default"))
        .EnableSensitiveDataLogging(false)
        .LogTo(_ => { }) // disables SQL logging
);
builder.Services.AddControllersWithViews();
builder.WebHost.UseUrls("http://0.0.0.0:8080");

var app = builder.Build();



// Check DB connection and ensure Movies table exists
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<MoviesDbContext>();
        db.Database.EnsureCreated(); // Creates DB and Movies table if not present
        // Try simple query to check connection
        db.Movies.FirstOrDefault();
        DbState.IsDbUp = true;
    }
    catch
    {
        DbState.IsDbUp = false;
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
