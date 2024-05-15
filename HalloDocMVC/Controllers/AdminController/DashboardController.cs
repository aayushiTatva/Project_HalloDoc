using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using LicenseContext = OfficeOpenXml.LicenseContext;

namespace HalloDocMVC.Controllers.AdminController
{
    public class DashboardController : Controller
    {
        #region Configuration
        private readonly IAdminDashboardService _IAdminDashboardService;
        private readonly IComboBoxService _IComboBoxService;
        private readonly ILogger<DashboardController> _Logger;
        public DashboardController(IAdminDashboardService iAdminDashboardService, IComboBoxService iComboBoxService)
        {
            _IAdminDashboardService = iAdminDashboardService;
            _IComboBoxService = iComboBoxService;
        }
        #endregion Configuration
        [CheckProviderAccess("Admin,Provider")]
        [Route("Provider/Dashboard")]
        [Route("Admin/Dashboard")]
        #region Index
        public async Task<IActionResult> Index()
        {
            ViewBag.ComboBoxRegion = await _IComboBoxService.ComboBoxRegions();
            ViewBag.ComboBoxCaseReason = await _IComboBoxService.ComboBoxCaseReasons();
            PaginationModel countRequest = _IAdminDashboardService.CardData(-1);
            if (CV.role() == "Provider")
            {
                countRequest = _IAdminDashboardService.CardData(Convert.ToInt32(CV.UserID()));
                return View("~/Views/AdminPanel/Dashboard/Index.cshtml", countRequest);
            }
            return View("~/Views/AdminPanel/Dashboard/Index.cshtml", countRequest);
        }
        #endregion Index

        #region _SearchResult
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SearchResult(string Status, string Filter, PaginationModel pagination)
        {
            Status ??= CV.CurrentStatus();
            Filter ??= CV.Filter();
            Response.Cookies.Append("Status", Status);
            Response.Cookies.Append("Filter", Filter);

            PaginationModel contacts = _IAdminDashboardService.GetRequests(Status, Filter, pagination);
            if (CV.role() == "Provider")
            {
                contacts = _IAdminDashboardService.GetRequests(Status, Filter, pagination, Convert.ToInt32(CV.UserID()));
            }

            switch (Status)
            {
                case "1":
                    return PartialView("~/Views/AdminPanel/Dashboard/_NewRequest.cshtml", contacts);
                    break;
                case "2":
                    return PartialView("~/Views/AdminPanel/Dashboard/_PendingRequest.cshtml", contacts);
                    break;
                case "4,5":
                    return PartialView("~/Views/AdminPanel/Dashboard/_ActiveRequest.cshtml", contacts);
                    break;
                case "6":
                    return PartialView("~/Views/AdminPanel/Dashboard/_ConcludeRequest.cshtml", contacts);
                    break;
                case "3,7,8":
                    return PartialView("~/Views/AdminPanel/Dashboard/_ToCloseRequest.cshtml", contacts);
                    break;
                case "9":
                    return PartialView("~/Views/AdminPanel/Dashboard/_UnpaidRequest.cshtml", contacts);
                    break;
            }

            return PartialView("");
        }
        #endregion _SearchResult

        public async Task<IActionResult> Login()
        {
            return View("~/Views/AdminPanel/Dashboard/Login.cshtml");
        }
        public async Task<IActionResult> ForgotPassword()
        {
            return View("~/Views/AdminPanel/Dashboard/ForgotPassword.cshtml");
        }
        #region Export
        public IActionResult Export(string status)
        {
            var requestData = _IAdminDashboardService.Export(status);
            List<int> statuslist = status.Split(',').Select(int.Parse).ToList();
            if (statuslist.Count > 1)
            {
                requestData = _IAdminDashboardService.Export(status);
            }
            else
            {
                var currentstatus = CV.CurrentStatus();
                requestData = _IAdminDashboardService.Export(currentstatus);
            }

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage package = new())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("RequestData");

                worksheet.Cells[1, 1].Value = "Name";
                worksheet.Cells[1, 2].Value = "Requestor";
                worksheet.Cells[1, 3].Value = "Request Date";
                worksheet.Cells[1, 4].Value = "Phone";
                worksheet.Cells[1, 5].Value = "Address";
                worksheet.Cells[1, 6].Value = "Notes";
                worksheet.Cells[1, 7].Value = "Physician";
                worksheet.Cells[1, 8].Value = "Birth Date";
                worksheet.Cells[1, 9].Value = "RequestTypeId";
                worksheet.Cells[1, 10].Value = "Email";
                worksheet.Cells[1, 11].Value = "RequestId";

                for (int i = 0; i < requestData.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = requestData[i].PatientName;
                    worksheet.Cells[i + 2, 2].Value = requestData[i].Requestor;
                    worksheet.Cells[i + 2, 3].Value = requestData[i].RequestedDate.ToString("MMM d, yyyy");
                    worksheet.Cells[i + 2, 4].Value = requestData[i].PatientPhoneNumber;
                    worksheet.Cells[i + 2, 5].Value = requestData[i].Address;
                    worksheet.Cells[i + 2, 6].Value = requestData[i].Notes;
                    worksheet.Cells[i + 2, 7].Value = requestData[i].ProviderName;
                    worksheet.Cells[i + 2, 8].Value = requestData[i].DateOfBirth.ToString("MMM d, yyyy");
                    worksheet.Cells[i + 2, 9].Value = requestData[i].RequestTypeId;
                    worksheet.Cells[i + 2, 10].Value = requestData[i].Email;
                    worksheet.Cells[i + 2, 11].Value = requestData[i].RequestId;
                }

                byte[] excelBytes = package.GetAsByteArray();

                return File(excelBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
            }
        }
        #endregion Export

        #region CreateRequest
        public IActionResult CreateRequest()
        {
            return View("~/Views/PatientPanel/CreateRequest/SubmitRequestPage.cshtml");
        }
        #endregion

        public IActionResult Chat()
        {
            return View("../AdminPanel/Admin/Chat/Index");
        }
    }
}
