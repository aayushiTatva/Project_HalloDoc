using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using System.Globalization;

namespace HalloDocMVC.Controllers.AdminController
{
    public class LoginController : Controller
    {
        #region Configuration
        private readonly IActions _IActions;
        private readonly IComboBox _IComboBox;
        private readonly INotyfService _INotyfService;
        private readonly ILogger<ActionsController> _logger;
        private readonly ILogin _ILogin;
        private readonly IJwtService _IJwtService;
        private readonly EmailConfiguration _emailConfiguration;

        public LoginController(ILogger<ActionsController> logger,
                                      IComboBox ComboBox,
                                      IActions Actions,
                                      INotyfService NotyfService,
                                      ILogin Login,
                                      IJwtService JwtService, EmailConfiguration emailConfiguration)
        {
            _IComboBox = ComboBox;
            _IActions = Actions;
            _INotyfService = NotyfService;
            _logger = logger;
            _ILogin = Login;
            _IJwtService = JwtService;
            _emailConfiguration = emailConfiguration;
        }
        #endregion Configuration

        public IActionResult Index()
        {
            return View("~/Views/AdminPanel/Home/Login.cshtml");
        }
        /*
        public IActionResult ResetPassword()
        {
            return View("~/Views/AdminPanel/Dashboard/ResetPassword.cshtml");
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Validate(Aspnetuser aspNetUser)
        {
            UserInformation u = await _ILogin.CheckAccessLogin(aspNetUser);

            if (u != null)
            {
                var jwttoken = _IJwtService.GenerateJWTAuthetication(u);
                Response.Cookies.Append("jwt", jwttoken);
                Response.Cookies.Append("Status", "1");
                Response.Cookies.Append("Filter", "1,2,3,4");
                if (u.Role == "Patient")
                {
                    return RedirectToAction("Index", "PatientDashboard");
                }
                if(u.Role == "Provider")
                {
                    return Redirect("~/Provider/Dashboard");
                }
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                ViewData["error"] = "Invalid Id or Password";
                return View("~/Views/AdminPanel/Home/Login.cshtml");
            }
        }
        #region Logout
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Index", "Login");
        }
        #endregion Logout
        public IActionResult AuthError()
        {
            return View("../AdminPanel/Login/AuthError");
        }

        #region SendMailResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMailResetPassword(string Email)
        {
            if (await _ILogin.CheckRegisterEmail(Email))
            {
                var Subject = "Change Your Password";
                var agreementUrl = "localhost:5171/Login/ResetPassword?Datetime=" + _emailConfiguration.Encode(DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss tt")) + "&email=" + _emailConfiguration.Encode(Email);
                _emailConfiguration.SendMail(Email, Subject, $"<a href='{agreementUrl}'>Reset Password</a>");
                _INotyfService.Success("Mail Sent Successfully");
            }
            else
            {
                ViewData["EmailCheck"] = "Your Email is not registered";
                return View("~/Views/AdminPanel/Dashboard/ForgotPassword.cshtml");
            }
            return RedirectToAction("Index");
        }
        #endregion
        #region ResetPassword
        public async Task<IActionResult> ResetPassword(string? Datetime,string? email)
        {
            string Decode = _emailConfiguration.Decode(email);
            DateTime s = DateTime.ParseExact(_emailConfiguration.Decode(Datetime), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);
            TimeSpan dif = s - DateTime.Now;
            if(dif.Hours < 24)
            {
                ViewBag.email = Decode;
                return View("~/Views/AdminPanel/Dashboard/ResetPassword.cshtml");
            }
            else
            {
                ViewBag.TotalHours = "Url is expired";
            }
            return View("~/Views/AdminPanel/Dashboard/ResetPassword.cshtml");
        }
        #endregion

        #region SavePassword
        public async Task<IActionResult> SavePassword(string ConfirmPassword, string Password, string Email)
        {
            if(Password != null)
            {
                if(ConfirmPassword != Password)
                {
                    return View("ResetPassword");
                }
                try
                {
                    if(Email == null)
                    {
                        _INotyfService.Error("Password remained unchanged");
                    }
                    else
                    {
                        if(await _ILogin.SavePassword(Email, Password))
                        {
                            _INotyfService.Success("Password saved successfully");
                        }
                        else
                        {
                            _INotyfService.Error("Password remains unchanged");
                        }
                    }
                }
                catch(DbUpdateConcurrencyException)
                {

                }
            }
            return View("~/Views/AdminPanel/Dashboard/ResetPassword.cshtml");
        }
        #endregion
    }
}
