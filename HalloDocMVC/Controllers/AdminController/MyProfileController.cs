using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Models;
using HalloDocMVC.Repositories.Admin.Repository;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    [CheckProviderAccess("Admin")]
    public class MyProfileController : Controller
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly IMyProfile _IMyProfile;
        private readonly IComboBox _IComboBox;
        private readonly INotyfService _INotyfService;

        public MyProfileController(HalloDocContext context, IMyProfile iMyProfile, IComboBox iComboBox, INotyfService iNotyfService)
        {
            _context = context;
            _IMyProfile = iMyProfile;
            _IComboBox = iComboBox;
            _INotyfService = iNotyfService;
        }
        #endregion Configuration

        #region Index
        public async Task<IActionResult> Index()
        {
            AdminProfileModel data = await _IMyProfile.GetProfile(Convert.ToInt32(CV.UserID()));
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            ViewBag.ComboBoxUserRole = await _IComboBox.ComboBoxUserRole();
            return View("~/Views/AdminPanel/Admin/Profile/Index.cshtml", data);
        }
        #endregion

        #region ResetPassword
        public async Task<IActionResult> ResetPassword(string Password)
        {
            if (await _IMyProfile.ResetPassword(Password, Convert.ToInt32(CV.UserID())))
            {
                _INotyfService.Success("Password changed Successfully.");
            }
            else
            {
                _INotyfService.Error("Password remains unchanged");
            }
            return RedirectToAction("Index");
        }
        #endregion ResetPassword

        #region EditAdministratorInfo
        [HttpPost]
        public async Task<IActionResult> EditAdministratorInfo(AdminProfileModel adminProfile)
        {
            if(await _IMyProfile.EditAdministratorInfo(adminProfile))
            {
                _INotyfService.Success("Edited successfully");
            }
            else
            {
                _INotyfService.Error("Edited unsuccessfull");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region EditBillingInfo
        [HttpPost]
        public async Task<IActionResult> EditBillingInfo(AdminProfileModel adminProfile)
        {
            if(await _IMyProfile.EditBillingInfo(adminProfile))
            {
                _INotyfService.Success("Edited successfully");
            }
            else
            {
                _INotyfService.Error("Edit unsuccessfull");
            }
            return RedirectToAction("Index","MyProfile");
        }
        #endregion
    }
}
