using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Office2010.Excel;
using HalloDocMVC.Controllers.AdminController;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using NuGet.Packaging.Signing;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HalloDocMVC.Controllers.AdminController
{
    public class AccessController : Controller
    {
        private readonly IAccess _IAccess;
        private readonly IComboBox _IComboBox;
        private readonly INotyfService _INotyfService;
        private readonly EmailConfiguration _emailConfiguration;
        private readonly IMyProfile _IMyProfile;
        private readonly IContactYourProvider _IContactYourProvider;

        public AccessController(IAccess iAccess, IComboBox iComboBox, INotyfService iNotyfService, EmailConfiguration emailConfiguration, IMyProfile iMyProfile, IContactYourProvider iContactYourProvider)
        {
            _IAccess = iAccess;
            _IComboBox = iComboBox;
            _INotyfService = iNotyfService;
            _emailConfiguration = emailConfiguration;
            _IMyProfile = iMyProfile;
            _IContactYourProvider = iContactYourProvider; 
        }
        /*public IActionResult Index()
        {
            return View("../AdminPanel/Admin/Access/Index");
        }*/
        public async Task<IActionResult> Index(PaginationRoles paginationRoles)
        {
            PaginationRoles v = _IAccess.GetRoleAccessDetails(paginationRoles);
            return View("../AdminPanel/Admin/Access/Index", v);
        }
        public Task<IActionResult> UserAccess(int? role, PaginationUserAccess paginationUserAccess)
        {
            PaginationUserAccess data =_IAccess.GetAllUserDetails(role, paginationUserAccess);
            if (role != null)
            {
                data =_IAccess.GetAllUserDetails(role, paginationUserAccess);
            }
            return Task.FromResult<IActionResult>(View("../AdminPanel/Admin/Access/UserAccess", data));
        }

        #region CreateRole
        public async Task<IActionResult> CreateRole(int? Id)
        {
            if (Id != null)
            {
                ViewData["RolesAddEdit"] = "Edit";
                RoleByMenuModel rbm = await _IAccess.GetRoleByMenu((int)Id);
                return View("../AdminPanel/Admin/Access/CreateRole", rbm);
            }
            ViewData["RolesAddEdit"] = "Create";
            return View("../AdminPanel/Admin/Access/CreateRole");
        }
        #endregion

        #region GetMenuByAccount
        public async Task<IActionResult> GetMenuByAccount(short AccountType, int RoleId)
        {
            if (AccountType == 0)
            {
                List<Menu> data = await _IAccess.GetMenuByAccount(1);
                return Json(data);
            }
            List<Menu> v = await _IAccess.GetMenuByAccount(AccountType);
            if (RoleId != null)
            {
                List<RoleByMenuModel.Menu> vm = new List<RoleByMenuModel.Menu>();
                List<int> rm = await _IAccess.CheckMenuByRole(RoleId);
                foreach (var item in v)
                {
                    RoleByMenuModel.Menu menu = new RoleByMenuModel.Menu();
                    menu.name = item.Name;
                    menu.Menuid = item.Menuid;

                    if(rm.Contains(item.Menuid)) {
                        menu.Checked = "checked";
                        vm.Add(menu);
                    }
                    else
                    {
                        vm.Add(menu);
                    }
                }
                return Json(vm);
            }
            return Json(v);
        }
        #endregion

        #region PostRoleMenu
        public async Task<IActionResult> PostRoleMenu(RoleByMenuModel role, string Menusid)
        {
            if (role.RoleId == 0)
            {
                if (await _IAccess.PostRoleMenu(role, Menusid, CV.ID()))
                {
                    _INotyfService.Success("Role Added Successfully...");
                }
                else
                {
                    _INotyfService.Error("Role not Added Successfully...");
                }
            }
            else
            {
                if (await _IAccess.PutRoleMenu(role, Menusid, CV.ID()))
                {
                    _INotyfService.Success("Role Modified Successfully...");
                }
                else
                {
                    _INotyfService.Error("Role not Modified...");
                }
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region DeletePhysician
        public async Task<IActionResult> DeleteRole(int roleid)
        {
            bool data = await _IAccess.DeleteRoles(roleid, CV.ID());
            if (data)
            {
                _INotyfService.Success("Role deleted successfully...");
                return RedirectToAction("Index");
            }
            else
            {
                _INotyfService.Success("Role not deleted successfully...");
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region AdminEdit
        public async Task<IActionResult> AdminEdit(int? Id)
        {
            AdminProfileModel p = await _IMyProfile.GetProfile((Id != null ? (int)Id : Convert.ToInt32(CV.UserID())));
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            ViewBag.ComboBoxUserRole = await _IComboBox.AdminRoleComboBox();
            return View("../AdminPanel/Admin/Profile/Index", p);
        }
        #endregion

        #region AddEdit_Profile
        public async Task<IActionResult> PhysicianAddEdit(int? id)
        {
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            ViewBag.UserRoleComboBox = await _IComboBox.PhysicianRoleComboBox();
            if (id == null)
            {
                ViewData["PhysicianAccount"] = "Add";
            }
            else
            {
                ViewData["PhysicianAccount"] = "Edit";
                ProviderModel v = await _IContactYourProvider.GetPhysicianById((int)id);
                return View("../AdminPanel/Admin/Provider/EditPhysician", v);
            }
            return View("../AdminPanel/Admin/Provider/EditPhysician");
        }
        #endregion

        #region AdminAdd

        public async Task<IActionResult> AdminAddEdit(int? id)
        {
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            ViewBag.ComboBoxUserRole = await _IComboBox.AdminRoleComboBox();
            if (id == null)
            {
                ViewData["AdminAccount"] = "Add Admin";
            }
            return View("../AdminPanel/Admin/Access/AdminAdd");
        }
        #endregion

        #region Create_Admin
        [HttpPost]
        public async Task<IActionResult> AdminAdd(AdminProfileModel vm)
        {
            ViewBag.RegionComboBox = await _IComboBox.ComboBoxRegions();
            ViewBag.ComboBoxUserRole = await _IComboBox.AdminRoleComboBox();
            if (await _IMyProfile.AdminPost(vm, CV.ID()))
            {
                _INotyfService.Success("Admin Added Successfully..!");
            }
            else
            {
                _INotyfService.Error("Admin not Added Successfully..!");
                return View("../AdminPanel/Admin/Access/AdminAdd", vm);
            }
            return RedirectToAction("UserAccess");
        }
        #endregion

        #region ResetPassword
        public async Task<IActionResult> ResetPassword(string Password, int AdminId)
        {
            if (await _IMyProfile.ResetPassword(Password, AdminId))
            {
                _INotyfService.Success("Password changed Successfully.");
            }
            else
            {
                _INotyfService.Error("Password remains unchanged");
            }
            return RedirectToAction("AdminEdit", new { id = AdminId });
        }
        #endregion ResetPassword

        #region EditAdministratorInfo
        public async Task<IActionResult> EditAdministratorInfo(AdminProfileModel profile)
        {
            bool data = await _IMyProfile.EditAdministratorInfo(profile);
            if (data)
            {
                _INotyfService.Success("Administration Information Changed.");
            }
            else
            {
                _INotyfService.Error("Administration Information not Changed.");
            }
            return RedirectToAction("AdminEdit", new { id = profile.AdminId });
        }
        #endregion EditAdministratorInfo

        #region EditBillingInfo
        [HttpPost]
        public async Task<IActionResult> EditBillingInfo(AdminProfileModel adminProfile)
        {
            if (await _IMyProfile.EditBillingInfo(adminProfile))
            {
                _INotyfService.Success("Edited successfully");
            }
            else
            {
                _INotyfService.Error("Edit unsuccessfull");
            }
            return RedirectToAction("AdminEdit", new { id = adminProfile.AdminId });
        }
        #endregion

        #region EditAdministratorInfo
        public async Task<IActionResult> EditAccountInfo(ProviderModel profile)
        {
            bool data = await _IContactYourProvider.EditAccountInfo(profile);
            if (data)
            {
                _INotyfService.Success("Account Information Changed.");
            }
            else
            {
                _INotyfService.Error("Account Information not Changed.");
            }
            return RedirectToAction("PhysicianAddEdit", new { id = profile.PhysicianId });
        }
        #endregion EditAdministratorInfo

        #region EditAdministratorInfo
        public async Task<IActionResult> EditPhysicianInfo(ProviderModel profile)
        {
            bool data = await _IContactYourProvider.EditPhysicianInfo(profile);
            if (data)
            {
                _INotyfService.Success("Physician Information Changed.");
            }
            else
            {
                _INotyfService.Error("Physician Information not Changed.");
            }
            return RedirectToAction("PhysicianAddEdit", new { id = profile.PhysicianId });
        }
        #endregion EditAdministratorInfo

        #region EditAdministratorInfo
        public async Task<IActionResult> EditMailingInfo(ProviderModel data)
        {
            if (await _IContactYourProvider.EditMailBillingInfo(data, CV.ID()))
            {
                _INotyfService.Success("mail and billing Information Changed Successfully...");
                return RedirectToAction("PhysicianAddEdit", new { id = data.PhysicianId });
            }
            else
            {
                _INotyfService.Error("mail and billing Information not Changed Successfully...");
                return RedirectToAction("PhysicianAddEdit", new { id = data.PhysicianId });
            }
        }
        #endregion EditAdministratorInfo

        #region EditProviderProfile
        public async Task<IActionResult> EditProviderProfile(ProviderModel data)
        {
            if (await _IContactYourProvider.EditProviderProfile(data, CV.ID()))
            {
                _INotyfService.Success("Profile Changed Successfully...");
                return RedirectToAction("PhysicianAddEdit", new { id = data.PhysicianId });
            }
            else
            {
                _INotyfService.Error("Profile not Changed");
                return RedirectToAction("PhysicianAddEdit", new { id = data.PhysicianId });
            }
        }
        #endregion

        #region EditProviderOnbording
        public async Task<IActionResult> EditProviderOnboarding(ProviderModel data)
        {
            if (await _IContactYourProvider.EditProviderOnbording(data, CV.ID()))
            {
                _INotyfService.Success("Provider Onboarding Changed Successfully...");
                return RedirectToAction("PhysicianAddEdit", new { id = data.PhysicianId });
            }
            else
            {
                _INotyfService.Error("Provider Onboarding not Changed");
                return RedirectToAction("PhysicianAddEdit", new { id = data.PhysicianId });
            }
        }
        #endregion
    }
}
