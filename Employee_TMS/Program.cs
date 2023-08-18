
using Employee_TMS.Entities;
using Employee_TMS.ExcelValidations;
using Employee_TMS.Repositories;
using Employee_TMS.Services;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using System.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
////dbcontext
builder.Services.Configure<GoogleCaptchaConfig>(builder.Configuration.GetSection("GoogleReCaptcha")); 

builder.Services.AddDbContext<ETMSContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("connection")));
builder.Services.AddTransient<ETMSRepository>();
builder.Services.AddTransient<EmployeeServices>();//services
builder.Services.AddTransient<TokenService>();
builder.Services.AddTransient<ExcelMethods>();
builder.Services.AddTransient<GoogleCaptchaService>();
builder.Services.AddTransient<APIservices>();


builder.Services.AddSession();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=LoginEmployee}/{id?}");

app.Run();
