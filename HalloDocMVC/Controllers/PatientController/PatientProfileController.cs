using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositories.Admin.Repository;
using HalloDocMVC.Repositories.Patient.Repository;
using HalloDocMVC.Repositories.Patient.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Controllers.PatientController
{
    public class PatientProfileController : Controller
    {
        #region Configuration
        private readonly IPatientProfile _IPatientProfile;
        private readonly INotyfService _INotyfService;
        public PatientProfileController(IPatientProfile iPatientProfile, INotyfService iNotyfService)
        {
            _IPatientProfile = iPatientProfile;
            _INotyfService = iNotyfService;
        }
        #endregion Configuration
        #region GetProfile
        public IActionResult Index()
        {
            ViewDataUserProfileModel model = _IPatientProfile.GetProfile();
            return View("~/Views/PatientPanel/Profile/Index.cshtml", model);
        }
        #endregion GetProfile
        #region EditProfile
        public async Task<IActionResult> EditProfile(ViewDataUserProfileModel model)
        {
            if (await _IPatientProfile.EditProfile(model))
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
