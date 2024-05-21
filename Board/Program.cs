using Board.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache.
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Use whatever time span you deem necessary for session timeout.
    options.Cookie.HttpOnly = true;  // To prevent access to the cookie via client-side scripts.
    options.Cookie.IsEssential = true;  // Make the session cookie essential.
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;  // Ensures that the session cookie is only sent over HTTPS.
});

builder.Services.AddDbContext<ApplicationDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("BoardApp")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=List}/{id?}");

app.Run();
