using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.PatientController
{
    public class PatientProfileController : Controller
    {
        #region Configuration
        private readonly IPatientProfileService _IPatientProfileService;
        private readonly INotyfService _INotyfService;
        public PatientProfileController(IPatientProfileService iPatientProfileService, INotyfService iNotyfService)
        {
            _IPatientProfileService = iPatientProfileService;
            _INotyfService = iNotyfService;
        }
        #endregion Configuration

        #region GetProfile
        public IActionResult Index()
        {
            ViewDataUserProfileModel model = _IPatientProfileService.GetProfile();
            return View("~/Views/PatientPanel/Profile/Index.cshtml", model);
        }
        #endregion GetProfile

        #region EditProfile
        public async Task<IActionResult> EditProfile(ViewDataUserProfileModel model)
        {
            if (await _IPatientProfileService.EditProfile(model))
            {
                _INotyfService.Success("Profile has been edited successfully.");
            }
            else
            {
                _INotyfService.Error("Profile has been edited successfully.");
            }
            return RedirectToAction("Index", "PatientProfile");
        }
        #endregion EditProfile
    }
}
