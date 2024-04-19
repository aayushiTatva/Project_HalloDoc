using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.Controllers.AdminController;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositories.Patient;
using HalloDocMVC.Repositories.Patient.Repository.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HalloDocMVC.Controllers.PatientController
{
    public class CreateRequestController : Controller
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly ICreateRequestRepository _ICreateRequestRepository;
        private readonly INotyfService _INotyfService;
        public CreateRequestController(HalloDocContext context, ICreateRequestRepository iActions, INotyfService iNotyfService)
        {
            _context = context;
            _ICreateRequestRepository = iActions;
            _INotyfService = iNotyfService;
        }
        #endregion Configuration
        public IActionResult Index()
        {
            return View("~/Views/PatientPanel/CreateRequest/SubmitRequestPage.cshtml");
        }

        #region CheckEmail

        [HttpPost]
        public async Task<IActionResult> CheckEmailAsync(string email)
        {
            string message;
            var aspnetuser = await _context.Aspnetusers.FirstOrDefaultAsync(m => m.Email == email);
            if (aspnetuser == null)
            {
                message = "False";
            }
            else
            {
                message = "Success";
            }
            return Json(new
            {
                isAspnetuser = aspnetuser == null
            });
        }
        #endregion

        #region PatientRequest
        public IActionResult CreatePatientRequest()
        {
            return View("~/Views/PatientPanel/CreateRequest/PatientRequestPage.cshtml");
        }
        public async Task<IActionResult> PatientRequest(ViewDataPatientRequestModel model)
        {
            if (await _ICreateRequestRepository.CreatePatientRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }
        #endregion

        #region FamilyRequest
        public IActionResult CreateFamilyRequest()
        {
            return View("~/Views/PatientPanel/CreateRequest/FamilyRequestPage.cshtml");
        }
        public async Task<IActionResult> FamilyRequest(ViewDataFamilyRequestModel model)
        {
            if (await _ICreateRequestRepository.CreateFamilyRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }
        #endregion
        

        #region ConciregeRequest
        public IActionResult CreateConciergeRequest()
        {
            return View("~/Views/PatientPanel/CreateRequest/ConciergeRequestPage.cshtml");
        }
        public async Task<IActionResult> ConciergeRequest(ViewDataConciergeRequestModel model)
        {
            if (await _ICreateRequestRepository.CreateConciergeRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }
        #endregion

        #region BusinessRequest
        public IActionResult CreateBusinessRequest()
        {
            return View("~/Views/PatientPanel/CreateRequest/BusinessRequestPage.cshtml");
        }
        public async Task<IActionResult> BusinessRequest(ViewDataBusinessRequestModel model)
        {
            if (await _ICreateRequestRepository.CreateBusinessRequest(model))
            {
                _INotyfService.Success("Request has been created successfully.");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully.");
            }
            return RedirectToAction("Index", "CreateRequest");
        }

        #endregion
    }
}
