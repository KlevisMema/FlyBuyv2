using FlyBuy.Data;
using FlyBuy.ReCAPTCHA;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FlyBuy.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options  => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultUI()
    .AddDefaultTokenProviders();
    
builder.Services.AddControllersWithViews();

builder.Services.AddMemoryCache();
builder.Services.AddSession();

builder.Services.AddAuthentication()
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
    })
    .AddLinkedIn(linkedinOptions =>
    {
        linkedinOptions.ClientId = builder.Configuration["Authentication:LinkedIn:ClientId"];
        linkedinOptions.ClientSecret = builder.Configuration["Authentication:LinkedIn:ClientSecret"];
    })
    .AddFacebook(facebookOptions =>
    {
        facebookOptions.ClientId = builder.Configuration["Authentication:Facebook:ClientId"];
        facebookOptions.ClientSecret = builder.Configuration["Authentication:Facebook:ClientSecret"];
    });

builder.Services.Configure<ReCAPTCHASettings>(builder.Configuration.GetSection("GoogleCAPTCHA"));
builder.Services.AddTransient(typeof(GoogleCaptchaService));

//builder.Services.AddTransient<IEmailSender, EmailSender>();
//builder.Services.Configure<AuthMessageSenderOptions>(builder.Configuration);


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
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
app.MapRazorPages();

app.Run();
