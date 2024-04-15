using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Data;

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

        public LoginController(ILogger<ActionsController> logger,
                                      IComboBox ComboBox,
                                      IActions Actions,
                                      INotyfService NotyfService,
                                      ILogin Login,
                                      IJwtService JwtService)
        {
            _IComboBox = ComboBox;
            _IActions = Actions;
            _INotyfService = NotyfService;
            _logger = logger;
            _ILogin = Login;
            _IJwtService = JwtService;
        }
        #endregion Configuration

        public IActionResult Index()
        {
            return View("~/Views/AdminPanel/Home/Login.cshtml");
        }
        
        public IActionResult ResetPassword()
        {
            return View("~/Views/AdminPanel/Dashboard/ResetPassword.cshtml");
        }

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
                    return RedirectToAction("Index", "ProviderDashboard");
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
    }
}
