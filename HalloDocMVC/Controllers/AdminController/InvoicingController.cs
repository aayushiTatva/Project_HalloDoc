using AspNetCoreHero.ToastNotification.Abstractions;
using DocumentFormat.OpenXml.Bibliography;
using HalloDocMVC.DataModels;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Repositories.Admin.Repository.Interface;
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
        private readonly INotyfService _INotyfService;
        private readonly IGenericRepository<TimesheetDetail> _timesheetDetailRepository;
        public InvoicingController (IInvoicingService invoicingService, IComboBoxService iComboBoxService, INotyfService iNotyfService, IGenericRepository<TimesheetDetail> itimesheetDetailRepository)
        {
            _IInvoicingService = invoicingService;
            _IComboBoxService = iComboBoxService;
            _INotyfService = iNotyfService;
            _timesheetDetailRepository = itimesheetDetailRepository;
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
        public IActionResult Payrate(int Id, PayrateModel models)
        {
            var model = _IInvoicingService.GetPayrateByProvider(Id, models);
            return View("../AdminPanel/Admin/Invoicing/Payrate", model);
        }

        public async Task<IActionResult> EditPayrateMethod(PayrateModel pm, int CategoryId, int physicianId)
        {
            var model = await _IInvoicingService.EditPayrate(pm, CategoryId, physicianId);
            if (model == true)
            {
                _INotyfService.Success("Payrate Updated Successfully");
            }
            else
            {
                _INotyfService.Error("Payrate Not Updated");
            }
            return RedirectToAction("Payrate", "Invoicing", new { id = pm.PhysicianId });
        }

        public IActionResult GetTimesheet(TimeSheetModel psm)
        {
            var model1 = _IInvoicingService.GetTimesheet(psm);
            return View("../AdminPanel/Admin/Invoicing/Timesheets", model1);
        }

        public IActionResult Add()
        {
            return View("../AdminPanel/Admin/Invoicing/AddReceipts");
        }

        //Extra//
        //public ActionResult CheckDateMatch(List<DateTime> datesToCheck)
        //{
        //    using (var db = new YourDbContext())
        //    {
        //        // Check if any date in the list matches a date in the PayrateByProvider table
        //        bool anyDateMatch = datesToCheck.Any(date =>
        //            db.PayrateByProvider.Any(p => p.Date == date.Date));

        //        // Return the result
        //        return View(new { AnyDateMatch = anyDateMatch });
        //    }
        //}

        public async Task<IActionResult> EditTimesheet(TimeSheetModel tsm, int TimesheetId)
        {
            /*return View("../AdminPanel/Admin/Invoicing/Timesheets");*/
            TimeSheetModel model = new TimeSheetModel();
            bool model1 = await _IInvoicingService.EditTimesheet(tsm, TimesheetId, CV.ID());
            if(model1 == true)
            {
                //return View("../AdminPanel/Admin/Invoicing/Timesheets", model);
                return RedirectToAction("GetTimesheet", "Invoicing", new { id = tsm.StartDate });
            }
            else
            {
                return RedirectToAction("Index", "Invoicing");
            }
        }
    }
}