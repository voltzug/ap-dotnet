using Lab6.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:8080");

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<MoviesDbContext>(
    options => options.UseSqlite("Data Source=movies.db")
        .LogTo(_ => { })
);

var app = builder.Build();


// Check DB connection and ensure Movies table exists
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<MoviesDbContext>();
        db.Database.EnsureCreated(); // Creates DB if not present
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
