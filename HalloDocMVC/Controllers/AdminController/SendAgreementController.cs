using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class SendAgreementController : Controller
    {
        #region Configuration
        private readonly IActionService _IActionService;
        private readonly INotyfService _INotyfService;
        public SendAgreementController(IActionService IActionService, INotyfService INotyfService)
        {
            _IActionService = IActionService;
            _INotyfService = INotyfService;
        }
        #endregion 

        #region Index
        public IActionResult Index(int RequestId)
        {
            TempData["RequestId"] = "" + RequestId;
            TempData["PatientName"] = "Aayushi Dhruva";
            return View();
        }
        #endregion 

        #region accept
        public IActionResult accept(int RequestId)
        {
            _IActionService.SendAgreementAccept(RequestId);
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion 

        #region reject
        public IActionResult reject(int RequestId, string Notes)
        {
            _IActionService.SendAgreementReject(RequestId, Notes);
            return RedirectToAction("Index", "Dashboard");
        }
        #endregion 
    }
}
