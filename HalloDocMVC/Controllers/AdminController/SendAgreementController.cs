using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class SendAgreementController : Controller
    {
        private readonly IActions _IActions;
        private readonly INotyfService _INotyfService;
        public SendAgreementController(IActions IActions, INotyfService INotyfService)
        {
            _IActions = IActions;
            _INotyfService = INotyfService;
        }
        public IActionResult Index(int RequestId)
        {
            TempData["RequestId"] = "" + RequestId;
            TempData["PatientName"] = "Aayushi Dhruva";
            return View();
        }
        public IActionResult accept(int RequestId)
        {
            _IActions.SendAgreementAccept(RequestId);
            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult reject(int RequestId,string Notes)
        {
            _IActions.SendAgreementReject(RequestId, Notes);
            return RedirectToAction("Index", "Dashboard");
        }
    }
}
