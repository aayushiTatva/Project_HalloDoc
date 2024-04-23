using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataContext;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.DBEntity.ViewModels.PatientPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using HalloDocMVC.Repositories.Patient.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.PatientController
{
    public class PatientDashboardController : Controller
    {
        #region Configuration
        private readonly HalloDocContext _context;
        private readonly IPatientDashboard _IPatientDashboard;
        private readonly INotyfService _INotyfService;
        private readonly IActions _IActions;
        public PatientDashboardController(HalloDocContext context, IPatientDashboard iPatientDashboard, INotyfService iNotyfService, IActions iAction)
        {
            _context = context;
            _IPatientDashboard = iPatientDashboard;
            _INotyfService = iNotyfService;
            _IActions = iAction;
        }
        #endregion Configuration

        #region Dashboard
        public async Task<IActionResult> Index(PatientDashboardModel model)
        {
            PatientDashboardModel data = _IPatientDashboard.GetPatientData(CV.UserID(), model);
            return View("~/Views/PatientPanel/Dashboard/PatientDashboard.cshtml", data);
        }
        #endregion Dashboard

        #region ViewDocuments
        public async Task<IActionResult> ViewDocuments(int? id, ViewUploadModel model)
        {
            ViewUploadModel vm = await _IActions.GetDocument(id, model);
            return View("~/Views/PatientPanel/Dashboard/ViewDocuments.cshtml", vm);
        }
        #endregion ViewDocuments

        #region UploadDocuments
        public async Task<IActionResult> UploadDoc(int RequestId, List<IFormFile> files)
        {
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (_IActions.UploadDocuments(RequestId, file))
                    {
                        _INotyfService.Success("File Uploaded Successfully.");
                    }
                    else
                    {
                        _INotyfService.Error("File not uploaded.");
                    }
                }
            }
            else
            {
                _INotyfService.Error("No files selected.");
            }

            return RedirectToAction("ViewDocuments", "PatientDashboard", new { id = RequestId });
        }
        #endregion UploadDocuments

        #region RequestForMe
        public IActionResult RequestForMe()
        {
            ViewDataPatientRequestModel model = _IPatientDashboard.RequestForMe();
            return View("~/Views/PatientPanel/Dashboard/RequestForMe.cshtml", model);
        }
        #endregion RequestForMe

        #region CreateRequestForMe
        public async Task<IActionResult> PostMe(ViewDataPatientRequestModel model)
        {
            if (await _IPatientDashboard.PostMe(model))
            {
                _INotyfService.Success("Request has been created successfully");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully");
            }
            return RedirectToAction("Index", "PatientDashboard");
        }
        #endregion CreateRequestForMe

        #region RequestForSomeoneElse
        public IActionResult RequestForSomeoneElse()
        {
            return View("~/Views/PatientPanel/Dashboard/RequestForSomeoneElse.cshtml");
        }
        #endregion RequestForSomeoneElse

        #region CreateRequestForSomeoneElse
        public async Task<IActionResult> PostSomeoneElse(ViewDataPatientRequestModel model)
        {
            if (await _IPatientDashboard.PostSomeoneElse(model))
            {
                _INotyfService.Success("Request has been created successfully");
            }
            else
            {
                _INotyfService.Error("Request has not been created successfully");
            }
            return RedirectToAction("Index", "PatientDashboard");
        }
        #endregion CreateRequestForSomeoneElse
    }
}