using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;

namespace HalloDocMVC.Controllers.AdminController
{
    public class ProviderLocationController : Controller
    {
        #region Constructor
        private readonly IContactYourProvider _IContactYourProvider;
        private readonly INotyfService _INotyfService;
        private readonly ILogger<ActionsController> _logger;

        public ProviderLocationController(ILogger<ActionsController> logger, IContactYourProvider iProvider, INotyfService iNotyfService)
        {
            _INotyfService = iNotyfService;
            _IContactYourProvider = iProvider;
            _logger = logger;
        }
        #endregion
        public async Task<IActionResult> Index()
        {
            ViewBag.Log = await _IContactYourProvider.FindPhysicianLocation();
            return View("~/Views/AdminPanel/Admin/ProviderLocation/Index.cshtml");
        }
    }
}
