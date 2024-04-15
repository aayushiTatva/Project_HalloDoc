using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class RecordsController : Controller
    {
        private readonly IComboBox _IComboBox;
        private readonly IRecords _IRecords;

        public RecordsController(IComboBox iComboBox, IRecords iRecords)
        {
            _IComboBox = iComboBox;
            _IRecords = iRecords;
        }
        public async Task<IActionResult> Index(RecordsModel model)
        {
            RecordsModel rm = await _IRecords.GetRecords(model);
            return View("../AdminPanel/Admin/Records/Index", rm);
        }
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
    }
}
