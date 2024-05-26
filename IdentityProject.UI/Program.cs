using IdentityProject.UI.Data;
using IdentityProject.UI.Helpers;
using IdentityProject.UI.Models.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<MyDbContext>(c => c.UseSqlServer("Server=.;Initial Catalog=MyDb;Integrated Security=true"));

builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<MyDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<CustomIdentityError>()
    .AddPasswordValidator<MyPasswordValidator>();

//builder.Services.AddAuthentication().AddGoogle(options =>
//{
//    options.ClientId = "";
//    options.ClientSecret = "";

//});

builder.Services.Configure<IdentityOptions>(option =>
{
    //UserSetting
    //option.User.AllowedUserNameCharacters = "abcd123";
    option.User.RequireUniqueEmail = true;

    //Password Setting
    option.Password.RequireDigit = false;
    option.Password.RequireLowercase = false;
    option.Password.RequireNonAlphanumeric = false;//!@#$%^&*()_+
    option.Password.RequireUppercase = false;
    option.Password.RequiredLength = 6;
    option.Password.RequiredUniqueChars = 1;

    //Lokout Setting
    option.Lockout.MaxFailedAccessAttempts = 3;
    option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMilliseconds(10);

    //SignIn Setting
    option.SignIn.RequireConfirmedAccount = false;
    option.SignIn.RequireConfirmedEmail = false;
    option.SignIn.RequireConfirmedPhoneNumber = false;

});


builder.Services.ConfigureApplicationCookie(option =>
{
    // cookie setting
    option.ExpireTimeSpan = TimeSpan.FromMinutes(10);

    option.LoginPath = "/account/login";
    option.AccessDeniedPath = "/Account/AccessDenied";
    option.SlidingExpiration = true;
});


//builder.Services.AddScoped<IUserClaimsPrincipalFactory<User>, AddMyClaims>();

builder.Services.AddScoped<IClaimsTransformation, AddClaim>();

builder.Services.AddSingleton<IAuthorizationHandler, UserCreditHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, IsBlogForUserAuthorization>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminUsers", policy =>
    {
        policy.RequireRole("Admin");
    });

    options.AddPolicy("Buyer", policy =>
    {
        policy.RequireClaim("Buyer");
    });

    options.AddPolicy("BloodType", policy =>
    {
        policy.RequireClaim("Blood", "Ap", "Op");
    });

    options.AddPolicy("Credit", policy =>
    {
        policy.Requirements.Add(new UserCreditRequirement(10000));
    });

    options.AddPolicy("IsBlogForUser", policy =>
    {
        policy.AddRequirements(new BlogRequirement());
    });
});



var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
