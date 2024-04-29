using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class PartnersController : Controller
    {
        #region Configuration
        private readonly IComboBoxService _IComboBoxService;
        private readonly IPartnersService _IPartnersService;
        private readonly INotyfService _INotyfService;
        public PartnersController(IComboBoxService iComboBoxService, IPartnersService iPartnersService, INotyfService iNotyfService)
        {
            _IComboBoxService = iComboBoxService;
            _IPartnersService = iPartnersService;
            _INotyfService = iNotyfService;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(int? ProfessionId, string? SearchInput, PaginationVendor paginationVendor)
        {
            ViewBag.VendorComboBox = await _IComboBoxService.ComboBoxHealthProfessionalType();
            PaginationVendor vm = _IPartnersService.GetPartners(ProfessionId, SearchInput, paginationVendor);
            return View("../AdminPanel/Admin/Partners/Index", vm);
        }
        #endregion

        #region AddEditBusiness
        public async Task<IActionResult> AddEditBusiness(int? Id)
        {
            ViewBag.VendorComboBox = await _IComboBoxService.ComboBoxHealthProfessionalType();
            if (Id == null)
            {
                ViewData["AddEditBusiness"] = "Add";
            }
            if (Id.HasValue)
            {
                var vim = await _IPartnersService.GetPartnerById(Id);
                ViewData["AddEditBusiness"] = "Edit";
                return View("../AdminPanel/Admin/Partners/AddEditBusiness", vim);
            }
            return View("../AdminPanel/Admin/Partners/AddEditBusiness");
        }
        #endregion

        #region EditVendorInfo
        public async Task<IActionResult> EditVendorInfo(VendorsModel vdm)
        {
            bool data = await _IPartnersService.EditVendorInfo(vdm);
            if (data)
            {
                _INotyfService.Success("Vendor Info edited successfully");
                return RedirectToAction("AddEditBusiness", new { id = vdm.VendorId });
            }
            else
            {
                _INotyfService.Error("Vendor Info not edited");
            }
            return View("../AdminPanel/Admin/Partners/AddEditBusiness");
        }
        #endregion

        #region AddBusiness
        public async Task<IActionResult> AddBusiness(VendorsModel data)
        {
            ViewBag.VendorComboBox = _IComboBoxService.ComboBoxHealthProfessionalType();
            bool vm = await _IPartnersService.AddVendor(data);
            if (vm)
            {
                _INotyfService.Success("Vendor created sucessfully");
            }
            else
            {
                _INotyfService.Error("Vendot not created");
            }
            return RedirectToAction("Index", new { id = data.VendorId });
        }
        #endregion

        #region DeleteVendor
        public async Task<IActionResult> DeleteVendor(int? VendorId)
        {
            bool data = await _IPartnersService.DeleteVendor(VendorId);
            if (data)
            {
                _INotyfService.Success("Vendor Deleted successfully");
            }
            else
            {
                _INotyfService.Error("Vendor not Deleted");
            }
            return RedirectToAction("Index");
        }
        #endregion
    }
}
