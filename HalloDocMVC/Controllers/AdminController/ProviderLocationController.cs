using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared;

namespace HalloDocMVC.Controllers.AdminController
{
    public class ProviderLocationController : Controller
    {
        #region Constructor
        private readonly IProviderService _IProviderService;
        private readonly INotyfService _INotyfService;
        private readonly ILogger<ActionsController> _logger;

        public ProviderLocationController(ILogger<ActionsController> logger, IProviderService iProviderService, INotyfService iNotyfService)
        {
            _INotyfService = iNotyfService;
            _IProviderService = iProviderService;
            _logger = logger;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index()
        {
            ViewBag.Log = await _IProviderService.FindPhysicianLocation();
            return View("~/Views/AdminPanel/Admin/ProviderLocation/Index.cshtml");
        }
        #endregion
    }
}
