using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Services;
using HalloDocMVC.Services.Interface;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using System.Web.WebPages;

namespace HalloDocMVC.Controllers.AdminController
{
    public class InvoicingController : Controller
    {
        #region Configuration
        private readonly IInvoicingService _IInvoicingService;
        private readonly IComboBoxService _IComboBoxService;
        public InvoicingController (IInvoicingService invoicingService, IComboBoxService iComboBoxService)
        {
            _IInvoicingService = invoicingService;
            _IComboBoxService = iComboBoxService;
        }
        #endregion
        public async Task<IActionResult> Index()
        {
            ViewBag.ProviderComboBox = await _IComboBoxService.ComboBoxProvider();

            return View("../AdminPanel/Admin/Invoicing/Index");
        }
        public IActionResult Timesheets()
        {
            return View("../AdminPanel/Admin/Invoicing/Timesheets");
        }
        public async Task<IActionResult> Payrate(int Id, PayrateModel models)
        {
            var model = await _IInvoicingService.GetPayrateByProvider(Id, models);
            return View("../AdminPanel/Admin/Invoicing/Payrate", model);
        }

        public async Task<IActionResult> EditPayrate(int payrate, int categoryId, int physicianId)
        {
            var model = await _IInvoicingService.EditPayrate(payrate, categoryId, physicianId);
            return RedirectToAction("Payrate", "Invoicing");
        }

        public IActionResult GetTimesheet(PendingTimeSheetModel psm)
        {
            int day = (int)psm.Date;
            int month = (int)psm.Month;
            int year = (int)psm.Year;
            return View("../AdminPanel/Admin/Invoicing/Timesheets");
        }
    }
}