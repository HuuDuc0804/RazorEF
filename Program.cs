using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Security.Requirements;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOptions();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddSingleton<IEmailSender, SendMailService>();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddDbContext<ArticleContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("ArticleContextConnectiton"))
);

//Đăng ký Identity
builder.Services.AddIdentity<AppUser, IdentityRole>()
    .AddEntityFrameworkStores<ArticleContext>()
    .AddDefaultTokenProviders();
// builder.Services.AddDefaultIdentity<AppUser>()
//     .AddEntityFrameworkStores<ArticleContext>()
//     .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/login/";
    options.LogoutPath = "/logout/";
    options.AccessDeniedPath = "/khongduoctruycap.html";
});

builder.Services.AddAuthentication()
        .AddGoogle(options =>
        {
            var googleConfig = builder.Configuration.GetSection("Authentication:Google");
            options.ClientId = googleConfig["ClientId"];
            options.ClientSecret = googleConfig["ClientSecret"];

            // mặc định: https://localhost:7073/signin-google
            options.CallbackPath = "/dang-nhap-google";
        })
        .AddFacebook(options =>
        {
            var facebookConfig = builder.Configuration.GetSection("Authentication:Facebook");
            options.AppId = facebookConfig["AppId"];
            options.AppSecret = facebookConfig["AppSecret"];
            options.CallbackPath = "/dang-nhap-facebook";

        });

// Đăng ký override Identity Erorr Describer (Custom)
builder.Services.AddSingleton<IdentityErrorDescriber, AppIdentityErrorDescriber>();

// Đăng ký Policy
builder.Services.AddAuthorization(options => {
    options.AddPolicy("AllowEditRole", policyBuilder => {
        // Điều kiện của Policy
        policyBuilder.RequireAuthenticatedUser(); // User phải đăng nhập
        policyBuilder.RequireRole("Admin");       // User phải có Role Admin

        policyBuilder.RequireClaim("AllowEdit", "Edit", "Sua");
        // Claims-based authorization
        // policyBuilder.RequireClaim("Ten claim", "value1", "value2");
        // policyBuilder.RequireClaim("Ten claim", new string[] {
        //     "value3",
        //     "value4"
        // });
        // IdentityRoleClaim<string> claim1;
        // IdentityUserClaim<string> claim2;
        // Claim claim3;
    });
    // Đăng ký Requirement Authorization
    options.AddPolicy("InGenZ", policyBuilder => {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.Requirements.Add(new GenZRequirement());
    });

    // Show admin
    options.AddPolicy("ShowAdminMenu", policyBuilder => {
        policyBuilder.RequireRole("Admin");
    });

    // Can Update
    options.AddPolicy("CanUpdateArticle", policyBuilder => {
        policyBuilder.Requirements.Add(new ArticleUpdateRequirement());
    });
});
// Đăng ký dịch vụ Authorization Handler
builder.Services.AddTransient<IAuthorizationHandler, AppAuthorizationHander>();

// Truy cập IdentityOptions
builder.Services.Configure<IdentityOptions>(options =>
{
    // Thiết lập về Password
    options.Password.RequireDigit = false; // Không bắt phải có số
    options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
    options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
    options.Password.RequireUppercase = false; // Không bắt buộc chữ in
    options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
    options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

    // Cấu hình Lockout - khóa user
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Khóa 5 phút
    options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
    options.Lockout.AllowedForNewUsers = true;

    // Cấu hình về User.
    options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Cấu hình đăng nhập.
    options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
    options.SignIn.RequireConfirmedAccount = true;

});

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

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
