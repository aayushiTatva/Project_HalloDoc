using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.Repositories.Admin.Repository;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Repositories;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Patient.Repository.Interface;
using HalloDocMVC.Repositories.Patient.Repository;
using Rotativa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
var emailConfig = builder.Configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<HalloDocContext>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSession();
builder.Services.AddScoped<IAdminDashboard, AdminDashboard>();
builder.Services.AddScoped<IActions, Actions>();
builder.Services.AddScoped<IComboBox, ComboBox>();
builder.Services.AddScoped<ICreateRequest, CreateRequest>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ILogin, Login>();
builder.Services.AddScoped<IPatientDashboard, PatientDashboard>();
builder.Services.AddScoped<IPatientProfile, PatientProfile>();
builder.Services.AddScoped<IMyProfile, MyProfile>();
builder.Services.AddScoped<IContactYourProvider, ContactYourProvider>();
builder.Services.AddScoped<IAccess, Access>();
builder.Services.AddScoped<IScheduling, Scheduling>();
builder.Services.AddScoped<IPartners, Partners>();
builder.Services.AddScoped<IRecords, Records>();
builder.Services.AddScoped<IConfirmationNumber, ConfirmationNumber>();
builder.Services.AddNotyf(config => { config.DurationInSeconds = 3; config.IsDismissable = true; config.Position = NotyfPosition.TopRight; });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseRotativa();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
app.UseNotyf();