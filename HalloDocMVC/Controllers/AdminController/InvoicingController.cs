using AspNetCoreHero.ToastNotification.Abstractions;
using HalloDocMVC.DBEntity.DataModels;
using HalloDocMVC.DBEntity.ViewModels;
using HalloDocMVC.DBEntity.ViewModels.AdminPanel;
using HalloDocMVC.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace HalloDocMVC.Controllers.AdminController
{
    public class InvoicingController : Controller
    {
        #region Configuration
        private readonly IInvoicingService _InvoicingService;
        private readonly INotyfService _INotyfService;

        public InvoicingController(IInvoicingService invoicingService, INotyfService iNotyfService)
        {
            _InvoicingService = invoicingService;
            _INotyfService = iNotyfService;
        }
        #endregion
        /*[CheckProviderAccess("Admin,Provider", "Invoicing")]
        [Route("Physician/Invoicing")]*/

        #region Index
        public IActionResult Index()
        {
            return View("../AdminPanel/Admin/Invoicing/Index");
        }
        [Route("/Admin/Invoicing")]
        public IActionResult IndexAdmin()
        {
            ViewBag.GetAllPhysicians = _InvoicingService.GetAllPhysicians();
            return View("../AdminPanel/Admin/Invoicing/IndexAdmin");
        }
        #endregion

        #region IsFinalizeSheet

        public IActionResult IsFinalizeSheet(int PhysicianId, DateOnly StartDate)
        {
            bool x = _InvoicingService.isFinalizeTimesheet(PhysicianId, StartDate);
            return Json(new { x });
        }
        public IActionResult IsApproveSheet(int PhysicianId, DateOnly StartDate)
        {
            var x = _InvoicingService.GetPendingTimesheet(PhysicianId, StartDate);
            if (x.Count() == 0)
            {
                return Json(new { x = true });
            }
            return PartialView("../AdminPanel/Admin/Invoicing/_PendingApproved", x);
        }
        #endregion

        #region TimeSheetDetailsAddEdit_PageData

        public async Task<IActionResult> Timesheet(int PhysicianId, DateOnly StartDate)
        {
            if (CV.role() == "Provider" && _InvoicingService.isFinalizeTimesheet(PhysicianId, StartDate))
            {
                _INotyfService.Error("Sheet Is Already Finalized");
                return RedirectToAction("Index");
            }
            int AfterDays = StartDate.Day == 1 ? 14 : DateTime.DaysInMonth(StartDate.Year, StartDate.Month) - 14; ;
            var TimeSheetDetails = _InvoicingService.PostTimesheetDetails(PhysicianId, StartDate, AfterDays, CV.ID());
            List<TimesheetdetailreimbursementModel> h = await _InvoicingService.GetTimesheetBills(TimeSheetDetails);
            var Timesheet = _InvoicingService.GetTimesheetDetails(TimeSheetDetails, h, PhysicianId);
            Timesheet.PhysicianId = PhysicianId;
            return View("../AdminPanel/Admin/Invoicing/TimeSheet", Timesheet);
        }
        #endregion

        #region GetTimesheetDetailData
        public async Task<IActionResult> GetTimesheetDetailsData(int PhysicianId, DateOnly StartDate)
        {
            var Timesheet = new TimeSheetModel();
            if (StartDate == DateOnly.MinValue)
            {
                Timesheet.TimesheetdetailsList = new List<TimesheetdetailModel> { };
                Timesheet.TimesheetdetailreimbursementList = new List<TimesheetdetailreimbursementModel> { };
            }
            else
            {
                List<TimesheetDetail> x = _InvoicingService.PostTimesheetDetails(PhysicianId, StartDate, 0, CV.ID());
                List<TimesheetdetailreimbursementModel> h = await _InvoicingService.GetTimesheetBills(x);
                Timesheet = _InvoicingService.GetTimesheetDetails(x, h, PhysicianId);
            }
            if (Timesheet == null)
            {
                var Timesheets = new TimeSheetModel();
                Timesheets.TimesheetdetailsList = new List<TimesheetdetailModel> { };
                Timesheets.TimesheetdetailreimbursementList = new List<TimesheetdetailreimbursementModel> { };
                return PartialView("../AdminPanel/Admin/Invoicing/_TimeSheetTable", Timesheets);
            }


            return PartialView("../AdminPanel/Admin/Invoicing/_TimeSheetTable", Timesheet);
        }
        #endregion

        #region TimeSheetDetailsEdit
        public IActionResult TimeSheetDetailsEdit(TimeSheetModel viewTimeSheet, int PhysicianId)
        {
            if (_InvoicingService.PutTimesheetDetails(viewTimeSheet.TimesheetdetailsList, CV.ID()))
            {
                _INotyfService.Success("TimeSheet Edited Successfully..!");
            }

            return RedirectToAction("Timesheet", new { PhysicianId, StartDate = viewTimeSheet.TimesheetdetailsList[0].Timesheetdate });
        }
        #endregion

        #region TimeSheetBillAddEdit
        public IActionResult TimeSheetBillAddEdit(int? Trid, DateOnly Timesheetdate, IFormFile file, int Timesheetdetailid, int Amount, string Item, int PhysicianId, DateOnly StartDate)
        {
            TimesheetdetailreimbursementModel timesheetdetailreimbursement = new TimesheetdetailreimbursementModel();
            timesheetdetailreimbursement.Timesheetdetailid = Timesheetdetailid;
            timesheetdetailreimbursement.Timesheetdetailreimbursementid = Trid;
            timesheetdetailreimbursement.Amount = Amount;
            timesheetdetailreimbursement.Billfile = file;
            timesheetdetailreimbursement.Itemname = Item;
            if (_InvoicingService.TimeSheetBillAddEdit(timesheetdetailreimbursement, CV.ID()))
            {
                _INotyfService.Success("Bill Change Successfully..!");
            }
            return RedirectToAction("Timesheet", new { PhysicianId = PhysicianId, StartDate = StartDate });
        }
        #endregion

        #region TimeSheetBill_Delete
        public IActionResult TimeSheetBillRemove(int? Trid, int PhysicianId, DateOnly StartDate)
        {
            TimesheetdetailreimbursementModel timesheetdetailreimbursement = new TimesheetdetailreimbursementModel();
            timesheetdetailreimbursement.Timesheetdetailreimbursementid = Trid;
            if (_InvoicingService.TimeSheetBillRemove(timesheetdetailreimbursement, CV.ID()))
            {
                _INotyfService.Success("Bill deleted Successfully..!");
            }
            return RedirectToAction("Timesheet", new { PhysicianId = PhysicianId, StartDate = StartDate });
        }
        #endregion

        #region SetToFinalize
        public IActionResult SetToFinalize(int timesheetid)
        {
            if (_InvoicingService.SetToFinalize(timesheetid, CV.ID()))
            {
                _INotyfService.Success("Sheet Finalized Successfully..!");
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region SetToApprove
        public async Task<IActionResult> SetToApprove(TimeSheetModel ts)
        {
            if (await _InvoicingService.SetToApprove(ts, CV.ID()))
            {
                _INotyfService.Success("Sheet Approved Successfully..!");
            }
            return RedirectToAction("IndexAdmin");
        }
        #endregion
        }
    }
