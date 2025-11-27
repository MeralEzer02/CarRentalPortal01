using CarRentalPortal01.Data;
using CarRentalPortal01.Repositories;
using Microsoft.EntityFrameworkCore;
using NToastNotify;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("CarRentalDbContext") ?? throw new InvalidOperationException("Connection string 'CarRentalDbContext' not found.");

builder.Services.AddDbContext<CarRentalDbContext>(options =>
    options.UseSqlServer(connectionString));
// Cookie Bazlý Kimlik Doðrulama Ayarý
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Login";
        options.AccessDeniedPath = "/Auth/Login";
        options.Cookie.Name = "CarRentalCookie";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
    });

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddNToastNotifyToastr(new ToastrOptions()
    {
        ProgressBar = true,
        PositionClass = ToastPositions.BottomRight,
        TimeOut = 5000,
        CloseButton = true
    });

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

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

app.UseAuthentication();

app.UseAuthorization();

app.UseNToastNotify();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
