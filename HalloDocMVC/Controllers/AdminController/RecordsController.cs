using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class RecordsController : Controller
    {
        private readonly IComboBox _IComboBox;
        private readonly IRecords _IRecords;
        private readonly INotyfService _INotyfService;

        public RecordsController(IComboBox iComboBox, IRecords iRecords, INotyfService iNotyfService)
        {
            _IComboBox = iComboBox;
            _IRecords = iRecords;
            _INotyfService = iNotyfService;
        }
        public async Task<IActionResult> Index(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetRecords(model);
            return View("../AdminPanel/Admin/Records/Index", rm);
        }
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
        public async Task<IActionResult> PatientHistory(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetPatientHistory(model);
            return View("../AdminPanel/Admin/Records/PatientHistory", rm);
        }
        public async Task<IActionResult> ExplorePatientCases(int UserId, RecordsModel records)
        {
            var r  = await _IRecords.GetPatientCases(UserId, records);
            return View("../AdminPanel/Admin/Records/ExplorePatientCases",r );
        }
        public async Task<IActionResult> EmailLogs(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetEmailLogs(model);
            return View("../AdminPanel/Admin/Records/EmailLogs", rm);
        }
        public async Task<IActionResult> SMSLogs(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetSMSLogs(model);
            return View("../AdminPanel/Admin/Records/SMSLogs", rm);
        }
        public async Task<IActionResult> BlockedHistory(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetBlockedHistory(model);
            return View("../AdminPanel/Admin/Records/BlockedHistory", rm);
        }
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
    }
}
