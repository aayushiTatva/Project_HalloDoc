using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class RecordsController : Controller
    {
        #region Configuration
        private readonly IComboBoxService _IComboBoxService;
        private readonly IRecordsService _IRecordService;
        private readonly INotyfService _INotyfService;
        private readonly IActionService _IActionService;

        public RecordsController(IComboBoxService iComboBoxService, IRecordsService iRecordService, INotyfService iNotyfService, IActionService iActionService)
        {
            _IComboBoxService = iComboBoxService;
            _IRecordService = iRecordService;
            _INotyfService = iNotyfService;
            _IActionService = iActionService;
        }
        #endregion

        #region Index
        public async Task<IActionResult> Index(RecordsModel model)
        {
            RecordsModel rm = await _IRecordService.GetRecords(model);
            return View("../AdminPanel/Admin/Records/Index", rm);
        }
        #endregion

        #region DeleteRequestSearchRecords
        public IActionResult DeleteRequest(int? RequestId)
        {
            if (_IRecordService.DeleteRequest(RequestId))
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
            RecordsModel rm = await _IRecordService.GetPatientHistory(model);
            return View("../AdminPanel/Admin/Records/PatientHistory", rm);
        }
        #endregion 

        #region ExplorePatientCases
        public async Task<IActionResult> ExplorePatientCases(int UserId, RecordsModel records)
        {
            var r = await _IRecordService.GetPatientCases(UserId, records);
            return View("../AdminPanel/Admin/Records/ExplorePatientCases", r);
        }
        #endregion 

        #region EmailLogs
        public async Task<IActionResult> EmailLogs(RecordsModel model)
        {
            RecordsModel rm = await _IRecordService.GetEmailLogs(model);
            return View("../AdminPanel/Admin/Records/EmailLogs", rm);
        }
        #endregion 

        #region SMSLogs
        public async Task<IActionResult> SMSLogs(RecordsModel model)
        {
            RecordsModel rm = await _IRecordService.GetSMSLogs(model);
            return View("../AdminPanel/Admin/Records/SMSLogs", rm);
        }
        #endregion 

        #region BlockedHistory
        public async Task<IActionResult> BlockedHistory(RecordsModel model)
        {
            RecordsModel rm = await _IRecordService.GetBlockedHistory(model);
            return View("../AdminPanel/Admin/Records/BlockedHistory", rm);
        }
        #endregion 

        #region Unblock
        public IActionResult Unblock(int RequestId)
        {
            if (_IRecordService.Unblock(RequestId, CV.ID()))
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
            ViewBag.ComboBoxRegion = await _IComboBoxService.ComboBoxRegions();
            ViewBag.ComboBoxCaseReason = await _IComboBoxService.ComboBoxCaseReasons();
            ViewCaseModel vcm = _IActionService.GetRequestForViewCase(Id);
            return View("~/Views/AdminPanel/Actions/ViewCase.cshtml", vcm);
        }
        #endregion 

        #region ViewDocuments
        public async Task<IActionResult> ViewDocuments(int? id, ViewUploadModel model)
        {
            ViewUploadModel vum = await _IActionService.GetDocument(id, model);
            return View("~/Views/AdminPanel/Actions/ViewUploads.cshtml", vum);
        }
        #endregion 
    }
}
