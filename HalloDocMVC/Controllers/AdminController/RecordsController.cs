using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class RecordsController : Controller
    {
        #region Configuration
        private readonly IComboBox _IComboBox;
        private readonly IRecords _IRecords;
        private readonly INotyfService _INotyfService;
        private readonly IActions _IActions;

        public RecordsController(IComboBox iComboBox, IRecords iRecords, INotyfService iNotyfService, IActions iActions)
        {
            _IComboBox = iComboBox;
            _IRecords = iRecords;
            _INotyfService = iNotyfService;
            _IActions = iActions;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetRecords(model);
            return View("../AdminPanel/Admin/Records/Index", rm);
        }
        #endregion

        #region DeleteRequestSearchRecords
        public IActionResult DeleteRequest(int? RequestId)
        {
            if (_IRecords.DeleteRequest(RequestId))
            {
                _INotyfService.Success("Request Deleted Successfully.");
            }
            else
            {
                _INotyfService.Error("Request not deleted");
            }
            return RedirectToAction("Index");
        }
        #endregion DeleteRequestSearchRecords

        #region PatientHistory
        public async Task<IActionResult> PatientHistory(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetPatientHistory(model);
            return View("../AdminPanel/Admin/Records/PatientHistory", rm);
        }
        #endregion 

        #region ExplorePatientCases
        public async Task<IActionResult> ExplorePatientCases(int UserId, RecordsModel records)
        {
            var r  = await _IRecords.GetPatientCases(UserId, records);
            return View("../AdminPanel/Admin/Records/ExplorePatientCases",r );
        }
        #endregion 

        #region EmailLogs
        public async Task<IActionResult> EmailLogs(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetEmailLogs(model);
            return View("../AdminPanel/Admin/Records/EmailLogs", rm);
        }
        #endregion 

        #region SMSLogs
        public async Task<IActionResult> SMSLogs(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetSMSLogs(model);
            return View("../AdminPanel/Admin/Records/SMSLogs", rm);
        }
        #endregion 

        #region BlockedHistory
        public async Task<IActionResult> BlockedHistory(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetBlockedHistory(model);
            return View("../AdminPanel/Admin/Records/BlockedHistory", rm);
        }
        #endregion 

        #region Unblock
        public IActionResult Unblock(int RequestId)
        {
            if (_IRecords.Unblock(RequestId, CV.ID()))
            {
                _INotyfService.Success("Case Unblocked Successfully.");
            }
            else
            {
                _INotyfService.Error("Case remains blocked.");
            }

            return RedirectToAction("BlockedHistory");
        }
        #endregion Unblock


        #region ViewCase
        public async Task<IActionResult> ViewCase(int Id)
        {
            ViewBag.ComboBoxRegion = await _IComboBox.ComboBoxRegions();
            ViewBag.ComboBoxCaseReason = await _IComboBox.ComboBoxCaseReasons();
            ViewCaseModel vcm = _IActions.GetRequestForViewCase(Id);
            return View("~/Views/AdminPanel/Actions/ViewCase.cshtml", vcm);
        }
        #endregion 

        #region ViewDocuments
        public async Task<IActionResult> ViewDocuments(int? id, ViewUploadModel model)
        {
            ViewUploadModel vum = await _IActions.GetDocument(id, model);
            return View("~/Views/AdminPanel/Actions/ViewUploads.cshtml", vum);
        }
        #endregion 
    }
}
