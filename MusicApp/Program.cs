using Microsoft.EntityFrameworkCore;
using MusicApp.Models;
using MusicApp.Services;
using SpotifyAPI.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DbContext
builder.Services.AddDbContext<MusicDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register SpotifyService with configuration
builder.Services.AddSingleton<SpotifyService>(provider =>
    new SpotifyService(
        builder.Configuration["Spotify:ClientId"],
        builder.Configuration["Spotify:ClientSecret"]
    ));

// Register MusicService
builder.Services.AddScoped<MusicService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
